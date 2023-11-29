using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace DiscordBot;

public class DiscordBot : IDiscordBot
{
    private readonly IDiscordSocketClientAdapter _client;
    private readonly IOptions<DiscordBotOptions> _discordOptions;

    public DiscordBot(IOptions<DiscordBotOptions> discordOptions, IDiscordSocketClientAdapter discordClient)
    {
        //When working with events that have Cacheable<IMessage, ulong> parameters,
        //you must enable the message cache in your config settings if you plan to use the cached message entity.
        //var discordSocketConfig = new DiscordSocketConfig { MessageCacheSize = 100 };
        _client = discordClient;
        _discordOptions = discordOptions;
    }

    public async Task RunAsync(IServiceProvider serviceProvider)
    {
        await _client.LoginAsync(TokenType.Bot, _discordOptions.Value.DiscordToken);
        await _client.StartAsync();
        await SetupInteractionService(serviceProvider);
        
        //_client.ButtonExecuted += _discordCommandController.HandleButtonCommandAsync;

        // Block this task until the program is closed.
        await Task.Delay(Timeout.Infinite);
    }

    /// <summary>
    /// Create a service provider for Discord.net's interactionService.
    /// The interaction service handles incoming commands, and calls the appropriate module.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    private async Task SetupInteractionService(IServiceProvider serviceProvider)
    {
        var interactionService = new InteractionService(_client.Rest);
        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
        
        // Add listeners.
        _client.SlashCommandExecuted += async (interaction) =>
        {
            var socketInteractionContext = new SocketInteractionContext<SocketSlashCommand>(_client.DiscordSocketClient, interaction);
            await interactionService.ExecuteCommandAsync(socketInteractionContext, serviceProvider);
        };
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