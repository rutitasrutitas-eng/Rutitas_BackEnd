using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Rutitas.Api.Services;
using Rutitas.Application.Interfaces;
using Rutitas.Infrastructure.Repositories;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class AccountDeletionWorkerSecurityTests
{
    private static WebApplicationBuilder CreateBuilder(
        string environment)
    {
        return WebApplication.CreateBuilder(
            new WebApplicationOptions
            {
                EnvironmentName = environment,
                Args = Array.Empty<string>()
            });
    }

    [Fact]
    public void DefaultOptions_WorkerDisabled()
    {
        var options = new AccountDeletionWorkerOptions();

        Assert.False(options.Enabled);
        Assert.True(options.IsValid());
    }

    [Fact]
    public void DisabledWorker_DoesNotRegisterDestructiveServices()
    {
        var builder = CreateBuilder("Production");

        builder.Configuration[
            "AccountDeletionWorker:Enabled"] = "false";

        AccountDeletionWorkerRegistration
            .AddAccountDeletionExecution(builder);

        Assert.DoesNotContain(
            builder.Services,
            service => service.ImplementationType ==
                typeof(AccountDeletionWorker));

        Assert.DoesNotContain(
            builder.Services,
            service => service.ServiceType ==
                typeof(IAccountDeletionExecutionRepository));

        Assert.DoesNotContain(
            builder.Services,
            service => service.ServiceType ==
                typeof(MongoAccountDeletionExecutionRepository));
    }

    [Fact]
    public void EnabledWorker_ProductionRejected()
    {
        var builder = CreateBuilder("Production");

        builder.Configuration[
            "AccountDeletionWorker:Enabled"] = "true";

        Assert.Throws<InvalidOperationException>(() =>
            AccountDeletionWorkerRegistration
                .AddAccountDeletionExecution(builder));
    }

    [Fact]
    public void EnabledWorker_DevelopmentRejected()
    {
        var builder = CreateBuilder("Development");

        builder.Configuration[
            "AccountDeletionWorker:Enabled"] = "true";

        Assert.Throws<InvalidOperationException>(() =>
            AccountDeletionWorkerRegistration
                .AddAccountDeletionExecution(builder));
    }

    [Fact]
    public void EnabledWorker_TestingRegistersServices()
    {
        var builder = CreateBuilder("Testing");

        builder.Configuration[
            "AccountDeletionWorker:Enabled"] = "true";

        AccountDeletionWorkerRegistration
            .AddAccountDeletionExecution(builder);

        Assert.Contains(
            builder.Services,
            service =>
                service.ServiceType == typeof(IHostedService)
                && service.ImplementationType ==
                    typeof(AccountDeletionWorker));

        Assert.Contains(
            builder.Services,
            service => service.ServiceType ==
                typeof(IAccountDeletionExecutionRepository));

        Assert.Contains(
            builder.Services,
            service => service.ServiceType ==
                typeof(MongoAccountDeletionExecutionRepository));
    }

    [Fact]
    public void InvalidOptions_AreRejectedByValidation()
    {
        Assert.False(
            new AccountDeletionWorkerOptions
            {
                PollIntervalSeconds = 0
            }.IsValid());

        Assert.False(
            new AccountDeletionWorkerOptions
            {
                LeaseDurationSeconds = 0
            }.IsValid());

        Assert.False(
            new AccountDeletionWorkerOptions
            {
                BatchSize = 1000
            }.IsValid());

        Assert.False(
            new AccountDeletionWorkerOptions
            {
                RetryBaseSeconds = -1
            }.IsValid());

        Assert.True(
            new AccountDeletionWorkerOptions().IsValid());
    }
}
