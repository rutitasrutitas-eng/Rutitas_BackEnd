using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Rutitas.IntegrationTests;
using Xunit;

namespace Rutitas.SecurityTests;
public class SecurityTests
{
    [Theory]
    [InlineData("/api/v1/users/me")][InlineData("/api/v1/routes")]
    [InlineData("/api/v1/routes/507f1f77bcf86cd799439012")]
    public async Task ProtectedEndpoint_RequiresToken(string url)
    {
        using var app=new TestApiFactory();using var client=app.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized,(await client.GetAsync(url)).StatusCode);
    }
    [Fact] public async Task ForgedJwt_IsRejected()
    {
        using var app=new TestApiFactory();using var client=app.CreateClient();
        client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer","invalid.jwt.signature");
        Assert.Equal(HttpStatusCode.Unauthorized,(await client.GetAsync("/api/v1/users/me")).StatusCode);
    }
    [Fact] public async Task EscalationByMassAssignment_IsRejected()
    {
        using var app=new TestApiFactory();using var client=app.CreateClient();
        Assert.Equal(HttpStatusCode.Created,(await client.PostAsJsonAsync("/api/v1/auth/register",new {email="alumno@example.com",password="ValidPassword123!",displayName="Alumno",role="admin"})).StatusCode);
        var login=await client.PostAsJsonAsync("/api/v1/auth/login",new {email="alumno@example.com",password="ValidPassword123!"});
        using var doc=JsonDocument.Parse(await login.Content.ReadAsStringAsync());
        Assert.Equal("user",doc.RootElement.GetProperty("user").GetProperty("role").GetString());
        client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",doc.RootElement.GetProperty("accessToken").GetString());
        var response=await client.PatchAsync("/api/v1/users/me",new StringContent("{\"role\":\"admin\"}",Encoding.UTF8,"application/json"));
        Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
    }
    [Theory][InlineData("?page=0")][InlineData("?pageSize=101")]
    public async Task Routes_RejectInvalidInputs(string q)
    {
        using var app=new TestApiFactory();using var client=app.CreateClient();
        await client.PostAsJsonAsync("/api/v1/auth/register",new {email="alumno@example.com",password="ValidPassword123!",displayName="Alumno"});
        var login=await client.PostAsJsonAsync("/api/v1/auth/login",new {email="alumno@example.com",password="ValidPassword123!"});
        using var doc=JsonDocument.Parse(await login.Content.ReadAsStringAsync());client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",doc.RootElement.GetProperty("accessToken").GetString());
        Assert.Equal(HttpStatusCode.BadRequest,(await client.GetAsync("/api/v1/routes"+q)).StatusCode);
    }
    [Fact] public async Task LoginRateLimit_RejectsSixthRequest()
    {
        using var app=new TestApiFactory();using var client=app.CreateClient();
        var statuses=new List<HttpStatusCode>();
        for(var i=0;i<6;i++)
        {
            var response=await client.PostAsJsonAsync("/api/v1/auth/login",new {email="alumno@example.com",password="WrongPassword123!"});
            statuses.Add(response.StatusCode);
        }
        Assert.Contains(HttpStatusCode.TooManyRequests,statuses);
    }
    [Fact] public async Task NoSecretsInLoginError()
    {
        using var app=new TestApiFactory();using var client=app.CreateClient();
        var response=await client.PostAsJsonAsync("/api/v1/auth/login",new {email="alumno@example.com",password="WrongPassword123!"});
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
        Assert.DoesNotContain("SigningKey",await response.Content.ReadAsStringAsync(),StringComparison.OrdinalIgnoreCase);
    }
}
