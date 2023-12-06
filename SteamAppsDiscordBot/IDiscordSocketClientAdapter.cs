using Discord;
using Discord.WebSocket;

namespace DiscordBot;

public interface IDiscordSocketClientAdapter
{
    DiscordSocketRestClient Rest { get; }
    IEnumerable<SocketGuild?> Guilds { get; }
    ConnectionState ConnectionState { get; }
    event Func<SocketSlashCommand, Task>? SlashCommandExecuted;
    event Func<SocketMessageComponent, Task>? ButtonExecuted;
    public event Func<SocketInteraction, Task>? InteractionCreated;
    
    public Task LoginAsync(TokenType tokenType, string? token, bool validateToken = true);
    public Task StartAsync();

    public Task RunAsync(int timeout = Timeout.Infinite);
    public void Initialize();
}