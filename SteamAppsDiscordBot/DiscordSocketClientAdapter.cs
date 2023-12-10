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
    private readonly DiscordSocketClient _clientAdaptee;
    private readonly IOptions<DiscordBotOptions> _discordOptions;
    private readonly IServiceProvider _serviceProvider;
    private readonly IInteractionServiceAdapter _interactionService;

    public DiscordSocketRestClient Rest => _clientAdaptee.Rest;
    public IEnumerable<SocketGuild?> Guilds => _clientAdaptee.Guilds;
    public ConnectionState ConnectionState => _clientAdaptee.ConnectionState;
    
    public event Func<SocketSlashCommand, Task>? SlashCommandExecuted
    {
        add => _clientAdaptee.SlashCommandExecuted += value;
        remove => _clientAdaptee.SlashCommandExecuted -= value;
    }
    public event Func<SocketMessageComponent, Task>? ButtonExecuted
    {
        add => _clientAdaptee.ButtonExecuted += value;
        remove => _clientAdaptee.ButtonExecuted -= value;
    }
    public event Func<SocketInteraction, Task>? InteractionCreated
    {
        add => _clientAdaptee.InteractionCreated += value;
        remove => _clientAdaptee.InteractionCreated -= value;
    }
    public event Func<Task>? Ready
    {
        add => _clientAdaptee.Ready += value;
        remove => _clientAdaptee.Ready -= value;
    }

    public DiscordSocketClientAdapter(DiscordSocketClient clientAdaptee, IOptions<DiscordBotOptions> discordOptions, 
        IInteractionServiceAdapter interactionService, IServiceProvider serviceProvider)
    {
        _clientAdaptee = clientAdaptee;
        _discordOptions = discordOptions;
        _serviceProvider = serviceProvider;
        _interactionService = interactionService;
    }

    /// <summary>
    /// Creates a new socket interaction context using our adapted DiscordSocketClient.
    /// </summary>
    /// <param name="interaction">The socket interaction.</param>
    /// <returns>
    /// A new instance of the <see cref="T:Discord.Interactions.SocketInteractionContext" />
    /// class using the provided socket interaction.
    /// </returns>
    public IInteractionContext CreateSocketInteractionContext(SocketInteraction interaction) 
        => new SocketInteractionContext(_clientAdaptee, interaction);

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
        //_client.Ready += ReadyAsync;
        
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly() ?? throw new InvalidOperationException(), _serviceProvider);
        //_client.InteractionCreated += HandleInteraction;
        
        await _clientAdaptee.LoginAsync(TokenType.Bot, _discordOptions.Value.DiscordToken);
        await _clientAdaptee.StartAsync();

        await Task.Delay(timeout);
    }

    /// <summary>
    /// Adapted class has no documentation.
    /// </summary>
    /// <param name="tokenType"></param>
    /// <param name="token"></param>
    /// <param name="validateToken"></param>
    public async Task LoginAsync(TokenType tokenType, string? token, bool validateToken = true) => 
        await _clientAdaptee.LoginAsync(tokenType, token, validateToken);

    /// <summary>
    /// Adapted class has no documentation.
    /// </summary>
    public async Task StartAsync() => await _clientAdaptee.StartAsync();
}