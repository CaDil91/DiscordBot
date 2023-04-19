using Discord.WebSocket;
using DiscordBot.DiscordBot.Commands;
using Microsoft.Extensions.Logging;

namespace DiscordBot.DiscordBot;

public class DiscordCommandHandler : IDiscordCommandHandler
{
    private readonly ILogger<DiscordCommandHandler> _logger;

    public DiscordCommandHandler(ILogger<DiscordCommandHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Verify SocketSlashCommand and send to wrapper/adapter.
    /// SocketSlashCommand is difficult to mock for unit testing. All private.
    /// </summary>
    /// <param name="socketSlashCommand">Discord.WebSocket.SocketSlashCommand</param>
    public async Task HandleSlashCommandAsync(SocketSlashCommand? socketSlashCommand)
    {
        if (socketSlashCommand == null)
            _logger.LogWarning("Null command received.");
        else
            await new SlashCommand(socketSlashCommand).ExecuteCommand();
    }
}