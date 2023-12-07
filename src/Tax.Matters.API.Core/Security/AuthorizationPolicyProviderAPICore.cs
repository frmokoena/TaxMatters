using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Tax.Matters.API.Core.Security;

public class AuthorizationPolicyProviderAPICore(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = new AuthorizationPolicyBuilder();
        policy.AddRequirements(new AuthorizationRequirementAPICore(policyName));
        return Task.FromResult<AuthorizationPolicy?>(policy.Build());
    }
}
