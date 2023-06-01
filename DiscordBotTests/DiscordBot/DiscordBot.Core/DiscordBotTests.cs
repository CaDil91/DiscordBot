using DiscordBot.DiscordBot.Core;
using Microsoft.Extensions.Options;
using Moq;

namespace DiscordBot.DiscordBot.DiscordBot.Core;

public class DiscordBotTests
{
    private readonly Mock<IOptions<DiscordBotOptions>> _optionsMock;
    private readonly Mock<IDiscordCommandHandler> _discordCommandHandler;
    private const string TEST_TOKEN = "MTAwMzA3NTcxMzAwNTUzMTIxNg.GsL_2g.7DxQHrpK31n1Bz6OxRYyMjl3xQD6U9Vvbowbuo";

    private global::DiscordBot.DiscordBot.Core.DiscordBot _subjectUnderTest;

    public DiscordBotTests()
    {
        _discordCommandHandler = new Mock<IDiscordCommandHandler>();
        _optionsMock = new Mock<IOptions<DiscordBotOptions>>();
        
        _subjectUnderTest = new global::DiscordBot.DiscordBot.Core.DiscordBot(_optionsMock.Object, _discordCommandHandler.Object);
    }

    [Fact]
    public Task DiscordBot_WithTestToken_CanConnectInFiveSeconds()
    {
        //Arrange.
        DiscordBotOptions discordBotOptions = new()
        {
            DiscordToken = TEST_TOKEN
        };
        _optionsMock.Setup(x => x.Value).Returns(discordBotOptions);
        _subjectUnderTest = new global::DiscordBot.DiscordBot.Core.DiscordBot(_optionsMock.Object, _discordCommandHandler.Object);

        //Act.
        _ = _subjectUnderTest.RunAsync();

        for (var i = 0; i < 10; i++)
        {
            if (_subjectUnderTest.IsConnected()) break;
            Thread.Sleep(500); //Sleep for 0.5 seconds ten times. Five Seconds
        }
        
        //Assert.
        Assert.True(_subjectUnderTest.IsConnected());
        return Task.CompletedTask;
    }

    /*[Fact]
    public async Task RegisterSlashCommands_ThrowsIfRegisteringWithoutConnecting()
    {
        // Arrange.
        _optionsMock.Setup(x => x.Value).Returns(new DiscordBotOptions());
        var discordCommandHandler = new DiscordCommandHandler(new Mock<ILogger<DiscordCommandHandler>>().Object, new Mock<SteamStoreService>().Object);
        DiscordBot discordBot = new(_optionsMock.Object, discordCommandHandler);

        var socketGuildMock = new Mock<SocketGuild>();

        // Act. 
        var exception = await Assert.ThrowsAsync<Exception>(() => 
            discordBot.RegisterSlashCommands("invalid", null, "invalid"));
        
        // Assert.
        Assert.Equal("Discord bot is not connected any guilds", exception.Message);
    }
    
    [Fact]
    public async Task RegisterSlashCommands_CanRegisterCommands()
    {
        //Arrange.
        _optionsMock.Setup(x => x.Value).Returns(new DiscordBotOptions());
        var discordCommandHandler = new DiscordCommandHandler(new Mock<ILogger<DiscordCommandHandler>>().Object, new Mock<SteamStoreService>().Object);
        DiscordBot discordBot = new(_optionsMock.Object, discordCommandHandler);

        //Act and Assert.
        var exception = await Assert.ThrowsAsync<Exception>(() => discordBot.RegisterSlashCommands("invalid", null, "invalid"));
        Assert.Equal("Discord bot is not connected any guilds", exception.Message);
    }*/
}