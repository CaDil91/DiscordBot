using System.Reflection;
using Discord;
using Discord.Rest;
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
    private Task _expectedLoginAsyncValue = Task.CompletedTask;
    private Task _expectedStartAsyncValue = Task.CompletedTask;
    private Task<IReadOnlyCollection<RestGlobalCommand>> _expectedRegisterCommandsGloballyAsyncValue;

    public DiscordBotTests()
    {
        _discordSocketClientAdapterMock = new Mock<IDiscordSocketClientAdapter>();
        _discordSocketClientAdapterMock.Setup(x => x.LoginAsync(It.IsAny<TokenType>(), It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(() => _expectedLoginAsyncValue);
        _discordSocketClientAdapterMock.Setup(x => x.StartAsync()).Returns(() => _expectedStartAsyncValue);
        
        
        _interactionServiceAdapterMock = new Mock<IInteractionServiceAdapter>();
        _interactionServiceAdapterMock.Setup(x => x.AddModulesAsync(It.IsAny<Assembly>(), It.IsAny<IServiceProvider>()))
            .Returns(_expectedAddModulesAsyncValue);
        _expectedRegisterCommandsGloballyAsyncValue = Task.FromResult<IReadOnlyCollection<RestGlobalCommand>>(new List<RestGlobalCommand>());
        _interactionServiceAdapterMock.Setup(x => x.RegisterCommandsGloballyAsync(It.IsAny<bool>()))
            .Returns(() => _expectedRegisterCommandsGloballyAsyncValue);
        
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
    
    [Fact]
    public async Task RunAsync_WithValidLogin_CompletesSuccessfully()
    {
        // Arrange
        _expectedLoginAsyncValue = Task.CompletedTask;
        
        // Act
        Task task = _subjectUnderTest.RunAsync(1);
        await Task.Delay(500);
        
        // Assert
        Assert.True(task.Status == TaskStatus.RanToCompletion);
    }
    
    [Fact]
    public async Task RunAsync_WithInvalidLogin_ThrowsException()
    {
        // Arrange
        _expectedLoginAsyncValue = Task.FromException(new Exception());
        
        // Act
        // Assert
        await Assert.ThrowsAsync<Exception>(async () => await _subjectUnderTest.RunAsync());
    }
    
    [Fact]
    public async Task RunAsync_WithValidStart_CompletesSuccessfully()
    {
        // Arrange
        _expectedStartAsyncValue = Task.CompletedTask;
        
        // Act
        Task task = _subjectUnderTest.RunAsync(1);
        await Task.Delay(500);
        
        // Assert
        Assert.True(task.Status == TaskStatus.RanToCompletion);
    }
    
    [Fact]
    public async Task RunAsync_WithInvalidStart_ThrowsException()
    {
        // Arrange
        _expectedStartAsyncValue = Task.FromException(new Exception());
        
        // Act
        // Assert
        await Assert.ThrowsAsync<Exception>(async () => await _subjectUnderTest.RunAsync());
    }
    
    [Fact]
    public async Task RunAsync_WithDelay_CompletesSuccessfully()
    {
        // Arrange
        _expectedLoginAsyncValue = Task.CompletedTask;
        _expectedStartAsyncValue = Task.CompletedTask;
        
        // Act
        Task task = _subjectUnderTest.RunAsync(1);
        await Task.Delay(500);
        
        // Assert
        Assert.True(task.Status == TaskStatus.RanToCompletion);
    }
    
    [Fact]
    public async Task RunAsync_WithoutDelay_DoesNotComplete()
    {
        // Arrange
        _expectedLoginAsyncValue = Task.CompletedTask;
        _expectedStartAsyncValue = Task.CompletedTask;
        
        // Act
        Task task = _subjectUnderTest.RunAsync();
        await Task.Delay(500);
        
        // Assert
        Assert.True(task.Status == TaskStatus.WaitingForActivation);
    }
    
    [Fact]
    public void ReadyAsync_WithSuccessfulCall_CompletesSuccessfully()
    {
        // Arrange
        _expectedRegisterCommandsGloballyAsyncValue = Task.FromResult<IReadOnlyCollection<RestGlobalCommand>>(new List<RestGlobalCommand>());
        
        // Act
        Task task = _subjectUnderTest.ReadyAsync();
        
        // Assert
        Assert.True(task.Status == TaskStatus.RanToCompletion);
    }
    
    [Fact]
    public void ReadyAsync_WithFailedCall_ThrowsException()
    {
        // Arrange
        _expectedRegisterCommandsGloballyAsyncValue = Task.FromException<IReadOnlyCollection<RestGlobalCommand>>(new Exception());
        
        // Act
        // Assert
        Assert.ThrowsAsync<Exception>(() => _subjectUnderTest.ReadyAsync());
    }

}