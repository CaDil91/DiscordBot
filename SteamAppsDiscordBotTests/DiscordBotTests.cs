using Discord;
using Microsoft.Extensions.Options;
using Moq;

namespace DiscordBot;

public class SteamAppsDiscordBotTests
{
    private Task _loginResult = Task.CompletedTask;
    private Task _startResult = Task.CompletedTask;
    private readonly DiscordBot _subjectUnderTest;
    private readonly Mock<IDiscordSocketClientAdapter> _clientMock = new();

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
        
        _subjectUnderTest = new DiscordBot(optionsMock.Object, _clientMock.Object);
    }

    [Fact]
    public async Task RunAsync_LoginAsyncIsCalled_Success()
    {
        // Arrange
        _loginResult = Task.CompletedTask;

        // Act
        await _subjectUnderTest.RunAsync(new Mock<IServiceProvider>().Object, 1);
            
        // Assert
        _clientMock.Verify(c => c.LoginAsync(TokenType.Bot, It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
    }
    
    [Fact]
    public void RunAsync_LoginAsyncFailure_Throws()
    {
        // Arrange
        _loginResult = Task.FromException(new Exception());

        // Act
       Task actTask = _subjectUnderTest.RunAsync(new Mock<IServiceProvider>().Object);
            
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
        await _subjectUnderTest.RunAsync(new Mock<IServiceProvider>().Object, 1);
            
        // Assert
        _clientMock.Verify(c => c.StartAsync(), Times.Once);
    }
    
    [Fact]
    public Task RunAsync_StartAsyncFailure_Throws()
    {
        // Arrange
        _startResult = Task.FromException(new Exception());

        // Act
        Task actTask = _subjectUnderTest.RunAsync(new Mock<IServiceProvider>().Object);
            
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
        Task runTask = _subjectUnderTest.RunAsync(new Mock<IServiceProvider>().Object, 1);
        await Task.Delay(500);
            
        // Assert
        Assert.Equal(TaskStatus.RanToCompletion, runTask.Status);
    }
}