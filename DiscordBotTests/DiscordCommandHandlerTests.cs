using Discord.WebSocket;
using Moq;

namespace DiscordBot;

public class DiscordCommandHandlerTests
{
    private readonly Mock<SteamService> _steamServiceMock;

    public DiscordCommandHandlerTests()
    {
        _steamServiceMock = new Mock<SteamService>();
    }

    //TODO: 
    [Fact]
    public async Task SlashCommandHandlerDoesNotThrowWithNullParameter()
    {
        //Arrange.
        var discordCommandHandler = new DiscordCommandHandler(_steamServiceMock.Object);
        
        //Act.
        //Exception? exception = await Record.ExceptionAsync(() => discordCommandHandler.HandleSlashCommand(null));
        
        //Assert.
        //Assert.Null(exception);
    }
    
    [Fact]
    public async Task HandleCommandCanHandleNullCommands()
    {
        //Arrange.
        var discordCommandHandler = new DiscordCommandHandler(_steamServiceMock.Object);
        
        //Act.
        Exception? exception = await Record.ExceptionAsync(() => discordCommandHandler.HandleCommand(null));
        
        //Assert.
        Assert.Null(exception);
    }
}