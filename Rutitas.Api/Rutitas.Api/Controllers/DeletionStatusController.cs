using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MongoDB.Driver;
using Rutitas.Application.Interfaces;

namespace Rutitas.Api.Controllers;

[ApiController]
[Route("api/v1/users/me/deletion")]
public sealed class DeletionStatusController : ControllerBase
{
    private readonly IRecoveryTokenRepository _tokens;
    private readonly TimeProvider _clock;

    public DeletionStatusController(
        IRecoveryTokenRepository tokens,
        TimeProvider clock)
    {
        _tokens = tokens;
        _clock = clock;
    }

    [HttpGet]
    [AllowAnonymous]
    [EnableRateLimiting("deletion-token")]
    public async Task<IActionResult> GetStatus(
        CancellationToken cancellationToken)
    {
        Response.Headers.CacheControl = "no-store";
        Response.Headers.Pragma = "no-cache";

        var authorization = Request.Headers.Authorization.ToString();

        if (!authorization.StartsWith(
                "Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized();
        }

        var token = authorization["Bearer ".Length..];

        // Nuestra credencial opaca contiene 32 bytes en hexadecimal.
        if (token.Length != 64 ||
            !token.All(Uri.IsHexDigit))
        {
            return Unauthorized();
        }

        var tokenHash = Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(token)));

        var now = _clock.GetUtcNow().UtcDateTime;

        try
        {
            var account = await _tokens.GetStatusAsync(
                tokenHash,
                now,
                cancellationToken);

            if (account is null ||
                account.DeletionRequestedAtUtc is null ||
                account.DeleteAfterUtc is null)
            {
                return Unauthorized();
            }

            if (account.Status == "deleting")
            {
                return StatusCode(410, new ProblemDetails
                {
                    Status = 410,
                    Title = "Account recovery is no longer available."
                });
            }

            var recoveryAllowed =
                account.Status == "pending_deletion" &&
                now < account.DeleteAfterUtc.Value;

            return Ok(new
            {
                status = account.Status,
                requestedAtUtc =
                    account.DeletionRequestedAtUtc.Value,
                deleteAfterUtc =
                    account.DeleteAfterUtc.Value,
                recoveryAllowed
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
                Title = "Deletion status temporarily unavailable."
            });
        }
    }
}