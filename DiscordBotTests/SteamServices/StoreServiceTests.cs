using GoogleService;
using Microsoft.Extensions.Logging;
using Moq;
using SteamServices;

namespace DiscordBot.SteamServices;

public class SteamServicesStoreServiceTests
{
    private readonly SteamStoreService _subjectUnderTest;

    public SteamServicesStoreServiceTests()
    {
        // Create subject under test.
        _subjectUnderTest = new SteamStoreService(new Mock<IHttpClientFactory>().Object, new Mock<ILogger<SteamStoreService>>().Object, new Mock<IGoogleSearchRepository>().Object);
    }
    /// <summary>
    /// GetAppsAsync() test.
    /// </summary>
    [InlineData("counter strike", 5)]
    [InlineData("d001441f-738b-43fc-a5d5-2b1225490c1fd001441f-738b-43fc-a5d5-2b1225490c1f", 10)]
    [InlineData("halo", 3)]
    [InlineData("", 1)]
    [InlineData("halo", -10)]
    [Theory]
    public async Task GetAppsAsync_ReturnsListOfApps(string sValidSearchTerm, int iAppReturnCountMax)
    {
        // Arrange.
        
        // Act.
        List<SteamApp> steamApps = await _subjectUnderTest.GetAppsAsync(sValidSearchTerm, iAppReturnCountMax);

        // Assert.
        Assert.NotNull(steamApps);
    }
}