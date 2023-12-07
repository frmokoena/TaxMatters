using Microsoft.AspNetCore.Http;

namespace Tax.Matters.Client.Extensions;

public static class HttpContextExtensions
{
    public const string NullIPv6Address = "::1";

    public static string ToUrlString(this HttpRequest request)
    {
        return $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
    }

    public static string? GetRequestIP(this HttpContext context)
    {
        var remoteIpAddress = context.Connection.RemoteIpAddress;

        return remoteIpAddress?.ToString();
    }
}
