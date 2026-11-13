using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Rutitas.IntegrationTests;
using Xunit;

namespace Rutitas.SecurityTests.Endpoints;

// Un caso de seguridad verificable por cada uno de los 7 endpoints.
public class EndpointSecurityTests
{
    private const string Email = "seguridad@example.com";
    private const string Password = "ValidPassword123!";
    private static async Task Authenticate(HttpClient client)
    {
        Assert.Equal(HttpStatusCode.Created,(await client.PostAsJsonAsync("/api/v1/auth/register",new {email=Email,password=Password,displayName="Usuario"})).StatusCode);
        var login = await client.PostAsJsonAsync("/api/v1/auth/login",new {email=Email,password=Password});
        Assert.Equal(HttpStatusCode.OK,login.StatusCode);
        using var doc=JsonDocument.Parse(await login.Content.ReadAsStringAsync());
        client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",doc.RootElement.GetProperty("accessToken").GetString());
    }
    [Fact] public async Task SEC_01_Register_RejectsShortPassword()
    {
        using var factory=new TestApiFactory();using var client=factory.CreateClient();
        var response=await client.PostAsJsonAsync("/api/v1/auth/register",new {email=Email,password="short",displayName="Usuario"});
        Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
        Assert.DoesNotContain("passwordHash",await response.Content.ReadAsStringAsync(),StringComparison.OrdinalIgnoreCase);
    }
    [Fact] public async Task SEC_02_Login_RejectsIncorrectPassword()
    {
        using var factory=new TestApiFactory();using var client=factory.CreateClient();
        await client.PostAsJsonAsync("/api/v1/auth/register",new {email=Email,password=Password,displayName="Usuario"});
        var response=await client.PostAsJsonAsync("/api/v1/auth/login",new {email=Email,password="WrongPassword123!"});
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
        Assert.DoesNotContain("passwordHash",await response.Content.ReadAsStringAsync(),StringComparison.OrdinalIgnoreCase);
    }
    [Fact] public async Task SEC_03_GetProfile_RejectsAnonymous()
    {
        using var factory=new TestApiFactory();using var client=factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized,(await client.GetAsync("/api/v1/users/me")).StatusCode);
    }
    [Fact] public async Task SEC_04_PatchProfile_RejectsRoleEscalation()
    {
        using var factory=new TestApiFactory();using var client=factory.CreateClient();
        await Authenticate(client);
        var response=await client.PatchAsync("/api/v1/users/me",new StringContent("{\"role\":\"admin\"}",Encoding.UTF8,"application/json"));
        Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
        var profile=await client.GetAsync("/api/v1/users/me");
        using var doc=JsonDocument.Parse(await profile.Content.ReadAsStringAsync());
        Assert.Equal("user",doc.RootElement.GetProperty("role").GetString());
    }
    [Fact] public async Task SEC_05_Logout_RejectsAnonymous()
    {
        using var factory=new TestApiFactory();using var client=factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized,(await client.PostAsync("/api/v1/auth/logout",null)).StatusCode);
    }
    [Fact] public async Task SEC_06_ListRoutes_RejectsForgedJwt()
    {
        using var factory=new TestApiFactory();using var client=factory.CreateClient();
        client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer","invalid.jwt.signature");
        Assert.Equal(HttpStatusCode.Unauthorized,(await client.GetAsync("/api/v1/routes")).StatusCode);
    }
    [Fact] public async Task SEC_07_GetRouteById_RejectsInvalidIdentifier()
    {
        using var factory=new TestApiFactory();using var client=factory.CreateClient();
        await Authenticate(client);
        var response=await client.GetAsync("/api/v1/routes/not-an-object-id");
        Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
        Assert.DoesNotContain("mongodb://",await response.Content.ReadAsStringAsync(),StringComparison.OrdinalIgnoreCase);
    }
}
