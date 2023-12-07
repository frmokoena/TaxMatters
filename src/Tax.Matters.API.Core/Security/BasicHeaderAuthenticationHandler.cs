using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Tax.Matters.API.Core.Security;

public class BasicHeaderAuthenticationHandler : AuthenticationHandler<BasicHeaderAuthenticationOptions>
{
    private readonly ClientListOptions _clientsOptions;
    private const string AuthKey = "Authorization";

    public BasicHeaderAuthenticationHandler(
        IOptionsMonitor<BasicHeaderAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IOptions<ClientListOptions> apiKeyListOptionsAccessor) : base(options, logger, encoder)
    {
        _clientsOptions = apiKeyListOptionsAccessor?.Value
            ?? throw new ArgumentNullException(nameof(apiKeyListOptionsAccessor));
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Headers.TryGetValue(AuthKey, out var authString))
        {
            return await Task.FromResult(AuthenticateResult.Fail("Authorization token was not provided"));
        }

        var authCredential = DecodeBasicAuthString(authString!);

        if (authCredential == null)
        {
            return await Task.FromResult(AuthenticateResult.Fail("Invalid client"));
        }

        var client =
            _clientsOptions?.Clients?.FirstOrDefault(
                client => client.Name.Equals(authCredential.Username, StringComparison.OrdinalIgnoreCase));

        if (client == null || client.Key != authCredential.Password)
        {
            return await Task.FromResult(AuthenticateResult.Fail("Unauthorized client"));
        }

        var claims = new[] { new Claim(ClaimTypes.Name, client.Name) };

        var principal = new ClaimsPrincipal(new[] { new ClaimsIdentity(claims, Options.AuthenticationType) });

        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    private static BasicAuthenticationCredential? DecodeBasicAuthString(string authenticationString)
    {
        var credtiatialEncoded = authenticationString?.Replace("Basic ", "");

        if (string.IsNullOrWhiteSpace(credtiatialEncoded))
        {
            return null;
        }

        string? credtiatialDecoded = null;
        try
        {
            credtiatialDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(credtiatialEncoded));
        }
        catch (Exception /* ex */)
        {

        }

        if (!string.IsNullOrWhiteSpace(credtiatialDecoded))
        {
            int seperatorIndex = credtiatialDecoded.IndexOf(':');

            if (seperatorIndex > 0 && seperatorIndex < credtiatialDecoded.Length - 1)
            {
                BasicAuthenticationCredential credential = new()
                {
                    Username = credtiatialDecoded[..seperatorIndex],
                    Password = credtiatialDecoded[(seperatorIndex + 1)..]
                };

                return credential;
            }
        }

        return null;
    }
}
