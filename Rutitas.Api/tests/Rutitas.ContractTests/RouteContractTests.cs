using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Rutitas.Api.Controllers;
using Xunit;
namespace Rutitas.ContractTests;
public class RouteContractTests
{
    [Fact] public void VersionedRoutes_RemainCompatible()
    {
        Assert.Equal("api/v1/auth",Attribute.GetCustomAttribute(typeof(AuthController),typeof(RouteAttribute)) is RouteAttribute auth ? auth.Template : null);
        Assert.Equal("api/v1/users",Attribute.GetCustomAttribute(typeof(UsersController),typeof(RouteAttribute)) is RouteAttribute users ? users.Template : null);
        Assert.Equal("api/v1/routes",Attribute.GetCustomAttribute(typeof(RoutesController),typeof(RouteAttribute)) is RouteAttribute routes ? routes.Template : null);
    }
    [Fact] public void ProtectedControllersRequireAuthentication()
    {
        Assert.NotNull(Attribute.GetCustomAttribute(typeof(UsersController),typeof(AuthorizeAttribute)));
        Assert.NotNull(Attribute.GetCustomAttribute(typeof(RoutesController),typeof(AuthorizeAttribute)));
        var logout=typeof(AuthController).GetMethod("Logout")!;
        Assert.NotNull(Attribute.GetCustomAttribute(logout,typeof(AuthorizeAttribute)));
    }
    [Fact] public void ExposesExactlySevenPlannedActions()
    {
        Assert.Equal(3,typeof(AuthController).GetMethods().Count(m=>m.DeclaringType==typeof(AuthController) && m.GetCustomAttributes(typeof(HttpMethodAttribute),true).Any()));
        Assert.Equal(2,typeof(UsersController).GetMethods().Count(m=>m.DeclaringType==typeof(UsersController) && m.GetCustomAttributes(typeof(HttpMethodAttribute),true).Any()));
        Assert.Equal(2,typeof(RoutesController).GetMethods().Count(m=>m.DeclaringType==typeof(RoutesController) && m.GetCustomAttributes(typeof(HttpMethodAttribute),true).Any()));
    }
}
