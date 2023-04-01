using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Moq;
using NuGet.Frameworks;

namespace DiscordBot;

public class DiscordBotTests
{
    private Mock<IOptions<DiscordBotOptions>> _discordOptions = new();

    [Fact]
    public async Task RegisterSlashCommandsThrowsIfRegisteringWithoutConnecting()
    {
        //Arrange.
        DiscordBotOptions discordBotOptions = new();
        var optionsMock = new Mock<IOptions<DiscordBotOptions>>();
        optionsMock.Setup(x => x.Value).Returns(discordBotOptions);
        DiscordBot discordBot = new(optionsMock.Object);

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
        DiscordBot discordBot = new(optionsMock.Object);

        //Act.
        Task _ = discordBot.RunAsync();
        Thread.Sleep(5000);
        bool connected = discordBot.IsConnected();

        //Assert.
        Assert.True(connected);
    }
}