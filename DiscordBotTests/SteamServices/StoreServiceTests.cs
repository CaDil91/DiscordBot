using Moq;
using SteamServices;

namespace DiscordBot.SteamServices;

public class SteamServicesStoreServiceTests
{
    private readonly StoreService _subjectUnderTest;

    public SteamServicesStoreServiceTests()
    {
        // Create mocks
        Mock<IAppRepository> appRepositoryMock = new();
        appRepositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new List<SteamApp>()
        {
            new() { AppId = 1, Name = "App 1" },
            new() { AppId = 2, Name = "App 2" },
            new() { AppId = 3, Name = "App 3" }
        });

        // Create subject under test.
        _subjectUnderTest = new StoreService(appRepositoryMock.Object);
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