using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Rutitas.Api.Services;
using Rutitas.Infrastructure.Repositories;
using Testcontainers.MongoDb;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class AccountDeletionNegativeSafetyTests
{
    [Fact]
    public async Task TransactionCapability_StandaloneIsRejected()
    {
        await using var mongo = new MongoDbBuilder("mongo:7.0")
            .Build();

        using var timeout = new CancellationTokenSource(
            TimeSpan.FromMinutes(2));

        await mongo.StartAsync(timeout.Token);

        var settings = MongoClientSettings
            .FromConnectionString(
                mongo.GetConnectionString());

        settings.DirectConnection = true;

        var client = new MongoClient(settings);

        var database = client.GetDatabase(
            "rutitas_delete001_test_" +
            Guid.NewGuid().ToString("N"));

        var repository =
            new MongoAccountDeletionExecutionRepository(
                database);

        // Las operaciones ordinarias funcionan,
        // pero no se debe aceptar este entorno
        // para la ejecucion transaccional.
        await repository.EnsureIndexesAsync(timeout.Token);

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => repository
                    .EnsureTransactionCapabilityAsync(
                        timeout.Token));

        Assert.Contains(
            "transaction-capable MongoDB",
            exception.Message);
    }

    [Fact]
    public async Task ExpiredStageBudget_DoesNotStartOperation()
    {
        using var provider =
            new ServiceCollection()
                .BuildServiceProvider();

        var worker = new AccountDeletionWorker(
            provider.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<AccountDeletionWorker>.Instance,
            Options.Create(
                new AccountDeletionWorkerOptions
                {
                    Enabled = true,
                    LeaseDurationSeconds = 30
                }));

        using var disposable = worker;

        var method = typeof(AccountDeletionWorker)
            .GetMethod(
                "ExecuteWithinLeaseBudgetAsync",
                BindingFlags.Instance |
                BindingFlags.NonPublic);

        Assert.NotNull(method);

        var invoked = false;

        Func<CancellationToken, Task<bool>> operation =
            token =>
            {
                invoked = true;
                return Task.FromResult(true);
            };

        var expiredTimestamp =
            Stopwatch.GetTimestamp() -
            (Stopwatch.Frequency * 60L);

        var task = Assert.IsAssignableFrom<Task<bool>>(
            method.Invoke(
                worker,
                new object[]
                {
                    operation,
                    expiredTimestamp,
                    CancellationToken.None
                }));

        await Assert.ThrowsAsync<TimeoutException>(
            async () => await task);

        Assert.False(invoked);
    }
}
