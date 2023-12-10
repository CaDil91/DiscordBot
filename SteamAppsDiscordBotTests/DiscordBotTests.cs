using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SteamAppsDiscordBot.Adapters;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace SteamAppsDiscordBot;

public class DiscordBotTests
{
    private readonly Mock<DiscordSocketClientAdapter> _discordSocketClientAdapterMock;
    private readonly Mock<InteractionServiceAdapter> _interactionServiceAdapterMock;
    private readonly OptionsWrapper<DiscordBotOptions> _discordBotOptionsMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<ILogger<DiscordBot>> _loggerMock;
    
    private readonly DiscordBot _subjectUnderTest;

    public DiscordBotTests()
    {
        _discordSocketClientAdapterMock = new Mock<DiscordSocketClientAdapter>();
        _interactionServiceAdapterMock = new Mock<InteractionServiceAdapter>();
        
        _discordBotOptionsMock = new OptionsWrapper<DiscordBotOptions>(new DiscordBotOptions
        {
            DiscordToken = "test"
        });
        
        _serviceProviderMock = new Mock<IServiceProvider>();
        _loggerMock = new Mock<ILogger<DiscordBot>>();
        
        _subjectUnderTest = new DiscordBot(_discordSocketClientAdapterMock.Object, _interactionServiceAdapterMock.Object
            , _discordBotOptionsMock, _serviceProviderMock.Object, _loggerMock.Object);
    }
    
    [Fact]
    public async Task InitializeInteractionServicesAsync_AttachesReadyEvent_Properly()
    {
        // Arrange
        
        // Act
        await _subjectUnderTest.InitializeInteractionServicesAsync();
        
        // Assert
        _discordSocketClientAdapterMock.VerifyAdd(x => x.Ready += It.IsAny<Func<Task>>(), Times.Once);
    }

}