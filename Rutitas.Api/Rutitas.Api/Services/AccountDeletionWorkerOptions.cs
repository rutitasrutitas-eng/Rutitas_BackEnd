namespace Rutitas.Api.Services;

public sealed class AccountDeletionWorkerOptions
{
    public const string SectionName = "AccountDeletionWorker";

    // Nunca habilitado implicitamente.
    public bool Enabled { get; set; } = false;

    public int PollIntervalSeconds { get; set; } = 30;

    public int LeaseDurationSeconds { get; set; } = 120;

    public int BatchSize { get; set; } = 20;

    public int RetryBaseSeconds { get; set; } = 10;

    public bool IsValid()
    {
        return PollIntervalSeconds is >= 1 and <= 3600
            && LeaseDurationSeconds is >= 30 and <= 3600
            && BatchSize is >= 1 and <= 100
            && RetryBaseSeconds is >= 1 and <= 300;
    }
}
