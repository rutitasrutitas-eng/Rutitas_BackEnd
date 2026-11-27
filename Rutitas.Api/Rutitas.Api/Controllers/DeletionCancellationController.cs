using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MongoDB.Driver;
using Rutitas.Application.Interfaces;

namespace Rutitas.Api.Controllers;

[ApiController]
[Route("api/v1/users/me/deletion/cancel")]
public sealed class DeletionCancellationController : ControllerBase
{
    private readonly IRecoveryTokenRepository _tokens;
    private readonly TimeProvider _clock;

    public DeletionCancellationController(
        IRecoveryTokenRepository tokens,
        TimeProvider clock)
    {
        _tokens = tokens;
        _clock = clock;
    }

    [HttpPost]
    [AllowAnonymous]
    [EnableRateLimiting("deletion-token")]
    public async Task<IActionResult> Cancel(
        CancellationToken cancellationToken)
    {
        Response.Headers.CacheControl = "no-store";
        Response.Headers.Pragma = "no-cache";

        var authorization =
            Request.Headers.Authorization.ToString();

        if (!authorization.StartsWith(
            "Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();

        var token = authorization["Bearer ".Length..];

        if (token.Length != 64 || !token.All(Uri.IsHexDigit))
            return Unauthorized();

        var tokenHash = Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(token)));

        var nowUtc = _clock.GetUtcNow().UtcDateTime;

        try
        {
            // Comprobar que la credencial todavia es valida.
            var account = await _tokens.GetStatusAsync(
                tokenHash, nowUtc, cancellationToken);

            if (account is null)
                return Unauthorized();

            // La eliminacion ya comenzo o el plazo termino.
            if (account.Status != "pending_deletion" ||
                account.DeleteAfterUtc is null ||
                nowUtc >= account.DeleteAfterUtc.Value)
            {
                return StatusCode(410, new ProblemDetails
                {
                    Status = 410,
                    Title = "Account recovery is no longer available."
                });
            }

            // Mongo vuelve a verificar las condiciones de
            // manera atomica. La consulta anterior no basta.
            var restored = await _tokens.CancelAsync(
                tokenHash, nowUtc, cancellationToken);

            if (restored is null)
            {
                // La cancelacion atomica no se confirmo.
                // Reconsultar antes de clasificar el conflicto.
                var checkedAtUtc = _clock.GetUtcNow().UtcDateTime;

                var current = await _tokens.GetStatusAsync(
                    tokenHash, checkedAtUtc, cancellationToken);

                if (current is not null &&
                    (current.Status == "deleting" ||
                     (current.DeleteAfterUtc is not null &&
                      checkedAtUtc >= current.DeleteAfterUtc.Value)))
                {
                    return StatusCode(410, new ProblemDetails
                    {
                        Status = 410,
                        Title = "Account recovery is no longer available."
                    });
                }

                return Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Account deletion state changed."
                });
            }

            return Ok(new
            {
                status = "active",
                deletionCancelled = true,
                requiresLogin = true
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
                Title = "Cancellation temporarily unavailable."
            });
        }
    }
}