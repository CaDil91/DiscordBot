using DiscordBot.DiscordBot.Core;
using Microsoft.Extensions.Logging;
using Moq;
using SteamServices;

namespace DiscordBot.DiscordBot.DiscordBot.Core;

public class DiscordCommandHandlerTests
{
    private readonly Mock<ILogger<DiscordCommandHandler>> _loggerMock = new();
    private readonly Mock<StoreService> _storeService = new();

    private readonly DiscordCommandHandler _subjectUnderTest;

    public DiscordCommandHandlerTests()
    {
        _subjectUnderTest = new DiscordCommandHandler(_loggerMock.Object, _storeService.Object);
    }

    [Fact]
    public async Task SocketSlashCommand_HandleSlashCommandAsync_LogsWarningForNullCommand()
    {
        // Arrange.
        // Act.
        await _subjectUnderTest.HandleSlashCommandAsync(null);
        
        // Assert
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == "Null command received."),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
    }
}