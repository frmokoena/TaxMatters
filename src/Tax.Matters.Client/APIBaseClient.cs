using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Tax.Matters.Client.Extensions;

namespace Tax.Matters.Client;

public abstract class APIBaseClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContext;
    private readonly ClientOptions? _clientOptions;

    public APIBaseClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContext,
        IOptions<ClientOptions> optionsAccessor)
    {
        _httpClient = httpClient;
        _httpContext = httpContext;
        _clientOptions = optionsAccessor?.Value;
    }

    protected async Task<IResponse<T>> SendRequestAsync<T>(
        HttpRequestMessage httpRequestMessage,
        string? clientName = null,
        string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponseMessage;

        if (string.IsNullOrWhiteSpace(clientName))
        {
            clientName = _clientOptions?.Name;
            apiKey = _clientOptions?.Key;
        }

        if (!string.IsNullOrWhiteSpace(clientName) && !string.IsNullOrWhiteSpace(apiKey))
        {
            httpRequestMessage.AddBasicAuthorization(clientName, apiKey);
        }

        httpRequestMessage.Headers.Add("client-page", _httpContext.HttpContext?.Request?.ToUrlString() ?? string.Empty);

        var deviceType = _httpContext!.HttpContext?.Request?.Cookies["client-devicetype"];

        if (!string.IsNullOrEmpty(deviceType))
        {
            httpRequestMessage.Headers.Add("client-devicetype", deviceType);
        }

        httpRequestMessage.Headers.Add("client-ip", _httpContext.HttpContext?.GetRequestIP() ?? string.Empty);

        try
        {
            httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new Response<T>(ex);
        }

        string? raw = null;
        if (httpResponseMessage.Content != null)
        {
            raw = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }

        return new Response<T>(
            raw,
            httpResponseMessage.StatusCode,
            httpResponseMessage.ReasonPhrase,
            httpResponseMessage.Headers);
    }
}
