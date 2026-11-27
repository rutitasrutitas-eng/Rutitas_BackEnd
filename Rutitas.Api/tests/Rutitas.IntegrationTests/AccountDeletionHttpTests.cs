using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class AccountDeletionHttpTests
{
    private const string Password = "ValidPassword123!";

    [Fact]
    public async Task DeleteWithoutJwt_Returns401()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        using var response = await client.DeleteAsync(
            "/api/v1/users/me");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task DeleteWithValidJwt_Returns202AndBlocksOldSession()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var email = $"delete-{Guid.NewGuid():N}@example.com";

        using var registration = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                email,
                password = Password,
                displayName = "Synthetic Delete Test"
            });

        Assert.Equal(
            HttpStatusCode.Created,
            registration.StatusCode);

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email, password = Password });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        using var loginJson = JsonDocument.Parse(
            await login.Content.ReadAsStringAsync());

        var token = loginJson.RootElement
            .GetProperty("accessToken").GetString();

        var userId = loginJson.RootElement
            .GetProperty("user")
            .GetProperty("id").GetString();

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.False(string.IsNullOrWhiteSpace(userId));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var deletion = await client.DeleteAsync(
            "/api/v1/users/me");

        Assert.Equal(
            HttpStatusCode.Accepted,
            deletion.StatusCode);

        Assert.Contains(
            deletion.Headers.CacheControl?.NoStore == true,
            new[] { true });

        using var deletionJson = JsonDocument.Parse(
            await deletion.Content.ReadAsStringAsync());

        var root = deletionJson.RootElement;

        Assert.Equal(4, root.EnumerateObject().Count());

        Assert.Equal(
            "pending_deletion",
            root.GetProperty("status").GetString());

        Assert.True(
            root.GetProperty("recoveryAllowed").GetBoolean());

        var requestedAt = root
            .GetProperty("requestedAtUtc").GetDateTimeOffset();

        var deleteAfter = root
            .GetProperty("deleteAfterUtc").GetDateTimeOffset();

        Assert.Equal(
            TimeSpan.FromDays(7),
            deleteAfter - requestedAt);

        var persisted = await app.Users.GetByIdAsync(userId!);

        Assert.NotNull(persisted);
        Assert.Equal("pending_deletion", persisted.Status);
        Assert.Equal(2L, persisted.SessionVersion);
        Assert.Equal(requestedAt.UtcDateTime,
            persisted.DeletionRequestedAtUtc);
        Assert.Equal(deleteAfter.UtcDateTime,
            persisted.DeleteAfterUtc);

        // El JWT ordinario anterior ya no debe funcionar.
        using var profile = await client.GetAsync(
            "/api/v1/users/me");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            profile.StatusCode);

        // Repetir DELETE no debe extender el plazo.
        using var duplicate = await client.DeleteAsync(
            "/api/v1/users/me");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            duplicate.StatusCode);

        var afterDuplicate = await app.Users.GetByIdAsync(userId!);

        Assert.NotNull(afterDuplicate);
        Assert.Equal(deleteAfter.UtcDateTime,
            afterDuplicate.DeleteAfterUtc);
        Assert.Equal(2L, afterDuplicate.SessionVersion);

        // El login normal queda bloqueado.
        using var secondLogin = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email, password = Password });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            secondLogin.StatusCode);
    }
}