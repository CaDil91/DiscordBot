/*using Discord.WebSocket;
using DiscordBot.Controllers;
using Microsoft.Extensions.Options;
using Moq;

namespace DiscordBot.DiscordBot.DiscordBot.Core;

public class DiscordBotTests
{
    private readonly Mock<IOptions<DiscordBotOptions>> _optionsMock;
    private readonly Mock<ICommandController> _discordCommandHandler;
    private const string TEST_TOKEN = "MTAwMzA3NTcxMzAwNTUzMTIxNg.GsL_2g.7DxQHrpK31n1Bz6OxRYyMjl3xQD6U9Vvbowbuo";

    private DiscordBot _subjectUnderTest;

    public DiscordBotTests()
    {
        _discordCommandHandler = new Mock<ICommandController>();
        _optionsMock = new Mock<IOptions<DiscordBotOptions>>();

        _subjectUnderTest = new DiscordBot(_optionsMock.Object,
            _discordCommandHandler.Object, new Mock<DiscordSocketClient>().Object);
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
        _subjectUnderTest = new global::DiscordBot.DiscordBot.Core.DiscordBot(_optionsMock.Object,
            _discordCommandHandler.Object, new Mock<DiscordSocketClient>().Object);

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
}*/