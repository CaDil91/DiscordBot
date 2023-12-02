using System.Diagnostics.CodeAnalysis;
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
    private readonly IServiceProvider _serviceProvider;
    private readonly IInteractionServiceAdapter _interactionService;

    public DiscordBot(IOptions<DiscordBotOptions> discordOptions, IDiscordSocketClientAdapter discordClient, 
        IServiceProvider serviceProvider)
    {
        //When working with events that have Cacheable<IMessage, ulong> parameters,
        //you must enable the message cache in your config settings if you plan to use the cached message entity.
        //var discordSocketConfig = new DiscordSocketConfig { MessageCacheSize = 100 };
        _client = discordClient;
        _discordOptions = discordOptions;
        _serviceProvider = serviceProvider;
        _interactionService = new InteractionServiceAdapter(_client.Rest);
    }
    
    // Overloaded constructor for testing purposes to allow injection of InteractionService mock
    public DiscordBot(IOptions<DiscordBotOptions> discordOptions, IDiscordSocketClientAdapter discordClient, 
        IServiceProvider serviceProvider, IInteractionServiceAdapter interactionService) 
        : this(discordOptions, discordClient, serviceProvider)
    {
        _interactionService = interactionService;
    }

    public async Task RunAsync(int timeout = Timeout.Infinite)
    {
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly() ?? throw new InvalidOperationException(), _serviceProvider);
        
        await _client.LoginAsync(TokenType.Bot, _discordOptions.Value.DiscordToken);
        await _client.StartAsync();
        _client.SlashCommandExecuted += OnSlashCommandExecuted;
        _client.ButtonExecuted += OnButtonExecuted;

        // Block this task until the program is closed.
        await Task.Delay(timeout);
    }
    
    /// <summary>
    /// This method is invoked when a slash command is executed in the discord server. It's responsible for creating
    /// the interaction context and passing it down for further execution.
    /// </summary>
    /// <remarks> Excluded from code coverage because this is just an override for SlashCommandExecuted </remarks>
    /// <param name="interaction">The slash command interaction data from the Discord client.</param>
    /// <returns>A Task representing the asynchronous operation of the command execution.</returns>
    [ExcludeFromCodeCoverage]
    private async Task OnSlashCommandExecuted(SocketSlashCommand interaction)
    {   
        var socketInteractionContext = new SocketInteractionContext<SocketSlashCommand>(_client.DiscordSocketClient, interaction);
        await _interactionService.ExecuteCommandAsync(socketInteractionContext, _serviceProvider);
    }
    
    /// <summary>
    /// This method is invoked when a button is clicked in the discord server. It's responsible for creating
    /// the interaction context and passing it down for further execution. 
    /// </summary>
    /// <remarks> Excluded from code coverage because this is just an override for SlashCommandExecuted </remarks>
    /// <param name="interaction"></param>
    [ExcludeFromCodeCoverage]
    private async Task OnButtonExecuted(SocketMessageComponent interaction)
    {
        await Task.CompletedTask; //TODO: Implement
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