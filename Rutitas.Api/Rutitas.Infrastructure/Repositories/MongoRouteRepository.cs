
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Rutitas.Application.Interfaces;
using Rutitas.Domain.Entities;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Rutitas.Infrastructure.Repositories;

public sealed class MongoRouteRepository : IRouteRepository
{
    private readonly IMongoCollection<Route> _routes;


    public MongoRouteRepository(IMongoDatabase database)
    {
        _routes = database.GetCollection<Route>("routes");
    }


    // ========================================
    // ENDPOINT 6: LISTADO DE RUTAS
    // ========================================

    public async Task<PagedRouteResult> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var builder = Builders<Route>.Filter;

        // Solamente rutas disponibles al publico.
        var filter = builder.And(
            builder.Eq(x => x.Status, "active"),
            builder.Eq(x => x.Visibility, "public"));

        if (!string.IsNullOrWhiteSpace(search))
        {
            // Escapar la busqueda para tratarla
            // como texto y no como una expresion
            // regular introducida por el usuario.
            var pattern = Regex.Escape(search.Trim());
            var regex = new BsonRegularExpression(pattern, "i");

            var searchFilter = builder.Or(
                builder.Regex(x => x.Code, regex),
                builder.Regex(x => x.Name, regex),
                builder.Regex(x => x.Origin.Name, regex),
                builder.Regex(x => x.Destination.Name, regex));

            filter = builder.And(filter, searchFilter);
        }

        var total = await _routes.CountDocumentsAsync(
            filter,
            cancellationToken: cancellationToken);

        var skip = checked((page - 1) * pageSize);

        var sort = Builders<Route>.Sort
            .Ascending(x => x.Code)
            .Ascending(x => x.Id);

        var items = await _routes
            .Find(filter)
            .Sort(sort)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedRouteResult(items, total);
    }

    // ========================================
    // ENDPOINT 7: DETALLE DE RUTA
    // ========================================

    public async Task<Route?> GetVisibleByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return null;
        }

        var builder = Builders<Route>.Filter;

        var filter = builder.And(
            builder.Eq(x => x.Id, id.ToLowerInvariant()),
            builder.Eq(x => x.Status, "active"),
            builder.Eq(x => x.Visibility, "public"));

        return await _routes
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
