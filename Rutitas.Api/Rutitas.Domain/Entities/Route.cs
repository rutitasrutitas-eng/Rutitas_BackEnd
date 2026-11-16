
namespace Rutitas.Domain.Entities;

// ========================================
// ENTIDAD PRINCIPAL: RUTA
// ========================================

public sealed class Route
{
    public string Id { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public RouteLocation Origin { get; set; } = new();

    public RouteLocation Destination { get; set; } = new();

    public List<RouteStop> Stops { get; set; } = new();

    public double DistanceKm { get; set; }

    public int EstimatedDurationMin { get; set; }

    // Campos internos
    public string Status { get; set; } = "active";

    public string Visibility { get; set; } = "public";

    public DateTime CreatedAt { get; set; } =
        DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } =
        DateTime.UtcNow;
}

// ========================================
// UBICACIONES
// ========================================

public sealed class RouteLocation
{
    public string Name { get; set; } = string.Empty;

    public double Lat { get; set; }

    public double Lng { get; set; }
}

// ========================================
// PARADAS
// ========================================

public sealed class RouteStop
{
    public int Order { get; set; }

    public string Name { get; set; } = string.Empty;

    public double Lat { get; set; }

    public double Lng { get; set; }
}
