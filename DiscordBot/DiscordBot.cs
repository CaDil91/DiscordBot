using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace DiscordBot;

public class DiscordBot
{
    private readonly DiscordSocketClient _client;
    private readonly IOptions<DiscordBotOptions> _discordOptions;
    private readonly IDiscordCommandHandler _discordCommandHandler;

    public DiscordBot(IOptions<DiscordBotOptions> discordOptions, IDiscordCommandHandler discordCommandHandler)
    {
        //When working with events that have Cacheable<IMessage, ulong> parameters, you must enable the message cache in your config settings if you plan to use the cached message entity.
        var discordSocketConfig = new DiscordSocketConfig { MessageCacheSize = 100 };
        _client = new DiscordSocketClient(discordSocketConfig);
        _discordOptions = discordOptions;
        _discordCommandHandler = discordCommandHandler;
    }

    public async Task RunAsync()
    {
        await _client.LoginAsync(TokenType.Bot, _discordOptions.Value.DiscordToken);
        await _client.StartAsync();
        
        // Commands only need to be registered once ever.
        if (_discordOptions.Value.RegisterSlashCommands)
        {
            await RegisterSlashCommands("steam", "Search the steam store");
            return; //Do not stay connected
        }

        // Add listeners.
        _client.SlashCommandExecuted += _discordCommandHandler.HandleSlashCommandAsync;

        await Task.Delay(Timeout.Infinite); // Block this task until the program is closed.
    }

    /// <summary>
    /// Register slash commands.
    /// </summary>
    /// <param name="sName"></param>
    /// <param name="sDescription"></param>
    /// <exception>Throws if _client has no guilds</exception>
    public async Task RegisterSlashCommands(string sName, string sDescription = "")
    {
        Thread.Sleep(6000); //Give time for bot to connect.
        
        //Create slash commands
        SlashCommandBuilder? guildCommand = new SlashCommandBuilder()
            .WithName(sName) //Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
            .WithDescription(sDescription) //Descriptions can have a max length of 100.
            .AddOption("title", ApplicationCommandOptionType.String, "Steam title to search the store for.");

        //Create slash command.
        SocketGuild? socketGuild = _client.Guilds?.FirstOrDefault();
        if (socketGuild == null) throw new Exception("Discord bot is not connected any guilds");
        
        SocketGuild guild = _client.GetGuild(socketGuild.Id);
        await guild.CreateApplicationCommandAsync(guildCommand.Build());
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