using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rutitas.Application.Interfaces;
using Rutitas.Infrastructure.Repositories;

namespace Rutitas.Api.Services;

public static class AccountDeletionWorkerRegistration
{
    public static void AddAccountDeletionExecution(
        WebApplicationBuilder builder)
    {
        var section = builder.Configuration.GetSection(
            AccountDeletionWorkerOptions.SectionName);

        var enabled = section.GetValue<bool>("Enabled");

        if (!enabled)
        {
            // No registrar repositorio ni servicio destructivo.
            return;
        }

        if (!builder.Environment.IsEnvironment("Testing"))
        {
            throw new InvalidOperationException(
                "Account deletion worker is restricted to Testing.");
        }

        builder.Services
            .AddOptions<AccountDeletionWorkerOptions>()
            .Bind(section)
            .Validate(
                options => options.IsValid(),
                "Invalid account deletion worker configuration.")
            .ValidateOnStart();

        builder.Services.AddScoped<
            MongoAccountDeletionExecutionRepository>();

        builder.Services.AddScoped<
            IAccountDeletionExecutionRepository>(
                sp => sp.GetRequiredService<
                    MongoAccountDeletionExecutionRepository>());

        builder.Services.AddHostedService<
            AccountDeletionWorker>();
    }
}
