using System.Reflection;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SteamAppsDiscordBot;
using SteamAppsDiscordBot.Adapters;
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace SteamAppsDiscordBotTests;

public class DiscordBotTests
{
    private readonly Mock<IDiscordSocketClientAdapter> _discordSocketClientAdapterMock;
    private readonly Mock<IInteractionServiceAdapter> _interactionServiceAdapterMock;
    private readonly OptionsWrapper<DiscordBotOptions> _discordBotOptionsMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<ILogger<DiscordBot>> _loggerMock;
    
    private readonly DiscordBot _subjectUnderTest;
    private Task _expectedAddModulesAsyncValue = Task.CompletedTask;

    public DiscordBotTests()
    {
        _discordSocketClientAdapterMock = new Mock<IDiscordSocketClientAdapter>();
        
        _interactionServiceAdapterMock = new Mock<IInteractionServiceAdapter>();
        _interactionServiceAdapterMock.Setup(x => x.AddModulesAsync(It.IsAny<Assembly>(), It.IsAny<IServiceProvider>()))
            .Returns(_expectedAddModulesAsyncValue);
        
        _discordBotOptionsMock = new OptionsWrapper<DiscordBotOptions>(new DiscordBotOptions
        {
            DiscordToken = "test"
        });
        
        _serviceProviderMock = new Mock<IServiceProvider>();
        _loggerMock = new Mock<ILogger<DiscordBot>>();
        
        _subjectUnderTest = new DiscordBot(_discordSocketClientAdapterMock.Object, _interactionServiceAdapterMock.Object,
            _discordBotOptionsMock, _serviceProviderMock.Object, _loggerMock.Object);
    }
    
    [Fact]
    public async Task InitializeInteractionServicesAsync_AddsModulesAsync_Success()
    {
        // Arrange
        _expectedAddModulesAsyncValue = Task.CompletedTask;
        
        // Act
        await _subjectUnderTest.InitializeInteractionServicesAsync(Assembly.GetEntryAssembly());
        
        // Assert
        _interactionServiceAdapterMock.Verify(x => x.AddModulesAsync(It.IsAny<Assembly>(), _serviceProviderMock.Object), Times.Once);
    }
    
    [Fact]
    public async Task InitializeInteractionServicesAsync_AddsModulesAsync_Failure()
    {
        // Arrange
        _expectedAddModulesAsyncValue = Task.FromException(new Exception());
        
        // Act
        await _subjectUnderTest.InitializeInteractionServicesAsync(Assembly.GetEntryAssembly());
        
        // Assert
        _interactionServiceAdapterMock.Verify(x => x.AddModulesAsync(It.IsAny<Assembly>(), _serviceProviderMock.Object), Times.Once);
    }
    
    [Fact]
    public async Task InitializeInteractionServicesAsync_WithInvalidGetEntryAssembly_ThrowsException()
    {
        // Arrange
        _expectedAddModulesAsyncValue = Task.CompletedTask;
        
        // Act
        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _subjectUnderTest.InitializeInteractionServicesAsync(null));
    }

}