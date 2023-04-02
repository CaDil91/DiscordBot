using Discord;
using Discord.WebSocket;
using Moq;

namespace DiscordBot;

public class DiscordCommandsTests
{
    private DiscordSocketClient _client = new();

    [Fact]
    public async Task SlashCommandHandlerDoesNotThrowWithNullParameter()
    {
        //Arrange.
        
        //Act.
        Exception? exception = await Record.ExceptionAsync(() => DiscordCommandHandler.HandleSlashCommand(null));
        
        //Assert.
        Assert.Null(exception);
    }
    
    [Fact]
    public async Task HandleCommandCanHandleNullCommands()
    {
        //Arrange.
        
        //Act.
        Exception? exception = await Record.ExceptionAsync(() => DiscordCommandHandler.HandleCommand(null));
        
        //Assert.
        Assert.Null(exception);
    }
}