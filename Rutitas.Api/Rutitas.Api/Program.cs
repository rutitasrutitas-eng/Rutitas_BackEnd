
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Rutitas.Application.Interfaces;
using Rutitas.Domain.Entities;
using Rutitas.Infrastructure.Repositories;

// ========================================
// INICIALIZACION
// ========================================

var builder = WebApplication.CreateBuilder(args);

// Registro seguro del mapeo BSON.
// Evita registros duplicados cuando xUnit
// inicia varias instancias de la API.
BsonClassMap.TryRegisterClassMap<Rutitas.Domain.Entities.Route>(cm =>
{
    cm.AutoMap();

    cm.MapIdMember(x => x.Id)
        .SetSerializer(
            new StringSerializer(BsonType.ObjectId));
});

// ========================================
// 1. CONTROLADORES Y OPENAPI
// ========================================

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ========================================
// 2. CONFIGURACION JWT
// ========================================

// WebApplicationFactory incorpora configuracion
// adicional durante la construccion del host.
// Los valores se validan despues de Build().

string jwtIssuer = string.Empty;
string jwtAudience = string.Empty;
byte[] jwtKeyBytes = Array.Empty<byte>();

// ========================================
// 3. CONFIGURACION MONGODB
// ========================================

// Resolver la configuracion mediante DI permite
// usar MongoDB Atlas en desarrollo y configuracion
// aislada en pruebas automatizadas.

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var configuration =
        sp.GetRequiredService<IConfiguration>();

    var connectionString =
        configuration["MongoDb:ConnectionString"];

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "MongoDB connection string is missing.");
    }

    return new MongoClient(connectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var configuration =
        sp.GetRequiredService<IConfiguration>();

    var databaseName =
        configuration["MongoDb:DatabaseName"];

    if (string.IsNullOrWhiteSpace(databaseName))
    {
        throw new InvalidOperationException(
            "MongoDB database name is missing.");
    }

    var client =
        sp.GetRequiredService<IMongoClient>();

    return client.GetDatabase(databaseName);
});

// ========================================
// 4. REPOSITORIOS
// ========================================

builder.Services.AddScoped<
    IUserRepository,
    MongoUserRepository>();

builder.Services.AddScoped<
    ITokenRevocationRepository,
    MongoTokenRevocationRepository>();

builder.Services.AddScoped<
    IRouteRepository,
    MongoRouteRepository>();

builder.Services.AddScoped<
    IPasswordHasher<User>,
    PasswordHasher<User>>();

// ========================================
// 5. AUTENTICACION JWT
// ========================================

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                ValidateAudience = true,
                ValidAudience = jwtAudience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(jwtKeyBytes),

                ValidateLifetime = true,
                RequireExpirationTime = true,
                RequireSignedTokens = true,

                ValidAlgorithms =
                    [SecurityAlgorithms.HmacSha256],

                ClockSkew = TimeSpan.FromSeconds(30),

                NameClaimType = "sub",
                RoleClaimType = "role"
            };

        // Validar que el token no haya sido revocado.
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var jti = context.Principal?
                    .FindFirst("jti")?.Value;

                if (string.IsNullOrWhiteSpace(jti))
                {
                    context.Fail(
                        "Token identifier missing.");
                    return;
                }

                try
                {
                    var repository =
                        context.HttpContext.RequestServices
                            .GetRequiredService<
                                ITokenRevocationRepository>();

                    var revoked =
                        await repository.IsRevokedAsync(
                            jti,
                            context.HttpContext.RequestAborted);

                    if (revoked)
                    {
                        context.Fail(
                            "Token has been revoked.");
                    }
                }
                catch (Exception ex)
                {
                    // Si falla la comprobacion, se deniega
                    // la autenticacion por seguridad.
                    var logger = context.HttpContext
                        .RequestServices
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("JwtRevocation");

                    logger.LogError(
                        "Revocation check failed: {ErrorType}",
                        ex.GetType().Name);

                    context.Fail(
                        "Unable to validate token status.");
                }
            }
        };
    });

builder.Services.AddAuthorization();

// ========================================
// 6. RATE LIMITING
// ========================================

