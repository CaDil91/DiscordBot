using Discord.WebSocket;

namespace DiscordBot.DiscordBot.Core;

public interface IDiscordCommandHandler
{
    public Task HandleSlashCommandAsync(SocketSlashCommand commandWrapper);
}