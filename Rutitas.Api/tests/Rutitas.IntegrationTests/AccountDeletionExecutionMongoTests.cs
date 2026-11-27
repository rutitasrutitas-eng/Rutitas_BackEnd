using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Rutitas.Api.Services;
using Rutitas.Domain.Entities;
using Rutitas.Infrastructure.Repositories;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class AccountDeletionExecutionMongoTests
{
    private static IMongoDatabase CreateDatabase()
    {
        var connection = Environment.GetEnvironmentVariable(
            "RUTITAS_DELETE001_TEST_MONGO");

        if (string.IsNullOrWhiteSpace(connection) ||
            !System.Text.RegularExpressions.Regex.IsMatch(
                connection,
                @"^mongodb://127\.0\.0\.1:\d+/\?directConnection=true&replicaSet=rs0$"))
        {
            throw new InvalidOperationException(
                "An isolated local replica set is required.");
        }

        var client = new MongoClient(connection);

        return client.GetDatabase(
            "rutitas_delete001_test_" +
            Guid.NewGuid().ToString("N"));
    }

    private static async Task<User> SeedAsync(
        IMongoDatabase database,
        string status = "pending_deletion",
        bool expired = true)
    {
        var now = DateTime.UtcNow;

        var deadline = expired
            ? now.AddMinutes(-5)
            : now.AddDays(2);

        var user = new User
        {
            Email = Guid.NewGuid().ToString("N")
                + "@example.invalid",
            DisplayName = "Synthetic test account",
            PasswordHash = "synthetic-test-value",
            Status = status,
            SessionVersion = status == "active" ? 1 : 2,
            DeletionRequestedAtUtc = status == "active"
                ? null
                : deadline.AddDays(-7),
            DeleteAfterUtc = status == "active"
                ? null
                : deadline,
            CreatedAt = now.AddDays(-30),
            UpdatedAt = now
        };

        var users = new MongoUserRepository(database);

        await users.CreateAsync(user);

        return user;
    }

    private static async Task<
        MongoAccountDeletionExecutionRepository> RepositoryAsync(
            IMongoDatabase database)
    {
        var repository =
            new MongoAccountDeletionExecutionRepository(database);

        await repository.EnsureIndexesAsync();

        return repository;
    }

    private static async Task ExecuteJobAsync(
        MongoAccountDeletionExecutionRepository repository,
        AccountDeletionJob job,
        string workerId)
    {
        Assert.True(await repository.TryBeginAccountDeletionAsync(
            job.Id, workerId, job.LeaseVersion));

        Assert.True(await repository.TryDeleteClaimedAccountAsync(
            job.Id, workerId, job.LeaseVersion));

        Assert.True(await repository.TryCompleteAsync(
            job.Id, workerId, job.LeaseVersion));
    }

    [Fact]
    public async Task ExpiredAccountDeleted_OtherDataPreserved()
    {
        var database = CreateDatabase();
        var repository = await RepositoryAsync(database);

        var target = await SeedAsync(database);

        var active = await SeedAsync(
            database, "active");

        var future = await SeedAsync(
            database, expired: false);

        var routes = database.GetCollection<BsonDocument>(
            "routes");

        var tokens = database.GetCollection<BsonDocument>(
            "revoked_tokens");

        var route = new BsonDocument
        {
            { "_id", ObjectId.GenerateNewId() },
            { "Name", "Synthetic shared route" }
        };

        var token = new BsonDocument
        {
            { "_id", Guid.NewGuid().ToString("N") },
            { "ExpiresAt", DateTime.UtcNow.AddHours(1) }
        };

        await routes.InsertOneAsync(route);
        await tokens.InsertOneAsync(token);

        var users = new MongoUserRepository(database);

        Assert.Equal(
            1,
            await repository.DiscoverDueAccountsAsync(20));

        Assert.Equal(
            0,
            await repository.DiscoverDueAccountsAsync(20));

        var job = await repository.TryClaimNextAsync(
            "worker-a", TimeSpan.FromMinutes(2));

        Assert.NotNull(job);
        Assert.Equal(target.Id, job.UserId);

        await ExecuteJobAsync(
            repository, job, "worker-a");

        Assert.Null(await users.GetByIdAsync(target.Id));

        Assert.NotNull(
            await users.GetByIdAsync(active.Id));

        Assert.NotNull(
            await users.GetByIdAsync(future.Id));

        var persistedRoute = await routes
            .Find(new BsonDocument())
            .SingleAsync();

        var persistedToken = await tokens
            .Find(new BsonDocument())
            .SingleAsync();

        Assert.Equal(route, persistedRoute);
        Assert.Equal(token, persistedToken);

        var completed = await database
            .GetCollection<AccountDeletionJob>(
                "account_deletion_jobs")
            .Find(x => x.Id == job.Id)
            .SingleAsync();

        Assert.Equal(
            AccountDeletionJobStates.Completed,
            completed.Status);

        Assert.Equal(
            AccountDeletionCheckpoints.Verified,
            completed.Checkpoint);

        Assert.False(
            await repository.TryDeleteClaimedAccountAsync(
                job.Id, "worker-a", job.LeaseVersion));

        Assert.Null(await repository.TryClaimNextAsync(
            "worker-b", TimeSpan.FromMinutes(2)));
    }

    [Fact]
    public async Task ConcurrentWorkers_OnlyOneClaimsJob()
    {
        var database = CreateDatabase();

        var repositoryA = await RepositoryAsync(database);
        var repositoryB = await RepositoryAsync(database);

        await SeedAsync(database);

        await Task.WhenAll(
            repositoryA.DiscoverDueAccountsAsync(20),
            repositoryB.DiscoverDueAccountsAsync(20));

        var claims = await Task.WhenAll(
            repositoryA.TryClaimNextAsync(
                "worker-a", TimeSpan.FromMinutes(2)),
            repositoryB.TryClaimNextAsync(
                "worker-b", TimeSpan.FromMinutes(2)));

        var valid = claims
            .Where(x => x is not null)
            .ToArray();

        Assert.Single(valid);

        var job = valid[0]!;

        var jobs = database.GetCollection<AccountDeletionJob>(
            "account_deletion_jobs");

        Assert.Equal(
            1L,
            await jobs.CountDocumentsAsync(
                Builders<AccountDeletionJob>.Filter.Empty));

        var wrongWorker = job.LeaseOwner == "worker-a"
            ? "worker-b"
            : "worker-a";

        Assert.False(
            await repositoryA.TryBeginAccountDeletionAsync(
                job.Id,
                wrongWorker,
                job.LeaseVersion));

        await ExecuteJobAsync(
            repositoryA,
            job,
            job.LeaseOwner!);
    }

    [Fact]
    public async Task ExpiredLease_ResumesAfterAccountClaimed()
    {
        var database = CreateDatabase();
        var repository = await RepositoryAsync(database);

        var target = await SeedAsync(database);

        await repository.DiscoverDueAccountsAsync(20);

        var first = await repository.TryClaimNextAsync(
            "worker-old", TimeSpan.FromMinutes(2));

        Assert.NotNull(first);

        Assert.True(
            await repository.TryBeginAccountDeletionAsync(
                first.Id,
                "worker-old",
                first.LeaseVersion));

        var jobs = database.GetCollection<AccountDeletionJob>(
            "account_deletion_jobs");

        // Simular interrupcion del proceso y lease vencido.
        await jobs.UpdateOneAsync(
            x => x.Id == first.Id,
            Builders<AccountDeletionJob>.Update.Set(
                x => x.LeaseExpiresAtUtc,
                DateTime.UtcNow.AddMinutes(-1)));

        var restarted = await RepositoryAsync(database);

        var second = await restarted.TryClaimNextAsync(
            "worker-new", TimeSpan.FromMinutes(2));

        Assert.NotNull(second);

        Assert.Equal(first.Id, second.Id);

        Assert.True(
            second.LeaseVersion > first.LeaseVersion);

        Assert.Equal(
            AccountDeletionCheckpoints.AccountClaimed,
            second.Checkpoint);

        Assert.False(
            await repository.TryDeleteClaimedAccountAsync(
                first.Id,
                "worker-old",
                first.LeaseVersion));

        await ExecuteJobAsync(
            restarted, second, "worker-new");

        var users = new MongoUserRepository(database);

        Assert.Null(await users.GetByIdAsync(target.Id));
    }

    [Fact]
    public async Task ObsoleteJob_DoesNotDeleteRecoveredAccount()
    {
        var database = CreateDatabase();
        var repository = await RepositoryAsync(database);

        var target = await SeedAsync(database);

        await repository.DiscoverDueAccountsAsync(20);

        var job = await repository.TryClaimNextAsync(
            "worker-a", TimeSpan.FromMinutes(2));

        Assert.NotNull(job);

        // Simular un trabajo obsoleto respecto al estado
        // y version persistidos del titular.
        var collection = database
            .GetCollection<User>("users");

        await collection.UpdateOneAsync(
            x => x.Id == target.Id,
            Builders<User>.Update
                .Set(x => x.Status, "active")
                .Inc(x => x.SessionVersion, 1L)
                .Unset(x => x.DeleteAfterUtc)
                .Unset(x => x.DeletionRequestedAtUtc));

        Assert.False(
            await repository.TryBeginAccountDeletionAsync(
                job.Id, "worker-a", job.LeaseVersion));

        Assert.False(
            await repository.TryDeleteClaimedAccountAsync(
                job.Id, "worker-a", job.LeaseVersion));

        var users = new MongoUserRepository(database);

        var persisted = await users.GetByIdAsync(target.Id);

        Assert.NotNull(persisted);
        Assert.Equal("active", persisted.Status);

        var cancelled = await database
            .GetCollection<AccountDeletionJob>(
                "account_deletion_jobs")
            .Find(x => x.Id == job.Id)
            .SingleAsync();

        Assert.Equal(
            AccountDeletionJobStates.Cancelled,
            cancelled.Status);
    }

    [Fact]
    public async Task RetryLimit_BlocksJobWithoutDeletingAccount()
    {
        var database = CreateDatabase();
        var repository = await RepositoryAsync(database);

        var target = await SeedAsync(database);

        await repository.DiscoverDueAccountsAsync(20);

        var max = MongoAccountDeletionExecutionRepository
            .MaxExecutionAttempts;

        for (var i = 1; i <= max; i++)
        {
            var job = await repository.TryClaimNextAsync(
                "worker-retry",
                TimeSpan.FromMinutes(2));

            Assert.NotNull(job);
            Assert.Equal(i, job.AttemptCount);

            Assert.True(
                await repository.TryScheduleRetryAsync(
                    job.Id,
                    "worker-retry",
                    job.LeaseVersion,
                    "mongo_transient",
                    TimeSpan.Zero,
                    max));
        }

        Assert.Null(await repository.TryClaimNextAsync(
            "worker-retry",
            TimeSpan.FromMinutes(2)));

        var blocked = await database
            .GetCollection<AccountDeletionJob>(
                "account_deletion_jobs")
            .Find(x => x.UserId == target.Id)
            .SingleAsync();

        Assert.Equal(
            AccountDeletionJobStates.Blocked,
            blocked.Status);

        var users = new MongoUserRepository(database);

        Assert.NotNull(await users.GetByIdAsync(target.Id));
    }

    [Fact]
    public async Task RestartAfterRemoval_CompletesVerification()
    {
        var database = CreateDatabase();
        var repository = await RepositoryAsync(database);

        var target = await SeedAsync(database);

        await repository.DiscoverDueAccountsAsync(20);

        var first = await repository.TryClaimNextAsync(
            "worker-old", TimeSpan.FromMinutes(2));

        Assert.NotNull(first);

        Assert.True(
            await repository.TryBeginAccountDeletionAsync(
                first.Id, "worker-old", first.LeaseVersion));

        Assert.True(
            await repository.TryDeleteClaimedAccountAsync(
                first.Id, "worker-old", first.LeaseVersion));

        var jobs = database.GetCollection<AccountDeletionJob>(
            "account_deletion_jobs");

        await jobs.UpdateOneAsync(
            x => x.Id == first.Id,
            Builders<AccountDeletionJob>.Update.Set(
                x => x.LeaseExpiresAtUtc,
                DateTime.UtcNow.AddMinutes(-1)));

        var restarted = await RepositoryAsync(database);

        var second = await restarted.TryClaimNextAsync(
            "worker-new", TimeSpan.FromMinutes(2));

        Assert.NotNull(second);

        Assert.Equal(
            AccountDeletionCheckpoints.AccountRemoved,
            second.Checkpoint);

        await ExecuteJobAsync(
            restarted, second, "worker-new");

        var users = new MongoUserRepository(database);

        Assert.Null(await users.GetByIdAsync(target.Id));

        var completed = await jobs
            .Find(x => x.Id == first.Id)
            .SingleAsync();

        Assert.Equal(
            AccountDeletionJobStates.Completed,
            completed.Status);
    }

    [Fact]
    public async Task Worker_ProcessesExpiredAccountAutomatically()
    {
        var database = CreateDatabase();

        var target = await SeedAsync(database);

        var active = await SeedAsync(
            database, "active");

        var services = new ServiceCollection();

        services.AddSingleton<IMongoDatabase>(database);

        services.AddScoped<
            MongoAccountDeletionExecutionRepository>();

        using var provider = services.BuildServiceProvider();

        using var worker = new AccountDeletionWorker(
            provider.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<AccountDeletionWorker>.Instance,
            Options.Create(new AccountDeletionWorkerOptions
            {
                Enabled = true,
                PollIntervalSeconds = 1,
                LeaseDurationSeconds = 120,
                BatchSize = 10,
                RetryBaseSeconds = 1
            }));

        var jobs = database.GetCollection<AccountDeletionJob>(
            "account_deletion_jobs");

        try
        {
            await worker.StartAsync(CancellationToken.None);

            using var timeout = new CancellationTokenSource(
                TimeSpan.FromSeconds(45));

            while (true)
            {
                timeout.Token.ThrowIfCancellationRequested();

                var job = await jobs
                    .Find(x => x.UserId == target.Id)
                    .FirstOrDefaultAsync(timeout.Token);

                if (job?.Status ==
                    AccountDeletionJobStates.Completed)
                {
                    break;
                }

                Assert.NotEqual(
                    AccountDeletionJobStates.Blocked,
                    job?.Status);

                await Task.Delay(
                    250, timeout.Token);
            }
        }
        finally
        {
            using var stop = new CancellationTokenSource(
                TimeSpan.FromSeconds(10));

            await worker.StopAsync(stop.Token);
        }

        var users = new MongoUserRepository(database);

        Assert.Null(await users.GetByIdAsync(target.Id));

        Assert.NotNull(await users.GetByIdAsync(active.Id));
    }
}
