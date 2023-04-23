using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AzureServices;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using SteamServices;

namespace DiscordBot.SteamServices;

public class SteamServicesStoreServiceTests
{
    private readonly StoreService _subjectUnderTest;

    public SteamServicesStoreServiceTests()
    {
        // Create mocks
        Mock<IOptions<AzureOptions>> azureOptionsMock = new();
        Mock<IHttpClientFactory> httpClientFactoryMock = new();

        // Create steam options mock, and Token.
        var azureOptions = new AzureOptions()
        {
            SteamSecret = "Steam"
        };
        azureOptionsMock.Setup(x => x.Value).Returns(azureOptions);

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
        httpClientFactoryMock.Setup(_ => _.CreateClient("hardcodedsteam")).Returns(httpClient);

        // Create subject under test.
        _subjectUnderTest = new StoreService(httpClientFactoryMock.Object, 
            new SecretClient(new Uri("https://justabotvault.vault.azure.net/"), new DefaultAzureCredential()), 
            azureOptionsMock.Object);
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