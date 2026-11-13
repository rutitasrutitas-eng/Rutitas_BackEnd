using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Rutitas.IntegrationTests.Endpoints;

// Pruebas HTTP de los 7 endpoints. Repositorios en memoria y aislamiento por prueba.
// Estas pruebas NO sustituyen las de persistencia MongoDB con Testcontainers.
public class EndpointIntegrationTests
{
    private const string Email = "demo.integration@example.com";
    private const string Password = "ValidPassword123!";
    private const string RouteId = "507f1f77bcf86cd799439012";

    private static async Task Register(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new {
            email = Email, password = Password, displayName = "Estudiante" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    private static async Task Authenticate(HttpClient client)
    {
        await Register(client);
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = Email, password = Password });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",doc.RootElement.GetProperty("accessToken").GetString());
    }

    [Fact] public async Task IT_01_Register_Returns201AndPublicProfile()
    {
        using var factory = new TestApiFactory(); using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new {email=Email,password=Password,displayName="Estudiante"});
        Assert.Equal(HttpStatusCode.Created,response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains(Email,json); Assert.DoesNotContain("passwordHash",json,StringComparison.OrdinalIgnoreCase);
    }
    [Fact] public async Task IT_02_Login_ReturnsBearerToken()
    {
        using var factory = new TestApiFactory(); using var client = factory.CreateClient();
        await Register(client);
        var response = await client.PostAsJsonAsync("/api/v1/auth/login",new {email=Email,password=Password});
        Assert.Equal(HttpStatusCode.OK,response.StatusCode);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Bearer",doc.RootElement.GetProperty("tokenType").GetString());
        Assert.False(string.IsNullOrWhiteSpace(doc.RootElement.GetProperty("accessToken").GetString()));
    }
    [Fact] public async Task IT_03_GetProfile_ReturnsAuthenticatedUser()
    {
        using var factory = new TestApiFactory(); using var client = factory.CreateClient();
        await Authenticate(client);
        var response = await client.GetAsync("/api/v1/users/me");
        Assert.Equal(HttpStatusCode.OK,response.StatusCode);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(Email,doc.RootElement.GetProperty("email").GetString());
    }
    [Fact] public async Task IT_04_PatchProfile_UpdatesDisplayName()
    {
        using var factory = new TestApiFactory(); using var client = factory.CreateClient();
        await Authenticate(client);
        var response = await client.PatchAsync("/api/v1/users/me",new StringContent("{\"displayName\":\"Nombre Actualizado\"}",Encoding.UTF8,"application/json"));
        Assert.Equal(HttpStatusCode.OK,response.StatusCode);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Nombre Actualizado",doc.RootElement.GetProperty("displayName").GetString());
        var get = await client.GetAsync("/api/v1/users/me");
        Assert.Contains("Nombre Actualizado",await get.Content.ReadAsStringAsync());
    }
    [Fact] public async Task IT_05_Logout_RevokesPreviouslyValidToken()
    {
        using var factory = new TestApiFactory(); using var client = factory.CreateClient();
        await Authenticate(client);
        Assert.Equal(HttpStatusCode.NoContent,(await client.PostAsync("/api/v1/auth/logout",null)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized,(await client.GetAsync("/api/v1/users/me")).StatusCode);
    }
    [Fact] public async Task IT_06_ListRoutes_ReturnsPagedJson()
    {
        using var factory = new TestApiFactory(); using var client = factory.CreateClient();
        await Authenticate(client);
        var response = await client.GetAsync("/api/v1/routes?page=1&pageSize=20");
        Assert.Equal(HttpStatusCode.OK,response.StatusCode);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(1,doc.RootElement.GetProperty("page").GetInt32());
        Assert.Equal(1,doc.RootElement.GetProperty("total").GetInt32());
    }
    [Fact] public async Task IT_07_GetRouteById_ReturnsDetail()
    {
        using var factory = new TestApiFactory(); using var client = factory.CreateClient();
        await Authenticate(client);
        var response = await client.GetAsync("/api/v1/routes/"+RouteId);
        Assert.Equal(HttpStatusCode.OK,response.StatusCode);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(RouteId,doc.RootElement.GetProperty("id").GetString());
    }
}
