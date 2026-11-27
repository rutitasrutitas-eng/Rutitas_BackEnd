using MongoDB.Driver;
using Rutitas.Domain.Entities;
using Rutitas.Infrastructure.Repositories;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class AccountDeletionMongoTests
    : IClassFixture<MongoFixture>
{
    private readonly MongoFixture _fixture;

    private static readonly DateTime RequestedAt =
        new(2026, 11, 18, 18, 0, 0, DateTimeKind.Utc);

    public AccountDeletionMongoTests(MongoFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<(MongoUserRepository Users,
        MongoAccountDeletionRepository Deletions,
        User User)> CreateUserAsync(string status = "active")
    {
        var database = _fixture.CreateDatabase();

        var users = new MongoUserRepository(database);
        var deletions =
            new MongoAccountDeletionRepository(database);

        var user = new User
        {
            Email = $"{Guid.NewGuid():N}@example.com",
            DisplayName = "Usuario Ficticio",
            Status = status,
            SessionVersion = 1
        };

        await users.CreateAsync(user);

        return (users, deletions, user);
    }

    [Fact]
    public async Task ScheduleDeletion_StoresAtomicTransition()
    {
        var (users, deletions, user) = await CreateUserAsync();

        var result = await deletions.ScheduleDeletionAsync(
            user.Id, 1, RequestedAt);

        Assert.NotNull(result);
        Assert.Equal("pending_deletion", result.Status);
        Assert.Equal(2, result.SessionVersion);
        Assert.Equal(RequestedAt, result.DeletionRequestedAtUtc);
        Assert.Equal(RequestedAt.AddDays(7), result.DeleteAfterUtc);

        var persisted = await users.GetByIdAsync(user.Id);

        Assert.NotNull(persisted);
        Assert.Equal("pending_deletion", persisted.Status);
        Assert.Equal(2, persisted.SessionVersion);
        Assert.Equal(RequestedAt, persisted.DeletionRequestedAtUtc);
        Assert.Equal(RequestedAt.AddDays(7), persisted.DeleteAfterUtc);
    }

    [Fact]
    public async Task RepeatedRequest_DoesNotExtendDeadline()
    {
        var (users, deletions, user) = await CreateUserAsync();

        var first = await deletions.ScheduleDeletionAsync(
            user.Id, 1, RequestedAt);

        var second = await deletions.ScheduleDeletionAsync(
            user.Id, 1, RequestedAt.AddHours(3));

        Assert.NotNull(first);
        Assert.Null(second);

        var persisted = await users.GetByIdAsync(user.Id);

        Assert.NotNull(persisted);
        Assert.Equal(2, persisted.SessionVersion);
        Assert.Equal(RequestedAt, persisted.DeletionRequestedAtUtc);
        Assert.Equal(RequestedAt.AddDays(7), persisted.DeleteAfterUtc);
    }

    [Fact]
    public async Task ConcurrentRequests_OnlyOneTransitionWins()
    {
        var (users, deletions, user) = await CreateUserAsync();

        var attempts = Enumerable.Range(0, 10)
            .Select(_ => deletions.ScheduleDeletionAsync(
                user.Id, 1, RequestedAt))
            .ToArray();

        var results = await Task.WhenAll(attempts);

        Assert.Single(results, result => result is not null);

        var persisted = await users.GetByIdAsync(user.Id);

        Assert.NotNull(persisted);
        Assert.Equal("pending_deletion", persisted.Status);
        Assert.Equal(2, persisted.SessionVersion);
        Assert.Equal(RequestedAt.AddDays(7), persisted.DeleteAfterUtc);
    }

    [Fact]
    public async Task OutdatedSessionVersion_CannotScheduleDeletion()
    {
        var (users, deletions, user) = await CreateUserAsync();

        var result = await deletions.ScheduleDeletionAsync(
            user.Id, 99, RequestedAt);

        Assert.Null(result);

        var persisted = await users.GetByIdAsync(user.Id);

        Assert.NotNull(persisted);
        Assert.Equal("active", persisted.Status);
        Assert.Equal(1, persisted.SessionVersion);
        Assert.Null(persisted.DeleteAfterUtc);
    }

    [Fact]
    public async Task InactiveUser_CannotScheduleDeletion()
    {
        var (users, deletions, user) =
            await CreateUserAsync("inactive");

        var result = await deletions.ScheduleDeletionAsync(
            user.Id, 1, RequestedAt);

        Assert.Null(result);

        var persisted = await users.GetByIdAsync(user.Id);

        Assert.NotNull(persisted);
        Assert.Equal("inactive", persisted.Status);
        Assert.Equal(1, persisted.SessionVersion);
        Assert.Null(persisted.DeletionRequestedAtUtc);
    }

    [Fact]
    public async Task NonUtcDate_IsRejectedWithoutMutation()
    {
        var (users, deletions, user) = await CreateUserAsync();

        var invalidDate = new DateTime(
            2026, 11, 18, 18, 0, 0,
            DateTimeKind.Unspecified);

        await Assert.ThrowsAsync<ArgumentException>(
            () => deletions.ScheduleDeletionAsync(
                user.Id, 1, invalidDate));

        var persisted = await users.GetByIdAsync(user.Id);

        Assert.NotNull(persisted);
        Assert.Equal("active", persisted.Status);
        Assert.Equal(1, persisted.SessionVersion);
    }
}
