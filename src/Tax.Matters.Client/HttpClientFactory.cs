using System.Net.Http.Headers;

namespace Tax.Matters.Client;

internal class HttpClientFactory: IHttpClientFactory
{
    public HttpClient CreateHttpClient()
    {
        var handler = new SocketsHttpHandler
        {
            // Can be configured to the desired interval, depending on expected DNS changes.
            // We set it to InfiniteTimeSpan for for our localhost endpoints here
            PooledConnectionLifetime = Timeout.InfiniteTimeSpan 
        };

        // Performance Note:
        //     The http client needs to be singleton to prevent the app from running out of connections.
        var client = new HttpClient(handler);

        SetupClientDefaults(client);
        return client;
    }

    protected static void SetupClientDefaults(HttpClient client)
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}
