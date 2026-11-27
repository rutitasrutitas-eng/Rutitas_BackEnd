using Rutitas.Domain.Entities;

namespace Rutitas.Application.Interfaces;

/// <summary>
/// Contrato del proceso durable de eliminacion.
/// Las implementaciones deben verificar estado, propiedad,
/// vencimiento y vigencia del lease de manera atomica.
/// </summary>
public interface IAccountDeletionExecutionRepository
{
    Task<int> DiscoverDueAccountsAsync(
        int batchSize,
        CancellationToken cancellationToken = default);

    Task<AccountDeletionJob?> TryClaimNextAsync(
        string workerId,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken = default);

    Task<bool> TryRenewLeaseAsync(
        string jobId,
        string workerId,
        long leaseVersion,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken = default);

    Task<bool> TryBeginAccountDeletionAsync(
        string jobId,
        string workerId,
        long leaseVersion,
        CancellationToken cancellationToken = default);

    Task<bool> TryDeleteClaimedAccountAsync(
        string jobId,
        string workerId,
        long leaseVersion,
        CancellationToken cancellationToken = default);

    Task<bool> TryCompleteAsync(
        string jobId,
        string workerId,
        long leaseVersion,
        CancellationToken cancellationToken = default);

    Task<bool> TryScheduleRetryAsync(
        string jobId,
        string workerId,
        long leaseVersion,
        string errorCode,
        TimeSpan retryDelay,
        int maxAttempts,
        CancellationToken cancellationToken = default);
}
