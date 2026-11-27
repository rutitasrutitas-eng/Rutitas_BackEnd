using MongoDB.Driver;
using Rutitas.Domain.Entities;
using Rutitas.Infrastructure.Repositories;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class DeletionIssueServerClockMongoTests
    : IClassFixture<MongoFixture>
{
    private readonly MongoFixture _fixture;

    public DeletionIssueServerClockMongoTests(MongoFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task StaleClock_CannotIssueTokenAfterDeletionDeadline()
    {
        var database = _fixture.CreateDatabase();

        var users = new MongoUserRepository(database);
        var deletions = new MongoAccountDeletionRepository(database);
        var tokens = new MongoRecoveryTokenRepository(database);

        var now = DateTime.UtcNow;

        var user = new User
        {
            Email = $"{Guid.NewGuid():N}@example.com",
            DisplayName = "Synthetic Issuance Clock",
            Status = "active",
            SessionVersion = 1
        };

        await users.CreateAsync(user);

        var scheduled = await deletions.ScheduleDeletionAsync(
            user.Id, 1, now.AddDays(-1));

        Assert.NotNull(scheduled);

        // Simular que el plazo real ya vencio.
        var collection = database.GetCollection<User>("users");

        await collection.UpdateOneAsync(
            Builders<User>.Filter.Eq(x => x.Id, user.Id),
            Builders<User>.Update.Set(
                x => x.DeleteAfterUtc,
                now.AddMinutes(-1)));

        // La aplicacion envia una hora antigua.
        var staleTime = now.AddMinutes(-10);
        var hash = Guid.NewGuid().ToString("N") +
                   Guid.NewGuid().ToString("N");

        var issued = await tokens.IssueAsync(
            user.Id,
            hash,
            staleTime,
            now.AddMinutes(5));

        // MongoDB debe impedir la emision.
        Assert.False(issued);

        var persisted = await users.GetByIdAsync(user.Id);

        Assert.NotNull(persisted);
        Assert.Equal("pending_deletion", persisted.Status);
        Assert.Null(persisted.RecoveryTokenHash);
        Assert.Equal(2L, persisted.SessionVersion);
    }
}