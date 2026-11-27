using Rutitas.Domain.Entities;

namespace Rutitas.Application.Interfaces;

public interface IAccountDeletionRepository
{
    Task<User?> ScheduleDeletionAsync(
        string userId,
        long expectedSessionVersion,
        DateTime requestedAtUtc,
        CancellationToken cancellationToken = default);
}
