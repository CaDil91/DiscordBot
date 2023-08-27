using Discord.WebSocket;

namespace DiscordBot.Controllers;

public interface ICommandController
{
    public Task RunSlashCommandAsync(SocketSlashCommand socketSlashCommand);
    public Task HandleButtonCommandAsync(SocketMessageComponent socketMessageComponent);
}