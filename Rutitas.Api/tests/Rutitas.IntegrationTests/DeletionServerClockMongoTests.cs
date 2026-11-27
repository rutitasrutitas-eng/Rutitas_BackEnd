using MongoDB.Driver;
using Rutitas.Domain.Entities;
using Rutitas.Infrastructure.Repositories;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class DeletionServerClockMongoTests
    : IClassFixture<MongoFixture>
{
    private readonly MongoFixture _fixture;

    public DeletionServerClockMongoTests(MongoFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task StaleApplicationClock_CannotCancelExpiredDeletion()
    {
        var database = _fixture.CreateDatabase();

        var users = new MongoUserRepository(database);
        var deletions = new MongoAccountDeletionRepository(database);
        var tokens = new MongoRecoveryTokenRepository(database);

        var now = DateTime.UtcNow;

        var user = new User
        {
            Email = $"{Guid.NewGuid():N}@example.com",
            DisplayName = "Synthetic Clock Test",
            Status = "active",
            SessionVersion = 1
        };

        await users.CreateAsync(user);

        var scheduled = await deletions.ScheduleDeletionAsync(
            user.Id, 1, now.AddDays(-1));

        Assert.NotNull(scheduled);

        var hash = Guid.NewGuid().ToString("N") +
                   Guid.NewGuid().ToString("N");

        Assert.True(await tokens.IssueAsync(
            user.Id,
            hash,
            now,
            now.AddMinutes(5)));

        // Simular una cuenta cuyo plazo YA vencio.
        var collection = database.GetCollection<User>("users");

        await collection.UpdateOneAsync(
            Builders<User>.Filter.Eq(x => x.Id, user.Id),
            Builders<User>.Update.Set(
                x => x.DeleteAfterUtc,
                now.AddMinutes(-1)));

        // Simular una hora antigua capturada por la aplicacion.
        var staleTime = now.AddMinutes(-10);

        var result = await tokens.CancelAsync(
            hash, staleTime);

        // La hora del servidor debe prevalecer.
        Assert.Null(result);

        var persisted = await users.GetByIdAsync(user.Id);

        Assert.NotNull(persisted);
        Assert.Equal("pending_deletion", persisted.Status);
        Assert.Equal(2L, persisted.SessionVersion);
    }
}