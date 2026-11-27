using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class DeletionCancellationHttpTests
{
    private const string Password = "ValidPassword123!";
    private const string CancelUrl =
        "/api/v1/users/me/deletion/cancel";

    private static async Task<(string Token, string UserId, string Email)>
        PrepareAsync(HttpClient client)
    {
        var email = $"cancel-{Guid.NewGuid():N}@example.com";

        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new { email, password = Password, displayName = "Cancel Test" });

        Assert.Equal(HttpStatusCode.Created, register.StatusCode);

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email, password = Password });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        using var loginBody = JsonDocument.Parse(
            await login.Content.ReadAsStringAsync());

        var jwt = loginBody.RootElement
            .GetProperty("accessToken").GetString()!;

        var userId = loginBody.RootElement
            .GetProperty("user").GetProperty("id").GetString()!;

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

        using var recoveryBody = JsonDocument.Parse(
            await recovery.Content.ReadAsStringAsync());

        var token = recoveryBody.RootElement
            .GetProperty("recoveryToken").GetString()!;

        return (token, userId, email);
    }

    [Fact]
    public async Task ValidToken_RestoresAccountAndConsumesToken()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var (token, userId, email) = await PrepareAsync(client);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var result = await client.PostAsync(CancelUrl, null);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        using var body = JsonDocument.Parse(
            await result.Content.ReadAsStringAsync());

        Assert.Equal("active",
            body.RootElement.GetProperty("status").GetString());

        Assert.True(body.RootElement
            .GetProperty("deletionCancelled").GetBoolean());

        Assert.True(body.RootElement
            .GetProperty("requiresLogin").GetBoolean());

        Assert.Equal(3, body.RootElement.EnumerateObject().Count());

        var user = await app.Users.GetByIdAsync(userId);

        Assert.NotNull(user);
        Assert.Equal("active", user.Status);
        Assert.Equal(3L, user.SessionVersion);
        Assert.Null(user.RecoveryTokenHash);
        Assert.Null(user.RecoveryTokenExpiresAtUtc);
        Assert.Null(user.DeletionRequestedAtUtc);
        Assert.Null(user.DeleteAfterUtc);

        using var replay = await client.PostAsync(CancelUrl, null);

        Assert.Equal(HttpStatusCode.Unauthorized, replay.StatusCode);

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email, password = Password });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
    }

    [Fact]
    public async Task ExpiredDeletionDeadline_Returns410()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var (token, userId, _) = await PrepareAsync(client);

        var user = await app.Users.GetByIdAsync(userId);
        Assert.NotNull(user);

        user.DeleteAfterUtc = DateTime.UtcNow.AddSeconds(-1);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.PostAsync(CancelUrl, null);

        Assert.Equal(HttpStatusCode.Gone, response.StatusCode);
        Assert.Equal("pending_deletion", user.Status);
    }

    [Fact]
    public async Task AtomicCancellationConflict_Returns409()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var (token, userId, _) = await PrepareAsync(client);

        // El plazo sigue vigente.
        // Simulamos que la actualizacion no se confirma.
        app.RecoveryTokens.ForceCancelConflict = true;

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var response =
            await client.PostAsync(CancelUrl, null);

        Assert.Equal(HttpStatusCode.Conflict,
            response.StatusCode);

        var persisted = await app.Users.GetByIdAsync(userId);

        Assert.NotNull(persisted);
        Assert.Equal("pending_deletion", persisted.Status);
        Assert.NotNull(persisted.RecoveryTokenHash);
        Assert.NotNull(persisted.DeleteAfterUtc);
    }
    [Fact]
    public async Task DeadlineChangesDuringCancellation_Returns410()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var (token, userId, _) = await PrepareAsync(client);

        // Simular el vencimiento despues de GetStatusAsync
        // y antes de la validacion final de CancelAsync.
        app.RecoveryTokens.BeforeCancelValidation = user =>
        {
            user.DeleteAfterUtc =
                DateTime.UtcNow.AddSeconds(-1);
        };

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var response =
            await client.PostAsync(CancelUrl, null);

        Assert.Equal(HttpStatusCode.Gone,
            response.StatusCode);

        var persisted = await app.Users.GetByIdAsync(userId);

        Assert.NotNull(persisted);
        Assert.Equal("pending_deletion", persisted.Status);
        Assert.NotNull(persisted.RecoveryTokenHash);
    }

    [Fact]
    public async Task ParallelCancellation_OnlyOneSucceeds()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var (token, userId, _) = await PrepareAsync(client);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var requests = Enumerable.Range(0, 8)
            .Select(_ => client.PostAsync(CancelUrl, null))
            .ToArray();

        var responses = await Task.WhenAll(requests);

        try
        {
            Assert.Single(responses,
                x => x.StatusCode == HttpStatusCode.OK);

            Assert.All(responses,
                response => Assert.Contains(response.StatusCode,
                    new[]
                    {
                        HttpStatusCode.OK,
                        HttpStatusCode.Unauthorized,
                        HttpStatusCode.Conflict
                    }));

            var user = await app.Users.GetByIdAsync(userId);

            Assert.NotNull(user);
            Assert.Equal("active", user.Status);
            Assert.Equal(3L, user.SessionVersion);
        }
        finally
        {
            foreach (var response in responses)
                response.Dispose();
        }
    }

    [Fact]
    public async Task FailureDuringStatusRead_Returns503ProblemDetails()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        var (token, userId, _) = await PrepareAsync(client);

        app.RecoveryTokens.FailOnStatusRead = true;

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/v1/users/me/deletion");

        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer", token);

        using var response = await client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.ServiceUnavailable,
            response.StatusCode);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        using var body = System.Text.Json.JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        Assert.Equal(
            503,
            body.RootElement.GetProperty("status").GetInt32());

        Assert.False(string.IsNullOrWhiteSpace(
            body.RootElement.GetProperty("title").GetString()));

        var persisted = await app.Users.GetByIdAsync(userId);

        Assert.NotNull(persisted);
        Assert.Equal("pending_deletion", persisted.Status);
        Assert.NotNull(persisted.DeleteAfterUtc);
    }

}