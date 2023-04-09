using Microsoft.Extensions.Options;
using Moq;
using SteamServices;

namespace DiscordBot;

public class DiscordBotTests
{
    private readonly Mock<IOptions<DiscordBotOptions>> _optionsMock;
    private readonly Mock<StoreService> _steamServiceMock;

    public DiscordBotTests()
    {
        _steamServiceMock = new Mock<StoreService>();
        _optionsMock = new Mock<IOptions<DiscordBotOptions>>();
    }

    [Fact]
    public void CanStartBotInFiveSeconds()
    {
        //Arrange.
        //TODO: Test options?
        DiscordBotOptions discordBotOptions = new()
        {
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
}