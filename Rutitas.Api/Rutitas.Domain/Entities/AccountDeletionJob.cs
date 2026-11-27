namespace Rutitas.Domain.Entities;

/// <summary>
/// Registro persistente del proceso de eliminacion.
/// No almacena credenciales ni datos de perfil.
/// </summary>
public sealed class AccountDeletionJob
{
    public string Id { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public long ExpectedSessionVersion { get; set; }

    public string Status { get; set; } = "pending";

    public string Checkpoint { get; set; } = "not_started";

    public DateTime RequestedAtUtc { get; set; }

    public DateTime DeleteAfterUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public DateTime? StartedAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    public DateTime? NextAttemptAtUtc { get; set; }

    public string? LeaseOwner { get; set; }

    public DateTime? LeaseExpiresAtUtc { get; set; }

    public long LeaseVersion { get; set; }

    public int AttemptCount { get; set; }

    public string? LastErrorCode { get; set; }
}

public static class AccountDeletionJobStates
{
    public const string Pending = "pending";
    public const string Processing = "processing";
    public const string RetryPending = "retry_pending";
    public const string Completed = "completed";
    public const string Cancelled = "cancelled";
    public const string Blocked = "blocked";
}

public static class AccountDeletionCheckpoints
{
    public const string NotStarted = "not_started";
    public const string AccountClaimed = "account_claimed";
    public const string AccountRemoved = "account_removed";
    public const string Verified = "verified";
}
