using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class DeletionRateLimitContractHttpTests
{
    [Fact]
    public async Task Status_Request31_Returns429ProblemDetails()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        const string url = "/api/v1/users/me/deletion";

        for (var i = 0; i < 30; i++)
        {
            using var response = await client.GetAsync(url);
            Assert.Equal(HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        using var rejected = await client.GetAsync(url);

        await Verify429Async(rejected);
    }

    [Fact]
    public async Task Recovery_Request6_Returns429ProblemDetails()
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();

        const string url = "/api/v1/auth/deletion/recovery";

        var request = new
        {
            email = $"rate-{Guid.NewGuid():N}@example.com",
            password = "SyntheticPassword123!"
        };

        for (var i = 0; i < 5; i++)
        {
            using var response =
                await client.PostAsJsonAsync(url, request);

            Assert.Equal(HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        using var rejected =
            await client.PostAsJsonAsync(url, request);

        await Verify429Async(rejected);
    }

    private static async Task Verify429Async(
        HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.TooManyRequests,
            response.StatusCode);

        Assert.True(response.Headers.TryGetValues(
            "Retry-After", out var values));

        Assert.True(int.TryParse(
            values!.Single(), out var seconds));

        Assert.True(seconds >= 0);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        Assert.Equal(429,
            document.RootElement
                .GetProperty("status").GetInt32());

        var title = document.RootElement
            .GetProperty("title").GetString();

        Assert.False(string.IsNullOrWhiteSpace(title));
    }
}