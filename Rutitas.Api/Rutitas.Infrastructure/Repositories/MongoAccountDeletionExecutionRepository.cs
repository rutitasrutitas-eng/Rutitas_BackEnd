using System.Globalization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Rutitas.Application.Interfaces;
using Rutitas.Domain.Entities;

namespace Rutitas.Infrastructure.Repositories;

public sealed class MongoAccountDeletionExecutionRepository
    : IAccountDeletionExecutionRepository
{
    public const int MaxExecutionAttempts = 5;

    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<User> _users;
    private readonly IMongoCollection<AccountDeletionJob> _jobs;

    private static readonly FilterDefinitionBuilder<User> UF =
        Builders<User>.Filter;

    private static readonly UpdateDefinitionBuilder<User> UU =
        Builders<User>.Update;

    private static readonly FilterDefinitionBuilder<AccountDeletionJob> JF =
        Builders<AccountDeletionJob>.Filter;

    private static readonly UpdateDefinitionBuilder<AccountDeletionJob> JU =
        Builders<AccountDeletionJob>.Update;

    public MongoAccountDeletionExecutionRepository(
        IMongoDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);

        // Restriccion temporal para la implementacion local.
        if (!database.DatabaseNamespace.DatabaseName.StartsWith(
                "rutitas_delete001_test_",
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Deletion execution requires an isolated test database.");
        }

        var allowedHosts = new[]
        {
            "localhost",
            "127.0.0.1",
            "::1",
            "[::1]"
        };

        if (database.Client.Settings.Servers.Any(server =>
                !allowedHosts.Contains(
                    server.Host,
                    StringComparer.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "Deletion execution requires local MongoDB.");
        }

        _database = database;

        _users = database.GetCollection<User>("users");

        _jobs = database.GetCollection<AccountDeletionJob>(
            "account_deletion_jobs");
    }

    private static string Field<T>(string property)
    {
        return BsonClassMap.LookupClassMap(typeof(T))
            .GetMemberMap(property)
            .ElementName;
    }

    private static FilterDefinition<T> CompareServerClock<T>(
        string property,
        string comparison)
    {
        return new BsonDocument(
            "$expr",
            new BsonDocument(
                comparison,
                new BsonArray
                {
                    "$" + Field<T>(property),
                    "$$NOW"
                }));
    }

    private async Task<DateTime> ServerNowAsync(
        CancellationToken ct)
    {
        var result = await _database.RunCommandAsync<BsonDocument>(
            new BsonDocument("hello", 1),
            cancellationToken: ct);

        return result["localTime"].AsBsonDateTime
            .ToUniversalTime();
    }

    public async Task EnsureIndexesAsync(
        CancellationToken ct = default)
    {
        var dueIndex = new CreateIndexModel<User>(
            Builders<User>.IndexKeys
                .Ascending(x => x.Status)
                .Ascending(x => x.DeleteAfterUtc),
            new CreateIndexOptions
            {
                Name = "ix_users_deletion_due"
            });

        var pendingIndex = new CreateIndexModel<AccountDeletionJob>(
            Builders<AccountDeletionJob>.IndexKeys
                .Ascending(x => x.Status)
                .Ascending(x => x.NextAttemptAtUtc)
                .Ascending(x => x.LeaseExpiresAtUtc),
            new CreateIndexOptions
            {
                Name = "ix_deletion_jobs_processing"
            });

        await _users.Indexes.CreateOneAsync(
            dueIndex,
            cancellationToken: ct);

        await _jobs.Indexes.CreateOneAsync(
            pendingIndex,
            cancellationToken: ct);
    }

    public async Task<int> DiscoverDueAccountsAsync(
        int batchSize,
        CancellationToken cancellationToken = default)
    {
        if (batchSize < 1 || batchSize > 1000)
        {
            throw new ArgumentOutOfRangeException(
                nameof(batchSize));
        }

        var now = await ServerNowAsync(cancellationToken);

        var filter = UF.And(
            UF.Eq(x => x.Status, "pending_deletion"),
            UF.Ne(x => x.DeleteAfterUtc, null),
            UF.Ne(x => x.DeletionRequestedAtUtc, null),
            UF.Gte(x => x.SessionVersion, 1L),
            CompareServerClock<User>(
                nameof(User.DeleteAfterUtc),
                "$lte"));

        var query = _users.Find(filter)
            .SortBy(x => x.DeleteAfterUtc)
            .Project(x => new
            {
                x.Id,
                x.SessionVersion,
                x.DeletionRequestedAtUtc,
                x.DeleteAfterUtc
            });

        using var cursor = await query.ToCursorAsync(
            cancellationToken);

        var created = 0;

        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var user in cursor.Current)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!ObjectId.TryParse(user.Id, out _) ||
                    user.DeletionRequestedAtUtc is null ||
                    user.DeleteAfterUtc is null ||
                    user.DeleteAfterUtc.Value -
                        user.DeletionRequestedAtUtc.Value
                        != TimeSpan.FromDays(7))
                {
                    continue;
                }

                var jobId = user.Id + ":" +
                    user.SessionVersion.ToString(
                        CultureInfo.InvariantCulture);

                var update = JU
                    .SetOnInsert(x => x.UserId, user.Id)
                    .SetOnInsert(
                        x => x.ExpectedSessionVersion,
                        user.SessionVersion)
                    .SetOnInsert(
                        x => x.Status,
                        AccountDeletionJobStates.Pending)
                    .SetOnInsert(
                        x => x.Checkpoint,
                        AccountDeletionCheckpoints.NotStarted)
                    .SetOnInsert(
                        x => x.RequestedAtUtc,
                        user.DeletionRequestedAtUtc.Value)
                    .SetOnInsert(
                        x => x.DeleteAfterUtc,
                        user.DeleteAfterUtc.Value)
                    .SetOnInsert(x => x.CreatedAtUtc, now)
                    .SetOnInsert(x => x.UpdatedAtUtc, now)
                    .SetOnInsert(x => x.LeaseVersion, 0L)
                    .SetOnInsert(x => x.AttemptCount, 0);

                try
                {
                    var result = await _jobs.UpdateOneAsync(
                        JF.Eq(x => x.Id, jobId),
                        update,
                        new UpdateOptions
                        {
                            IsUpsert = true
                        },
                        cancellationToken);

                    if (result.UpsertedId is not null)
                    {
                        created++;
                    }
                }
                catch (MongoWriteException ex)
                    when (ex.WriteError?.Category ==
                          ServerErrorCategory.DuplicateKey)
                {
                    // Otro worker registro el mismo trabajo.
                }

                if (created >= batchSize)
                {
                    return created;
                }
            }
        }

        return created;
    }

    public async Task<AccountDeletionJob?> TryClaimNextAsync(
        string workerId,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workerId);

        if (leaseDuration < TimeSpan.FromSeconds(1) ||
            leaseDuration > TimeSpan.FromHours(1))
        {
            throw new ArgumentOutOfRangeException(
                nameof(leaseDuration));
        }

        var now = await ServerNowAsync(cancellationToken);

        var pending = JF.Eq(
            x => x.Status,
            AccountDeletionJobStates.Pending);

        var retry = JF.And(
            JF.Eq(
                x => x.Status,
                AccountDeletionJobStates.RetryPending),
            JF.Ne(x => x.NextAttemptAtUtc, null),
            CompareServerClock<AccountDeletionJob>(
                nameof(AccountDeletionJob.NextAttemptAtUtc),
                "$lte"));

        var abandoned = JF.And(
            JF.Eq(
                x => x.Status,
                AccountDeletionJobStates.Processing),
            JF.Ne(x => x.LeaseExpiresAtUtc, null),
            CompareServerClock<AccountDeletionJob>(
                nameof(AccountDeletionJob.LeaseExpiresAtUtc),
                "$lte"));

        // Bloquear trabajos agotados que ya no tienen un lease vigente.
        var exhaustedFilter = JF.And(
            JF.Or(pending, retry, abandoned),
            JF.Gte(x => x.AttemptCount, MaxExecutionAttempts));

        await _jobs.UpdateManyAsync(
            exhaustedFilter,
            JU.Set(
                    x => x.Status,
                    AccountDeletionJobStates.Blocked)
                .Set(x => x.LastErrorCode, "attempts_exhausted")
                .Unset(x => x.LeaseOwner)
                .Unset(x => x.LeaseExpiresAtUtc)
                .Unset(x => x.NextAttemptAtUtc)
                .CurrentDate(x => x.UpdatedAtUtc),
            cancellationToken: cancellationToken);

        var filter = JF.And(
            JF.Or(pending, retry, abandoned),
            JF.Lt(x => x.AttemptCount, MaxExecutionAttempts),
            CompareServerClock<AccountDeletionJob>(
                nameof(AccountDeletionJob.DeleteAfterUtc),
                "$lte"));

        var update = JU
            .Set(
                x => x.Status,
                AccountDeletionJobStates.Processing)
            .Set(x => x.LeaseOwner, workerId)
            .Set(
                x => x.LeaseExpiresAtUtc,
                now.Add(leaseDuration))
            .Unset(x => x.NextAttemptAtUtc)
            .Inc(x => x.LeaseVersion, 1L)
            .Inc(x => x.AttemptCount, 1)
            .CurrentDate(x => x.UpdatedAtUtc);

        return await _jobs.FindOneAndUpdateAsync(
            filter,
            update,
            new FindOneAndUpdateOptions<AccountDeletionJob>
            {
                ReturnDocument = ReturnDocument.After,
                Sort = Builders<AccountDeletionJob>.Sort
                    .Ascending(x => x.DeleteAfterUtc)
            },
            cancellationToken);
    }

    private static FilterDefinition<AccountDeletionJob> Lease(
        string jobId,
        string workerId,
        long leaseVersion)
    {
        return JF.And(
            JF.Eq(x => x.Id, jobId),
            JF.Eq(
                x => x.Status,
                AccountDeletionJobStates.Processing),
            JF.Eq(x => x.LeaseOwner, workerId),
            JF.Eq(x => x.LeaseVersion, leaseVersion),
            JF.Ne(x => x.LeaseExpiresAtUtc, null),
            CompareServerClock<AccountDeletionJob>(
                nameof(AccountDeletionJob.LeaseExpiresAtUtc),
                "$gt"));
    }

    private static FilterDefinition<User> TargetAccount(
        AccountDeletionJob job,
        string status)
    {
        return UF.And(
            UF.Eq(x => x.Id, job.UserId),
            UF.Eq(x => x.Status, status),
            UF.Eq(
                x => x.SessionVersion,
                job.ExpectedSessionVersion),
            UF.Eq(
                x => x.DeletionRequestedAtUtc,
                job.RequestedAtUtc),
            UF.Eq(
                x => x.DeleteAfterUtc,
                job.DeleteAfterUtc),
            CompareServerClock<User>(
                nameof(User.DeleteAfterUtc),
                "$lte"));
    }

    private async Task<bool> TransactionAsync(
        Func<IClientSessionHandle,
            CancellationToken, Task<bool>> operation,
        CancellationToken ct)
    {
        using var session =
            await _database.Client.StartSessionAsync(
                cancellationToken: ct);

        return await session.WithTransactionAsync(
            operation,
            new TransactionOptions(
                readConcern: ReadConcern.Snapshot,
                readPreference: ReadPreference.Primary,
                writeConcern: WriteConcern.WMajority),
            ct);
    }

    private async Task SaveJobAsync(
        IClientSessionHandle session,
        FilterDefinition<AccountDeletionJob> lease,
        AccountDeletionJob job,
        UpdateDefinition<AccountDeletionJob> update,
        CancellationToken ct)
    {
        var filter = JF.And(
            lease,
            JF.Eq(x => x.Checkpoint, job.Checkpoint));

        var result = await _jobs.UpdateOneAsync(
            session,
            filter,
            JU.Combine(
                update,
                JU.CurrentDate(x => x.UpdatedAtUtc)),
            cancellationToken: ct);

        if (result.MatchedCount != 1)
        {
            // La transaccion completa debe abortarse.
            throw new InvalidOperationException(
                "Deletion job ownership was lost.");
        }
    }

    private async Task<bool> StopJobAsync(
        IClientSessionHandle session,
        FilterDefinition<AccountDeletionJob> lease,
        AccountDeletionJob job,
        string status,
        string errorCode,
        CancellationToken ct)
    {
        await SaveJobAsync(
            session,
            lease,
            job,
            JU.Set(x => x.Status, status)
                .Set(x => x.LastErrorCode, errorCode)
                .Unset(x => x.LeaseOwner)
                .Unset(x => x.LeaseExpiresAtUtc),
            ct);

        return false;
    }

    public Task<bool> TryBeginAccountDeletionAsync(
        string jobId,
        string workerId,
        long leaseVersion,
        CancellationToken cancellationToken = default)
    {
        var lease = Lease(jobId, workerId, leaseVersion);

        return TransactionAsync(async (session, ct) =>
        {
            var job = await _jobs.Find(session, lease)
                .FirstOrDefaultAsync(ct);

            if (job is null)
            {
                return false;
            }

            if (job.Checkpoint ==
                    AccountDeletionCheckpoints.AccountClaimed ||
                job.Checkpoint ==
                    AccountDeletionCheckpoints.AccountRemoved)
            {
                return true;
            }

            if (job.Checkpoint !=
                AccountDeletionCheckpoints.NotStarted)
            {
                return false;
            }

            if (!ObjectId.TryParse(job.UserId, out _) ||
                job.ExpectedSessionVersion < 1 ||
                job.DeleteAfterUtc - job.RequestedAtUtc
                    != TimeSpan.FromDays(7) ||
                job.Id != job.UserId + ":" +
                    job.ExpectedSessionVersion.ToString(
                        CultureInfo.InvariantCulture))
            {
                return await StopJobAsync(
                    session,
                    lease,
                    job,
                    AccountDeletionJobStates.Blocked,
                    "invalid_job_identity",
                    ct);
            }

            var result = await _users.UpdateOneAsync(
                session,
                TargetAccount(job, "pending_deletion"),
                UU.Set(x => x.Status, "deleting")
                    .CurrentDate(x => x.UpdatedAt),
                cancellationToken: ct);

            if (result.MatchedCount != 1)
            {
                var current = await _users.Find(
                        session,
                        UF.Eq(x => x.Id, job.UserId))
                    .FirstOrDefaultAsync(ct);

                var obsolete = current is not null &&
                    (current.Status == "active" ||
                     (current.Status == "pending_deletion" &&
                      current.SessionVersion !=
                        job.ExpectedSessionVersion));

                return await StopJobAsync(
                    session,
                    lease,
                    job,
                    obsolete
                        ? AccountDeletionJobStates.Cancelled
                        : AccountDeletionJobStates.Blocked,
                    obsolete
                        ? "obsolete_deletion"
                        : "account_precondition_failed",
                    ct);
            }

            await SaveJobAsync(
                session,
                lease,
                job,
                JU.Set(
                        x => x.Checkpoint,
                        AccountDeletionCheckpoints.AccountClaimed)
                    .CurrentDate(x => x.StartedAtUtc),
                ct);

            return true;

        }, cancellationToken);
    }

    public Task<bool> TryDeleteClaimedAccountAsync(
        string jobId,
        string workerId,
        long leaseVersion,
        CancellationToken cancellationToken = default)
    {
        var lease = Lease(jobId, workerId, leaseVersion);

        return TransactionAsync(async (session, ct) =>
        {
            var job = await _jobs.Find(session, lease)
                .FirstOrDefaultAsync(ct);

            if (job is null)
            {
                return false;
            }

            if (job.Checkpoint ==
                AccountDeletionCheckpoints.AccountRemoved)
            {
                return true;
            }

            if (job.Checkpoint !=
                AccountDeletionCheckpoints.AccountClaimed)
            {
                return false;
            }

            // Unicamente el documento del titular.
            var result = await _users.DeleteOneAsync(
                session,
                TargetAccount(job, "deleting"),
                cancellationToken: ct);

            if (result.DeletedCount != 1)
            {
                return await StopJobAsync(
                    session,
                    lease,
                    job,
                    AccountDeletionJobStates.Blocked,
                    "delete_precondition_failed",
                    ct);
            }

            // El borrado y el checkpoint se confirman juntos.
            await SaveJobAsync(
                session,
                lease,
                job,
                JU.Set(
                    x => x.Checkpoint,
                    AccountDeletionCheckpoints.AccountRemoved),
                ct);

            return true;

        }, cancellationToken);
    }

    public Task<bool> TryCompleteAsync(
        string jobId,
        string workerId,
        long leaseVersion,
        CancellationToken cancellationToken = default)
    {
        var lease = Lease(jobId, workerId, leaseVersion);

        return TransactionAsync(async (session, ct) =>
        {
            var job = await _jobs.Find(session, lease)
                .FirstOrDefaultAsync(ct);

            if (job is null ||
                job.Checkpoint !=
                    AccountDeletionCheckpoints.AccountRemoved)
            {
                return false;
            }

            var existing = await _users.Find(
                    session,
                    UF.Eq(x => x.Id, job.UserId))
                .FirstOrDefaultAsync(ct);

            if (existing is not null)
            {
                return await StopJobAsync(
                    session,
                    lease,
                    job,
                    AccountDeletionJobStates.Blocked,
                    "account_still_exists",
                    ct);
            }

            await SaveJobAsync(
                session,
                lease,
                job,
                JU.Set(
                        x => x.Status,
                        AccountDeletionJobStates.Completed)
                    .Set(
                        x => x.Checkpoint,
                        AccountDeletionCheckpoints.Verified)
                    .CurrentDate(x => x.CompletedAtUtc)
                    .Unset(x => x.LeaseOwner)
                    .Unset(x => x.LeaseExpiresAtUtc)
                    .Unset(x => x.NextAttemptAtUtc)
                    .Unset(x => x.LastErrorCode),
                ct);

            return true;

        }, cancellationToken);
    }

    public async Task<bool> TryScheduleRetryAsync(
        string jobId,
        string workerId,
        long leaseVersion,
        string errorCode,
        TimeSpan retryDelay,
        int maxAttempts,
        CancellationToken cancellationToken = default)
    {
        if (maxAttempts < 1 ||
            retryDelay < TimeSpan.Zero ||
            retryDelay > TimeSpan.FromDays(1))
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxAttempts));
        }

        var lease = Lease(jobId, workerId, leaseVersion);

        var job = await _jobs.Find(lease)
            .FirstOrDefaultAsync(cancellationToken);

        if (job is null)
        {
            return false;
        }

        var now = await ServerNowAsync(cancellationToken);

        var exhausted = job.AttemptCount >= maxAttempts;

        // No persistir mensajes de excepciones arbitrarios.
        var safeCode = errorCode switch
        {
            "mongo_transient" => "mongo_transient",
            "lease_lost" => "lease_lost",
            "unexpected_error" => "unexpected_error",
            _ => "execution_error"
        };

        var update = JU
            .Set(
                x => x.Status,
                exhausted
                    ? AccountDeletionJobStates.Blocked
                    : AccountDeletionJobStates.RetryPending)
            .Set(x => x.LastErrorCode, safeCode)
            .Set(
                x => x.NextAttemptAtUtc,
                exhausted
                    ? (DateTime?)null
                    : now.Add(retryDelay))
            .Unset(x => x.LeaseOwner)
            .Unset(x => x.LeaseExpiresAtUtc)
            .CurrentDate(x => x.UpdatedAtUtc);

        var result = await _jobs.UpdateOneAsync(
            JF.And(
                lease,
                JF.Eq(
                    x => x.AttemptCount,
                    job.AttemptCount)),
            update,
            cancellationToken: cancellationToken);

        return result.MatchedCount == 1;
    }
}
