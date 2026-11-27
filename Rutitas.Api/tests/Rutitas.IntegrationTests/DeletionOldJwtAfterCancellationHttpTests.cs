using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class DeletionOldJwtAfterCancellationHttpTests
{
    private const string Password = "ValidPassword123!";

    [Fact]
    public async Task DEL_V17_OldJwtRemainsInvalidAfterCancellation()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var email = $"jwt-revocation-{Guid.NewGuid():N}@example.com";

        using var registration = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new {
                email,
                password = Password,
                displayName = "Synthetic JWT Test"
            });

        Assert.Equal(HttpStatusCode.Created, registration.StatusCode);

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email, password = Password });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        using var loginJson = JsonDocument.Parse(
            await login.Content.ReadAsStringAsync());

        var oldJwt = loginJson.RootElement
            .GetProperty("accessToken").GetString();

        Assert.False(string.IsNullOrWhiteSpace(oldJwt));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", oldJwt);

        using var deletion = await client.DeleteAsync(
            "/api/v1/users/me");

        Assert.Equal(HttpStatusCode.Accepted, deletion.StatusCode);

        client.DefaultRequestHeaders.Authorization = null;

        using var recovery = await client.PostAsJsonAsync(
            "/api/v1/auth/deletion/recovery",
            new { email, password = Password });

        Assert.Equal(HttpStatusCode.OK, recovery.StatusCode);

        using var recoveryJson = JsonDocument.Parse(
            await recovery.Content.ReadAsStringAsync());

        var recoveryToken = recoveryJson.RootElement
            .GetProperty("recoveryToken").GetString();

        Assert.False(string.IsNullOrWhiteSpace(recoveryToken));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", recoveryToken);

        using var cancellation = await client.PostAsync(
            "/api/v1/users/me/deletion/cancel", null);

        Assert.Equal(HttpStatusCode.OK, cancellation.StatusCode);

        using var cancelJson = JsonDocument.Parse(
            await cancellation.Content.ReadAsStringAsync());

        Assert.Equal("active", cancelJson.RootElement
            .GetProperty("status").GetString());

        Assert.True(cancelJson.RootElement
            .GetProperty("requiresLogin").GetBoolean());

        // JWT anterior: debe permanecer invalidado.
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", oldJwt);

        using var oldProfile = await client.GetAsync(
            "/api/v1/users/me");

        Assert.Equal(HttpStatusCode.Unauthorized,
            oldProfile.StatusCode);

        using var oldRoutes = await client.GetAsync(
            "/api/v1/routes");

        Assert.Equal(HttpStatusCode.Unauthorized,
            oldRoutes.StatusCode);

        // Nueva autenticacion ordinaria.
        client.DefaultRequestHeaders.Authorization = null;

        using var newLogin = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email, password = Password });

        Assert.Equal(HttpStatusCode.OK, newLogin.StatusCode);

        using var newLoginJson = JsonDocument.Parse(
            await newLogin.Content.ReadAsStringAsync());

        var newJwt = newLoginJson.RootElement
            .GetProperty("accessToken").GetString();

        Assert.False(string.IsNullOrWhiteSpace(newJwt));
        Assert.NotEqual(oldJwt, newJwt);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", newJwt);

        using var newProfile = await client.GetAsync(
            "/api/v1/users/me");

        Assert.Equal(HttpStatusCode.OK, newProfile.StatusCode);

        // El JWT antiguo tampoco revive tras el nuevo login.
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", oldJwt);

        using var replay = await client.GetAsync(
            "/api/v1/users/me");

        Assert.Equal(HttpStatusCode.Unauthorized, replay.StatusCode);
    }
}