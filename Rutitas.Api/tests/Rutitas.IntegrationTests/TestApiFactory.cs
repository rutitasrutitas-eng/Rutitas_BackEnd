using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rutitas.Application.Interfaces;
using Rutitas.Domain.Entities;

namespace Rutitas.IntegrationTests;

// Fixture entirely in memory. Never reads user secrets and never touches Atlas.
public sealed class TestApiFactory : WebApplicationFactory<Program>
{
    public FakeUsers Users { get; } = new();
    public FakeRevocations Revocations { get; } = new();
    public FakeRoutes Routes { get; } = new();
    public FakeAccountDeletions Deletions { get; } = new();
    public FakeRecoveryTokens RecoveryTokens { get; } = new();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Deletions.FindUserAsync = Users.GetByIdAsync;
        RecoveryTokens.FindUserAsync = Users.GetByIdAsync;
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_,config) => config.AddInMemoryCollection(new Dictionary<string,string?> {
            ["MongoDb:ConnectionString"]="mongodb://127.0.0.1:1",
            ["MongoDb:DatabaseName"]="rutitas_testing_never_used",
            ["Jwt:Issuer"]="Rutitas.Api",
            ["Jwt:Audience"]="Rutitas.Mobile",
            ["Jwt:SigningKey"]=Convert.ToBase64String(new byte[32])
        }));
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IUserRepository>();services.AddSingleton<IUserRepository>(Users);
            services.RemoveAll<ITokenRevocationRepository>();services.AddSingleton<ITokenRevocationRepository>(Revocations);
            services.RemoveAll<IRouteRepository>();services.AddSingleton<IRouteRepository>(Routes);
            services.RemoveAll<IAccountDeletionRepository>();
            services.AddSingleton<IAccountDeletionRepository>(Deletions);
            services.RemoveAll<IRecoveryTokenRepository>();
            services.AddSingleton<IRecoveryTokenRepository>(RecoveryTokens);
        });
    }
}
public sealed class FakeUsers : IUserRepository
{
    private readonly Dictionary<string,User> _users = new(StringComparer.OrdinalIgnoreCase);
    public Task<User?> GetByIdAsync(string id,CancellationToken ct=default) => Task.FromResult(_users.Values.FirstOrDefault(x=>x.Id==id));
    public Task<User?> GetByEmailAsync(string email,CancellationToken ct=default) => Task.FromResult(_users.GetValueOrDefault(email));
    public Task CreateAsync(User user,CancellationToken ct=default){user.Id=Guid.NewGuid().ToString("N");_users[user.Email]=user;return Task.CompletedTask;}
    public Task<User?> UpdateProfileAsync(string id,string? name,string? phone,bool updateName,bool updatePhone,CancellationToken ct=default)
    {
        var u=_users.Values.FirstOrDefault(x=>x.Id==id);
        if(u!=null){if(updateName)u.DisplayName=name!;if(updatePhone)u.Phone=phone;}return Task.FromResult(u);
    }
}
public sealed class FakeRevocations : ITokenRevocationRepository
{
    private readonly HashSet<string> _revoked=new();
    public bool FailOnRead { get; set; }
    public bool FailOnWrite { get; set; }
    public Task RevokeAsync(
        string jti,
        DateTime expiresAt,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (FailOnWrite)
        {
            return Task.FromException(
                new InvalidOperationException(
                    "Synthetic MongoDB write failure"));
        }

        lock (_revoked)
        {
            _revoked.Add(jti);
        }

        return Task.CompletedTask;
    }
    public Task<bool> IsRevokedAsync(string jti,CancellationToken ct=default){if(FailOnRead)throw new InvalidOperationException("Synthetic MongoDB failure");lock(_revoked)return Task.FromResult(_revoked.Contains(jti));}
}
public sealed class FakeRoutes : IRouteRepository
{
    public Task<PagedRouteResult> GetPagedAsync(int page,int size,string? q,CancellationToken ct=default) => Task.FromResult(new PagedRouteResult(new [] {new Route {Id="507f1f77bcf86cd799439012",Name="Coyote UTTT"}},1));
    public Task<Route?> GetVisibleByIdAsync(string id,CancellationToken ct=default) => Task.FromResult<Route?>(id=="507f1f77bcf86cd799439012"?new Route {Id=id,Name="Coyote UTTT"}:null);
}

public sealed class FakeAccountDeletions : IAccountDeletionRepository
{
    private readonly object _sync = new();
    private readonly Dictionary<string, User> _scheduled = new();

    public Func<string, CancellationToken, Task<User?>>?
        FindUserAsync { get; set; }

