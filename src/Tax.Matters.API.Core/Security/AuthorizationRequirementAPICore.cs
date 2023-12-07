namespace Tax.Matters.API.Core.Security;

public class AuthorizationRequirementAPICore(string policy) : Show404Requirement
{
    public List<string> Policies { get; } = [.. policy.Split(['|', ','])];
}
