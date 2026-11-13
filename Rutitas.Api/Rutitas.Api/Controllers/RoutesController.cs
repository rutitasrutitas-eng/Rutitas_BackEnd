
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MongoDB.Bson;
using Rutitas.Application.Interfaces;

// Evita la confusion con Microsoft.AspNetCore.Routing.Route
using DomainRoute = Rutitas.Domain.Entities.Route;
using RouteLocation = Rutitas.Domain.Entities.RouteLocation;

namespace Rutitas.Api.Controllers;

[ApiController]
[Route("api/v1/routes")]
[Authorize]
public sealed class RoutesController : ControllerBase
{
    private readonly IRouteRepository _routes;

    public RoutesController(IRouteRepository routes)
    {
        _routes = routes;
    }

    // ========================================
    // ENDPOINT 6: LISTAR RUTAS
    // GET /api/v1/routes
    // ========================================

    [HttpGet]
    [EnableRateLimiting("routes")]
    public async Task<IActionResult> GetRoutes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? q = null,
        CancellationToken cancellationToken = default)
    {
        var errors = new Dictionary<string, string[]>();

        if (page < 1)
        {
            errors["page"] =
                ["Page must be at least 1."];
        }

        if (pageSize < 1 || pageSize > 100)
        {
            errors["pageSize"] =
                ["Page size must be between 1 and 100."];
        }

        if (q is not null &&
            (string.IsNullOrWhiteSpace(q) ||
             q.Length > 100))
        {
            errors["q"] =
                ["Search must contain 1 to 100 characters."];
        }

        if (page >= 1 &&
            pageSize >= 1 &&
            (long)(page - 1) * pageSize > int.MaxValue)
        {
            errors["page"] =
                ["Requested page is outside the supported range."];
        }

        if (errors.Count > 0)
        {
            return BadRequest(
                new ValidationProblemDetails(errors)
                {
                    Title = "Invalid route query.",
                    Status = 400
                });
        }

        var result = await _routes.GetPagedAsync(
            page,
            pageSize,
            q,
            cancellationToken);

        // Convertir entidades a respuestas publicas.
        RouteSummaryResponse[] items = result.Items
            .Select(route => ToSummary(route))
            .ToArray();

        var totalPages = (long)Math.Ceiling(
            result.Total / (double)pageSize);

        var response = new PagedRoutesResponse(
            Items: items,
            Page: page,
            PageSize: pageSize,
            Total: result.Total,
            TotalPages: totalPages);

        return Ok(response);
    }

    // ========================================
    // ENDPOINT 7: DETALLE DE RUTA
    // GET /api/v1/routes/{id}
    // ========================================

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRouteById(
        string id,
        CancellationToken cancellationToken)
    {
        if (!Regex.IsMatch(
                id,
                "^[a-fA-F0-9]{24}$") ||
            !ObjectId.TryParse(id, out _))
        {
            var errors = new Dictionary<string, string[]>
            {
                ["id"] =
                [
                    "Route ID must be a 24-character hexadecimal ObjectId."
                ]
            };

            return BadRequest(
                new ValidationProblemDetails(errors)
                {
                    Title = "Invalid route ID.",
                    Status = 400
                });
        }

        DomainRoute? route =
            await _routes.GetVisibleByIdAsync(
                id,
                cancellationToken);

        if (route is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Route not found.",
                Status = 404
            });
        }

        return Ok(ToDetail(route));
    }

    // ========================================
    // CONVERTIR RUTA A RESUMEN
    // ========================================

    private static RouteSummaryResponse ToSummary(
        DomainRoute route)
    {
        return new RouteSummaryResponse(
            Id: route.Id,
            Code: route.Code,
            Name: route.Name,
            Origin: ToLocation(route.Origin),
            Destination: ToLocation(route.Destination),
            DistanceKm: route.DistanceKm,
            EstimatedDurationMin:
                route.EstimatedDurationMin);
    }

    // ========================================
    // CONVERTIR RUTA A DETALLE
    // ========================================

    private static RouteDetailResponse ToDetail(
        DomainRoute route)
    {
        RouteStopResponse[] stops = route.Stops
            .OrderBy(stop => stop.Order)
            .Select(stop => new RouteStopResponse(
                Order: stop.Order,
                Name: stop.Name,
                Lat: stop.Lat,
                Lng: stop.Lng))
            .ToArray();

        return new RouteDetailResponse(
            Id: route.Id,
            Code: route.Code,
            Name: route.Name,
            Description: route.Description,
            Origin: ToLocation(route.Origin),
            Destination: ToLocation(route.Destination),
            Stops: stops,
            DistanceKm: route.DistanceKm,
            EstimatedDurationMin:
                route.EstimatedDurationMin);
    }

    // ========================================
    // CONVERTIR UBICACION
    // ========================================

    private static RouteLocationResponse ToLocation(
        RouteLocation location)
    {
        return new RouteLocationResponse(
            Name: location.Name,
            Lat: location.Lat,
            Lng: location.Lng);
    }
}

// ========================================
// RESPUESTAS DEL CONTRATO OPENAPI
// ========================================

public sealed record RouteLocationResponse(
    string Name,
    double Lat,
    double Lng);

public sealed record RouteStopResponse(
    int Order,
    string Name,
    double Lat,
    double Lng);

public sealed record RouteSummaryResponse(
    string Id,
    string Code,
    string Name,
    RouteLocationResponse Origin,
    RouteLocationResponse Destination,
    double DistanceKm,
    int EstimatedDurationMin);

public sealed record RouteDetailResponse(
    string Id,
    string Code,
    string Name,
    string Description,
    RouteLocationResponse Origin,
    RouteLocationResponse Destination,
    RouteStopResponse[] Stops,
    double DistanceKm,
    int EstimatedDurationMin);

public sealed record PagedRoutesResponse(
    RouteSummaryResponse[] Items,
    int Page,
    int PageSize,
    long Total,
    long TotalPages);
