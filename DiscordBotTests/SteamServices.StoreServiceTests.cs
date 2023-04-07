using System.Net;
using Moq;
using Moq.Language.Flow;
using Moq.Protected;
using SteamServices;

namespace DiscordBot;

public class SteamServicesStoreServiceTests
{
    private readonly StoreService _subjectUnderTest;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory = new();

    public SteamServicesStoreServiceTests()
    {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var result = new HttpResponseMessage();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(result)
            .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://store.steampowered.com"),
        };

        _mockHttpClientFactory.Setup(_ => _.CreateClient("hardcodedsteam")).Returns(httpClient);
        
        _subjectUnderTest = new StoreService(_mockHttpClientFactory.Object);
    }
    
    [Fact]
    public async Task SearchStoreAsync_CanMakeHttpRequestsToStore()
    {
        //Arrange.
        //Act.
        HttpResponseMessage response = await _subjectUnderTest.RequestSteamStore();
        
        //Assert.
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task SearchStoreAsync_ReturnsListOfAppIds()
    {
        //Arrange.
        
        //Act.
        
        //Assert.
    }

    [Fact]
    public async Task SearchStoreAsync_ReturnsErrorResponseIfNoAppsFound()
    {
        //Arrange.

        //Act.

        //Assert.
    }
}