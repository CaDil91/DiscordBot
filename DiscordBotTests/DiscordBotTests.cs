using Microsoft.Extensions.Options;
using Moq;

namespace DiscordBot;

public class DiscordBotTests
{
    private Mock<IOptions<DiscordOptions>> _discordOptions = new();

    [Fact]
    public async Task CanStartBot()
    {
        //Arrange.
        IOptions<DiscordOptions> options = Options.Create<DiscordOptions>(new DiscordOptions());
        var optionsMock = new Mock<IOptions<DiscordOptions>>();
        DiscordBot discordBot = new(optionsMock.Object);

        //Act.
        await discordBot.RunAsync();
        bool connected = discordBot.IsConnected();

        //Assert.
        Assert.True(connected);
    }
}