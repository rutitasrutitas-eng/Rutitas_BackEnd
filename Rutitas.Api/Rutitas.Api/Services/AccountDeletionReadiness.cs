using Microsoft.Extensions.Hosting;

namespace Rutitas.Api.Services;

public interface IAccountDeletionAdmission
{
    bool CanSchedule { get; }
}

public sealed class AccountDeletionReadiness(
    IHostEnvironment environment,
    TimeProvider timeProvider)
    : IAccountDeletionAdmission
{
    private readonly object _sync = new();

    private long? _lastReadyTimestamp;
    private bool _stopped;

    private static readonly TimeSpan MaximumAge =
        TimeSpan.FromSeconds(60);

    public bool CanSchedule
    {
        get
        {
            lock (_sync)
            {
                if (_stopped ||
                    !environment.IsEnvironment("Testing") ||
                    _lastReadyTimestamp is not long timestamp)
                {
                    return false;
                }

                return timeProvider.GetElapsedTime(timestamp)
                    < MaximumAge;
            }
        }
    }

    internal void MarkReady()
    {
        lock (_sync)
        {
            if (!_stopped)
            {
                _lastReadyTimestamp =
                    timeProvider.GetTimestamp();
            }
        }
    }

    internal void MarkUnavailable()
    {
        lock (_sync)
        {
            _lastReadyTimestamp = null;
        }
    }

    internal void MarkStopped()
    {
        lock (_sync)
        {
            _stopped = true;
            _lastReadyTimestamp = null;
        }
    }
}
