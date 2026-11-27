using System.Net;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class DeletionRateLimitingHttpTests
{
    [Fact]
    public async Task StatusEndpoint_Request31_Returns429()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        const string url = "/api/v1/users/me/deletion";

        for (var attempt = 1; attempt <= 30; attempt++)
        {
            using var response = await client.GetAsync(url);

            // No hay token: debe responder 401.
            // Todavia no debe agotarse el limite.
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        using var rejected = await client.GetAsync(url);

        Assert.Equal(
            HttpStatusCode.TooManyRequests,
            rejected.StatusCode);
    }

    [Fact]
    public async Task StatusEndpoint_429_HasRetryAfter()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        const string url = "/api/v1/users/me/deletion";

        for (var i = 0; i < 30; i++)
        {
            using var response = await client.GetAsync(url);
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        using var rejected = await client.GetAsync(url);

        Assert.Equal(
            HttpStatusCode.TooManyRequests,
            rejected.StatusCode);

        Assert.True(
            rejected.Headers.TryGetValues(
                "Retry-After", out var values),
            "La respuesta 429 no contiene Retry-After.");

        Assert.True(
            int.TryParse(values!.Single(), out var seconds)
                && seconds >= 0,
            "Retry-After debe contener segundos enteros.");
    }}