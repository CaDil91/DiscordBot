using Discord.WebSocket;
using DiscordBot.DiscordBot.Commands;
using Microsoft.Extensions.Logging;
using SteamServices;

namespace DiscordBot.DiscordBot.Core;

public class DiscordCommandHandler : IDiscordCommandHandler
{
    private readonly ILogger<DiscordCommandHandler> _logger;
    private readonly IStoreService _steamService;

    public DiscordCommandHandler(ILogger<DiscordCommandHandler> logger, IStoreService steamService)
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
        if (!slashCommand.ValidateCommand()) return;
        string commandResult = await RunSlashCommandAsync(slashCommand);
        await slashCommand.FollowupAsync(commandResult);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="slashCommand"></param>
    private async Task<string> RunSlashCommandAsync(ICommand slashCommand)
    {
        List<SteamApp> steamApps = new();
        try
        {
            steamApps = await _steamService.GetAppsAsync("halo", 3);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error getting SteamApps.");
        }

        // Return results to user.
        List<string> steamAppUrls;
        if (steamApps.Count == 0)
        {
            steamAppUrls = new List<string> {"No results found."};
        }
        else
        {
            steamAppUrls = steamApps
                .Select(steamApp => $"steam://openurl/{steamApp.Url}")
                .ToList();
        }

        return string.Join("\n", steamAppUrls);
    }
}