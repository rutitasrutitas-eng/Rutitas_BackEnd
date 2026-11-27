using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class DeletionRecoveryScopeHttpTests
{
    private const string Password = "ValidPassword123!";

    [Fact]
    public async Task DEL_V05_V22_RecoveryToken_CannotAccessNormalResources()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var email = $"scope-{Guid.NewGuid():N}@example.com";

        // 1. Registrar cuenta ficticia.
        using var registration = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                email,
                password = Password,
                displayName = "Synthetic Scope Test"
            });

        Assert.Equal(HttpStatusCode.Created,
            registration.StatusCode);

        // 2. Iniciar sesion ordinaria.
        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email, password = Password });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        using var loginJson = JsonDocument.Parse(
            await login.Content.ReadAsStringAsync());

        var jwt = loginJson.RootElement
            .GetProperty("accessToken").GetString();

        Assert.False(string.IsNullOrWhiteSpace(jwt));

        // 3. Programar eliminacion.
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", jwt);

        using var deletion = await client.DeleteAsync(
            "/api/v1/users/me");

        Assert.Equal(HttpStatusCode.Accepted,
            deletion.StatusCode);

        client.DefaultRequestHeaders.Authorization = null;

        // 4. Obtener credencial restringida.
        using var recovery = await client.PostAsJsonAsync(
            "/api/v1/auth/deletion/recovery",
            new { email, password = Password });

        Assert.Equal(HttpStatusCode.OK,
            recovery.StatusCode);

        using var recoveryJson = JsonDocument.Parse(
            await recovery.Content.ReadAsStringAsync());

        var token = recoveryJson.RootElement
            .GetProperty("recoveryToken").GetString();

        Assert.False(string.IsNullOrWhiteSpace(token));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // 5. Comprobar que la credencial restringida
        // SI puede consultar el estado de eliminacion.
        using var status = await client.GetAsync(
            "/api/v1/users/me/deletion");

        Assert.Equal(HttpStatusCode.OK,
            status.StatusCode);

        // 6. Comprobar que NO funciona en endpoints normales.
        var rutasProtegidas = new[]
        {
            "/api/v1/users/me",
            "/api/v1/routes",
            "/api/v1/routes/507f1f77bcf86cd799439012"
        };

        foreach (var ruta in rutasProtegidas)
        {
            using var response = await client.GetAsync(ruta);

            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        // 7. Tampoco debe permitir modificar el perfil.
        using var modification = await client.PatchAsJsonAsync(
            "/api/v1/users/me",
            new { displayName = "Unauthorized Change" });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            modification.StatusCode);

        // 8. Los intentos rechazados no deben invalidar
        // ni consumir la credencial de consulta.
        using var secondStatus = await client.GetAsync(
            "/api/v1/users/me/deletion");

        Assert.Equal(HttpStatusCode.OK,
            secondStatus.StatusCode);

        using var statusJson = JsonDocument.Parse(
            await secondStatus.Content.ReadAsStringAsync());

        Assert.Equal(
            "pending_deletion",
            statusJson.RootElement
                .GetProperty("status").GetString());
    }
}