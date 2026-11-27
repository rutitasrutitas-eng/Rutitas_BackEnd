using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Rutitas.Domain.Entities;
using Rutitas.Infrastructure.Repositories;

namespace Rutitas.Api.Services;

public sealed class AccountDeletionWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AccountDeletionWorker> _logger;
    private readonly AccountDeletionWorkerOptions _options;

    private readonly string _workerId =
        Guid.NewGuid().ToString("N");

    public AccountDeletionWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<AccountDeletionWorker> logger,
        IOptions<AccountDeletionWorkerOptions> options)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            return;
        }

        var initialized = false;

        _logger.LogInformation(
            "Account deletion test worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var repository = scope.ServiceProvider
                    .GetRequiredService<
                        MongoAccountDeletionExecutionRepository>();

                if (!initialized)
                {
                    await repository.EnsureIndexesAsync(
                        stoppingToken);

                    initialized = true;
                }

                await repository.DiscoverDueAccountsAsync(
                    _options.BatchSize,
                    stoppingToken);

                for (var i = 0;
                     i < _options.BatchSize;
                     i++)
                {
                    stoppingToken.ThrowIfCancellationRequested();

                    var job = await repository.TryClaimNextAsync(
                        _workerId,
                        TimeSpan.FromSeconds(
                            _options.LeaseDurationSeconds),
                        stoppingToken);

                    if (job is null)
                    {
                        break;
                    }

                    await ProcessJobAsync(
                        repository,
                        job,
                        stoppingToken);
                }
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (MongoException)
            {
                // No registrar mensajes que puedan incluir datos.
                _logger.LogWarning(
                    "Deletion worker database operation failed.");
            }
            catch (Exception)
            {
                _logger.LogError(
                    "Deletion worker cycle failed.");
            }

            try
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(
                        _options.PollIntervalSeconds),
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation(
            "Account deletion test worker stopped.");
    }

    private async Task ProcessJobAsync(
        MongoAccountDeletionExecutionRepository repository,
        AccountDeletionJob job,
        CancellationToken ct)
    {
        try
        {
            var begun = await repository
                .TryBeginAccountDeletionAsync(
                    job.Id,
                    _workerId,
                    job.LeaseVersion,
                    ct);

            if (!begun)
            {
                return;
            }

            var removed = await repository
                .TryDeleteClaimedAccountAsync(
                    job.Id,
                    _workerId,
                    job.LeaseVersion,
                    ct);

            if (!removed)
            {
                return;
            }

            var completed = await repository.TryCompleteAsync(
                job.Id,
                _workerId,
                job.LeaseVersion,
                ct);

            if (completed)
            {
                _logger.LogInformation(
                    "Account deletion job verified.");
            }
        }
        catch (OperationCanceledException)
            when (ct.IsCancellationRequested)
        {
            // El lease permitira recuperar el trabajo.
            throw;
        }
        catch (MongoException)
        {
            await ScheduleRetrySafelyAsync(
                repository,
                job,
                "mongo_transient",
                ct);
        }
        catch (Exception)
        {
            await ScheduleRetrySafelyAsync(
                repository,
                job,
                "unexpected_error",
                ct);
        }
    }

    private async Task ScheduleRetrySafelyAsync(
        MongoAccountDeletionExecutionRepository repository,
        AccountDeletionJob job,
        string errorCode,
        CancellationToken ct)
    {
        var exponent = Math.Clamp(
            job.AttemptCount - 1,
            0,
            5);

        var delaySeconds = Math.Min(
            1800,
            _options.RetryBaseSeconds *
            Math.Pow(2, exponent));

        try
        {
            var scheduled = await repository
                .TryScheduleRetryAsync(
                    job.Id,
                    _workerId,
                    job.LeaseVersion,
                    errorCode,
                    TimeSpan.FromSeconds(delaySeconds),
                    MongoAccountDeletionExecutionRepository
                        .MaxExecutionAttempts,
                    ct);

            if (scheduled)
            {
                _logger.LogWarning(
                    "Deletion job failure recorded. Code: {Code}",
                    errorCode);
            }
        }
        catch (OperationCanceledException)
            when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            // Si no se puede persistir el reintento,
            // el lease vencido permite reconciliar el trabajo.
            _logger.LogWarning(
                "Deletion retry state could not be recorded.");
        }
    }
}
