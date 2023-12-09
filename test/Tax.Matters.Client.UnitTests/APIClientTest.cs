using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using System.Net.Http.Headers;
using Tax.Matters.Client.Extensions;

namespace Tax.Matters.Client.UnitTests;

public class APIClientTest
{
    private readonly Mock<MockHttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly Mock<IOptions<ClientOptions>> _mockClientOptionsAccessor;
    private readonly Mock<IHttpContextAccessor> _mockContextAccessor;

    public APIClientTest()
    {
        _mockHandler = new Mock<MockHttpMessageHandler>() { CallBase = true };
        _httpClient = new HttpClient(_mockHandler.Object);
        _mockClientOptionsAccessor = new Mock<IOptions<ClientOptions>>();
        _mockContextAccessor = new Mock<IHttpContextAccessor>();
    }

    [Test]
    public async Task Create_Returns_ResponseWithCreateResponseContent()
    {
        // Arrange
        string api = "https://localhost:5443", key = "1-2-3", name = "web", endpoint = "create", createId = "1";

        _mockClientOptionsAccessor.Setup(m => m.Value).Returns(new ClientOptions
        {
            API = api,
            Key = key,
            Name = name
        });

        var entity = new FakeEntity
        {
            Id = createId,
            Name = "stub me"
        };

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(entity.ToJsonString())
        };

        _mockHandler
            .Setup(handler => handler
                .Send(It.Is<HttpRequestMessage>(msg =>
                    msg.Method == HttpMethod.Post &&
                    msg.RequestUri!.ToString() == $"{api}/{endpoint}"
                    )
                )
            )
            .Returns(response);

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var client = new APIClient(_httpClient, _mockContextAccessor.Object, _mockClientOptionsAccessor.Object);

        // Act
        var result = await client.CreateAsync<FakeEntity, FakeEntity>(
            entity,
            endpoint,
            api,
            name,
            key);

        // Assert
        Assert.That(result, Is.TypeOf<Response<FakeEntity>>());
        Assert.That(result.Content!.Id, Is.EqualTo(createId));
    }
}
