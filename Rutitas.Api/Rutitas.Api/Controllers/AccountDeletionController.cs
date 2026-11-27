using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Rutitas.Api.Services;
using MongoDB.Driver;
using Rutitas.Application.Interfaces;

namespace Rutitas.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public sealed class AccountDeletionController : ControllerBase
{
    private readonly IAccountDeletionRepository _deletions;
    private readonly ILogger<AccountDeletionController> _logger;
    private readonly TimeProvider _timeProvider;

    public AccountDeletionController(
        IAccountDeletionRepository deletions,
        ILogger<AccountDeletionController> logger,
        TimeProvider timeProvider)
    {
        _deletions = deletions;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    // BACKEND-DELETE-001
    // DELETE /api/v1/users/me

    [HttpDelete("me")]
    [EnableRateLimiting("account-deletion")]
    [ServiceFilter(typeof(AccountDeletionAdmissionFilter))]
    public async Task<IActionResult> ScheduleDeletion(
        CancellationToken cancellationToken)
    {
        Response.Headers.CacheControl = "no-store";

        var userId = User.FindFirst(
            JwtRegisteredClaimNames.Sub)?.Value;

        var sessionVersionClaim = User.FindFirst(
            "sessionVersion")?.Value;

        if (string.IsNullOrWhiteSpace(userId) ||
            !long.TryParse(
                sessionVersionClaim,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var sessionVersion) ||
            sessionVersion < 1)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Invalid authentication token.",
                Status = 401
            });
        }

        var requestedAtUtc = _timeProvider.GetUtcNow().UtcDateTime;

        try
        {
            var result = await _deletions.ScheduleDeletionAsync(
                userId,
                sessionVersion,
                requestedAtUtc,
                cancellationToken);

            if (result is null)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Account state conflict.",
                    Status = 409
                });
            }

            if (result.DeletionRequestedAtUtc is null ||
                result.DeleteAfterUtc is null)
            {
                _logger.LogError(
                    "Account deletion returned incomplete dates.");

                return StatusCode(503, new ProblemDetails
                {
                    Title = "Account deletion temporarily unavailable.",
                    Status = 503
                });
            }

            return StatusCode(202, new
            {
                status = result.Status,
                requestedAtUtc = result.DeletionRequestedAtUtc.Value,
                deleteAfterUtc = result.DeleteAfterUtc.Value,
                recoveryAllowed = true
            });
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (MongoException ex)
        {
            _logger.LogError(
                ex,
                "Account deletion persistence failure.");

            return StatusCode(503, new ProblemDetails
            {
                Title = "Account deletion temporarily unavailable.",
                Status = 503
            });
        }
    }
}
