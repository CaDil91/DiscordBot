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
        DiscordOptions discordOptions = new()
        {
            DiscordToken = "invalid"
        };
        var optionsMock = new Mock<IOptions<DiscordOptions>>();
        optionsMock.Setup(x => x.Value).Returns(discordOptions);
        DiscordBot discordBot = new(optionsMock.Object);

        //Act.
        await discordBot.RunAsync();
        bool connected = discordBot.IsConnected();

        //Assert.
        Assert.True(connected);
    }
}