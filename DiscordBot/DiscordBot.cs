using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace DiscordBot;

public class DiscordBot
{
    private readonly DiscordSocketClient _client;
    private readonly IOptions<DiscordOptions> _discordOptions;

    public DiscordBot(IOptions<DiscordOptions> discordOptions)
    {
        //When working with events that have Cacheable<IMessage, ulong> parameters, you must enable the message cache in your config settings if you plan to use the cached message entity.
        var discordSocketConfig = new DiscordSocketConfig { MessageCacheSize = 100 };
        _client = new DiscordSocketClient(discordSocketConfig);
        _discordOptions = discordOptions;
    }

    public async Task RunAsync()
    {
        await _client.LoginAsync(TokenType.Bot, _discordOptions.Value.DiscordToken);
        await _client.StartAsync();
        _client.Ready += () =>
        {
            Debug.WriteLine("Bot is connected!");
            return Task.CompletedTask;
        };

        /*//Create basic commandHandler, and setup
        _commandHandler = new CommandHandler(_client, new CommandService(), _services);
        await _commandHandler.InstallCommandsAsync();

        await Task.CompletedTask;*/
    }

    /// <summary>
    ///     Check if DiscordBotSocketClient.ConnectionState.Connected == True.
    /// </summary>
    /// <returns>bool</returns>
    public bool IsConnected()
    {
        return _client.ConnectionState == ConnectionState.Connected;
    }
}