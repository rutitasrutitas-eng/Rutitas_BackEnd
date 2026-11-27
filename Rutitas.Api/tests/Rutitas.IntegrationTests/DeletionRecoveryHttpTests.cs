using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class DeletionRecoveryHttpTests
{
    private const string Password = "ValidPassword123!";

    private static async Task<(string Email, string UserId)>
        CreatePendingAccount(TestApiFactory app, HttpClient client)
    {
        var email = $"recover-{Guid.NewGuid():N}@example.com";

        using var registration = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                email,
                password = Password,
                displayName = "Synthetic Recovery Test"
            });

        Assert.Equal(HttpStatusCode.Created,
            registration.StatusCode);

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email, password = Password });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        using var json = JsonDocument.Parse(
            await login.Content.ReadAsStringAsync());

        var token = json.RootElement
            .GetProperty("accessToken").GetString()!;

        var userId = json.RootElement
            .GetProperty("user")
            .GetProperty("id").GetString()!;

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var deletion = await client.DeleteAsync(
            "/api/v1/users/me");

        Assert.Equal(HttpStatusCode.Accepted,
            deletion.StatusCode);

        client.DefaultRequestHeaders.Authorization = null;

        return (email, userId);
    }

    [Fact]
    public async Task PendingAccount_CanGetOpaqueRecoveryToken()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var (email, userId) =
            await CreatePendingAccount(app, client);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/deletion/recovery",
            new { email, password = Password });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.CacheControl?.NoStore);
        Assert.Contains("no-cache",
            response.Headers.Pragma.Select(x => x.Name));

        using var json = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        var root = json.RootElement;

        Assert.Equal(3, root.EnumerateObject().Count());

        var token = root.GetProperty("recoveryToken").GetString()!;

        Assert.Equal(64, token.Length);
        Assert.Equal("Bearer",
            root.GetProperty("tokenType").GetString());
        Assert.Equal(300,
            root.GetProperty("expiresIn").GetInt32());

        var storedHash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        var user = await app.Users.GetByIdAsync(userId);

        Assert.NotNull(user);
        Assert.Equal(storedHash, user.RecoveryTokenHash);
        Assert.NotEqual(token, user.RecoveryTokenHash);

        Assert.Equal(
            TimeSpan.FromSeconds(300),
            user.RecoveryTokenExpiresAtUtc!.Value -
            app.RecoveryTokens.LastIssuedAtUtc!.Value);

        // Una credencial opaca no debe funcionar como JWT ordinario.
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var profile = await client.GetAsync(
            "/api/v1/users/me");

        Assert.Equal(HttpStatusCode.Unauthorized,
            profile.StatusCode);
    }

    [Fact]
    public async Task WrongPassword_Returns401()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var (email, _) =
            await CreatePendingAccount(app, client);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/deletion/recovery",
            new { email, password = "WrongPassword123!" });

        Assert.Equal(HttpStatusCode.Unauthorized,
            response.StatusCode);

        Assert.Null(app.RecoveryTokens.LastTokenHash);
    }

    [Fact]
    public async Task ActiveAccount_CannotGetRecoveryToken()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var email = $"active-{Guid.NewGuid():N}@example.com";

        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                email,
                password = Password,
                displayName = "Active User"
            });

        Assert.Equal(HttpStatusCode.Created,
            register.StatusCode);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/deletion/recovery",
            new { email, password = Password });

        Assert.Equal(HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task UnknownAccount_ReturnsGeneric401()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/deletion/recovery",
            new
            {
                email = "unknown@example.com",
                password = Password
            });

        Assert.Equal(HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task NewRecoveryToken_InvalidatesPreviousHash()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var (email, userId) =
            await CreatePendingAccount(app, client);

        using var first = await client.PostAsJsonAsync(
            "/api/v1/auth/deletion/recovery",
            new { email, password = Password });

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        var firstTokenHash = app.RecoveryTokens.LastTokenHash;
        var user = await app.Users.GetByIdAsync(userId);

        Assert.NotNull(user);

        var originalDeadline = user.DeleteAfterUtc;
        var originalRequest = user.DeletionRequestedAtUtc;

        using var second = await client.PostAsJsonAsync(
            "/api/v1/auth/deletion/recovery",
            new { email, password = Password });

        Assert.Equal(HttpStatusCode.OK, second.StatusCode);

        Assert.NotEqual(
            firstTokenHash,
            app.RecoveryTokens.LastTokenHash);

        Assert.Equal(
            originalDeadline,
            user.DeleteAfterUtc);

        Assert.Equal(
            originalRequest,
            user.DeletionRequestedAtUtc);
    }

    [Fact]
    public async Task UnknownAndWrongPassword_HaveSamePublicResponse()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var (email, _) =
            await CreatePendingAccount(app, client);

        using var unknown = await client.PostAsJsonAsync(
            "/api/v1/auth/deletion/recovery",
            new
            {
                email = $"unknown-{Guid.NewGuid():N}@example.com",
                password = Password
            });

        using var wrongPassword = await client.PostAsJsonAsync(
            "/api/v1/auth/deletion/recovery",
            new
            {
                email,
                password = "DefinitelyWrongPassword123!"
            });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            unknown.StatusCode);

        Assert.Equal(
            unknown.StatusCode,
            wrongPassword.StatusCode);

        var unknownBody =
            await unknown.Content.ReadAsStringAsync();

        var wrongBody =
            await wrongPassword.Content.ReadAsStringAsync();

        using var first = JsonDocument.Parse(unknownBody);
        using var second = JsonDocument.Parse(wrongBody);

        Assert.Equal(
            first.RootElement.GetProperty("title").GetString(),
            second.RootElement.GetProperty("title").GetString());

        Assert.True(unknown.Headers.CacheControl?.NoStore);
        Assert.True(wrongPassword.Headers.CacheControl?.NoStore);

        Assert.Null(app.RecoveryTokens.LastTokenHash);
    }}