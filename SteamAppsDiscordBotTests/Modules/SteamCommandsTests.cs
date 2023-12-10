using Discord;
using Microsoft.Extensions.Logging;
using Moq;
using SteamAppsDiscordBot.Services;
using SteamAppsDiscordBot.Services.DTO;

namespace SteamAppsDiscordBot.Modules;

public class SteamCommandsTests
{
        private readonly Mock<ISteamStoreService> _steamStoreServiceMock = new();
        
        private readonly SteamCommands _subjectUnderTest;
        private List<SteamApp> _steamApps = new();

        public SteamCommandsTests()
        {
            var logger = Mock.Of<ILogger<SteamCommands>>();
            _steamStoreServiceMock.Setup(_ => _.GetAppsAsync(It.IsAny<string>(), 1)).ReturnsAsync(() => _steamApps);
            _subjectUnderTest = new SteamCommands(logger, _steamStoreServiceMock.Object);
        }
    
        [Fact]
        public async Task GetSteamAppAsync_ReturnsResults_WhenSteamAppsAreNotFound()
        {
            // Arrange
            // _steamApps is empty.

            // Act
            await _subjectUnderTest.GetSteamAppAsync("anything");

            // Assert _subjectUnderTest.FollowupAsyncCaller was called with "No results found."
            Mock.Get(_subjectUnderTest).Verify(x => x.FollowupAsyncCaller("No results found.", It.IsAny<Embed[]>(), false, false, null, null, null), Times.Once);   
        }

        [Fact]
        public async Task GetSteamAppAsync_ResultsFound()
        {
            // Arrange
            _steamApps = new List<SteamApp> { new() { Name = "Test App", ShortDescription = "Test Description", HeaderImage = "https://testapp.com/image.jpg" } };

            // Act
            await _subjectUnderTest.GetSteamAppAsync("appName");

            // Assert
            Mock.Get(_subjectUnderTest).Verify(x => x.FollowupAsyncCaller("", It.IsAny<Embed[]>(), false, false, null, null, null), Times.Once);
        }

}