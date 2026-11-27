using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MongoDB.Driver;
using Rutitas.Application.Interfaces;
using Rutitas.Domain.Entities;

namespace Rutitas.Api.Controllers;

[ApiController]
[Route("api/v1/auth/deletion")]
public sealed class DeletionRecoveryController : ControllerBase
{
    private const int RecoveryTokenSeconds = 300;

    // Usuario ficticio exclusivamente para igualar el trabajo
    // criptografico cuando el correo no existe.
    // Nunca se almacena en MongoDB.
    private static readonly User DummyUser = new()
    {
        Email = "nonexistent-recovery@example.invalid",
        Status = "inactive"
    };

    private static readonly string DummyPasswordHash =
        new PasswordHasher<User>().HashPassword(
            DummyUser,
            "SyntheticDummyPasswordNotForAuthentication!");

    private readonly IUserRepository _users;
    private readonly IRecoveryTokenRepository _tokens;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly TimeProvider _clock;

    public DeletionRecoveryController(
        IUserRepository users,
        IRecoveryTokenRepository tokens,
        IPasswordHasher<User> passwordHasher,
        TimeProvider clock)
    {
        _users = users;
        _tokens = tokens;
        _passwordHasher = passwordHasher;
        _clock = clock;
    }

    [HttpPost("recovery")]
    [AllowAnonymous]
    [EnableRateLimiting("deletion-recovery")]
    public async Task<IActionResult> Recover(
        [FromBody] RecoveryCredentials request,
        CancellationToken cancellationToken)
    {
        Response.Headers.CacheControl = "no-store";
        Response.Headers.Pragma = "no-cache";

        var email = request.Email?.Trim().ToLowerInvariant();
        var password = request.Password;

        if (string.IsNullOrWhiteSpace(email) ||
            email.Length > 254 ||
            string.IsNullOrWhiteSpace(password) ||
            password.Length > 128)
        {
            return BadRequest(new ProblemDetails
            {
                Status = 400,
                Title = "Invalid recovery data."
            });
        }

        try
        {
            var user = await _users.GetByEmailAsync(
                email, cancellationToken);

            if (user is null)
            {
                // Ejecutar una verificacion costosa tambien
                // cuando la cuenta no existe.
                _passwordHasher.VerifyHashedPassword(
                    DummyUser,
                    DummyPasswordHash,
                    password);

                return InvalidCredentials();
            }

            var verification =
                _passwordHasher.VerifyHashedPassword(
                    user, user.PasswordHash, password);

            var now = _clock.GetUtcNow().UtcDateTime;

            if (verification == PasswordVerificationResult.Failed ||
                user.Status != "pending_deletion" ||
                user.DeleteAfterUtc is null ||
                now >= user.DeleteAfterUtc.Value)
            {
                return InvalidCredentials();
            }

            // 32 bytes aleatorios = 256 bits.
            var recoveryToken = Convert.ToHexString(
                RandomNumberGenerator.GetBytes(32));

            var hash = Convert.ToHexString(
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(recoveryToken)));

            var expiry = now.AddSeconds(RecoveryTokenSeconds);

            var stored = await _tokens.IssueAsync(
                user.Id,
                hash,
                now,
                expiry,
                cancellationToken);

            if (!stored)
                return InvalidCredentials();

            return Ok(new
            {
                recoveryToken,
                tokenType = "Bearer",
                expiresIn = RecoveryTokenSeconds
            });
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (MongoException)
        {
            return StatusCode(503, new ProblemDetails
            {
                Status = 503,
                Title = "Recovery temporarily unavailable."
            });
        }
    }

    private IActionResult InvalidCredentials()
    {
        return Unauthorized(new ProblemDetails
        {
            Status = 401,
            Title = "Invalid credentials or recovery unavailable."
        });
    }
}

public sealed record RecoveryCredentials(
    string? Email,
    string? Password);