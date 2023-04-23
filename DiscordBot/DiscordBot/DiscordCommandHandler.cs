using Discord.WebSocket;
using DiscordBot.DiscordBot.Commands;
using Microsoft.Extensions.Logging;
using SteamServices;

namespace DiscordBot.DiscordBot;

public class DiscordCommandHandler : IDiscordCommandHandler
{
    private readonly ILogger<DiscordCommandHandler> _logger;
    private readonly StoreService _steamService;

    public DiscordCommandHandler(ILogger<DiscordCommandHandler> logger, StoreService steamService)
    {
        _logger = logger;
        _steamService = steamService;
    }

    /// <summary>
    /// Verify SocketSlashCommand and send to wrapper/adapter.
    /// SocketSlashCommand is difficult to mock for unit testing. All private.
    /// </summary>
    /// <param name="socketSlashCommand">Discord.WebSocket.SocketSlashCommand</param>
    public async Task HandleSlashCommandAsync(SocketSlashCommand? socketSlashCommand)
    {
        if (socketSlashCommand == null)
        {
            _logger.LogWarning("Null command received.");
            return;
        }

        var slashCommand = new SlashCommand(socketSlashCommand);
        await slashCommand.DeferAsync();
        if (slashCommand.ValidateCommand()) await RunSlashCommandAsync(slashCommand);
        await slashCommand.FollowupAsync();
    }

    private async Task RunSlashCommandAsync(SlashCommand slashCommand)
    {
        slashCommand.ResponseMessage = await _steamService.GetAppsFromStoreAsync("halo");
        await Task.CompletedTask;
    }
}