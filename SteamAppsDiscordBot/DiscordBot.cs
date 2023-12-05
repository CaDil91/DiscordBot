using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace DiscordBot;

public class DiscordBot : IDiscordBot
{
    private readonly DiscordSocketClient _client;
    private readonly IOptions<DiscordBotOptions> _discordOptions;
    private readonly IServiceProvider _serviceProvider;
    private InteractionService _interactionService;

    public DiscordBot(IOptions<DiscordBotOptions> discordOptions, DiscordSocketClient discordClient, 
        IServiceProvider serviceProvider)
    {
        //When working with events that have Cacheable<IMessage, ulong> parameters,
        //you must enable the message cache in your config settings if you plan to use the cached message entity.
        //var discordSocketConfig = new DiscordSocketConfig { MessageCacheSize = 100 };
        _client = discordClient;
        _discordOptions = discordOptions;
        _serviceProvider = serviceProvider;
        _interactionService = new InteractionService(_client.Rest);
    }
    
    // Overloaded constructor for testing purposes to allow injection of InteractionService mock
    public DiscordBot(IOptions<DiscordBotOptions> discordOptions, DiscordSocketClient discordClient, 
        IServiceProvider serviceProvider, InteractionService interactionService) 
        : this(discordOptions, discordClient, serviceProvider)
    {
        _interactionService = interactionService;
    }

    
    /// <summary>
    /// An asynchronous method that allows the bot to run with support for optional program cancellation after a specific timeout.
    /// It performs several steps including:
    /// 1. Adding modules from the entry assembly to the interaction service using the provided service provider.
    /// 2. Logging into the client with the bot token.
    /// 3. Starting the client.
    /// 4. Hooking up handlers for slash command and button execution events.
    /// 5. Blocking the current task until the given timeout or until the program is closed.
    /// </summary>
    /// <param name="timeout">Optional timeout for program cancellation, infinite by default.</param>
    public async Task RunAsync(int timeout = Timeout.Infinite)
    {
        _client.Ready += ReadyAsync;
        
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly() ?? throw new InvalidOperationException(), _serviceProvider);
        _client.InteractionCreated += HandleInteraction;
        
        await _client.LoginAsync(TokenType.Bot, _discordOptions.Value.DiscordToken);
        await _client.StartAsync();

        await Task.Delay(timeout);
    }

    private async Task ReadyAsync() => await _interactionService.RegisterCommandsGloballyAsync();
    
    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
            var context = new SocketInteractionContext(_client, interaction);

            // Execute the incoming command.
            IResult? result = await _interactionService.ExecuteCommandAsync(context, _serviceProvider);

            if (!result.IsSuccess)
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // TODO: implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        break;
                    case InteractionCommandError.ConvertFailed:
                        break;
                    case InteractionCommandError.BadArgs:
                        break;
                    case InteractionCommandError.Exception:
                        break;
                    case InteractionCommandError.Unsuccessful:
                        break;
                    case InteractionCommandError.ParseFailed:
                        break;
                    case null:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }
        catch
        {
            // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist.
            // It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (interaction.Type is InteractionType.ApplicationCommand)
                await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
        }
    }

    /*//TODO: move
    // Commands only need to be registered once ever.
    await RegisterSlashCommands("steam", _client.Guilds?.FirstOrDefault(), "Search the Steam store");
    return; //Do not stay connected*/
    /// <summary>
    /// Register slash commands.
    /// </summary>
    /// <param name="sName"></param>
    /// <param name="socketGuild"></param>
    /// <param name="sDescription"></param>
    /// <exception>Throws if _client has no guilds</exception>
    [ExcludeFromCodeCoverage]
    private async Task RegisterSlashCommands(string sName, string sDescription = "")
    {
        await Task.Delay(10000); //Give time for bot to connect.

        SocketGuild? socketGuild = _client.Guilds.FirstOrDefault();
        if (socketGuild == null) throw new Exception("Discord bot is not connected any guilds");

        //Create slash commands
        SlashCommandBuilder? guildCommand = new SlashCommandBuilder()
            .WithName(sName) //Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
            .WithDescription(sDescription) //Descriptions can have a max length of 100.
            .AddOption("title", ApplicationCommandOptionType.String, "Steam title to search the store for.");

        
        await socketGuild.CreateApplicationCommandAsync(guildCommand.Build());
    }
}