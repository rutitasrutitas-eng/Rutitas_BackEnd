
using Rutitas.Domain.Entities;

namespace Rutitas.Application.Interfaces;

public interface IUserRepository
{
    // Buscar usuario por ID
    Task<User?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default);

    // Buscar usuario por correo
    Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    // Crear usuario
    Task CreateAsync(
        User user,
        CancellationToken cancellationToken = default);

    // Actualizar usuario
    Task UpdateAsync(
        User user,
        CancellationToken cancellationToken = default);

    // Actualizar únicamente datos permitidos del perfil
    Task<User?> UpdateProfileAsync(
        string userId,
        string? displayName,
        string? phone,
        bool updateDisplayName,
        bool updatePhone,
        CancellationToken cancellationToken = default);
}
