using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using SteamServices.DTOs;
using SteamServices.Services;

namespace DiscordBot.Modules;

/// <summary>
/// Command modules are transient objects.
/// A new module instance is created before a command execution starts.
/// It will be disposed right after the method returns.
/// 
/// Every command module exposes a set of helper methods, namely:
///    - RespondAsync() => Respond to the interaction
///    - FollowupAsync() => Create a followup message for an interaction
///    - ReplyAsync() => Send a message to the origin channel of the interaction
///    - DeleteOriginalResponseAsync() => Delete the original interaction response
/// 
/// - More info: https://discordnet.dev/guides/int_framework/intro.html
/// </summary>
public class SteamCommands : InteractionModuleBase //InteractionModuleBase "modules/commands" are transient objects.
{
    private readonly ILogger<SteamCommands> _logger;
    private readonly SteamStoreService _steamStoreService;
    private const string STEAM_FOLLOW_CUSTOM_ID = "Steam_Follow";
    private const string STEAM_UNFOLLOW_CUSTOM_ID = "Steam_Unfollow";

    public SteamCommands(ILogger<SteamCommands> logger, SteamStoreService steamStoreService)
    {
        _logger = logger;
        _steamStoreService = steamStoreService;
    }

    /// <summary>
    /// This command will be executed when a user types /steam
    /// </summary>
    /// <param name="appName"></param>
    [SlashCommand("steam", "Search the steam store")]
    public async Task GetSteamAppAsync(string appName)
    {
        // Defer the response to avoid the "Thinking..." state
        await DeferAsync();

        // Get the first result
        List<SteamApp> steamApps = await _steamStoreService.GetAppsAsync(appName, 1);
        if (steamApps.Count == 0)
        {
            await FollowupAsync("No results found.");
            return;
        }
        SteamApp steamApp = steamApps.First();
        
        // Get the embed
        Embed? embed = GetEmbed(steamApp);
        if (embed == null)
        {
            await FollowupAsync("No results found.");
            return;
        }
        
        // Get the component
        MessageComponent component = GetFollowUnfollowComponent();
        
        // Send the response
        await FollowupAsync("", embeds: new[] { embed }, components: component);
    }

    /// <summary>
    /// TODO: Comment
    /// </summary>
    /// <returns></returns>
    private static MessageComponent GetFollowUnfollowComponent()
    {
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
        return component;
    }

    /// <summary>
    /// TODO: Comment
    /// </summary>
    /// <param name="steamApp"></param>
    /// <returns></returns>
    private static Embed? GetEmbed(SteamApp steamApp)
    {
        Embed? embed = new EmbedBuilder()
            .WithTitle(steamApp.Name)
            .WithUrl(steamApp.Url)
            .WithDescription(steamApp.ShortDescription)
            .WithImageUrl(steamApp.HeaderImage)
            .Build();
        return embed;
    }
}