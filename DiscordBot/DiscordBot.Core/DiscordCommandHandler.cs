using Discord;
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
    /// <param name="discordNetSlashCommand">Discord.WebSocket.SocketSlashCommand</param>
    public async Task HandleSlashCommandAsync(SocketSlashCommand? discordNetSlashCommand)
    {
        if (discordNetSlashCommand == null)
        {
            _logger.LogWarning("Null command received.");
            return;
        }

        var slashCommand = new SlashCommand(discordNetSlashCommand);
        if (!slashCommand.ValidateCommand()) return;
        
        await slashCommand.DeferAsync();
        List<Embed> commandResult = await RunSlashCommandAsync(slashCommand);

        if (commandResult.Count > 0)
        {
            await slashCommand.FollowupAsync("", embeds: commandResult.ToArray());
        }
        else
        {
            await slashCommand.FollowupAsync("No results found.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="slashCommand"></param>
    private async Task<List<Embed>> RunSlashCommandAsync(SlashCommand slashCommand)
    {
        SocketSlashCommandDataOption? test = slashCommand.Data?.Options.FirstOrDefault();
        
        List<SteamApp> steamApps = new();
        try
        {
            steamApps = await _steamService.GetAppsAsync(test?.Value.ToString() ?? string.Empty, 3);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error getting SteamApps.");
        }

        // Return results to user.
        List<Embed> embeds = steamApps
            .Select(steamApp => new EmbedBuilder()
                .WithTitle(steamApp.Name)
                .WithUrl(steamApp.Url)
                .WithDescription(steamApp.ShortDescription)
                .WithImageUrl(steamApp.HeaderImage)
                .Build())
            .ToList();

        return embeds;
    }
}