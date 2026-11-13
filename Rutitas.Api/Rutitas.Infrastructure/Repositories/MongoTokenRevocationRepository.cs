
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Rutitas.Application.Interfaces;

namespace Rutitas.Infrastructure.Repositories;

public sealed class MongoTokenRevocationRepository
    : ITokenRevocationRepository
{
    private readonly IMongoCollection<RevokedToken> _tokens;

    public MongoTokenRevocationRepository(
        IMongoDatabase database)
    {
        _tokens = database.GetCollection<RevokedToken>(
            "revoked_tokens");
    }

    // ========================================
    // REVOCAR TOKEN JWT
    // ========================================

    public async Task RevokeAsync(
        string jti,
        DateTime expiresAt,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(jti))
        {
            throw new ArgumentException(
                "Token JTI is required.",
                nameof(jti));
        }

        if (expiresAt.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException(
                "Expiration must be UTC.",
                nameof(expiresAt));
        }

        // El identificador JTI funciona como clave unica.
        // Una revocacion repetida no crea duplicados.
        var filter = Builders<RevokedToken>.Filter
            .Eq(x => x.Jti, jti);

        var update = Builders<RevokedToken>.Update
            .SetOnInsert(x => x.ExpiresAt, expiresAt)
            .SetOnInsert(x => x.RevokedAt, DateTime.UtcNow);

        await _tokens.UpdateOneAsync(
            filter,
            update,
            new UpdateOptions { IsUpsert = true },
            cancellationToken);
    }

    // ========================================
    // COMPROBAR REVOCACION
    // ========================================

    public async Task<bool> IsRevokedAsync(
        string jti,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(jti))
        {
            return true;
        }

        var filter = Builders<RevokedToken>.Filter
            .Eq(x => x.Jti, jti);

        return await _tokens
            .Find(filter)
            .AnyAsync(cancellationToken);
    }
}

// ========================================
// DOCUMENTO MONGODB
// ========================================

public sealed class RevokedToken
{
    [BsonId]
    public string Jti { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime RevokedAt { get; set; }
}
