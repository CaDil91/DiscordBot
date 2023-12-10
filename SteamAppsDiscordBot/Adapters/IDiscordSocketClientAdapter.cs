using Discord;
using Discord.WebSocket;

namespace SteamAppsDiscordBot.Adapters;

public interface IDiscordSocketClientAdapter
{
    DiscordSocketRestClient Rest { get; }
    IEnumerable<SocketGuild?> Guilds { get; }
    ConnectionState ConnectionState { get; }
    event Func<SocketInteraction, Task>? InteractionCreated;
    event Func<Task>? Ready;
    
    public Task LoginAsync(TokenType tokenType, string? token, bool validateToken = true);
    public Task StartAsync();

    IInteractionContext CreateSocketInteractionContext(SocketInteraction interaction);
}