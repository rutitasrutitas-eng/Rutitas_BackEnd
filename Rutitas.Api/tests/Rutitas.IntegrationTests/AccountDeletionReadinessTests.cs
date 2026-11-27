using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Rutitas.Api.Services;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class AccountDeletionReadinessTests
{
    private sealed class ManualClock : TimeProvider
    {
        private long _ticks;

        public override long TimestampFrequency =>
            TimeSpan.TicksPerSecond;

        public override long GetTimestamp() => _ticks;

        public void Advance(TimeSpan duration)
        {
            _ticks += duration.Ticks;
        }
    }

    private static AccountDeletionReadiness Create(
        string environment,
        ManualClock clock)
    {
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions
            {
                EnvironmentName = environment,
                Args = Array.Empty<string>()
            });

        return new AccountDeletionReadiness(
            builder.Environment, clock);
    }

    private static void Signal(
        AccountDeletionReadiness readiness,
        string method)
    {
        var target = typeof(AccountDeletionReadiness)
            .GetMethod(
                method,
                BindingFlags.Instance |
                BindingFlags.NonPublic);

        Assert.NotNull(target);

        target.Invoke(readiness, null);
    }

    [Fact]
    public void DefaultState_IsUnavailable()
    {
        var readiness = Create(
            "Testing", new ManualClock());

        Assert.False(readiness.CanSchedule);
    }

    [Theory]
    [InlineData("Production")]
    [InlineData("Development")]
    [InlineData("Staging")]
    public void UnauthorizedEnvironment_RemainsUnavailable(
        string environment)
    {
        var readiness = Create(
            environment, new ManualClock());

        Signal(readiness, "MarkReady");

        Assert.False(readiness.CanSchedule);
    }

    [Fact]
    public void Heartbeat_ExpiresAfter60Seconds()
    {
        var clock = new ManualClock();
        var readiness = Create("Testing", clock);

        Signal(readiness, "MarkReady");

        Assert.True(readiness.CanSchedule);

        clock.Advance(TimeSpan.FromSeconds(59));

        Assert.True(readiness.CanSchedule);

        clock.Advance(TimeSpan.FromSeconds(1));

        Assert.False(readiness.CanSchedule);
    }

    [Fact]
    public void DependencyFailure_BlocksAdmission()
    {
        var readiness = Create(
            "Testing", new ManualClock());

        Signal(readiness, "MarkReady");

        Assert.True(readiness.CanSchedule);

        Signal(readiness, "MarkUnavailable");

        Assert.False(readiness.CanSchedule);
    }

    [Fact]
    public void DependencyRecovery_RestoresAdmission()
    {
        var readiness = Create(
            "Testing", new ManualClock());

        Signal(readiness, "MarkReady");
        Signal(readiness, "MarkUnavailable");

        Assert.False(readiness.CanSchedule);

        Signal(readiness, "MarkReady");

        Assert.True(readiness.CanSchedule);
    }

    [Fact]
    public void Shutdown_CannotBeReopenedByLateHeartbeat()
    {
        var readiness = Create(
            "Testing", new ManualClock());

        Signal(readiness, "MarkReady");

        Assert.True(readiness.CanSchedule);

        Signal(readiness, "MarkStopped");

        Assert.False(readiness.CanSchedule);

        Signal(readiness, "MarkReady");

        Assert.False(readiness.CanSchedule);
    }
}
