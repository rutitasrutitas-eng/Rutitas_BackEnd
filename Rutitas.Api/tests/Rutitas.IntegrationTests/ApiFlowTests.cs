using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Rutitas.IntegrationTests;

public class ApiFlowTests
{
    private static async Task<string> SignIn(HttpClient client)
    {
        var register=await client.PostAsJsonAsync("/api/v1/auth/register",new {email="alumno@example.com", password="ValidPassword123!",displayName="Alumno"});
        Assert.Equal(HttpStatusCode.Created,register.StatusCode);
        var response=await client.PostAsJsonAsync("/api/v1/auth/login",new {email="alumno@example.com",password="ValidPassword123!"});
        Assert.Equal(HttpStatusCode.OK,response.StatusCode);
        using var body=JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return body.RootElement.GetProperty("accessToken").GetString()!;
    }
    [Fact] public async Task RegistrationAndLoginAndProfile_Work()
    {
        using var app=new TestApiFactory();using var client=app.CreateClient();
        var token=await SignIn(client);client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",token);
        var response=await client.GetAsync("/api/v1/users/me");Assert.Equal(HttpStatusCode.OK,response.StatusCode);
    }
    [Fact] public async Task LogoutRevokesJwt()
    {
        using var app=new TestApiFactory();using var client=app.CreateClient();var token=await SignIn(client);
        client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",token);
        Assert.Equal(HttpStatusCode.NoContent,(await client.PostAsync("/api/v1/auth/logout",null)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized,(await client.GetAsync("/api/v1/users/me")).StatusCode);
    }
    [Fact] public async Task RoutesListAndDetailsWork()
    {
        using var app=new TestApiFactory();using var client=app.CreateClient();var token=await SignIn(client);
        client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",token);
        var list=await client.GetAsync("/api/v1/routes?page=1&pageSize=20");Assert.Equal(HttpStatusCode.OK,list.StatusCode);
        using var json=JsonDocument.Parse(await list.Content.ReadAsStringAsync());Assert.Equal(1,json.RootElement.GetProperty("total").GetInt32());
        Assert.Equal(HttpStatusCode.OK,(await client.GetAsync("/api/v1/routes/507f1f77bcf86cd799439012")).StatusCode);
    }
}
