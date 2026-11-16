
namespace Rutitas.Application.Interfaces;

public interface ITokenRevocationRepository
{
    // Guardar el identificador del JWT revocado
    Task RevokeAsync(
        string jti,
        DateTime expiresAt,
        CancellationToken cancellationToken = default);

    // Comprobar si el JWT ya fue revocado
    Task<bool> IsRevokedAsync(
        string jti,
        CancellationToken cancellationToken = default);
}
