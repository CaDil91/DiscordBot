using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Moq;

namespace DiscordBot;

public class SteamAppsDiscordBotTests
{
    private Task _loginResult = Task.CompletedTask;
    private readonly DiscordBot _subjectUnderTest;
    private readonly Mock<IDiscordSocketClientAdapter> _clientMock = new();

    public SteamAppsDiscordBotTests()
    {
        var optionsMock = new Mock<IOptions<DiscordBotOptions>>();
        optionsMock.Setup(o => o.Value).Returns(new DiscordBotOptions { DiscordToken = "token" });
        
        // Setup so _loginResult can be dynamically changed in tests.
        _clientMock.Setup(c => c.LoginAsync(TokenType.Bot, It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(() => _loginResult);
        
        _subjectUnderTest = new DiscordBot(optionsMock.Object, _clientMock.Object);
    }

    [Fact]
    public Task RunAsync_LoginAsyncIsCalled_Success()
    {
        // Arrange
        _loginResult = Task.CompletedTask;

        // Act
        Task _ = _subjectUnderTest.RunAsync(new Mock<IServiceProvider>().Object);
            
        // Assert
        _clientMock.Verify(c => c.LoginAsync(TokenType.Bot, It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        return Task.CompletedTask;
    }
}