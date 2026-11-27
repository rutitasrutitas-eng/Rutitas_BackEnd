using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;
using Moq;
using Rutitas.Api.Controllers;
using Rutitas.Application.Interfaces;
using Rutitas.Domain.Entities;
using Xunit;

namespace Rutitas.UnitTests;

public sealed class AccountDeletionControllerTests
{
    private static readonly DateTime RequestedAt =
        new(2026, 11, 18, 15, 0, 0, DateTimeKind.Utc);

    private static AccountDeletionController CreateController(
        Mock<IAccountDeletionRepository> repository,
        string? userId = "507f1f77bcf86cd799439011",
        string? version = "1")
    {
        var claims = new List<Claim>();

        if (userId is not null)
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userId));

        if (version is not null)
            claims.Add(new Claim("sessionVersion", version));

        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(claims, "TestAuthentication"));

        return new AccountDeletionController(
            repository.Object,
            NullLogger<AccountDeletionController>.Instance,
            TimeProvider.System)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            }
        };
    }

    [Fact]
    public async Task ValidAccount_Returns202WithContractFields()
    {
        var repository = new Mock<IAccountDeletionRepository>();

        repository.Setup(x => x.ScheduleDeletionAsync(
                It.IsAny<string>(),
                1,
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((string id, long version,
                DateTime requestedAt, CancellationToken ct) =>
                new User
                {
                    Id = id,
                    Status = "pending_deletion",
                    SessionVersion = version + 1,
                    DeletionRequestedAtUtc = requestedAt,
                    DeleteAfterUtc = requestedAt.AddDays(7)
                });

        var controller = CreateController(repository);

        var result = await controller.ScheduleDeletion(
            CancellationToken.None);

        var response = Assert.IsType<ObjectResult>(result);

        Assert.Equal(202, response.StatusCode);

        var json = JsonSerializer.Serialize(
            response.Value,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        using var document = JsonDocument.Parse(json);

        Assert.Equal(
            "pending_deletion",
            document.RootElement.GetProperty("status").GetString());

        Assert.True(
            document.RootElement.GetProperty("recoveryAllowed").GetBoolean());

        Assert.Equal(4,
            document.RootElement.EnumerateObject().Count());

        Assert.Equal(
            TimeSpan.FromDays(7),
            document.RootElement.GetProperty("deleteAfterUtc").GetDateTime() -
            document.RootElement.GetProperty("requestedAtUtc").GetDateTime());

        Assert.Equal(
            "no-store",
            controller.Response.Headers.CacheControl.ToString());

        repository.Verify(x => x.ScheduleDeletionAsync(
            "507f1f77bcf86cd799439011",
            1,
            It.IsAny<DateTime>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(null, "1")]
    [InlineData("507f1f77bcf86cd799439011", null)]
    [InlineData("507f1f77bcf86cd799439011", "abc")]
    [InlineData("507f1f77bcf86cd799439011", "0")]
    public async Task InvalidClaims_Return401(
        string? userId,
        string? version)
    {
        var repository = new Mock<IAccountDeletionRepository>();
        var controller = CreateController(repository, userId, version);

        var result = await controller.ScheduleDeletion(
            CancellationToken.None);

        var response = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, response.StatusCode);

        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RepositoryConflict_Return409()
    {
        var repository = new Mock<IAccountDeletionRepository>();

        repository.Setup(x => x.ScheduleDeletionAsync(
                It.IsAny<string>(),
                It.IsAny<long>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var controller = CreateController(repository);

        var result = await controller.ScheduleDeletion(
            CancellationToken.None);

        var response = Assert.IsType<ConflictObjectResult>(result);

        Assert.Equal(409, response.StatusCode);
    }

    [Fact]
    public async Task MongoFailure_Return503()
    {
        var repository = new Mock<IAccountDeletionRepository>();

        repository.Setup(x => x.ScheduleDeletionAsync(
                It.IsAny<string>(),
                It.IsAny<long>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new MongoException("Synthetic Mongo failure"));

        var controller = CreateController(repository);

        var result = await controller.ScheduleDeletion(
            CancellationToken.None);

        var response = Assert.IsType<ObjectResult>(result);

        Assert.Equal(503, response.StatusCode);
    }

    [Fact]
    public async Task MissingDeletionDates_Return503()
    {
        var repository = new Mock<IAccountDeletionRepository>();

        repository.Setup(x => x.ScheduleDeletionAsync(
                It.IsAny<string>(),
                It.IsAny<long>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User
            {
                Status = "pending_deletion"
            });

        var controller = CreateController(repository);

        var result = await controller.ScheduleDeletion(
            CancellationToken.None);

        var response = Assert.IsType<ObjectResult>(result);

        Assert.Equal(503, response.StatusCode);
    }

    [Fact]
    public async Task FixedClock_IsPassedToDeletionRepository()
    {
        var instant = new DateTimeOffset(
            2026, 11, 18, 15, 0, 0, TimeSpan.Zero);

        var repository = new Mock<IAccountDeletionRepository>();

        DateTime? capturedTime = null;

        repository
            .Setup(x => x.ScheduleDeletionAsync(
                It.IsAny<string>(),
                It.IsAny<long>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, long, DateTime, CancellationToken>(
                (id, version, timestamp, ct) =>
                    capturedTime = timestamp)
            .ReturnsAsync((string id, long version,
                DateTime timestamp, CancellationToken ct) =>
                new User
                {
                    Id = id,
                    Status = "pending_deletion",
                    DeletionRequestedAtUtc = timestamp,
                    DeleteAfterUtc = timestamp.AddDays(7)
                });

        var controller = new AccountDeletionController(
            repository.Object,
            NullLogger<AccountDeletionController>.Instance,
            new FixedDeletionClock(instant))
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(new[]
                        {
                            new Claim(
                                JwtRegisteredClaimNames.Sub,
                                "507f1f77bcf86cd799439011"),
                            new Claim("sessionVersion", "1")
                        }, "TestAuthentication"))
                }
            }
        };

        var result = await controller.ScheduleDeletion(
            CancellationToken.None);

        var response = Assert.IsType<ObjectResult>(result);

        Assert.Equal(202, response.StatusCode);
        Assert.Equal(instant.UtcDateTime, capturedTime);
        Assert.Equal(DateTimeKind.Utc, capturedTime!.Value.Kind);
    }}
internal sealed class FixedDeletionClock : TimeProvider
{
    private readonly DateTimeOffset _now;

    public FixedDeletionClock(DateTimeOffset now)
    {
        _now = now;
    }

    public override DateTimeOffset GetUtcNow() => _now;
}