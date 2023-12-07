using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace Tax.Matters.API.Core.Security;

public class AuthorizationMiddlewareResultHandlerCore : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        // If the authorization was forbidden and the resource had a specific requirement,
        // provide a custom 404 response.
        if ((authorizeResult.Challenged && !authorizeResult.Succeeded) || (authorizeResult.Forbidden
            && authorizeResult.AuthorizationFailure!.FailedRequirements
                .OfType<Show404Requirement>().Any()))
        {
            // Return a 404 to make it appear as if the resource doesn't exist.
            context.Response.StatusCode = StatusCodes.Status404NotFound;


            await context.Response.WriteAsJsonAsync(new
            {
                Title = "Not Found",
                Description = "Endpoint not found",
                StatusCode = StatusCodes.Status404NotFound
            });

            return;
        }

        // Fall back to the default implementation.
        await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
