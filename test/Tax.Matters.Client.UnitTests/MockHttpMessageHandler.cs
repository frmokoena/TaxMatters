namespace Tax.Matters.Client.UnitTests;

public class MockHttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Send(request));
    }

    public virtual HttpResponseMessage Send(HttpRequestMessage request)
    {
        throw new NotImplementedException("Try to mock me if you dare!");
    }
}
