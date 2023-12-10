using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SteamAppsDiscordBot.Adapters;

namespace SteamAppsDiscordBot;

public class DiscordBot
{
    private readonly DiscordSocketClientAdapter _client;
    private readonly InteractionServiceAdapter _interactionService;
    
    private readonly IOptions<DiscordBotOptions> _discordOptions;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DiscordBot> _logger;

    public DiscordBot(DiscordSocketClientAdapter client, InteractionServiceAdapter interactionService, IOptions<DiscordBotOptions> discordOptions, IServiceProvider serviceProvider, ILogger<DiscordBot> logger)
    {
        _client = client;
        _interactionService = interactionService;
        _discordOptions = discordOptions;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Asynchronously initializes our Command handling using Discord.Net's InteractionService implementation.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the operated assembly cannot be retrieved.</exception>
    public async Task InitializeInteractionServicesAsync()
    {
        // Process when the client is ready, so we can register our commands.
        _client.Ready += ReadyAsync;

        // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly() ?? throw new InvalidOperationException(),
            _serviceProvider);

        // Process the InteractionCreated payloads to execute Interactions commands
        _client.InteractionCreated += HandleInteractionAsync;
    }

    /// <summary>
    /// Logs in and starts the bot, then delays execution for a specified timeout.
    /// </summary>
    /// <param name="timeout">Delay before the method completes, default is infinite.</param>
    public async Task RunAsync(int timeout = Timeout.Infinite)
    {
        await InitializeInteractionServicesAsync().ConfigureAwait(false);
        
        await _client.LoginAsync(TokenType.Bot, _discordOptions.Value.DiscordToken).ConfigureAwait(false);
        await _client.StartAsync().ConfigureAwait(false);

        await Task.Delay(timeout);
    }

    private async Task HandleInteractionAsync(SocketInteraction interaction)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
            IInteractionContext context = _client.CreateSocketInteractionContext(interaction);

            // Execute the incoming command.
            IResult result = await _interactionService.ExecuteCommandAsync(context, _serviceProvider);

            if (!result.IsSuccess)
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                    case InteractionCommandError.UnknownCommand:
                    case InteractionCommandError.ConvertFailed:
                    case InteractionCommandError.BadArgs:
                    case InteractionCommandError.Exception:
                    case InteractionCommandError.Unsuccessful:
                    case InteractionCommandError.ParseFailed:
                    case null:
                        _logger.LogError(result.ErrorReason);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(result.Error.ToString());
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
    
    private async Task ReadyAsync() => await _interactionService.RegisterCommandsGloballyAsync();
}