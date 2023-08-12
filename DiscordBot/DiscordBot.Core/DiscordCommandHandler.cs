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
    private readonly DiscordButtonController _discordButtonController;
    private const string STEAM_FOLLOW_CUSTOM_ID = "Steam_Follow";
    private const string STEAM_UNFOLLOW_CUSTOM_ID = "Steam_Unfollow";

    public DiscordCommandHandler(ILogger<DiscordCommandHandler> logger, IStoreService steamService, DiscordButtonController discordButtonController)
    {
        _logger = logger;
        _steamService = steamService;
        _discordButtonController = discordButtonController;
    }

    /// <summary>
    /// Verify SocketSlashCommand and send to wrapper/adapter.
    /// SocketSlashCommand is difficult to mock for unit testing. All private.
    /// </summary>
    /// <param name="discordNetSlashCommand">Discord.WebSocket.SocketSlashCommand</param>
    public async Task HandleSlashCommandAsync(SocketSlashCommand? discordNetSlashCommand)
    {
        // Get valid command from slash command.
        if (discordNetSlashCommand == null)
        {
            _logger.LogError("Null slash command received.");
            return;
        }
        var slashCommand = new SocketSlashCommandAdapter(discordNetSlashCommand);
        await slashCommand.DeferAsync(); // Defer response to avoid timeout.
        
        //if (!slashCommand.ValidateCommand()) return; // TODO: Move to RunSlashCommandAsync?
        
        List<DiscordResponse> discordResponses = await RunSlashCommandAsync(slashCommand);
        foreach (DiscordResponse response in discordResponses) await slashCommand.FollowupAsync
            (response.Message, embeds: response.Embeds?.ToArray() ?? null, components: response.MessageComponents ?? null);
    }

    public async Task HandleButtonCommandAsync(SocketMessageComponent component)
    {
        var socketMessageComponentAdapter = new SocketMessageComponentAdapter(component); // Convert to adapter.
        await socketMessageComponentAdapter.DeferAsync(); // Defer response to avoid timeout.
        
        string? commandName = socketMessageComponentAdapter.GetCommandName();
        if (commandName == null)
        {
            _logger.LogError("Unable to get command name from button.");
            await socketMessageComponentAdapter.RespondAsync("Unable to find command name from component.");
            return;
        }

        var discordResponses = new List<DiscordResponse>();
        switch (commandName)
        {
            case STEAM_FOLLOW_CUSTOM_ID:
                discordResponses = await _discordButtonController.FollowSteamAppAsync(socketMessageComponentAdapter);
                break;
            case STEAM_UNFOLLOW_CUSTOM_ID:
                discordResponses = await _discordButtonController.UnfollowSteamAppAsync(socketMessageComponentAdapter);
                break;
            default:
                _logger.LogError($"Invalid command name {commandName}.");
                await socketMessageComponentAdapter.RespondAsync("Invalid command name.");
                break;
        }
        
        foreach (DiscordResponse discordResponse in discordResponses) await socketMessageComponentAdapter.FollowupAsync(discordResponse.Message);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="socketSlashCommandAdapter"></param>
    private async Task<List<DiscordResponse>> RunSlashCommandAsync(SocketSlashCommandAdapter socketSlashCommandAdapter)
    {
        SocketSlashCommandDataOption? dataOptions = socketSlashCommandAdapter.Data?.Options.FirstOrDefault();
        
        List<SteamApp> steamApps = new();
        try
        {
            steamApps = await _steamService.GetAppsAsync(dataOptions?.Value.ToString() ?? string.Empty, 1);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error getting SteamApps.");
        }

        return steamApps.Select(CreateResponse).ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="steamApp"></param>
    /// <returns></returns>
    private static DiscordResponse CreateResponse(SteamApp steamApp)
    {
        // Create embeds
        Embed? embed = new EmbedBuilder()
            .WithTitle(steamApp.Name)
            .WithUrl(steamApp.Url)
            .WithDescription(steamApp.ShortDescription)
            .WithImageUrl(steamApp.HeaderImage)
            .Build();
        if (embed == null) return new DiscordResponse { Message = "Error finding results"};

        ButtonBuilder followButton = new()
        {
            Label = "Follow",
            Style = ButtonStyle.Primary,
            CustomId = STEAM_FOLLOW_CUSTOM_ID
        };
        ButtonBuilder unfollowButton = new()
        {
            Label = "Unfollow",
            Style = ButtonStyle.Danger,
            CustomId = STEAM_UNFOLLOW_CUSTOM_ID
        };
        
        ComponentBuilder componentBuilder = new();
        MessageComponent component = componentBuilder.WithButton(followButton).WithButton(unfollowButton).Build();

        var response = new DiscordResponse
        {
            Embeds = new List<Embed> { embed },
            MessageComponents = component
        };

        return response;
    }
}