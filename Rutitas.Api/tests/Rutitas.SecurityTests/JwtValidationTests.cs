using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Rutitas.IntegrationTests;
using Xunit;

namespace Rutitas.SecurityTests;

public sealed class JwtValidationTests
{
    private const string Email = "jwt@example.com";
    private const string Password = "ValidPassword123!";

    private static async Task<(string Token, string UserId)> RegisterAndLogin(HttpClient client)
    {
        var register = await client.PostAsJsonAsync("/api/v1/auth/register",
            new { email = Email, password = Password, displayName = "JWT Test" });
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        var login = await client.PostAsJsonAsync("/api/v1/auth/login",
            new { email = Email, password = Password });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        using var body = JsonDocument.Parse(await login.Content.ReadAsStringAsync());
        return (body.RootElement.GetProperty("accessToken").GetString()!,
            body.RootElement.GetProperty("user").GetProperty("id").GetString()!);
    }

    private static string SignedToken(string userId, string issuer = "Rutitas.Api",
        string audience = "Rutitas.Mobile", DateTime? expires = null, bool includeJti = true)
    {
        var claims = new List<Claim> { new("sub", userId) };
        if (includeJti) claims.Add(new Claim("jti", Guid.NewGuid().ToString("N")));
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow.AddMinutes(-10),
            expires: expires ?? DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(new byte[32]), SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Theory]
    [InlineData("other-issuer", "Rutitas.Mobile", false, true)]
    [InlineData("Rutitas.Api", "other-audience", false, true)]
    [InlineData("Rutitas.Api", "Rutitas.Mobile", true, true)]
    [InlineData("Rutitas.Api", "Rutitas.Mobile", false, false)]
    public async Task InvalidJwtClaims_AreRejected(string issuer, string audience,
        bool expired, bool includeJti)
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();
        var (_, userId) = await RegisterAndLogin(client);
        var token = SignedToken(userId, issuer, audience,
            expired ? DateTime.UtcNow.AddMinutes(-2) : null, includeJti);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        Assert.Equal(HttpStatusCode.Unauthorized,
            (await client.GetAsync("/api/v1/routes")).StatusCode);
    }

    [Fact]
    public async Task RevocationStoreFailure_FailsClosed()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();
        var (token, _) = await RegisterAndLogin(client);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        app.Revocations.FailOnRead = true;
        Assert.Equal(HttpStatusCode.Unauthorized,
            (await client.GetAsync("/api/v1/routes")).StatusCode);
    }

    [Fact]
    public async Task InactiveUser_CannotAccessRoutesWithValidToken()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();
        var (token, _) = await RegisterAndLogin(client);
        (await app.Users.GetByEmailAsync(Email))!.Status = "inactive";
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        Assert.Equal(HttpStatusCode.Unauthorized,
            (await client.GetAsync("/api/v1/routes")).StatusCode);
    }
}
