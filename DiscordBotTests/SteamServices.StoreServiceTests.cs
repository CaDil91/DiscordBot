using Moq;
using Moq.Protected;
using SteamServices;

namespace DiscordBot;

public class SteamServicesStoreServiceTests
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory = new();
    private readonly StoreService _subjectUnderTest;

    public SteamServicesStoreServiceTests()
    {
        //Create http client factory mock, and "hardcodedsteam"
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        var result = new HttpResponseMessage();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(result)
            .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://store.steampowered.com")
        };
        _mockHttpClientFactory.Setup(_ => _.CreateClient("hardcodedsteam")).Returns(httpClient);

        _subjectUnderTest = new StoreService(_mockHttpClientFactory.Object);
    }

    [Fact]
    public async Task SearchStoreAsync_()
    {
        //Arrange.
        //Act.
        List<SteamApp> steamApps = await _subjectUnderTest.GetAppsFromStoreAsync("halo");

        //Assert.
        Assert.NotNull(steamApps);
    }
}