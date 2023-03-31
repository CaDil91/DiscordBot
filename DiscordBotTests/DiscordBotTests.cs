using Microsoft.Extensions.Options;
using Moq;

namespace DiscordBot;

public class DiscordBotTests
{
    private Mock<IOptions<DiscordBotOptions>> _discordOptions = new();

    [Fact]
    public async Task CanStartBotInFiveSeconds()
    {
        //Arrange.
        DiscordBotOptions discordBotOptions = new();
        var optionsMock = new Mock<IOptions<DiscordBotOptions>>();
        optionsMock.Setup(x => x.Value).Returns(discordBotOptions);
        DiscordBot discordBot = new(optionsMock.Object);

        //Act.
        await discordBot.RunAsync();
        Thread.Sleep(15000);
        bool connected = discordBot.IsConnected();

        //Assert.
        Assert.True(connected);
    }
}