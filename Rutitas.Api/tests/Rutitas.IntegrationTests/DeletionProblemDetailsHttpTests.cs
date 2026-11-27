using System.Net;
using System.Text.Json;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class DeletionProblemDetailsHttpTests
{
    [Theory]
    [InlineData("GET", "/api/v1/users/me/deletion")]
    [InlineData("POST", "/api/v1/users/me/deletion/cancel")]
    public async Task MissingRecoveryToken_Returns401ProblemDetails(
        string method,
        string url)
    {
        using var app = new TestApiFactory();
        using var client = app.CreateClient();
        using var request = new HttpRequestMessage(
            new HttpMethod(method), url);

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized,
            response.StatusCode);

        Assert.Equal("application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        using var body = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        Assert.Equal(401,
            body.RootElement.GetProperty("status").GetInt32());

        var title = body.RootElement
            .GetProperty("title").GetString();

        Assert.False(string.IsNullOrWhiteSpace(title));

        Assert.True(response.Headers.CacheControl?.NoStore);
    }
}