namespace Tax.Matters.Client;

public interface IHttpClientFactory
{
    public HttpClient CreateHttpClient();
}
