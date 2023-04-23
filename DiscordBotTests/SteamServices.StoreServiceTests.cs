using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using SteamServices;

namespace DiscordBot;

public class SteamServicesStoreServiceTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<IOptions<SteamOptions>> _azureOptionsMock;

    private readonly StoreService _subjectUnderTest;

    public SteamServicesStoreServiceTests()
    {
        // Create mocks
        _azureOptionsMock = new Mock<IOptions<SteamOptions>>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        // Create steam options mock, and Token.
        _azureOptionsMock.Setup(x => x.Value.Token).Returns("token");

        // Create http client factory mock, and "hardcodedsteam".
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
        _httpClientFactoryMock.Setup(_ => _.CreateClient("hardcodedsteam")).Returns(httpClient);

        // Create subject under test.
        _subjectUnderTest = new StoreService(_httpClientFactoryMock.Object, new SecretClient(new Uri(""), new DefaultAzureCredential()), _azureOptionsMock.Object);
    }

    [Fact]
    public async Task SearchStoreAsync_()
    {
        //Arrange.
        //Act.
        string steamApps = await _subjectUnderTest.GetAppsFromStoreAsync("halo");

        //Assert.
        Assert.NotNull(steamApps);
    }
}