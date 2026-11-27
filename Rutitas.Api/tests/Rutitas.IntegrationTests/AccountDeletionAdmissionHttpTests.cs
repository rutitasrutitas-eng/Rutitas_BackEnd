using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class AccountDeletionAdmissionHttpTests
{
    private static async Task<string> AuthenticateAsync(
        HttpClient client)
    {
        var email =
            $"admission-{Guid.NewGuid():N}@example.invalid";

        var password = "SyntheticPassword123!";

        using var registration = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                email,
                password,
                displayName = "Synthetic Admission Test"
            });

        Assert.Equal(
            HttpStatusCode.Created,
            registration.StatusCode);

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { email, password });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        using var document = JsonDocument.Parse(
            await login.Content.ReadAsStringAsync());

        var root = document.RootElement;

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                root.GetProperty("accessToken").GetString());

        return root.GetProperty("user")
            .GetProperty("id").GetString()!;
    }

    [Fact]
    public async Task ExistingDeletion_RecoveryRemainsAvailable()
    {
        using var app = new TestApiFactory();

        app.Admission.CanSchedule = true;

        using var client = app.CreateClient();

        var userId = await AuthenticateAsync(client);

        var user = await app.Users.GetByIdAsync(userId);
        Assert.NotNull(user);

        var email = user.Email;

        using var scheduled = await client.DeleteAsync(
            "/api/v1/users/me");

        Assert.Equal(
            HttpStatusCode.Accepted,
            scheduled.StatusCode);

        // El proceso de purga deja de estar disponible.
        app.Admission.CanSchedule = false;

        client.DefaultRequestHeaders.Authorization = null;

        using var recovery = await client.PostAsJsonAsync(
            "/api/v1/auth/deletion/recovery",
            new
            {
                email,
                password = "SyntheticPassword123!"
            });

        Assert.Equal(
            HttpStatusCode.OK,
            recovery.StatusCode);

        using var document = JsonDocument.Parse(
            await recovery.Content.ReadAsStringAsync());

        var recoveryToken = document.RootElement
            .GetProperty("recoveryToken").GetString();

        Assert.False(
            string.IsNullOrWhiteSpace(recoveryToken));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer", recoveryToken);

        using var status = await client.GetAsync(
            "/api/v1/users/me/deletion");

        Assert.Equal(
            HttpStatusCode.OK,
            status.StatusCode);

        using var cancelled = await client.PostAsync(
            "/api/v1/users/me/deletion/cancel",
            null);

        Assert.Equal(
            HttpStatusCode.OK,
            cancelled.StatusCode);

        var restored = await app.Users.GetByIdAsync(userId);

        Assert.NotNull(restored);
        Assert.Equal("active", restored.Status);
        Assert.Equal(3L, restored.SessionVersion);
        Assert.Null(restored.DeletionRequestedAtUtc);
        Assert.Null(restored.DeleteAfterUtc);
        Assert.Null(restored.RecoveryTokenHash);

        // Verificar que puede iniciar sesion nuevamente.
        client.DefaultRequestHeaders.Authorization = null;

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                email,
                password = "SyntheticPassword123!"
            });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        Assert.False(app.Admission.CanSchedule);
    }

    [Fact]
    public async Task Unavailable_Returns503WithoutMutation()
    {
        using var app = new TestApiFactory();
        app.Admission.CanSchedule = false;

        using var client = app.CreateClient();
        var userId = await AuthenticateAsync(client);

        using var response = await client.DeleteAsync(
            "/api/v1/users/me");

        Assert.Equal(
            HttpStatusCode.ServiceUnavailable,
            response.StatusCode);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        Assert.True(response.Headers.CacheControl?.NoStore);

        var user = await app.Users.GetByIdAsync(userId);

        Assert.NotNull(user);
        Assert.Equal("active", user.Status);
        Assert.Equal(1L, user.SessionVersion);
        Assert.Null(user.DeletionRequestedAtUtc);
        Assert.Null(user.DeleteAfterUtc);

        using var profile = await client.GetAsync(
            "/api/v1/users/me");

        Assert.Equal(HttpStatusCode.OK, profile.StatusCode);
    }

    [Fact]
    public async Task AvailabilityRestored_AllowsScheduling()
    {
        using var app = new TestApiFactory();
        app.Admission.CanSchedule = false;

        using var client = app.CreateClient();
        var userId = await AuthenticateAsync(client);

        using var denied = await client.DeleteAsync(
            "/api/v1/users/me");

        Assert.Equal(
            HttpStatusCode.ServiceUnavailable,
            denied.StatusCode);

        app.Admission.CanSchedule = true;

        using var accepted = await client.DeleteAsync(
            "/api/v1/users/me");

        Assert.Equal(
            HttpStatusCode.Accepted,
            accepted.StatusCode);

        var user = await app.Users.GetByIdAsync(userId);

        Assert.NotNull(user);
        Assert.Equal("pending_deletion", user.Status);
        Assert.Equal(2L, user.SessionVersion);
    }

    [Fact]
    public async Task Delete_Request6_Returns429ProblemDetails()
    {
        using var app = new TestApiFactory();
        app.Admission.CanSchedule = false;

        using var client = app.CreateClient();
        var userId = await AuthenticateAsync(client);

        for (var attempt = 0; attempt < 5; attempt++)
        {
            using var response = await client.DeleteAsync(
                "/api/v1/users/me");

            Assert.Equal(
                HttpStatusCode.ServiceUnavailable,
                response.StatusCode);
        }

        using var rejected = await client.DeleteAsync(
            "/api/v1/users/me");

        Assert.Equal(
            HttpStatusCode.TooManyRequests,
            rejected.StatusCode);

        Assert.True(rejected.Headers.TryGetValues(
            "Retry-After", out var values));

        Assert.True(int.TryParse(
            values!.Single(), out var seconds));

        Assert.True(seconds >= 0);

        Assert.Equal(
            "application/problem+json",
            rejected.Content.Headers.ContentType?.MediaType);

        using var json = JsonDocument.Parse(
            await rejected.Content.ReadAsStringAsync());

        Assert.Equal(
            429,
            json.RootElement.GetProperty("status").GetInt32());

        var user = await app.Users.GetByIdAsync(userId);
        Assert.NotNull(user);
        Assert.Equal("active", user.Status);
        Assert.Equal(1L, user.SessionVersion);
    }

    [Fact]
    public async Task RateLimit_IsolatedByAuthenticatedUser()
    {
        using var app = new TestApiFactory();
        app.Admission.CanSchedule = false;

        using var first = app.CreateClient();
        using var second = app.CreateClient();

        await AuthenticateAsync(first);
        await AuthenticateAsync(second);

        for (var attempt = 0; attempt < 5; attempt++)
        {
            using var response = await first.DeleteAsync(
                "/api/v1/users/me");

            Assert.Equal(
                HttpStatusCode.ServiceUnavailable,
                response.StatusCode);
        }

        using var limited = await first.DeleteAsync(
            "/api/v1/users/me");

        Assert.Equal(
            HttpStatusCode.TooManyRequests,
            limited.StatusCode);

        using var independent = await second.DeleteAsync(
            "/api/v1/users/me");

        Assert.Equal(
            HttpStatusCode.ServiceUnavailable,
            independent.StatusCode);
    }
}
