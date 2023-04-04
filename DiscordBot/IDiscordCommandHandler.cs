using Discord.WebSocket;

namespace DiscordBot;

public interface IDiscordCommandHandler
{
    public Task HandleSlashCommandAsync(SocketSlashCommand command);
}