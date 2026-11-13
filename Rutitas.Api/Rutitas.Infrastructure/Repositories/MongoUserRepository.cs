
using MongoDB.Bson;
using MongoDB.Driver;
using Rutitas.Application.Exceptions;
using Rutitas.Application.Interfaces;
using Rutitas.Domain.Entities;

namespace Rutitas.Infrastructure.Repositories;

public class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public MongoUserRepository(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("users");
    }

    // ========================================
    // 1. BUSCAR USUARIO POR ID
    // ========================================

    public async Task<User?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return null;
        }

        return await _users
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    // ========================================
    // 2. BUSCAR USUARIO POR EMAIL
    // ========================================

    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await _users
            .Find(x => x.Email == normalizedEmail)
            .FirstOrDefaultAsync(cancellationToken);
    }

    // ========================================
    // 3. CREAR USUARIO
    // ========================================

    public async Task CreateAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        user.Id = ObjectId.GenerateNewId().ToString();

        user.Email = user.Email.Trim().ToLowerInvariant();

        try
        {
            await _users.InsertOneAsync(
                user,
                cancellationToken: cancellationToken);
        }
        catch (MongoWriteException ex)
            when (ex.WriteError?.Category ==
                  ServerErrorCategory.DuplicateKey)
        {
            throw new DuplicateEmailException();
        }
    }

    // ========================================
    // 4. ACTUALIZAR USUARIO COMPLETO
    // Uso interno controlado
    // ========================================

    public async Task UpdateAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(user.Id, out _))
        {
            throw new ArgumentException(
                "Invalid user ID.", nameof(user));
        }

        user.UpdatedAt = DateTime.UtcNow;

        await _users.ReplaceOneAsync(
            x => x.Id == user.Id,
            user,
            cancellationToken: cancellationToken);
    }

    // ========================================
    // 5. ACTUALIZAR PERFIL PARCIALMENTE
    // PATCH /api/v1/users/me
    // ========================================

    public async Task<User?> UpdateProfileAsync(
        string userId,
        string? displayName,
        string? phone,
        bool updateDisplayName,
        bool updatePhone,
        CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out _))
        {
            return null;
        }

        // No permitir actualizaciones vacias.
        if (!updateDisplayName && !updatePhone)
        {
            throw new ArgumentException(
                "At least one field must be provided.");
        }

        var updates = new List<UpdateDefinition<User>>();

        // Actualizar nombre solamente si fue enviado.
        if (updateDisplayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException(
                    "Display name cannot be empty.");
            }

            updates.Add(
                Builders<User>.Update.Set(
                    x => x.DisplayName,
                    displayName.Trim()));
        }

        // Permitir eliminar el telefono con null.
        if (updatePhone)
        {
            updates.Add(
                Builders<User>.Update.Set(
                    x => x.Phone,
                    phone));
        }

        // Actualizar fecha de modificacion.
        updates.Add(
            Builders<User>.Update.Set(
                x => x.UpdatedAt,
                DateTime.UtcNow));

        var updateDefinition =
            Builders<User>.Update.Combine(updates);

        var options = new FindOneAndUpdateOptions<User>
        {
            ReturnDocument = ReturnDocument.After
        };

        // No actualizar usuarios inactivos.
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(x => x.Id, userId),
            Builders<User>.Filter.Eq(x => x.Status, "active"));

        return await _users.FindOneAndUpdateAsync(
            filter,
            updateDefinition,
            options,
            cancellationToken);
    }
}
