using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class DeletionMultiUserHttpTests
{
    private const string Password = "SyntheticPassword123!";

    private static async Task<string> CreatePendingAsync(
        HttpClient client,
        string email)
    {
        using var registration = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                email,
                password = Password,
                displayName = "Synthetic User"
            });

        Assert.Equal(HttpStatusCode.Created, registration.StatusCode);

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email, password = Password });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        using var loginJson = JsonDocument.Parse(
            await login.Content.ReadAsStringAsync());

        var jwt = loginJson.RootElement
            .GetProperty("accessToken").GetString()!;

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", jwt);

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

        return recoveryJson.RootElement
            .GetProperty("recoveryToken").GetString()!;
    }

    [Fact]
    public async Task TwoUsers_RecoveryTokensRemainIndependent()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var emailA = $"first-{Guid.NewGuid():N}@example.com";
        var emailB = $"second-{Guid.NewGuid():N}@example.com";

        var tokenA = await CreatePendingAsync(client, emailA);
        var tokenB = await CreatePendingAsync(client, emailB);

        Assert.NotEqual(tokenA, tokenB);

        // Cancelar solo la eliminacion del usuario A.
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenA);

        using var cancelA = await client.PostAsync(
            "/api/v1/users/me/deletion/cancel", null);

        Assert.Equal(HttpStatusCode.OK, cancelA.StatusCode);

        // B debe conservar su estado pendiente.
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenB);

        using var statusB = await client.GetAsync(
            "/api/v1/users/me/deletion");

        Assert.Equal(HttpStatusCode.OK, statusB.StatusCode);

        using var statusJson = JsonDocument.Parse(
            await statusB.Content.ReadAsStringAsync());

        Assert.Equal(
            "pending_deletion",
            statusJson.RootElement
                .GetProperty("status").GetString());

        // B tambien puede cancelar con su propio token.
        using var cancelB = await client.PostAsync(
            "/api/v1/users/me/deletion/cancel", null);

        Assert.Equal(HttpStatusCode.OK, cancelB.StatusCode);

        var userA = await app.Users.GetByEmailAsync(emailA);
        var userB = await app.Users.GetByEmailAsync(emailB);

        Assert.NotNull(userA);
        Assert.NotNull(userB);

        Assert.Equal("active", userA.Status);
        Assert.Equal("active", userB.Status);

        Assert.Equal(3L, userA.SessionVersion);
        Assert.Equal(3L, userB.SessionVersion);

        // A y B ya no pueden reutilizar sus tokens.
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenA);

        using var reusedA = await client.GetAsync(
            "/api/v1/users/me/deletion");

        Assert.Equal(HttpStatusCode.Unauthorized,
            reusedA.StatusCode);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenB);

        using var reusedB = await client.GetAsync(
            "/api/v1/users/me/deletion");

        Assert.Equal(HttpStatusCode.Unauthorized,
            reusedB.StatusCode);
    }
}