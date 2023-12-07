using Microsoft.AspNetCore.Authentication;

namespace Tax.Matters.API.Core.Security;

public static class AuthenticationExtensions
{
    public static AuthenticationBuilder AddBasicHeaderAuthentication(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        string displayName,
        Action<BasicHeaderAuthenticationOptions> configureOptions)
    {
        return builder.AddScheme<BasicHeaderAuthenticationOptions, BasicHeaderAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
    }
}
