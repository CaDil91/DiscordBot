using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Moq;

namespace DiscordBot;

public class SteamAppsDiscordBotTests
{
    private Task _loginResult = Task.CompletedTask;
    private Task _startResult = Task.CompletedTask;
    private readonly Mock<IDiscordSocketClientAdapter> _clientMock = new();
    private readonly Mock<IServiceProvider> _serviceProviderMock = new();
    private Mock<IInteractionServiceAdapter> _interactionServiceMock = new();
    
    private readonly DiscordBot _subjectUnderTest;

    public SteamAppsDiscordBotTests()
    {
        var optionsMock = new Mock<IOptions<DiscordBotOptions>>();
        optionsMock.Setup(o => o.Value).Returns(new DiscordBotOptions { DiscordToken = "token" });
        
        // Setup so _loginResult can be dynamically changed in tests.
        _clientMock.Setup(c => c.LoginAsync(TokenType.Bot, It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(() => _loginResult);
        
        // Setup so _startResult can be dynamically changed in tests.
        _clientMock.Setup(c => c.StartAsync())
            .Returns(() => _startResult);
        
        _subjectUnderTest = new DiscordBot(optionsMock.Object, _clientMock.Object, _serviceProviderMock.Object, 
            _interactionServiceMock.Object);
    }

    [Fact]
    public async Task RunAsync_LoginAsyncIsCalled_Success()
    {
        // Arrange
        _loginResult = Task.CompletedTask;

        // Act
        await _subjectUnderTest.RunAsync(1);
            
        // Assert
        _clientMock.Verify(c => c.LoginAsync(TokenType.Bot, It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
    }
    
    [Fact]
    public void RunAsync_LoginAsyncFailure_Throws()
    {
        // Arrange
        _loginResult = Task.FromException(new Exception());

        // Act
       Task actTask = _subjectUnderTest.RunAsync();
            
        // Assert
        _clientMock.Verify(c => c.LoginAsync(TokenType.Bot, It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        Assert.Equal(TaskStatus.Faulted, actTask.Status);
    }
    
    [Fact]
    public async Task RunAsync_StartAsyncIsCalled_Success()
    {
        // Arrange
        _startResult = Task.CompletedTask;

        // Act
        await _subjectUnderTest.RunAsync(1);
            
        // Assert
        _clientMock.Verify(c => c.StartAsync(), Times.Once);
    }
    
    [Fact]
    public Task RunAsync_StartAsyncFailure_Throws()
    {
        // Arrange
        _startResult = Task.FromException(new Exception());

        // Act
        Task actTask = _subjectUnderTest.RunAsync();
            
        // Assert
        _clientMock.Verify(c => c.StartAsync(), Times.Once);
        Assert.Equal(TaskStatus.Faulted, actTask.Status);
        return Task.CompletedTask;
    }
    
    [Fact]
    public async Task RunAsync_CallsDelay()
    {
        // Arrange

        // Act
        Task runTask = _subjectUnderTest.RunAsync(1);
        await Task.Delay(500);
            
        // Assert
        Assert.Equal(TaskStatus.RanToCompletion, runTask.Status);
    }
    
    [Fact]
    public async Task RunAsync_CallsDelayWithTimeoutInfinite()
    {
        // Arrange

        // Act
        Task runTask = _subjectUnderTest.RunAsync(Timeout.Infinite);
        await Task.Delay(500);
            
        // Assert
        Assert.Equal(TaskStatus.WaitingForActivation, runTask.Status);
    }

    [Fact]
    public async Task RunAsync_CallsAddModulesAsync()
    {
        // Arrange
        var assembly = Assembly.GetEntryAssembly();

        // Act
        await _subjectUnderTest.RunAsync(1);
            
        // Assert
        _interactionServiceMock.Verify(i => i.AddModulesAsync(It.IsAny<Assembly>(), _serviceProviderMock.Object), Times.Once);
    }
    
    
    
}