
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text;
using Tax.Matters.Client.Extensions;

namespace Tax.Matters.Client;

public class APIClient(
    HttpClient httpClient,
    IHttpContextAccessor httpContext,
    IOptions<ClientOptions> optionsAccessor) : APIBaseClient(httpClient, httpContext, optionsAccessor), IAPIClient
{
    public async Task<IResponse<T>> CreateAsync<T, TContent>(
        string uri,
        TContent content,      
        string? baseUri = null,
        string? clientName = null,
        string? apiKey = null,           
        CancellationToken cancellationToken = default) where TContent : class
    {
        if (content is string) return new Response<T>(new ArgumentException("value can not be string", nameof(content)));

        string jsonData = content.ToJsonString();

        var data = new StringContent(
            content: jsonData,
            encoding: Encoding.UTF8,
            mediaType: "application/json");

        if (!string.IsNullOrWhiteSpace(baseUri))
        {
            uri = baseUri + "/" + uri;
        }

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = data
        };

        var response = await SendRequestAsync<T>(httpRequestMessage, clientName, apiKey, cancellationToken);

        return response;
    }

    public async Task<IResponse<T>> DeleteAsync<T>(
        string uri,
        string? baseUri = null,
        string? clientName = null,
        string? apiKey = null,            
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(baseUri))
        {
            uri = baseUri + "/" + uri;
        }

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

        var response = await SendRequestAsync<T>(
            httpRequestMessage,
            clientName, apiKey,
            cancellationToken: cancellationToken);

        return response;
    }

    public async Task<IResponse<T>> EditAsync<T, TContent>(
        string uri,
        TContent content,
        string? baseUri = null,
        string? clientName = null,
        string? apiKey = null,
        CancellationToken cancellationToken = default) where TContent : class
    {
        if (content is string) return new Response<T>(new ArgumentException("value can not be string", nameof(content)));

        string jsonContent = content.ToJsonString();

        if (!string.IsNullOrWhiteSpace(baseUri))
        {
            uri = baseUri + "/" + uri;
        }

        var data = new StringContent(
           content: jsonContent,
           encoding: Encoding.UTF8,
           mediaType: "application/json");

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, uri)
        {
            Content = data
        };

        var response = await SendRequestAsync<T>(
            httpRequestMessage, 
            clientName,
            apiKey,
            cancellationToken);

        return response;
    }

    public async Task<IResponse<T>> GetAsync<T>(
        string uri,
        string? baseUri = null,
        string? clientName = null,
        string? apiKey = null,           
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(baseUri))
        {
            uri = baseUri + "/" + uri;
        }

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

        var response = await SendRequestAsync<T>(
            httpRequestMessage, 
            clientName,
            apiKey,
            cancellationToken);

        return response;
    }
}
