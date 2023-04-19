using Discord.WebSocket;

namespace DiscordBot.DiscordBot;

public interface IDiscordCommandHandler
{
    public Task HandleSlashCommandAsync(SocketSlashCommand commandWrapper);
}