
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rutitas.Application.Interfaces;
using Rutitas.Domain.Entities;

namespace Rutitas.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly IUserRepository _users;

    public UsersController(IUserRepository users)
    {
        _users = users;
    }

    // ========================================
    // ENDPOINT 3: CONSULTAR MI PERFIL
    // GET /api/v1/users/me
    // ========================================

    [HttpGet("me")]
    public async Task<IActionResult> GetMe(
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(
            JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return UnauthorizedProblem();
        }

        var user = await _users.GetByIdAsync(
            userId, cancellationToken);

        if (user is null || user.Status != "active")
        {
            return UnauthorizedProblem();
        }

        return Ok(ToProfile(user));
    }

    // ========================================
    // ENDPOINT 4: ACTUALIZAR MI PERFIL
    // PATCH /api/v1/users/me
    // ========================================

    [HttpPatch("me")]
    public async Task<IActionResult> UpdateMe(
        [FromBody] JsonElement request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(
            JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return UnauthorizedProblem();
        }

        var errors = new Dictionary<string, string[]>();

        if (request.ValueKind != JsonValueKind.Object)
        {
            errors["body"] =
                ["Request body must be a JSON object."];

            return InvalidRequest(errors);
        }

        bool updateDisplayName = false;
        bool updatePhone = false;

        string? displayName = null;
        string? phone = null;

        foreach (var property in request.EnumerateObject())
        {
            switch (property.Name)
            {
                case "displayName":
                    updateDisplayName = true;

                    if (property.Value.ValueKind !=
                        JsonValueKind.String)
                    {
                        errors["displayName"] =
                            ["Display name must be a string."];
                        break;
                    }

                    displayName =
                        property.Value.GetString()?.Trim();

                    if (string.IsNullOrWhiteSpace(displayName) ||
                        displayName.Length > 80)
                    {
                        errors["displayName"] =
                            ["Display name must contain 1 to 80 characters."];
                    }

                    break;

                case "phone":
                    updatePhone = true;

                    if (property.Value.ValueKind ==
                        JsonValueKind.Null)
                    {
                        phone = null;
                    }
                    else if (property.Value.ValueKind ==
                             JsonValueKind.String)
                    {
                        phone = property.Value.GetString()?.Trim();

                        if (phone is null ||
                            !Regex.IsMatch(
                                phone,
                                @"^\+[1-9][0-9]{7,14}$"))
                        {
                            errors["phone"] =
                                ["Phone must use international format."];
                        }
                    }
                    else
                    {
                        errors["phone"] =
                            ["Phone must be a string or null."];
                    }

                    break;

                default:
                    errors[property.Name] =
                        ["This field cannot be updated."];
                    break;
            }
        }

        if (!updateDisplayName && !updatePhone)
        {
            errors["body"] =
                ["Provide displayName or phone."];
        }

        if (errors.Count > 0)
        {
            return InvalidRequest(errors);
        }

        var updatedUser = await _users.UpdateProfileAsync(
            userId,
            displayName,
            phone,
            updateDisplayName,
            updatePhone,
            cancellationToken);

        if (updatedUser is null)
        {
            return UnauthorizedProblem();
        }

        return Ok(ToProfile(updatedUser));
    }

    // ========================================
    // RESPUESTAS
    // ========================================

    private IActionResult UnauthorizedProblem()
    {
        return Unauthorized(new ProblemDetails
        {
            Title = "User is not available.",
            Status = 401
        });
    }

    private IActionResult InvalidRequest(
        Dictionary<string, string[]> errors)
    {
        return BadRequest(new ValidationProblemDetails(errors)
        {
            Title = "Invalid profile data.",
            Status = 400
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
