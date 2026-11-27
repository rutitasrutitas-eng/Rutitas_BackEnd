using System.Security.Cryptography;
using MongoDB.Driver;
using Rutitas.Domain.Entities;
using Rutitas.Infrastructure.Repositories;
using Xunit;

namespace Rutitas.IntegrationTests;

public sealed class DeletionCancellationMongoTests
    : IClassFixture<MongoFixture>
{
    private readonly MongoFixture _fixture;

    // Fecha relativa: las cancelaciones normales siguen
    // siendo vigentes frente al reloj del servidor MongoDB.
    private static readonly DateTime RequestedAt =
        TruncateToMilliseconds(DateTime.UtcNow.AddMinutes(-1));

    private static DateTime TruncateToMilliseconds(DateTime value) =>
        new DateTime(
            value.Ticks - value.Ticks % TimeSpan.TicksPerMillisecond,
            DateTimeKind.Utc);

    private static readonly DateTime CancellationAt =
        RequestedAt.AddMinutes(10);

    public DeletionCancellationMongoTests(MongoFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<(
        MongoUserRepository Users,
        MongoRecoveryTokenRepository Tokens,
        User User,
        string TokenHash)> PrepareAsync()
    {
        var database = _fixture.CreateDatabase();

        var users = new MongoUserRepository(database);
        var deletions = new MongoAccountDeletionRepository(database);
        var tokens = new MongoRecoveryTokenRepository(database);

        var user = new User
        {
            Email = $"{Guid.NewGuid():N}@example.com",
            DisplayName = "Synthetic Mongo Cancellation",
            Status = "active",
            SessionVersion = 1
        };

        await users.CreateAsync(user);

        var scheduled = await deletions.ScheduleDeletionAsync(
            user.Id, 1, RequestedAt);

        Assert.NotNull(scheduled);
        Assert.Equal("pending_deletion", scheduled.Status);
        Assert.Equal(2L, scheduled.SessionVersion);

        var tokenHash = NewTokenHash();

        var issued = await tokens.IssueAsync(
            user.Id,
            tokenHash,
            RequestedAt.AddMinutes(1),
            RequestedAt.AddMinutes(6));

        Assert.True(issued);

        return (users, tokens, user, tokenHash);
    }

    private static string NewTokenHash()
    {
        return Convert.ToHexString(
            RandomNumberGenerator.GetBytes(32));
    }

    [Fact]
    public async Task ParallelCancellation_OnlyOneMongoUpdateWins()
    {
        var (users, tokens, user, hash) = await PrepareAsync();

        // El token emitido dura cinco minutos.
        // Esta prueba debe ejecutarse antes de su vencimiento.
        var now = RequestedAt.AddMinutes(2);

        var tasks = Enumerable.Range(0, 16)
            .Select(_ => tokens.CancelAsync(hash, now))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        Assert.Single(results, result => result is not null);

        var persisted = await users.GetByIdAsync(user.Id);

        Assert.NotNull(persisted);
        Assert.Equal("active", persisted.Status);
        Assert.Equal(3L, persisted.SessionVersion);

        Assert.Null(persisted.RecoveryTokenHash);
        Assert.Null(persisted.RecoveryTokenExpiresAtUtc);
        Assert.Null(persisted.DeletionRequestedAtUtc);
        Assert.Null(persisted.DeleteAfterUtc);
    }

    [Fact]
    public async Task ReusedToken_CannotCancelAgain()
    {
        var (users, tokens, user, hash) = await PrepareAsync();

        var now = RequestedAt.AddMinutes(2);

        var first = await tokens.CancelAsync(hash, now);
        var second = await tokens.CancelAsync(hash, now);

        Assert.NotNull(first);
        Assert.Null(second);

        var persisted = await users.GetByIdAsync(user.Id);

        Assert.NotNull(persisted);
        Assert.Equal("active", persisted.Status);
        Assert.Equal(3L, persisted.SessionVersion);
    }

    [Fact]
    public async Task ExpiredToken_CannotRestoreAccount()
    {
        var (users, tokens, user, hash) = await PrepareAsync();

        // El token caduca exactamente a los seis minutos.
        var expiration = RequestedAt.AddMinutes(6);

        var result = await tokens.CancelAsync(hash, expiration);

        Assert.Null(result);

        var persisted = await users.GetByIdAsync(user.Id);

        Assert.NotNull(persisted);
        Assert.Equal("pending_deletion", persisted.Status);
        Assert.Equal(2L, persisted.SessionVersion);
    }

    [Fact]
    public async Task ReissuedToken_InvalidatesOldHash()
    {
        var (users, tokens, user, oldHash) = await PrepareAsync();

        var newHash = NewTokenHash();

        var issued = await tokens.IssueAsync(
            user.Id,
            newHash,
            RequestedAt.AddMinutes(2),
            RequestedAt.AddMinutes(7));

        Assert.True(issued);

        var oldResult = await tokens.CancelAsync(
            oldHash, RequestedAt.AddMinutes(3));

        Assert.Null(oldResult);

        var newResult = await tokens.CancelAsync(
            newHash, RequestedAt.AddMinutes(3));

        Assert.NotNull(newResult);

        var persisted = await users.GetByIdAsync(user.Id);

        Assert.NotNull(persisted);
        Assert.Equal("active", persisted.Status);
        Assert.Equal(3L, persisted.SessionVersion);
    }

    [Fact]
    public async Task DifferentUsers_CannotBeCancelledWithAnotherToken()
    {
        var database = _fixture.CreateDatabase();

        var users = new MongoUserRepository(database);
        var deletions = new MongoAccountDeletionRepository(database);
        var tokens = new MongoRecoveryTokenRepository(database);

        var firstUser = new User
        {
            Email = $"{Guid.NewGuid():N}@example.com",
            DisplayName = "User One",
            Status = "active",
            SessionVersion = 1
        };

        var secondUser = new User
        {
            Email = $"{Guid.NewGuid():N}@example.com",
            DisplayName = "User Two",
            Status = "active",
            SessionVersion = 1
        };

        await users.CreateAsync(firstUser);
        await users.CreateAsync(secondUser);

        Assert.NotNull(await deletions.ScheduleDeletionAsync(
            firstUser.Id, 1, RequestedAt));

        Assert.NotNull(await deletions.ScheduleDeletionAsync(
            secondUser.Id, 1, RequestedAt));

        var firstHash = NewTokenHash();
        var secondHash = NewTokenHash();

        Assert.True(await tokens.IssueAsync(
            firstUser.Id, firstHash,
            RequestedAt.AddMinutes(1),
            RequestedAt.AddMinutes(6)));

        Assert.True(await tokens.IssueAsync(
            secondUser.Id, secondHash,
            RequestedAt.AddMinutes(1),
            RequestedAt.AddMinutes(6)));

        var result = await tokens.CancelAsync(
            firstHash, RequestedAt.AddMinutes(2));

        Assert.NotNull(result);
        Assert.Equal(firstUser.Id, result.Id);

        var firstPersisted = await users.GetByIdAsync(firstUser.Id);
        var secondPersisted = await users.GetByIdAsync(secondUser.Id);

        Assert.NotNull(firstPersisted);
        Assert.NotNull(secondPersisted);

        Assert.Equal("active", firstPersisted.Status);
        Assert.Equal(3L, firstPersisted.SessionVersion);

        Assert.Equal("pending_deletion", secondPersisted.Status);
        Assert.Equal(2L, secondPersisted.SessionVersion);
        Assert.Equal(secondHash, secondPersisted.RecoveryTokenHash);
    }

    [Fact]
    public async Task ExactlyAtSevenDays_CancellationIsRejected()
    {
        var (users, tokens, user, _) = await PrepareAsync();

        var deadline = RequestedAt.AddDays(7);

        // Token nuevo que sigue vigente despues del plazo
        // de eliminacion: debe prevalecer el plazo de 7 dias.
        var validHash = NewTokenHash();

        var issued = await tokens.IssueAsync(
            user.Id,
            validHash,
            deadline.AddMinutes(-2),
            deadline.AddMinutes(3));

        Assert.True(issued);

        var result = await tokens.CancelAsync(
            validHash, deadline);

        Assert.Null(result);

        var persisted = await users.GetByIdAsync(user.Id);

        Assert.NotNull(persisted);
        Assert.Equal("pending_deletion", persisted.Status);
        Assert.Equal(2L, persisted.SessionVersion);
        Assert.Equal(deadline, persisted.DeleteAfterUtc);
        Assert.Equal(validHash, persisted.RecoveryTokenHash);
    }

    [Fact]
    public async Task OneSecondBeforeSevenDays_CancellationSucceeds()
    {
        var (users, tokens, user, _) = await PrepareAsync();

        var deadline = RequestedAt.AddDays(7);
        var hash = NewTokenHash();

        var issued = await tokens.IssueAsync(
            user.Id,
            hash,
            deadline.AddMinutes(-2),
            deadline.AddMinutes(3));

        Assert.True(issued);

        var result = await tokens.CancelAsync(
            hash, deadline.AddSeconds(-1));

        Assert.NotNull(result);

        var persisted = await users.GetByIdAsync(user.Id);

        Assert.NotNull(persisted);
        Assert.Equal("active", persisted.Status);
        Assert.Equal(3L, persisted.SessionVersion);
        Assert.Null(persisted.DeleteAfterUtc);
        Assert.Null(persisted.RecoveryTokenHash);
    }}