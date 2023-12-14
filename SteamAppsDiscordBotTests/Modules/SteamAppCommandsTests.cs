using Microsoft.Extensions.Logging;
using Moq;
using SteamAppsDiscordBot.Modules;
using SteamAppsDiscordBot.Services;
using SteamAppsDiscordBot.Services.DTO;

namespace SteamAppsDiscordBotTests.Modules;

public class SteamAppCommandsTests
{
        private readonly Mock<ILogger<SteamAppCommands>> _loggerMock = new();
        private readonly Mock<ISteamStoreService> _steamStoreServiceMock = new();
        
        private readonly SteamAppCommands _subjectUnderTest;
        private List<SteamApp> _steamAppsReturnValue = new();

        public SteamAppCommandsTests()
        {
            _steamStoreServiceMock.Setup(_ => _.GetAppsAsync(It.IsAny<string>(), 1)).ReturnsAsync(() => _steamAppsReturnValue);
            _subjectUnderTest = new SteamAppCommands(_loggerMock.Object, _steamStoreServiceMock.Object);
        }
    
        [Fact]
        public async Task GetSteamAppAsync_WhenNoResultsFound_ShouldReturnNoResultsFound()
        {
            // Arrange
            _steamAppsReturnValue = new List<SteamApp>();
            
            // Act
            await _subjectUnderTest.GetSteamAppAsync("test");
            
            // FollowupAsync will fail, because I'm not a real InteractionModuleBase<SocketInteractionContext>.
            // We can verify that the logger was called with the expected message.
            const string expectedFollowupMessage = "No results found.";
            
            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No results found.")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
        }
        
        [Fact]
        public async Task GetSteamAppAsync_WhenResultsFound_ShouldReturnResults()
        {
            // Arrange
            const string appName = "Test App";
            _steamAppsReturnValue = new List<SteamApp>
            {
                new()
                {
                    Name = appName,
                    SteamAppid = 12345,
                    HeaderImage = "https://test.com/test.png",
                    ShortDescription = "Test description"
                }
            };
            
            // Act
            await _subjectUnderTest.GetSteamAppAsync("test");
            
            // Assert
            // FollowupAsync will fail, because I'm not a real InteractionModuleBase<SocketInteractionContext>.
            // We can verify that the logger was called with the expected message.
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(appName)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
        }
        
        [Fact]
        public async Task FollowupWrapperAsync_WithInvalidSteamApp_ShouldReturnErrorMessage()
        {
            // Arrange
            var emptySteamApp = new SteamApp();
            const string expectedFollowupMessage = "Unable to collect app details.";
            
            // Act
            await _subjectUnderTest.FollowupWrapperAsync(emptySteamApp);
            
            // Assert
            // FollowupAsync will fail, because I'm not a real InteractionModuleBase<SocketInteractionContext>.
            // We can verify that the logger was called with the expected message.
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(expectedFollowupMessage)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
        }
        
        [Fact]
        public async Task FollowupWrapperAsync_WithValidSteamApp_ShouldReturnAppDetails()
        {
            // Arrange
            const string appName = "Test App";
            var steamApp = new SteamApp
            {
                Name = appName,
                SteamAppid = 12345,
                HeaderImage = "https://test.com/test.png",
                ShortDescription = "Test description"
            };
            
            // Act
            await _subjectUnderTest.FollowupWrapperAsync(steamApp);
            
            // Assert
            // FollowupAsync will fail, because I'm not a real InteractionModuleBase<SocketInteractionContext>.
            // We can verify that the logger was called with the expected message.
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Embed: {appName}")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
        }

}