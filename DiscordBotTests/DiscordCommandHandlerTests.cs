using Discord.WebSocket;
using Moq;
using SteamServices;

namespace DiscordBot;

public class DiscordCommandHandlerTests
{
    private readonly DiscordCommandHandler _subjectUnderTest;

    public DiscordCommandHandlerTests()
    {
        _subjectUnderTest = new DiscordCommandHandler(new Mock<StoreService>().Object);
    }

    [Fact]
    public async Task HandleSlashCommandAsync_HandlesInvalidSlashCommand()
    {
        // Arrange.
        var slashCommand = new Mock<SlashCommandWrapper>();
        
        // Act.
        await _subjectUnderTest.HandleSlashCommandAsync(slashCommand.Object);
        
        // Assert.
    }
}