using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Rutitas.Api.Controllers;
using Xunit;

namespace Rutitas.UnitTests.Endpoints;

// Comprobaciones unitarias adicionales del contrato de acciones de cada endpoint.
// Las reglas funcionales de los 7 endpoints ya se prueban en ControllerUnitTests.cs.
public class EndpointUnitCoverageTests
{
    [Theory]
    [InlineData(typeof(AuthController),"Register","register")]
    [InlineData(typeof(AuthController),"Login","login")]
    [InlineData(typeof(AuthController),"Logout","logout")]
    public void UT_AuthEndpoint_ExposesExpectedPostRoute(Type controller,string action,string expectedRoute)
    {
        var method=controller.GetMethod(action)!;
        var route=method.GetCustomAttribute<HttpPostAttribute>();
        Assert.NotNull(route);Assert.Equal(expectedRoute,route.Template);
    }
    [Theory]
    [InlineData("GetMe", "me")]
    [InlineData("UpdateMe", "me")]
    public void UT_ProfileEndpoint_ExposesExpectedRoute(string action,string route)
    {
        var method=typeof(UsersController).GetMethod(action)!;
        var attribute=action=="GetMe"?(HttpMethodAttribute?)method.GetCustomAttribute<HttpGetAttribute>():method.GetCustomAttribute<HttpPatchAttribute>();
        Assert.NotNull(attribute);Assert.Equal(route,attribute.Template);
    }
    [Fact] public void UT_ListRoutes_UsesGet()
    {
        Assert.NotNull(typeof(RoutesController).GetMethod("GetRoutes")!.GetCustomAttribute<HttpGetAttribute>());
    }
    [Fact] public void UT_RouteDetail_UsesIdGet()
    {
        Assert.Equal("{id}",typeof(RoutesController).GetMethod("GetRouteById")!.GetCustomAttribute<HttpGetAttribute>()!.Template);
    }
}
