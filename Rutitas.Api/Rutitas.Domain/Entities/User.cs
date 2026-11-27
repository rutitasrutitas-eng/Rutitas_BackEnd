
namespace Rutitas.Domain.Entities;

public class User
{
    public string Id { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string Role { get; set; } = "user";

    public string Status { get; set; } = "active";

    // Version actual de las sesiones del usuario.
    // Incrementar atomicamente al solicitar o cancelar
    // la eliminacion.
    public long SessionVersion { get; set; } = 1;

    // Fechas UTC de la eliminacion programada.
    public DateTime? DeletionRequestedAtUtc { get; set; }

    public DateTime? DeleteAfterUtc { get; set; }


    public string? RecoveryTokenHash { get; set; }
    public DateTime? RecoveryTokenExpiresAtUtc { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }
}
