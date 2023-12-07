using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Tax.Matters.API.Core.Security;

public class AuthorizationHandlerAPICore : AuthorizationHandler<AuthorizationRequirementAPICore>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, AuthorizationRequirementAPICore requirement)
    {
        var nameClaim = context.User.FindFirst(c => c.Type == ClaimTypes.Name);

        if (nameClaim is null)
        {
            return Task.CompletedTask;
        }

        if (requirement.Policies.Any(p => p.Equals(nameClaim.Value, StringComparison.OrdinalIgnoreCase)))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
