/*using DiscordBot.Controllers;
using DiscordBot.DiscordBot.Core;
using Microsoft.Extensions.Logging;
using Moq;
using SteamServices.Controllers;

namespace DiscordBot.DiscordBot.DiscordBot.Core;

public class DiscordCommandHandlerTests
{
    private readonly Mock<ILogger<DiscordCommandController>> _loggerMock;

    private readonly DiscordCommandController _subjectUnderTest;

    public DiscordCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<DiscordCommandController>>();
        Mock<IController> storeService = new();
        
        _subjectUnderTest = new DiscordCommandController(_loggerMock.Object, storeService.Object, new Mock<DiscordButtonController>().Object);
    }
    
    

    [Fact]
    public async Task SocketSlashCommand_HandleSlashCommandAsync_LogsWarningForNullCommand()
    {
        // Arrange.
        // Act.
        await _subjectUnderTest.RunSlashCommandAsync(null);
        
        // Assert
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == "Null command received."),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
    }
}*/