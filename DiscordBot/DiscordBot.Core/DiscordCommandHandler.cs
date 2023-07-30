using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.DiscordBot.Commands;
using Microsoft.Extensions.Logging;
using SteamServices;

namespace DiscordBot.DiscordBot.Core;

public class DiscordCommandHandler : IDiscordCommandHandler
{
    private readonly ILogger<DiscordCommandHandler> _logger;
    private readonly IStoreService _steamService;
    private readonly DiscordSocketClient _discordClient;
    private readonly ButtonController _buttonController;
    private const string STEAM_FOLLOW_CUSTOM_ID = "Steam_Follow";
    private const string STEAM_UNFOLLOW_CUSTOM_ID = "Steam_Unfollow";

    public DiscordCommandHandler(ILogger<DiscordCommandHandler> logger, IStoreService steamService, 
        DiscordSocketClient discordClient, ButtonController buttonController)
    {
        _logger = logger;
        _steamService = steamService;
        _discordClient = discordClient;
        _buttonController = buttonController;
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
        List<DiscordResponse> discordResponses = await RunSlashCommandAsync(slashCommand);

        foreach (DiscordResponse response in discordResponses) await slashCommand.FollowupAsync
            (response.Message, embeds: response.Embeds?.ToArray() ?? null, components: response.MessageComponents ?? null);
    }

    public async Task ButtonHandlerAsync(SocketMessageComponent component)
    {
        var componentAdapter = new SocketMessageComponentAdapter(component); // Convert to adapter. TODO: Factory?

        string? commandName = componentAdapter.GetCommandName();
        if (commandName == null)
        {
            _logger.LogError("Unable to get command name from button.");
            await componentAdapter.RespondAsync("Unable to find command name from component.");
            return;
        }

        switch (commandName)
        {
            case STEAM_FOLLOW_CUSTOM_ID:
                await _buttonController.FollowSteamAppAsync(componentAdapter);
                break;
            case STEAM_UNFOLLOW_CUSTOM_ID:
                await _buttonController.UnfollowSteamAppAsync(componentAdapter);
                break;
            default:
                _logger.LogError($"Invalid command name {commandName}.");
                await componentAdapter.RespondAsync("Invalid command name.");
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="slashCommand"></param>
    private async Task<List<DiscordResponse>> RunSlashCommandAsync(SlashCommand slashCommand)
    {
        SocketSlashCommandDataOption? dataOptions = slashCommand.Data?.Options.FirstOrDefault();
        
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
            Label = "Follow/Unfollow",
            Style = ButtonStyle.Primary,
            CustomId = STEAM_FOLLOW_CUSTOM_ID
        };
        ComponentBuilder componentBuilder = new();
        MessageComponent component = componentBuilder.WithButton(followButton).Build();

        var response = new DiscordResponse
        {
            Embeds = new List<Embed> { embed },
            MessageComponents = component
        };

        return response;
    }
}

/// <summary>
/// Holds components for a Discord.Net response.
/// </summary>
internal class DiscordResponse
{
    public List<Embed>? Embeds { get; set; }
    public MessageComponent? MessageComponents { get; set; }
    public string Message { get; set; } = "";
}