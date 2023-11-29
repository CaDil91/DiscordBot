using Discord;
using Discord.WebSocket;

namespace DiscordBot;

public interface IDiscordSocketClientAdapter
{
    DiscordSocketClient DiscordSocketClient { get; }
    DiscordSocketRestClient Rest { get; }
    event Func<SocketSlashCommand, Task>? SlashCommandExecuted;
    
    public Task LoginAsync(TokenType tokenType, string token, bool validateToken = true);
    public Task StartAsync();
}