builder.Services.AddRateLimiter(options =>
{
    // Registro: 5 solicitudes por minuto.
    options.AddPolicy("register", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?
                .ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));

    // Login: 5 solicitudes por minuto.
    options.AddPolicy("login", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?
                .ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));

    // Consultas de rutas: 60 por minuto.
    options.AddPolicy("routes", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?
                .ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));

    options.RejectionStatusCode = 429;

    options.OnRejected = async (context, token) =>
    {
        var response = context.HttpContext.Response;

        response.StatusCode = 429;
        response.ContentType = "application/problem+json";
        response.Headers.RetryAfter = "60";

        await response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Title = "Too many requests.",
                Status = 429,
                Extensions =
                {
                    ["traceId"] =
                        context.HttpContext.TraceIdentifier
                }
            },
            cancellationToken: token);
    };
});

// ========================================
// 7. CONSTRUIR APLICACION
// ========================================

var app = builder.Build();

// ========================================
// 8. VALIDAR CONFIGURACION
// ========================================

// En este punto ya puede estar incorporada la
// configuracion de WebApplicationFactory.

var connectionString =
    app.Configuration["MongoDb:ConnectionString"];

var databaseName =
    app.Configuration["MongoDb:DatabaseName"];

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "MongoDB connection string is missing.");
}

if (string.IsNullOrWhiteSpace(databaseName))
{
    throw new InvalidOperationException(
        "MongoDB database name is missing.");
}

jwtIssuer =
    app.Configuration["Jwt:Issuer"] ?? string.Empty;

jwtAudience =
    app.Configuration["Jwt:Audience"] ?? string.Empty;

var jwtSigningKey =
    app.Configuration["Jwt:SigningKey"];

if (string.IsNullOrWhiteSpace(jwtIssuer) ||
    string.IsNullOrWhiteSpace(jwtAudience) ||
    string.IsNullOrWhiteSpace(jwtSigningKey))
{
    throw new InvalidOperationException(
        "JWT configuration is missing.");
}

try
{
    jwtKeyBytes =
        Convert.FromBase64String(jwtSigningKey);
}
catch (FormatException ex)
{
    throw new InvalidOperationException(
        "JWT signing key must be valid Base64.", ex);
}

if (jwtKeyBytes.Length < 32)
{
    throw new InvalidOperationException(
        "JWT signing key must contain at least 32 bytes.");
}

// ========================================
// 9. CREAR INDICES MONGODB
// ========================================

// Las pruebas actuales de WebApplicationFactory
// usan repositorios simulados y no necesitan
// crear indices en Atlas.
//
// En Development y Production se mantienen.

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();

    var database = scope.ServiceProvider
        .GetRequiredService<IMongoDatabase>();

    // Indice unico para correos.
    var users =
        database.GetCollection<User>("users");

    var emailIndex = new CreateIndexModel<User>(
        Builders<User>.IndexKeys
            .Ascending(x => x.Email),
        new CreateIndexOptions
        {
            Unique = true,
            Name = "ux_users_email"
        });

    await users.Indexes.CreateOneAsync(emailIndex);

    // Indice TTL para tokens revocados.
    var revokedTokens =
        database.GetCollection<RevokedToken>(
            "revoked_tokens");

    var ttlIndex = new CreateIndexModel<RevokedToken>(
        Builders<RevokedToken>.IndexKeys
            .Ascending(x => x.ExpiresAt),
        new CreateIndexOptions
        {
            Name = "ttl_revoked_tokens_expiration",
            ExpireAfter = TimeSpan.Zero
        });

    await revokedTokens.Indexes.CreateOneAsync(
        ttlIndex);
}

// ========================================
// 10. HERRAMIENTAS DE DESARROLLO
// ========================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapGet("/api/test/mongodb",
        async (
            IMongoDatabase database,
            ILogger<Program> logger,
            CancellationToken cancellationToken) =>
        {
            try
            {
                await database.RunCommandAsync<BsonDocument>(
                    new BsonDocument("ping", 1),
                    cancellationToken: cancellationToken);

                return Results.Ok(new
                {
                    connected = true,

                    database =
                        database.DatabaseNamespace.DatabaseName,

                    message =
                        "Rutitas connected to MongoDB Atlas"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(
                    "MongoDB connection failed: {ErrorType}",
                    ex.GetType().Name);

                return Results.Problem(
                    title: "Database connection failed",
                    statusCode: 503);
            }
        })
        .ExcludeFromDescription()
        .RequireHost("localhost:*");
}

// ========================================
// 11. MIDDLEWARE
// ========================================

app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// ========================================
// 12. ENDPOINTS
// ========================================

app.MapControllers();

// ========================================
// 13. INICIAR BACKEND
// ========================================

app.Run();

// Permite crear el host desde
// WebApplicationFactory<Program>.
public partial class Program { }