    public async Task<User?> ScheduleDeletionAsync(
        string userId,
        long expectedSessionVersion,
        DateTime requestedAtUtc,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (FindUserAsync is null)
            throw new InvalidOperationException(
                "Synthetic user lookup is missing.");

        var user = await FindUserAsync(
            userId, cancellationToken);

        if (user is null)
            return null;

        lock (_sync)
        {
            if (user.Status != "active" ||
                user.SessionVersion != expectedSessionVersion ||
                expectedSessionVersion < 1)
                return null;

            user.Status = "pending_deletion";
            user.DeletionRequestedAtUtc = requestedAtUtc;
            user.DeleteAfterUtc = requestedAtUtc.AddDays(7);
            user.UpdatedAt = requestedAtUtc;
            user.SessionVersion++;

            _scheduled[userId] = user;

            return user;
        }
    }
}
public sealed class FakeRecoveryTokens : IRecoveryTokenRepository
{
    private readonly object _sync = new();

    private readonly Dictionary<string, string> _userByTokenHash =
        new(StringComparer.Ordinal);

    public string? LastUserId { get; private set; }
    public string? LastTokenHash { get; private set; }
    public DateTime? LastIssuedAtUtc { get; private set; }
    public DateTime? LastExpiresAtUtc { get; private set; }

    public Func<string, CancellationToken, Task<User?>>?
        FindUserAsync { get; set; }

    public async Task<bool> IssueAsync(
        string userId,
        string tokenHash,
        DateTime issuedAtUtc,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (FindUserAsync is null)
            throw new InvalidOperationException(
                "Synthetic user lookup missing.");

        var user = await FindUserAsync(
            userId, cancellationToken);

        if (user is null)
            return false;

        lock (_sync)
        {
            if (user.Status != "pending_deletion" ||
                user.DeleteAfterUtc is null ||
                issuedAtUtc >= user.DeleteAfterUtc.Value ||
                expiresAtUtc <= issuedAtUtc)
                return false;

            if (user.RecoveryTokenHash is not null)
                _userByTokenHash.Remove(user.RecoveryTokenHash);

            user.RecoveryTokenHash = tokenHash;
            user.RecoveryTokenExpiresAtUtc = expiresAtUtc;

            _userByTokenHash[tokenHash] = userId;

            LastUserId = userId;
            LastTokenHash = tokenHash;
            LastIssuedAtUtc = issuedAtUtc;
            LastExpiresAtUtc = expiresAtUtc;

            return true;
        }
    }

    public bool FailOnStatusRead { get; set; }

    public async Task<User?> GetStatusAsync(
        string tokenHash,
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (FailOnStatusRead)
            throw new MongoDB.Driver.MongoException(
                "Synthetic MongoDB status failure");

        if (FindUserAsync is null)
            return null;

        string userId;

        lock (_sync)
        {
            if (!_userByTokenHash.TryGetValue(
                tokenHash, out var foundUserId))
                return null;

            userId = foundUserId;
        }

        var user = await FindUserAsync(userId, cancellationToken);

        lock (_sync)
        {
            if (user is null ||
                user.RecoveryTokenHash != tokenHash ||
                user.RecoveryTokenExpiresAtUtc is null ||
                nowUtc >= user.RecoveryTokenExpiresAtUtc.Value ||
                (user.Status != "pending_deletion" &&
                 user.Status != "deleting"))
                return null;

            return user;
        }
    }

    public bool ForceCancelConflict { get; set; }

    public Action<User>? BeforeCancelValidation { get; set; }

    public async Task<User?> CancelAsync(
        string tokenHash,
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (FindUserAsync is null)
            return null;

        string userId;

        lock (_sync)
        {
            if (!_userByTokenHash.TryGetValue(
                tokenHash, out var foundUserId))
                return null;

            userId = foundUserId;
        }

        var user = await FindUserAsync(userId, cancellationToken);

        if (user is not null)
            BeforeCancelValidation?.Invoke(user);

        lock (_sync)
        {
            if (ForceCancelConflict)
                return null;

            if (user is null ||
                user.RecoveryTokenHash != tokenHash ||
                user.RecoveryTokenExpiresAtUtc is null ||
                nowUtc >= user.RecoveryTokenExpiresAtUtc.Value ||
                user.DeleteAfterUtc is null ||
                nowUtc >= user.DeleteAfterUtc.Value ||
                user.Status != "pending_deletion")
                return null;

            user.Status = "active";
            user.SessionVersion++;
            user.UpdatedAt = nowUtc;

            user.RecoveryTokenHash = null;
            user.RecoveryTokenExpiresAtUtc = null;
            user.DeletionRequestedAtUtc = null;
            user.DeleteAfterUtc = null;

            _userByTokenHash.Remove(tokenHash);

            return user;
        }
    }
}