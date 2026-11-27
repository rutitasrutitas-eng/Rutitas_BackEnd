using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Rutitas.Api.Services;

public sealed class AccountDeletionAdmissionFilter(
    IAccountDeletionAdmission admission) : IActionFilter
{
    public void OnActionExecuting(
        ActionExecutingContext context)
    {
        if (admission.CanSchedule)
            return;

        context.HttpContext.Response.Headers.CacheControl =
            "no-store";

        context.Result = new ObjectResult(
            new ProblemDetails
            {
                Title = "Account deletion temporarily unavailable.",
                Status = 503
            })
        {
            StatusCode = 503,
            ContentTypes = { "application/problem+json" }
        };
    }

    public void OnActionExecuted(
        ActionExecutedContext context)
    {
    }
}
