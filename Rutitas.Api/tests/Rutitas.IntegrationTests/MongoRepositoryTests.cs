
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Rutitas.Domain.Entities;
using Rutitas.Infrastructure.Repositories;
using Testcontainers.MongoDb;
using Xunit;
using DomainRoute = Rutitas.Domain.Entities.Route;

namespace Rutitas.IntegrationTests;

// MongoDB temporal compartido por esta clase de pruebas.
public sealed class MongoFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _container =
        new MongoDbBuilder("mongo:7.0").Build();

    private MongoClient _client = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        _client = new MongoClient(
            _container.GetConnectionString());

        BsonClassMap.TryRegisterClassMap<DomainRoute>(cm =>
        {
            cm.AutoMap();

            cm.MapIdMember(x => x.Id)
                .SetSerializer(
                    new StringSerializer(BsonType.ObjectId));
        });
    }

    public IMongoDatabase CreateDatabase()
    {
        // Base diferente para cada prueba.
        return _client.GetDatabase(
            "rutitas_test_" +
            Guid.NewGuid().ToString("N"));
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}

public sealed class MongoRepositoryTests
    : IClassFixture<MongoFixture>
{
    private readonly MongoFixture _fixture;

    public MongoRepositoryTests(MongoFixture fixture)
    {
        _fixture = fixture;
    }

    // =========================================
    // PRUEBA 1 - USUARIOS REALES EN MONGODB
    // =========================================

    [Fact]
    public async Task Mongo_Usuario_SeGuardaYConsulta()
    {
        var database = _fixture.CreateDatabase();

        var repository =
            new MongoUserRepository(database);

        var user = new User
        {
            Email = "ALUMNO@EXAMPLE.COM",
            DisplayName = "Alumno Rutitas",
            Role = "user",
            Status = "active"
        };

        await repository.CreateAsync(user);

        var recovered =
            await repository.GetByEmailAsync(
                "alumno@example.com");

        Assert.NotNull(recovered);

        Assert.Equal(
            "alumno@example.com",
            recovered.Email);

        Assert.Equal("user", recovered.Role);

        var updated =
            await repository.UpdateProfileAsync(
                user.Id,
                "Alumno Actualizado",
                null,
                true,
                false);

        Assert.NotNull(updated);

        Assert.Equal(
            "Alumno Actualizado",
            updated.DisplayName);
    }

    // =========================================
    // PRUEBA 2 - CONSULTAR RUTAS REALES
    // =========================================

    [Fact]
    public async Task Mongo_Rutas_FiltraLasPublicas()
    {
        var database = _fixture.CreateDatabase();

        var collection =
            database.GetCollection<DomainRoute>("routes");

        await collection.InsertManyAsync(
            new[]
            {
                new DomainRoute
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Code = "RUT-001",
                    Name = "Coyote UTTT",
                    Status = "active",
                    Visibility = "public"
                },

                new DomainRoute
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Code = "RUT-002",
                    Name = "Ruta privada",
                    Status = "active",
                    Visibility = "private"
                }
            });

        var repository =
            new MongoRouteRepository(database);

        var result =
            await repository.GetPagedAsync(
                1, 20, null);

        Assert.Equal(1, result.Total);

        var route = Assert.Single(result.Items);

        Assert.Equal("RUT-001", route.Code);

        var detail =
            await repository.GetVisibleByIdAsync(route.Id);

        Assert.NotNull(detail);

        Assert.Equal("Coyote UTTT", detail.Name);
    }

    // =========================================
    // PRUEBA 3 - REVOCACION REAL DE JWT
    // =========================================

    [Fact]
    public async Task Mongo_Token_SeRevocaCorrectamente()
    {
        var database = _fixture.CreateDatabase();

        var repository =
            new MongoTokenRevocationRepository(database);

        var jti = Guid.NewGuid().ToString("N");

        var before =
            await repository.IsRevokedAsync(jti);

        Assert.False(before);

        await repository.RevokeAsync(
            jti,
            DateTime.UtcNow.AddMinutes(30));

        var after =
            await repository.IsRevokedAsync(jti);

        Assert.True(after);

        // Repetir la revocacion no debe duplicarla.
        await repository.RevokeAsync(
            jti,
            DateTime.UtcNow.AddMinutes(30));

        var count = await database
            .GetCollection<RevokedToken>("revoked_tokens")
            .CountDocumentsAsync(
                Builders<RevokedToken>.Filter
                    .Eq(x => x.Jti, jti));

        Assert.Equal(1, count);
    }
}
