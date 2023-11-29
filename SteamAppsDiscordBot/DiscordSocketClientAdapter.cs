using Discord;
using Discord.WebSocket;

namespace DiscordBot;

/// <summary>
/// Adapter for DiscordNet's Discord.WebSocket.DiscordSocketClient.
/// </summary>
public class DiscordSocketClientAdapter : IDiscordSocketClientAdapter
{
    public DiscordSocketClient DiscordSocketClient { get; }

    public DiscordSocketRestClient Rest => DiscordSocketClient.Rest;
    public event Func<SocketSlashCommand, Task>? SlashCommandExecuted;

    public DiscordSocketClientAdapter(DiscordSocketClient client)
    {
        DiscordSocketClient = client;
        DiscordSocketClient.SlashCommandExecuted += client_SlashCommandExecuted;
    }

    private Task client_SlashCommandExecuted(SocketSlashCommand arg) => SlashCommandExecuted?.Invoke(arg) 
                                                                        ?? Task.CompletedTask;

    /// <summary>
    /// Adapted class has no documentation.
    /// </summary>
    /// <param name="tokenType"></param>
    /// <param name="token"></param>
    /// <param name="validateToken"></param>
    public async Task LoginAsync(TokenType tokenType, string token, bool validateToken = true) => 
        await DiscordSocketClient.LoginAsync(tokenType, token, validateToken);

    /// <summary>
    /// Adapted class has no documentation.
    /// </summary>
    public async Task StartAsync() => await DiscordSocketClient.StartAsync();
}