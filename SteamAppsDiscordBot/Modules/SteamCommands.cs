using Discord;
using Discord.Interactions;
using DiscordBot.Services;
using DiscordBot.Services.DTO;
using Microsoft.Extensions.Logging;

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
public class SteamCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<SteamCommands> _logger;
    private readonly ISteamStoreService _steamStoreServices;
    private const string STEAM_FOLLOW_CUSTOM_ID = "Steam_Follow";
    private const string STEAM_UNFOLLOW_CUSTOM_ID = "Steam_Unfollow";

    public SteamCommands(ILogger<SteamCommands> logger, ISteamStoreService steamStoreServices)
    {
        _logger = logger;
        _steamStoreServices = steamStoreServices;
    }

    /// <summary>
    /// This command will be executed when a user types /steam
    /// </summary>
    /// <param name="appName"></param>
    [SlashCommand("steam", "Search the Steam store")]
    public async Task GetSteamAppAsync(string appName)
    {
        // Defer the response to avoid the "Thinking..." state
        await DeferAsyncCaller();

        // TODO: Get ResponseObjectName from _steamStoreServices
        // Get the first result only.
        List<SteamApp> steamApps = await _steamStoreServices.GetAppsAsync(appName, 1);
        
        // TODO: Move to _steamStoreServices? Or wrap _steamStoreServices inside a DiscordService?
        Embed[]? embed = CreateEmbed(steamApps.FirstOrDefault());
        
        // TODO: Move to service
        MessageComponent component = GetFollowUnfollowComponent();
        
        // Send the response.
        await FollowupAsyncCaller("", embeds: embed, components: component);
    }

    /// <summary>
    /// Wrapper for DeferAsync().
    /// Created for mocking in unit tests.
    /// </summary>
    private async Task DeferAsyncCaller()
    {
        try
        {
            await DeferAsync();
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Unable to defer response");
        }
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
    private static Embed[]? CreateEmbed(SteamApp? steamApp)
    {
        if (steamApp == null) return null;
        
        Embed? embed = new EmbedBuilder()
            .WithTitle(steamApp.Name)
            .WithUrl(steamApp.Url)
            .WithDescription(steamApp.ShortDescription)
            .WithImageUrl(steamApp.HeaderImage)
            .Build();
        
        return embed != null ? new[] {embed} : null;
    }


    /// <summary>
    /// Wrapper for FollowupAsync().
    /// Created for mocking in unit tests.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="embeds"></param>
    /// <param name="isTTS"></param>
    /// <param name="ephemeral"></param>
    /// <param name="allowedMentions"></param>
    /// <param name="options"></param>
    /// <param name="components"></param>
    /// <returns></returns>
    public async Task FollowupAsyncCaller(string message, Embed[]? embeds = null, bool isTTS = false, 
        bool ephemeral = false, AllowedMentions? allowedMentions = null, RequestOptions? options = null, 
        MessageComponent? components = null)
    {
        try
        {
            await FollowupAsync(message, embeds, isTTS, ephemeral, allowedMentions, options, components);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Unable to send followup message");
        }
    }
}