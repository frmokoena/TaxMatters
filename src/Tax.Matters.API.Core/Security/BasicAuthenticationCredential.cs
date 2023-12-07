namespace Tax.Matters.API.Core.Security;

internal class BasicAuthenticationCredential
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}
