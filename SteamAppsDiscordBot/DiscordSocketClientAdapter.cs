using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace DiscordBot;

/// <summary>
/// Adapter for DiscordNet's Discord.WebSocket.DiscordSocketClient.
/// </summary>
public class DiscordSocketClientAdapter : IDiscordSocketClientAdapter
{
    private readonly DiscordSocketClient _client;
    private readonly IOptions<DiscordBotOptions> _discordOptions;
    private readonly IServiceProvider _serviceProvider;
    private readonly IInteractionServiceAdapter _interactionService;

    public DiscordSocketRestClient Rest => _client.Rest;
    public IEnumerable<SocketGuild?> Guilds => _client.Guilds;
    public ConnectionState ConnectionState => _client.ConnectionState;
    
    public event Func<SocketSlashCommand, Task>? SlashCommandExecuted;
    public event Func<SocketMessageComponent, Task>? ButtonExecuted;
    public event Func<SocketInteraction, Task>? InteractionCreated;
    public event Func<Task>? Ready;

    public DiscordSocketClientAdapter(DiscordSocketClient client, IOptions<DiscordBotOptions> discordOptions, 
        IInteractionServiceAdapter interactionService, IServiceProvider serviceProvider)
    {
        _client = client;
        _discordOptions = discordOptions;
        _serviceProvider = serviceProvider;
        _interactionService = interactionService;
    }
    
    /// <summary>
    /// Initializes the application's client event handlers. 
    /// </summary>
    public void Initialize()
    {
        _client.SlashCommandExecuted += client_SlashCommandExecuted;
        _client.ButtonExecuted += client_ButtonExecuted;
        _client.InteractionCreated += client_InteractionCreated;
        _client.Ready += client_Ready;
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

    /// <summary>
    /// Adapted class has no documentation.
    /// </summary>
    /// <param name="tokenType"></param>
    /// <param name="token"></param>
    /// <param name="validateToken"></param>
    public async Task LoginAsync(TokenType tokenType, string? token, bool validateToken = true) => 
        await _client.LoginAsync(tokenType, token, validateToken);

    /// <summary>
    /// Adapted class has no documentation.
    /// </summary>
    public async Task StartAsync() => await _client.StartAsync();

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
            var context = new SocketInteractionContext(_client, interaction);

            // Execute the incoming command.
            IResult result = await _interactionService.ExecuteCommandAsync(context, _serviceProvider);

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
    
    private async Task ReadyAsync() => await _interactionService.RegisterCommandsGloballyAsync();
    private Task client_SlashCommandExecuted(SocketSlashCommand arg) => SlashCommandExecuted?.Invoke(arg) ?? Task.CompletedTask;
    private Task client_ButtonExecuted(SocketMessageComponent arg) => ButtonExecuted?.Invoke(arg) ?? Task.CompletedTask;
    private Task client_InteractionCreated(SocketInteraction arg) => InteractionCreated?.Invoke(arg) ?? Task.CompletedTask;
    private Task client_Ready() => Ready?.Invoke() ?? Task.CompletedTask;
}