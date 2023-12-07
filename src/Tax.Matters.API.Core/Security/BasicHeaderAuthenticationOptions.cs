using Microsoft.AspNetCore.Authentication;

namespace Tax.Matters.API.Core.Security;

public class BasicHeaderAuthenticationOptions : AuthenticationSchemeOptions
{
    public string AuthenticationType { get; set; } = default!;
}
