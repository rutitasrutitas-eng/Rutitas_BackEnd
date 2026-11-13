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
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
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
        });
    }
}
public sealed class FakeUsers : IUserRepository
{
    private readonly Dictionary<string,User> _users = new(StringComparer.OrdinalIgnoreCase);
    public Task<User?> GetByIdAsync(string id,CancellationToken ct=default) => Task.FromResult(_users.Values.FirstOrDefault(x=>x.Id==id));
    public Task<User?> GetByEmailAsync(string email,CancellationToken ct=default) => Task.FromResult(_users.GetValueOrDefault(email));
    public Task CreateAsync(User user,CancellationToken ct=default){user.Id=Guid.NewGuid().ToString("N");_users[user.Email]=user;return Task.CompletedTask;}
    public Task UpdateAsync(User user,CancellationToken ct=default){_users[user.Email]=user;return Task.CompletedTask;}
    public Task<User?> UpdateProfileAsync(string id,string? name,string? phone,bool updateName,bool updatePhone,CancellationToken ct=default)
    {
        var u=_users.Values.FirstOrDefault(x=>x.Id==id);
        if(u!=null){if(updateName)u.DisplayName=name!;if(updatePhone)u.Phone=phone;}return Task.FromResult(u);
    }
}
public sealed class FakeRevocations : ITokenRevocationRepository
{
    private readonly HashSet<string> _revoked=new();
    public Task RevokeAsync(string jti,DateTime expiresAt,CancellationToken ct=default){lock(_revoked)_revoked.Add(jti);return Task.CompletedTask;}
    public Task<bool> IsRevokedAsync(string jti,CancellationToken ct=default){lock(_revoked)return Task.FromResult(_revoked.Contains(jti));}
}
public sealed class FakeRoutes : IRouteRepository
{
    public Task<PagedRouteResult> GetPagedAsync(int page,int size,string? q,CancellationToken ct=default) => Task.FromResult(new PagedRouteResult(new [] {new Route {Id="507f1f77bcf86cd799439012",Name="Coyote UTTT"}},1));
    public Task<Route?> GetVisibleByIdAsync(string id,CancellationToken ct=default) => Task.FromResult<Route?>(id=="507f1f77bcf86cd799439012"?new Route {Id=id,Name="Coyote UTTT"}:null);
}
