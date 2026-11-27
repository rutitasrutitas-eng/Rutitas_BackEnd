using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class DeletionUnknownResultHttpTests
{
    private const string Password = "ValidPassword123!";

    [Fact]
    public async Task DEL_V24_LostCancellationResponse_NewLoginVerifiesActive()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var email = $"unknown-result-{Guid.NewGuid():N}@example.com";

        using var registration = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                email,
                password = Password,
                displayName = "Synthetic Unknown Result"
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

        // Enviar la cancelacion exactamente una vez.
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", recoveryToken);

        // Simular que el cliente no utiliza la respuesta.
        // No es una simulacion de desconexion real de red.
        using (var ignoredResponse = await client.PostAsync(
            "/api/v1/users/me/deletion/cancel", null))
        {
            // No comprobar el codigo ni leer el cuerpo.
        }

        // No reintentar POST /deletion/cancel.
        // Verificar mediante una nueva autenticacion ordinaria.
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

        using var profile = await client.GetAsync(
            "/api/v1/users/me");

        Assert.Equal(HttpStatusCode.OK, profile.StatusCode);

        // La credencial anterior sigue sin funcionar.
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", oldJwt);

        using var oldProfile = await client.GetAsync(
            "/api/v1/users/me");

        Assert.Equal(HttpStatusCode.Unauthorized,
            oldProfile.StatusCode);

        // El token restringido consumido no permite
        // consultar nuevamente el estado.
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", recoveryToken);

        using var restrictedStatus = await client.GetAsync(
            "/api/v1/users/me/deletion");

        Assert.Equal(HttpStatusCode.Unauthorized,
            restrictedStatus.StatusCode);
    }
}