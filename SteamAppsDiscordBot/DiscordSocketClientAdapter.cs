using System.Diagnostics.CodeAnalysis;
using Discord;
using Discord.WebSocket;

namespace DiscordBot;

/// <summary>
/// Adapter for DiscordNet's Discord.WebSocket.DiscordSocketClient.
/// </summary>
[ExcludeFromCodeCoverage]
public class DiscordSocketClientAdapter : IDiscordSocketClientAdapter
{
    public DiscordSocketClient DiscordClient { get; }

    public DiscordSocketRestClient Rest => DiscordClient.Rest;
    public IEnumerable<SocketGuild?> Guilds => DiscordClient.Guilds;


    public ConnectionState ConnectionState => DiscordClient.ConnectionState;
    
    public event Func<SocketSlashCommand, Task>? SlashCommandExecuted;
    public event Func<SocketMessageComponent, Task>? ButtonExecuted;
    public event Func<SocketInteraction, Task>? InteractionCreated;

    public DiscordSocketClientAdapter()
    {
        // When working with events that have Cacheable<IMessage, ulong> parameters,
        // you must enable the message cache in your config settings if you plan to
        // use the cached message entity. 
        DiscordSocketConfig config = new() { MessageCacheSize = 100 };
        DiscordClient = new DiscordSocketClient(config);
        
        DiscordClient.SlashCommandExecuted += client_SlashCommandExecuted;
        DiscordClient.ButtonExecuted += client_ButtonExecuted;
        DiscordClient.InteractionCreated += client_InteractionCreated;
    }

    private Task client_SlashCommandExecuted(SocketSlashCommand arg) => SlashCommandExecuted?.Invoke(arg) 
                                                                        ?? Task.CompletedTask;
    private Task client_ButtonExecuted(SocketMessageComponent arg) => ButtonExecuted?.Invoke(arg) 
                                                                      ?? Task.CompletedTask;
    private Task client_InteractionCreated(SocketInteraction arg) => InteractionCreated?.Invoke(arg) 
                                                                     ?? Task.CompletedTask;

    /// <summary>
    /// Adapted class has no documentation.
    /// </summary>
    /// <param name="tokenType"></param>
    /// <param name="token"></param>
    /// <param name="validateToken"></param>
    public async Task LoginAsync(TokenType tokenType, string? token, bool validateToken = true) => 
        await DiscordClient.LoginAsync(tokenType, token, validateToken);

    /// <summary>
    /// Adapted class has no documentation.
    /// </summary>
    public async Task StartAsync() => await DiscordClient.StartAsync();
}