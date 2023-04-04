using Discord.WebSocket;
using Moq;

namespace DiscordBot;

public class DiscordCommandHandlerTests
{
    private readonly DiscordCommandHandler _subjectUnderTest;

    public DiscordCommandHandlerTests()
    {
        _subjectUnderTest = new DiscordCommandHandler(new Mock<SteamService>().Object);
    }
    
    [Fact]
    public async Task HandleSlashCommand_DoesntThrow()
    {
        //Arrange.
        await _subjectUnderTest.HandleSlashCommandAsync(null);

        //Act.
        Exception? exception = await Record.ExceptionAsync(() => _subjectUnderTest.HandleSlashCommandAsync(null));

        //Assert.
        Assert.Null(exception);
    }
}