using MongoDB.Bson;
using MongoDB.Driver;
using Rutitas.Application.Interfaces;
using Rutitas.Domain.Entities;

namespace Rutitas.Infrastructure.Repositories;

public sealed class MongoAccountDeletionRepository
    : IAccountDeletionRepository
{
    private readonly IMongoCollection<User> _users;

    public MongoAccountDeletionRepository(IMongoDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _users = database.GetCollection<User>("users");
    }

    public async Task<User?> ScheduleDeletionAsync(
        string userId,
        long expectedSessionVersion,
        DateTime requestedAtUtc,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out _) ||
            expectedSessionVersion < 1)
        {
            return null;
        }

        if (requestedAtUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException(
                "Date must be UTC.",
                nameof(requestedAtUtc));
        }

        var deadline = requestedAtUtc.AddDays(7);

        // Solo una cuenta activa con version vigente
        // puede iniciar esta transicion.
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(x => x.Id, userId),
            Builders<User>.Filter.Eq(x => x.Status, "active"),
            Builders<User>.Filter.Eq(
                x => x.SessionVersion,
                expectedSessionVersion));

        // Estado, fechas y version en UNA operacion.
        var update = Builders<User>.Update
            .Set(x => x.Status, "pending_deletion")
            .Set(x => x.DeletionRequestedAtUtc, requestedAtUtc)
            .Set(x => x.DeleteAfterUtc, deadline)
            .Set(x => x.UpdatedAt, requestedAtUtc)
            .Inc(x => x.SessionVersion, 1);

        var options = new FindOneAndUpdateOptions<User>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await _users.FindOneAndUpdateAsync(
            filter,
            update,
            options,
            cancellationToken);
    }
}
