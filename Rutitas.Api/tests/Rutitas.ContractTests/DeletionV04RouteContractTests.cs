using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.RateLimiting;
using Rutitas.Api.Controllers;
using Xunit;

namespace Rutitas.ContractTests;

public sealed class DeletionV04RouteContractTests
{
    [Theory]
    [InlineData(typeof(AccountDeletionController),
        "ScheduleDeletion", "api/v1/users", "me", "DELETE")]
    [InlineData(typeof(DeletionStatusController),
        "GetStatus", "api/v1/users/me/deletion", "", "GET")]
    [InlineData(typeof(DeletionRecoveryController),
        "Recover", "api/v1/auth/deletion", "recovery", "POST")]
    [InlineData(typeof(DeletionCancellationController),
        "Cancel", "api/v1/users/me/deletion/cancel", "", "POST")]
    public void DeletionRoutes_MatchVersion04(
        Type controllerType,
        string methodName,
        string controllerRoute,
        string actionRoute,
        string httpMethod)
    {
        var route = controllerType.GetCustomAttribute<RouteAttribute>();

        Assert.NotNull(route);
        Assert.Equal(controllerRoute, route.Template);

        var method = controllerType.GetMethod(methodName);

        Assert.NotNull(method);

        var http = method.GetCustomAttributes()
            .OfType<HttpMethodAttribute>()
            .Single();

        Assert.Equal(actionRoute, http.Template ?? "");
        Assert.Contains(httpMethod, http.HttpMethods);
    }

    [Fact]
    public void Scheduling_RequiresOrdinaryAuthentication()
    {
        Assert.NotNull(
            typeof(AccountDeletionController)
                .GetCustomAttribute<AuthorizeAttribute>());

        var method = typeof(AccountDeletionController)
            .GetMethod("ScheduleDeletion");

        Assert.NotNull(method);

        Assert.Null(
            method.GetCustomAttribute<AllowAnonymousAttribute>());
    }

    [Fact]
    public void RecoveryEndpoints_UseRestrictedTokenHandling()
    {
        var actions = new[]
        {
            typeof(DeletionStatusController).GetMethod("GetStatus"),
            typeof(DeletionRecoveryController).GetMethod("Recover"),
            typeof(DeletionCancellationController).GetMethod("Cancel")
        };

        foreach (var action in actions)
        {
            Assert.NotNull(action);

            // AllowAnonymous evita la validacion JWT ordinaria.
            // El token restringido se valida dentro del controlador.
            Assert.NotNull(
                action.GetCustomAttribute<AllowAnonymousAttribute>());

            Assert.NotNull(
                action.GetCustomAttribute<EnableRateLimitingAttribute>());
        }
    }
}