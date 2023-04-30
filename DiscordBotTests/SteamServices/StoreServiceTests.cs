using AzureServices;
using Microsoft.Extensions.Options;
using Moq;
using SteamServices;

namespace DiscordBot.SteamServices;

public class SteamServicesStoreServiceTests
{
    private readonly StoreService _subjectUnderTest;

    public SteamServicesStoreServiceTests()
    {
        // Create mocks
        Mock<IHttpClientFactory> httpClientFactoryMock = new();

        //Create optional setup for the IOptions<SteamOptions> mock.
        Mock<IOptions<SteamOptions>> steamOptionsMock = new();
        steamOptionsMock.Setup(x => x.Value).Returns(new SteamOptions());
        
        // Setup the HttpClientFactory mock to return a new HttpClient.
        httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient());

        // Create subject under test.
        _subjectUnderTest = new StoreService(httpClientFactoryMock.Object);
    }

    /// <summary>
    /// GetAppsFromStoreAsync() test.
    /// </summary>
    [InlineData("counter strike", 5)]
    [InlineData("d001441f-738b-43fc-a5d5-2b1225490c1fd001441f-738b-43fc-a5d5-2b1225490c1f", 10)]
    [InlineData("halo", 3)]
    [Theory]
    public async Task GetAppsFromStoreAsync_ReturnsBetweenZeroAndRequestedAppReturnCount(string sValidSearchTerm, 
        int iAppReturnCountMax)
    {
        // Arrange.
        
        // Act.
        List<SteamApp> steamApps = await _subjectUnderTest.GetAppsFromStoreAsync(sValidSearchTerm, iAppReturnCountMax);

        // Assert.
        Assert.InRange(steamApps.Count, 0, iAppReturnCountMax);
    }
}