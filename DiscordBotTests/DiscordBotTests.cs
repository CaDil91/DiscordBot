using Microsoft.Extensions.Options;
using Moq;

namespace DiscordBot;

public class DiscordBotTests
{
    private readonly Mock<SteamService> _steamServiceMock;
    private readonly Mock<IOptions<DiscordBotOptions>> _optionsMock;

    public DiscordBotTests()
    {
        _steamServiceMock = new Mock<SteamService>();
        _optionsMock = new Mock<IOptions<DiscordBotOptions>>();
    }
    
    [Fact]
    public async Task RegisterSlashCommandsThrowsIfRegisteringWithoutConnecting()
    {
        //Arrange.
        _optionsMock.Setup(x => x.Value).Returns(new DiscordBotOptions());
        var discordCommandHandler = new DiscordCommandHandler(_steamServiceMock.Object);
        DiscordBot discordBot = new(_optionsMock.Object, discordCommandHandler);

        //Act and Assert.
        var exception = await Assert.ThrowsAsync<Exception>(() => discordBot.RegisterSlashCommands("invalid",
            "should unregister this command"));
        Assert.Equal("Discord bot is not connected any guilds", exception.Message);
    }

    [Fact]
    public void CanStartBotInFiveSeconds()
    {
        //Arrange.
        DiscordBotOptions discordBotOptions = new()
        {
            //TODO: Test options?
            DiscordToken = "MTAwMzA3NTcxMzAwNTUzMTIxNg.GsL_2g.7DxQHrpK31n1Bz6OxRYyMjl3xQD6U9Vvbowbuo"
        };
        var optionsMock = new Mock<IOptions<DiscordBotOptions>>();
        optionsMock.Setup(x => x.Value).Returns(discordBotOptions);
        var discordCommandHandler = new DiscordCommandHandler(_steamServiceMock.Object);
        DiscordBot discordBot = new(optionsMock.Object, discordCommandHandler);

        //Act.
        Task _ = discordBot.RunAsync();
        Thread.Sleep(5000);
        bool connected = discordBot.IsConnected();

        //Assert.
        Assert.True(connected);
    }
}