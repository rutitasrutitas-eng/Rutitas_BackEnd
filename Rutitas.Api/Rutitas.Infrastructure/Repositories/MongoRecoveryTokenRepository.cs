using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Rutitas.Application.Interfaces;
using Rutitas.Domain.Entities;

namespace Rutitas.Infrastructure.Repositories;

public sealed class MongoRecoveryTokenRepository
    : IRecoveryTokenRepository
{
    private readonly IMongoCollection<User> _users;

    public MongoRecoveryTokenRepository(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("users");
    }

    public async Task<User?> GetStatusAsync(
        string tokenHash,
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tokenHash) ||
            nowUtc.Kind != DateTimeKind.Utc)
            return null;

        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(
                x => x.RecoveryTokenHash, tokenHash),
            Builders<User>.Filter.Gt(
                x => x.RecoveryTokenExpiresAtUtc, nowUtc),
            Builders<User>.Filter.In(
                x => x.Status,
                new[] { "pending_deletion", "deleting" }));

        return await _users.Find(filter)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> CancelAsync(
        string tokenHash,
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tokenHash) ||
            nowUtc.Kind != DateTimeKind.Utc)
            return null;

        // Usar los nombres BSON reales definidos por el driver.
        var userMap = BsonClassMap.LookupClassMap(typeof(User));

        var tokenExpiryField = userMap
            .GetMemberMap(nameof(User.RecoveryTokenExpiresAtUtc))
            .ElementName;

        var deletionDeadlineField = userMap
            .GetMemberMap(nameof(User.DeleteAfterUtc))
            .ElementName;

        // MongoDB evalua $$NOW al ejecutar la operacion.
        // No confiamos solo en nowUtc recibido de la API.
        FilterDefinition<User> serverClockFilter =
            new BsonDocument("$expr",
                new BsonDocument("$and", new BsonArray
                {
                    new BsonDocument("$gt", new BsonArray
                    {
                        "$" + tokenExpiryField,
                        "$$NOW"
                    }),
                    new BsonDocument("$gt", new BsonArray
                    {
                        "$" + deletionDeadlineField,
                        "$$NOW"
                    })
                }));
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(
                x => x.RecoveryTokenHash, tokenHash),
            Builders<User>.Filter.Eq(
                x => x.Status, "pending_deletion"),
            Builders<User>.Filter.Gt(
                x => x.RecoveryTokenExpiresAtUtc, nowUtc),
            Builders<User>.Filter.Gt(
                x => x.DeleteAfterUtc, nowUtc),
            serverClockFilter);

        var update = Builders<User>.Update
            .Set(x => x.Status, "active")
            .Set(x => x.UpdatedAt, nowUtc)
            .Inc(x => x.SessionVersion, 1)
            .Unset(x => x.RecoveryTokenHash)
            .Unset(x => x.RecoveryTokenExpiresAtUtc)
            .Unset(x => x.DeletionRequestedAtUtc)
            .Unset(x => x.DeleteAfterUtc);

        return await _users.FindOneAndUpdateAsync(
            filter,
            update,
            new FindOneAndUpdateOptions<User>
            {
                ReturnDocument = ReturnDocument.After
            },
            cancellationToken);
    }
    public async Task<bool> IssueAsync(
        string userId,
        string tokenHash,
        DateTime issuedAtUtc,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out _) ||
            string.IsNullOrWhiteSpace(tokenHash))
            return false;

        if (issuedAtUtc.Kind != DateTimeKind.Utc ||
            expiresAtUtc.Kind != DateTimeKind.Utc ||
            expiresAtUtc <= issuedAtUtc)
            throw new ArgumentException("Invalid UTC interval.");

        // Verificar el vencimiento usando la hora de MongoDB.
        var userMap = BsonClassMap.LookupClassMap(typeof(User));

        var deletionDeadlineField = userMap
            .GetMemberMap(nameof(User.DeleteAfterUtc))
            .ElementName;

        FilterDefinition<User> issueServerClockFilter =
            new BsonDocument("$expr",
                new BsonDocument("$gt", new BsonArray
                {
                    "$" + deletionDeadlineField,
                    "$$NOW"
                }));

        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(x => x.Id, userId),
            Builders<User>.Filter.Eq(
                x => x.Status, "pending_deletion"),
            Builders<User>.Filter.Gt(
                x => x.DeleteAfterUtc, issuedAtUtc),
            issueServerClockFilter);

        var update = Builders<User>.Update
            .Set(x => x.RecoveryTokenHash, tokenHash)
            .Set(x => x.RecoveryTokenExpiresAtUtc, expiresAtUtc);

        var result = await _users.FindOneAndUpdateAsync(
            filter,
            update,
            new FindOneAndUpdateOptions<User>
            {
                ReturnDocument = ReturnDocument.After
            },
            cancellationToken);

        return result is not null;
    }
}