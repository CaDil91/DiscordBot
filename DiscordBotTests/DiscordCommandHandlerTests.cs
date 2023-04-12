using Castle.Core.Logging;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Moq;
using SteamServices;

namespace DiscordBot;

public class DiscordCommandHandlerTests
{
    private readonly Mock<ILogger<DiscordCommandHandler>> _mockLogger = new();

    private readonly DiscordCommandHandler _subjectUnderTest;

    public DiscordCommandHandlerTests()
    {
        _subjectUnderTest = new DiscordCommandHandler(new Mock<StoreService>().Object, _mockLogger.Object);
    }

    [Fact]
    public async Task HandleSlashCommandAsync_DoesNotThrow()
    {
        // Arrange.
        SocketSlashCommand? socketSlashCommand = null;
        
        // Act.
        await _subjectUnderTest.HandleSlashCommandAsync(socketSlashCommand);

        // Assert
        // Asserting nothing. This method is passing along it's param to a mockable wrapper.
    }
}