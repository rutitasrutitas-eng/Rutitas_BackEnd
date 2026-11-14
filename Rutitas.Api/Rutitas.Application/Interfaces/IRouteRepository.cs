
using Rutitas.Domain.Entities;

namespace Rutitas.Application.Interfaces;

public interface IRouteRepository
{
    // Obtener rutas activas y visibles.
    Task<PagedRouteResult> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default);

    // Obtener una ruta visible por su identificador.
    Task<Route?> GetVisibleByIdAsync(
        string id,
        CancellationToken cancellationToken = default);
}

public sealed record PagedRouteResult(
    IReadOnlyList<Route> Items,
    long Total);
