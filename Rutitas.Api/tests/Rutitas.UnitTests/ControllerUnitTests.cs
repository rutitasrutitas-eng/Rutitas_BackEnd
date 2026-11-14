
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Rutitas.Api.Controllers;
using Rutitas.Application.Interfaces;
using Rutitas.Domain.Entities;
using Xunit;
using DomainRoute = Rutitas.Domain.Entities.Route;

namespace Rutitas.UnitTests;

public class ControllerUnitTests
{
    private static readonly CancellationToken Ct =
        CancellationToken.None;

    // ========================================
    // CONFIGURACION DEL CONTROLADOR AUTH
    // ========================================

    private static AuthController Auth(
        Mock<IUserRepository>? users = null,
        Mock<ITokenRevocationRepository>? revocations = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Jwt:Issuer"] = "Rutitas.Api",
                    ["Jwt:Audience"] = "Rutitas.Mobile",
                    ["Jwt:SigningKey"] =
                        Convert.ToBase64String(new byte[32])
                })
            .Build();

        var controller = new AuthController(
            (users ?? new Mock<IUserRepository>()).Object,
            (revocations ??
                new Mock<ITokenRevocationRepository>()).Object,
            new PasswordHasher<User>(),
            configuration,
            NullLogger<AuthController>.Instance
        );

        // CORRECCION:
        // Simular una solicitud HTTP con URL valida.
        // Es necesaria para probar Created(201).

        var httpContext = new DefaultHttpContext();

        httpContext.Request.Scheme = "https";
        httpContext.Request.Host =
            new HostString("localhost");

        controller.ControllerContext =
            new ControllerContext
            {
                HttpContext = httpContext
            };

        return controller;
    }

    // ========================================
    // USUARIO SIMULADO
    // ========================================

    private static User MakeUser(
        string id = "507f1f77bcf86cd799439011")
    {
        return new User
        {
            Id = id,
            Email = "alumno@example.com",
            DisplayName = "Alumno",
            Role = "user",
            Status = "active"
        };
    }

    // ========================================
    // AUTENTICACION SIMULADA
    // ========================================

    private static void Authenticate(
        ControllerBase controller,
        string id = "507f1f77bcf86cd799439011")
    {
        var claims = new[]
        {
            new Claim("sub", id),
            new Claim("jti", "token123"),
            new Claim(
                "exp",
                DateTimeOffset.UtcNow
                    .AddMinutes(30)
                    .ToUnixTimeSeconds()
                    .ToString())
        };

        var identity =
            new ClaimsIdentity(claims, "test");

        var principal =
            new ClaimsPrincipal(identity);

        controller.ControllerContext =
            new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
    }

    // ========================================
    // ENDPOINT 1 - REGISTRO
    // ========================================

    [Theory]
    [InlineData("bad", "ValidPassword123!", "Alumno")]
    [InlineData("alumno@example.com", "short", "Alumno")]
    [InlineData("alumno@example.com", "ValidPassword123!", "")]
    public async Task Register_RejectsInvalidData(
        string email,
        string password,
        string name)
    {
        var response = await Auth().Register(
            new RegisterRequest(
                email,
                password,
                name,
                null),
            Ct);

        var result =
            Assert.IsAssignableFrom<ObjectResult>(response);

        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task Register_RejectsDuplicateEmail()
    {
        var repo = new Mock<IUserRepository>();

        repo.Setup(x => x.GetByEmailAsync(
                "alumno@example.com",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(MakeUser());

        var response = await Auth(repo).Register(
            new RegisterRequest(
                "alumno@example.com",
                "ValidPassword123!",
                "Alumno",
                null),
            Ct);

        Assert.IsType<ConflictObjectResult>(response);
    }

    [Fact]
    public async Task Register_ForcesUserRole()
    {
        var repo = new Mock<IUserRepository>();
        User? saved = null;

        repo.Setup(x => x.CreateAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>(
                (user, _) => saved = user)
            .Returns(Task.CompletedTask);

        var response = await Auth(repo).Register(
            new RegisterRequest(
                "ALUMNO@EXAMPLE.COM",
                "ValidPassword123!",
                "Alumno",
                null),
            Ct);

        // Verificar respuesta real HTTP 201.
        var created = Assert.IsType<CreatedResult>(response);

        Assert.Equal(201, created.StatusCode);

        // Verificar la URL generada.
        Assert.Equal(
            "https://localhost/api/v1/users/me",
            created.Location);

        // Verificar datos guardados.
        Assert.NotNull(saved);

        // La API debe asignar siempre el rol user.
        Assert.Equal("user", saved.Role);

        // El correo debe normalizarse.
        Assert.Equal(
            "alumno@example.com",
            saved.Email);

        // La contraseña no debe almacenarse en texto.
        Assert.NotEqual(
            "ValidPassword123!",
            saved.PasswordHash);

        Assert.False(
            string.IsNullOrWhiteSpace(saved.PasswordHash));

        // Confirmar que se solicitó guardar una sola vez.
        repo.Verify(
            x => x.CreateAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ========================================
    // ENDPOINT 2 - LOGIN
    // ========================================

    [Fact]
    public async Task Login_RejectsMissingAccount()
    {
        var response = await Auth().Login(
            new LoginRequest(
                "alumno@example.com",
                "AnyPass123!"),
            Ct);

        var result =
            Assert.IsType<ObjectResult>(response);

        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public async Task Login_RejectsIncorrectPassword()
    {
        var user = MakeUser();

        user.PasswordHash =
            new PasswordHasher<User>().HashPassword(
                user,
                "RightPassword123!");

        var repo = new Mock<IUserRepository>();

        repo.Setup(x => x.GetByEmailAsync(
                user.Email,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var response = await Auth(repo).Login(
            new LoginRequest(
                user.Email,
                "WrongPassword123!"),
            Ct);

        var result =
            Assert.IsType<ObjectResult>(response);

        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsJwtForValidUser()
    {
        var user = MakeUser();

        user.PasswordHash =
            new PasswordHasher<User>().HashPassword(
                user,
                "RightPassword123!");

        var repo = new Mock<IUserRepository>();

        repo.Setup(x => x.GetByEmailAsync(
                user.Email,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var response = await Auth(repo).Login(
            new LoginRequest(
                user.Email,
                "RightPassword123!"),
            Ct);

        var result =
            Assert.IsType<OkObjectResult>(response);

        var payload =
            Assert.IsType<LoginResponse>(result.Value);

        Assert.Equal("Bearer", payload.TokenType);
        Assert.Equal(1800, payload.ExpiresIn);
        Assert.NotEmpty(payload.AccessToken);
    }

    // ========================================
    // ENDPOINT 5 - LOGOUT
    // ========================================

    [Fact]
    public async Task Logout_RevokesToken()
    {
        var revocations =
            new Mock<ITokenRevocationRepository>();

        revocations.Setup(x => x.RevokeAsync(
                "token123",
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var controller =
            Auth(revocations: revocations);

        Authenticate(controller);

        var response =
            await controller.Logout(Ct);

        Assert.IsType<NoContentResult>(response);

        revocations.Verify(
            x => x.RevokeAsync(
                "token123",
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Logout_RejectsTokenWithoutJti()
    {
        var controller = Auth();

        var response =
            await controller.Logout(Ct);

        Assert.IsType<UnauthorizedObjectResult>(response);
    }

    // ========================================
    // ENDPOINT 3 - CONSULTAR PERFIL
    // ========================================

    [Fact]
    public async Task GetMe_ReturnsOwnProfile()
    {
        var repo = new Mock<IUserRepository>();

        repo.Setup(x => x.GetByIdAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(MakeUser());

        var controller = new UsersController(repo.Object);

        Authenticate(controller);

        var response =
            await controller.GetMe(Ct);

        var result =
            Assert.IsType<OkObjectResult>(response);

        Assert.IsType<UserProfileResponse>(result.Value);
    }

    [Fact]
    public async Task GetMe_RejectsMissingUser()
    {
        var controller = new UsersController(
            new Mock<IUserRepository>().Object);

        Authenticate(controller);

        var response =
            await controller.GetMe(Ct);

        Assert.IsType<UnauthorizedObjectResult>(response);
    }

    // ========================================
    // ENDPOINT 4 - ACTUALIZAR PERFIL
    // ========================================

    [Fact]
    public async Task UpdateMe_RejectsPrivilegeEscalation()
    {
        var controller = new UsersController(
            new Mock<IUserRepository>().Object);

        Authenticate(controller);

        using var json =
            JsonDocument.Parse(
                "{\"role\":\"admin\"}");

        var response = await controller.UpdateMe(
            json.RootElement,
            Ct);

        Assert.IsType<BadRequestObjectResult>(response);
    }

    [Fact]
    public async Task UpdateMe_UpdatesOnlyAllowedFields()
    {
        var repo = new Mock<IUserRepository>();

        repo.Setup(x => x.UpdateProfileAsync(
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(MakeUser());

        var controller = new UsersController(repo.Object);

        Authenticate(controller);

        using var json =
            JsonDocument.Parse(
                "{\"displayName\":\"Alumno Nuevo\"}");

        var response = await controller.UpdateMe(
            json.RootElement,
            Ct);

        Assert.IsType<OkObjectResult>(response);

        repo.Verify(x => x.UpdateProfileAsync(
                It.IsAny<string>(),
                "Alumno Nuevo",
                null,
                true,
                false,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ========================================
    // ENDPOINT 6 - LISTAR RUTAS
    // ========================================

    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 101)]
    [InlineData(-1, 20)]
    public async Task Routes_RejectsInvalidPagination(
        int page,
        int size)
    {
        var controller = new RoutesController(
            new Mock<IRouteRepository>().Object);

        var response = await controller.GetRoutes(
            page,
            size,
            null,
            Ct);

        Assert.IsType<BadRequestObjectResult>(response);
    }

    [Fact]
    public async Task Routes_ListsVisibleRoutes()
    {
        var repo = new Mock<IRouteRepository>();

        repo.Setup(x => x.GetPagedAsync(
                1,
                20,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new PagedRouteResult(
                    new[]
                    {
                        new DomainRoute
                        {
                            Id = "507f1f77bcf86cd799439012",
                            Name = "Coyote UTTT"
                        }
                    },
                    1));

        var controller =
            new RoutesController(repo.Object);

        var response = await controller.GetRoutes(
            1,
            20,
            null,
            Ct);

        var result =
            Assert.IsType<OkObjectResult>(response);

        var payload =
            Assert.IsType<PagedRoutesResponse>(result.Value);

        Assert.Single(payload.Items);
        Assert.Equal(1, payload.Total);
    }

    // ========================================
    // ENDPOINT 7 - DETALLE DE RUTA
    // ========================================

    [Theory]
    [InlineData("bad")]
    [InlineData("123")]
    public async Task RouteById_RejectsInvalidId(
        string id)
    {
        var controller = new RoutesController(
            new Mock<IRouteRepository>().Object);

        var response =
            await controller.GetRouteById(id, Ct);

        Assert.IsType<BadRequestObjectResult>(response);
    }

    [Fact]
    public async Task RouteById_Returns404IfMissing()
    {
        var controller = new RoutesController(
            new Mock<IRouteRepository>().Object);

        var response = await controller.GetRouteById(
            "507f1f77bcf86cd799439012",
            Ct);

        Assert.IsType<NotFoundObjectResult>(response);
    }

    [Fact]
    public async Task RouteById_ReturnsDetails()
    {
        var repo = new Mock<IRouteRepository>();

        repo.Setup(x => x.GetVisibleByIdAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new DomainRoute
                {
                    Id = "507f1f77bcf86cd799439012",
                    Name = "Coyote UTTT"
                });

        var controller =
            new RoutesController(repo.Object);

        var response =
            await controller.GetRouteById(
                "507f1f77bcf86cd799439012",
                Ct);

        var result =
            Assert.IsType<OkObjectResult>(response);

        Assert.IsType<RouteDetailResponse>(result.Value);
    }
}
