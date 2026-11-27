namespace Rutitas.Application.Interfaces;

public interface IRecoveryTokenRepository
{
    Task<Rutitas.Domain.Entities.User?> GetStatusAsync(
        string tokenHash,
        DateTime nowUtc,
        CancellationToken cancellationToken = default);

    Task<Rutitas.Domain.Entities.User?> CancelAsync(
        string tokenHash,
        DateTime nowUtc,
        CancellationToken cancellationToken = default);
    Task<bool> IssueAsync(
        string userId,
        string tokenHash,
        DateTime issuedAtUtc,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken = default);
}