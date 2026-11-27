using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class DeletionStatusHttpTests
{
    private const string Password = "ValidPassword123!";
    private const string StatusUrl = "/api/v1/users/me/deletion";

    private static async Task<(string Token, string UserId)>
        CreateRecoveryToken(HttpClient client)
    {
        var email = $"status-{Guid.NewGuid():N}@example.com";

        using var registration = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                email,
                password = Password,
                displayName = "Synthetic Status Test"
            });

        Assert.Equal(HttpStatusCode.Created,
            registration.StatusCode);

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email, password = Password });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        using var loginJson = JsonDocument.Parse(
            await login.Content.ReadAsStringAsync());

        var jwt = loginJson.RootElement
            .GetProperty("accessToken").GetString()!;

        var userId = loginJson.RootElement
            .GetProperty("user")
            .GetProperty("id").GetString()!;

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", jwt);

        using var deletion = await client.DeleteAsync(
            "/api/v1/users/me");

        Assert.Equal(HttpStatusCode.Accepted,
            deletion.StatusCode);

        client.DefaultRequestHeaders.Authorization = null;

        using var recovery = await client.PostAsJsonAsync(
            "/api/v1/auth/deletion/recovery",
            new { email, password = Password });

        Assert.Equal(HttpStatusCode.OK,
            recovery.StatusCode);

        using var recoveryJson = JsonDocument.Parse(
            await recovery.Content.ReadAsStringAsync());

        var token = recoveryJson.RootElement
            .GetProperty("recoveryToken").GetString()!;

        return (token, userId);
    }

    [Fact]
    public async Task ValidRecoveryToken_ReturnsStatusWithoutConsumingIt()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var (token, userId) =
            await CreateRecoveryToken(client);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var first = await client.GetAsync(StatusUrl);

        Assert.Equal(HttpStatusCode.OK,
            first.StatusCode);

        Assert.True(first.Headers.CacheControl?.NoStore);

        using var json = JsonDocument.Parse(
            await first.Content.ReadAsStringAsync());

        var root = json.RootElement;

        Assert.Equal(4, root.EnumerateObject().Count());

        Assert.Equal("pending_deletion",
            root.GetProperty("status").GetString());

        Assert.True(
            root.GetProperty("recoveryAllowed").GetBoolean());

        var requestedAt = root.GetProperty(
            "requestedAtUtc").GetDateTimeOffset();

        var deleteAfter = root.GetProperty(
            "deleteAfterUtc").GetDateTimeOffset();

        Assert.Equal(
            TimeSpan.FromDays(7),
            deleteAfter - requestedAt);

        // La consulta no debe consumir el token.
        using var second = await client.GetAsync(StatusUrl);

        Assert.Equal(HttpStatusCode.OK,
            second.StatusCode);

        var user = await app.Users.GetByIdAsync(userId);

        Assert.NotNull(user);
        Assert.Equal(app.RecoveryTokens.LastTokenHash,
            user.RecoveryTokenHash);
    }

    [Fact]
    public async Task WithoutRecoveryToken_Returns401()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        using var response = await client.GetAsync(StatusUrl);

        Assert.Equal(HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task InvalidRecoveryToken_Returns401()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer", new string('A', 64));

        using var response = await client.GetAsync(StatusUrl);

        Assert.Equal(HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task ExpiredRecoveryToken_Returns401()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var (token, userId) =
            await CreateRecoveryToken(client);

        var user = await app.Users.GetByIdAsync(userId);

        Assert.NotNull(user);

        // Simulamos el vencimiento solo en memoria.
        user.RecoveryTokenExpiresAtUtc =
            DateTime.UtcNow.AddSeconds(-1);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.GetAsync(StatusUrl);

        Assert.Equal(HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task AfterDeletionDeadline_RecoveryIsNotAllowed()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var (token, userId) =
            await CreateRecoveryToken(client);

        var user = await app.Users.GetByIdAsync(userId);

        Assert.NotNull(user);

        // La cuenta sigue pendiente, pero ya vencio
        // el plazo de siete dias. El token sigue vigente.
        user.DeleteAfterUtc = DateTime.UtcNow.AddSeconds(-1);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.GetAsync(StatusUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        Assert.Equal("pending_deletion",
            json.RootElement.GetProperty("status").GetString());

        Assert.False(json.RootElement
            .GetProperty("recoveryAllowed").GetBoolean());
    }

    [Fact]
    public async Task DeletingAccount_Returns410()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var (token, userId) =
            await CreateRecoveryToken(client);

        var user = await app.Users.GetByIdAsync(userId);

        Assert.NotNull(user);

        user.Status = "deleting";

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.GetAsync(StatusUrl);

        Assert.Equal(HttpStatusCode.Gone, response.StatusCode);
        Assert.True(response.Headers.CacheControl?.NoStore);
    }}