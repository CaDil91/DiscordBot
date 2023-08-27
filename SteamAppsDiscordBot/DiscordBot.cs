using Discord;
using Discord.WebSocket;
using DiscordBot.Controllers;
using Microsoft.Extensions.Options;

namespace DiscordBot;

public class DiscordBot : IDiscordBot
{
    private readonly DiscordSocketClient _client;
    private readonly ICommandController _discordCommandController;
    private readonly IOptions<DiscordBotOptions> _discordOptions;

    public DiscordBot(IOptions<DiscordBotOptions> discordOptions, ICommandController discordCommandController, DiscordSocketClient discordClient)
    {
        //When working with events that have Cacheable<IMessage, ulong> parameters,
        //you must enable the message cache in your config settings if you plan to use the cached message entity.
        //var discordSocketConfig = new DiscordSocketConfig { MessageCacheSize = 100 };
        _client = discordClient;
        _discordOptions = discordOptions;
        _discordCommandController = discordCommandController;
    }

    public async Task RunAsync()
    {
        await _client.LoginAsync(TokenType.Bot, _discordOptions.Value.DiscordToken);
        await _client.StartAsync();

        // Add listeners.
        _client.SlashCommandExecuted += _discordCommandController.RunSlashCommandAsync;
        _client.ButtonExecuted += _discordCommandController.HandleButtonCommandAsync;

        await Task.Delay(Timeout.Infinite); // Block this task until the program is closed.
    }

    
        
/*#if REGISTER_COMMANDS
        //TODO: move
        // Commands only need to be registered once ever.
        await RegisterSlashCommands("steam", _client.Guilds?.FirstOrDefault(), "Search the steam store");
        return; //Do not stay connected
#endif
    /// <summary>
    /// Register slash commands.
    /// </summary>
    /// <param name="sName"></param>
    /// <param name="socketGuild"></param>
    /// <param name="sDescription"></param>
    /// <exception>Throws if _client has no guilds</exception>
    public async Task RegisterSlashCommands(string sName, SocketGuild? socketGuild, string sDescription = "")
    {
        Thread.Sleep(6000); //Give time for bot to connect.

        //Create slash commands
        SlashCommandBuilder? guildCommand = new SlashCommandBuilder()
            .WithName(sName) //Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
            .WithDescription(sDescription) //Descriptions can have a max length of 100.
            .AddOption("title", ApplicationCommandOptionType.String, "Steam title to search the store for.");

        //Create slash command.
        if (socketGuild == null) throw new Exception("Discord bot is not connected any guilds");
        
        await socketGuild.CreateApplicationCommandAsync(guildCommand.Build());
    }

    /// <summary>
    /// Check if DiscordBotSocketClient.ConnectionState.Connected == True.
    /// </summary>
    /// <returns>bool</returns>
    public bool IsConnected() => _client.ConnectionState == ConnectionState.Connected;*/
}