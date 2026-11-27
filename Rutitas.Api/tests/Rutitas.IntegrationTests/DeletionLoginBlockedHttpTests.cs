using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class DeletionLoginBlockedHttpTests
{
    private const string Password = "ValidPassword123!";

    [Fact]
    public async Task DEL_V23_NormalLoginBlocked_InPendingAndDeleting()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var email = $"login-block-{Guid.NewGuid():N}@example.com";

        using var registration = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new {
                email,
                password = Password,
                displayName = "Synthetic Login Test"
            });

        Assert.Equal(HttpStatusCode.Created, registration.StatusCode);

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email, password = Password });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        using var loginJson = JsonDocument.Parse(
            await login.Content.ReadAsStringAsync());

        var jwt = loginJson.RootElement
            .GetProperty("accessToken").GetString();

        var userId = loginJson.RootElement
            .GetProperty("user")
            .GetProperty("id").GetString();

        Assert.False(string.IsNullOrWhiteSpace(jwt));
        Assert.False(string.IsNullOrWhiteSpace(userId));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", jwt);

        using var deletion = await client.DeleteAsync(
            "/api/v1/users/me");

        Assert.Equal(HttpStatusCode.Accepted, deletion.StatusCode);

        client.DefaultRequestHeaders.Authorization = null;

        var user = await app.Users.GetByIdAsync(userId!);

        Assert.NotNull(user);
        Assert.Equal("pending_deletion", user.Status);

        // Estado pending_deletion: login ordinario bloqueado.
        using var pendingLogin = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email, password = Password });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            pendingLogin.StatusCode);

        // Simular en memoria que comenzó el borrado irreversible.
        // No se ejecuta worker ni se elimina ningún dato.
        user.Status = "deleting";
        user.SessionVersion++;

        Assert.Equal("deleting", user.Status);

        // Estado deleting: login ordinario también bloqueado.
        using var deletingLogin = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email, password = Password });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            deletingLogin.StatusCode);

        // Un JWT anterior tampoco debe recuperar acceso.
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", jwt);

        using var profile = await client.GetAsync(
            "/api/v1/users/me");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            profile.StatusCode);
    }
}