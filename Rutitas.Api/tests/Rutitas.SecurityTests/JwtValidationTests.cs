using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

    private static string SignedToken(IConfiguration configuration,
        IEnumerable<Claim> claims, string? issuer = null,
        string? audience = null, DateTime? expires = null)
    {
        var token = new JwtSecurityToken(
            issuer: issuer ?? configuration["Jwt:Issuer"],
            audience: audience ?? configuration["Jwt:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow.AddMinutes(-10),
            expires: expires ?? DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Convert.FromBase64String(
                    configuration["Jwt:SigningKey"]
                    ?? throw new InvalidOperationException("Test JWT signing key is missing."))),
                SecurityAlgorithms.HmacSha256));
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
        var claims = new List<Claim> { new("sub", userId) };
        if (includeJti) claims.Add(new Claim("jti", Guid.NewGuid().ToString("N")));
        var configuration = app.Services.GetRequiredService<IConfiguration>();
        var token = SignedToken(configuration, claims, issuer, audience,
            expired ? DateTime.UtcNow.AddMinutes(-2) : null);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        Assert.Equal(HttpStatusCode.Unauthorized,
            (await client.GetAsync("/api/v1/routes")).StatusCode);
    }

    [Fact]
    public async Task SignedJwtWithoutSubjectIsRejectedWithValidControl()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();
        var (_, userId) = await RegisterAndLogin(client);
        var configuration = app.Services.GetRequiredService<IConfiguration>();
        var validControlToken = SignedToken(configuration,
        [
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        ]);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", validControlToken);
        Assert.Equal(HttpStatusCode.OK,
            (await client.GetAsync(new Uri("/api/v1/routes", UriKind.Relative))).StatusCode);

        var jti = Guid.NewGuid().ToString("N");
        var tokenWithoutSubject = SignedToken(configuration,
        [
            new Claim(JwtRegisteredClaimNames.Jti, jti)
        ]);
        var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(tokenWithoutSubject);
        Assert.Equal(configuration["Jwt:Issuer"], decodedToken.Issuer);
        Assert.Equal(configuration["Jwt:Audience"], Assert.Single(decodedToken.Audiences));
        Assert.True(decodedToken.ValidTo > DateTime.UtcNow);
        Assert.DoesNotContain(decodedToken.Claims,
            claim => claim.Type == JwtRegisteredClaimNames.Sub);
        Assert.Contains(decodedToken.Claims,
            claim => claim.Type == JwtRegisteredClaimNames.Jti && claim.Value == jti);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenWithoutSubject);
        Assert.Equal(HttpStatusCode.Unauthorized,
            (await client.GetAsync(new Uri("/api/v1/routes", UriKind.Relative))).StatusCode);
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
