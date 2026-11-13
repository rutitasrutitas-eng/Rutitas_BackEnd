
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Rutitas.Application.Exceptions;
using Rutitas.Application.Interfaces;
using Rutitas.Domain.Entities;

namespace Rutitas.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly ITokenRevocationRepository _revocations;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserRepository users,
        ITokenRevocationRepository revocations,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _users = users;
        _revocations = revocations;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _logger = logger;
    }

    // ========================================
    // ENDPOINT 1: REGISTRO
    // POST /api/v1/auth/register
    // ========================================

    [HttpPost("register")]
    [EnableRateLimiting("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        var email =
            request.Email?.Trim().ToLowerInvariant() ?? "";

        var name = request.DisplayName?.Trim() ?? "";
        var password = request.Password ?? "";
        var phone = request.Phone?.Trim();

        if (email.Length == 0 ||
            email.Length > 254 ||
            !new EmailAddressAttribute().IsValid(email))
        {
            errors["email"] = ["Invalid email address."];
        }

        if (password.Length < 12 ||
            password.Length > 128)
        {
            errors["password"] =
                ["Password must contain 12 to 128 characters."];
        }

        if (name.Length < 1 || name.Length > 80)
        {
            errors["displayName"] =
                ["Display name must contain 1 to 80 characters."];
        }

        if (phone is not null &&
            !Regex.IsMatch(
                phone,
                @"^\+[1-9][0-9]{7,14}$"))
        {
            errors["phone"] =
                ["Phone must use international format."];
        }

        if (errors.Count > 0)
        {
            return ValidationProblem(
                new ValidationProblemDetails(errors)
                {
                    Status = 400,
                    Title = "Invalid registration data.",
                    Extensions =
                    {
                        ["traceId"] =
                            HttpContext.TraceIdentifier
                    }
                });
        }

        try
        {
            var existing = await _users.GetByEmailAsync(
                email,
                cancellationToken);

            if (existing is not null)
            {
                return ConflictProblem();
            }

            var now = DateTime.UtcNow;

            var user = new User
            {
                Email = email,
                DisplayName = name,
                Phone = phone,
                Role = "user",
                Status = "active",
                CreatedAt = now,
                UpdatedAt = now
            };

            user.PasswordHash =
                _passwordHasher.HashPassword(user, password);

            await _users.CreateAsync(
                user,
                cancellationToken);

            var location = new Uri(
                $"{Request.Scheme}://{Request.Host}/api/v1/users/me");

            return Created(location, ToProfile(user));
        }
        catch (DuplicateEmailException)
        {
            return ConflictProblem();
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Registration failed: {ErrorType}",
                ex.GetType().Name);

            return Problem(
                statusCode: 500,
                title: "Internal server error.");
        }
    }

    // ========================================
    // ENDPOINT 2: LOGIN
    // POST /api/v1/auth/login
    // ========================================

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var email =
            request.Email?.Trim().ToLowerInvariant() ?? "";

        var password = request.Password ?? "";

        var errors = new Dictionary<string, string[]>();

        if (email.Length == 0 ||
            email.Length > 254 ||
            !new EmailAddressAttribute().IsValid(email))
        {
            errors["email"] = ["Invalid email address."];
        }

        if (password.Length == 0 ||
            password.Length > 128)
        {
            errors["password"] =
                ["Password must contain 1 to 128 characters."];
        }

        if (errors.Count > 0)
        {
            return ValidationProblem(
                new ValidationProblemDetails(errors)
                {
                    Status = 400,
                    Title = "Invalid login data.",
                    Extensions =
                    {
                        ["traceId"] =
                            HttpContext.TraceIdentifier
                    }
                });
        }

        try
        {
            var user = await _users.GetByEmailAsync(
                email,
                cancellationToken);

            if (user is null)
            {
                return InvalidCredentials();
            }

            var verification =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    password);

            if (verification ==
                    PasswordVerificationResult.Failed ||
                user.Status != "active")
            {
                return InvalidCredentials();
            }

            var token = GenerateJwt(user);

            return Ok(new LoginResponse(
                AccessToken: token,
                TokenType: "Bearer",
                ExpiresIn: 1800,
                User: ToProfile(user)));
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Login failed: {ErrorType}",
                ex.GetType().Name);

            return Problem(
                statusCode: 500,
                title: "Internal server error.");
        }
    }

    // ========================================
    // ENDPOINT 5: LOGOUT
    // POST /api/v1/auth/logout
    // ========================================

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
    {
        // Estas claims proceden de un JWT ya validado.
        var jti = User.FindFirst(
            JwtRegisteredClaimNames.Jti)?.Value;

        var expiration = User.FindFirst(
            JwtRegisteredClaimNames.Exp)?.Value;

        if (string.IsNullOrWhiteSpace(jti) ||
            !long.TryParse(
                expiration,
                out var expirationSeconds))
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Invalid authentication token.",
                Status = 401
            });
        }

        DateTime expiresAt;

        try
        {
            expiresAt = DateTimeOffset
                .FromUnixTimeSeconds(expirationSeconds)
                .UtcDateTime;
        }
        catch (ArgumentOutOfRangeException)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Invalid token expiration.",
                Status = 401
            });
        }

        try
        {
            await _revocations.RevokeAsync(
                jti,
                expiresAt,
                cancellationToken);

            // Logout exitoso, sin cuerpo de respuesta.
            return NoContent();
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Token revocation failed: {ErrorType}",
                ex.GetType().Name);

            // No reportar exito si no se guardo
            // la revocacion en MongoDB.
            return Problem(
                statusCode: 503,
                title: "Logout temporarily unavailable.");
        }
    }

    // ========================================
    // GENERAR JWT
    // ========================================

    private string GenerateJwt(User user)
    {
        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "JWT issuer is missing.");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "JWT audience is missing.");

        var secret = _configuration["Jwt:SigningKey"]
            ?? throw new InvalidOperationException(
                "JWT signing key is missing.");

        var keyBytes = Convert.FromBase64String(secret);

        if (keyBytes.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT signing key must contain at least 32 bytes.");
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;

        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id),

            new Claim(
                JwtRegisteredClaimNames.Email,
                user.Email),

            new Claim("role", user.Role),

            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString("N")),

            new Claim(
                JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(now)
                    .ToUnixTimeSeconds()
                    .ToString(),
                ClaimValueTypes.Integer64)
        };

        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(jwt);
    }

    // ========================================
    // RESPUESTAS COMUNES
    // ========================================

    private IActionResult InvalidCredentials()
    {
        return Problem(
            statusCode: 401,
            title: "Invalid email or password.");
    }

    private IActionResult ConflictProblem()
    {
        return Conflict(new ProblemDetails
        {
            Type =
                "https://tools.ietf.org/html/rfc9110#section-15.5.10",
            Title = "Email already registered.",
            Status = 409,
            Extensions =
            {
                ["traceId"] = HttpContext.TraceIdentifier
            }
        });
    }

    private static UserProfileResponse ToProfile(User user)
    {
        return new UserProfileResponse(
            user.Id,
            user.Email,
            user.DisplayName,
            user.Phone,
            user.Role,
            user.CreatedAt);
    }
}

// ========================================
// MODELOS
// ========================================

public sealed record RegisterRequest(
    string? Email,
    string? Password,
    string? DisplayName,
    string? Phone);

public sealed record LoginRequest(
    string? Email,
    string? Password);

public sealed record UserProfileResponse(
    string Id,
    string Email,
    string DisplayName,
    string? Phone,
    string Role,
    DateTime CreatedAt);

public sealed record LoginResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    UserProfileResponse User);
