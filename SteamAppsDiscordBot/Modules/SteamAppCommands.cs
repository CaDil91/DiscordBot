using System.Diagnostics.CodeAnalysis;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using SteamAppsDiscordBot.Services;
using SteamAppsDiscordBot.Services.DTO;

namespace SteamAppsDiscordBot.Modules;

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
public class SteamAppCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<SteamAppCommands> _logger;
    private readonly ISteamStoreService _steamStoreServices;
    private const string STEAM_FOLLOW_CUSTOM_ID = "Steam_Follow";
    private const string STEAM_UNFOLLOW_CUSTOM_ID = "Steam_Unfollow";

    public SteamAppCommands(ILogger<SteamAppCommands> logger, ISteamStoreService steamStoreServices)
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
        await DeferWrapperAsync();
        
        // Get the first result only.
        List<SteamApp> steamApps = await _steamStoreServices.GetAppsAsync(appName, 1).ConfigureAwait(false);
        SteamApp? steamApp = steamApps.FirstOrDefault();
        if (steamApp?.Name == null)
        {
            await FollowupWrapperAsync("No results found.").ConfigureAwait(false);
            return;
        }
        
        // Send the response.
        await FollowupWrapperAsync(steamApp).ConfigureAwait(false);
    }

    /// <summary>
    /// Wraps the FollowupAsync method with additional parameters for a given SteamApp and sends a follow-up message.
    /// </summary>
    /// <param name="steamApp">The SteamApp object representing the app for which the follow-up message is being sent.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task FollowupWrapperAsync(SteamApp steamApp)
    {
        if (steamApp.SteamAppid == 0)
        {
            await FollowupWrapperAsync("Unable to collect app details.").ConfigureAwait(false);
            return;
        }
        Embed embed = new EmbedBuilder()
            .WithTitle(steamApp.Name)
            .WithUrl(steamApp.Url)
            .WithDescription(steamApp.ShortDescription)
            .WithImageUrl(steamApp.HeaderImage)
            .Build();
        
        MessageComponent component = GetFollowUnfollowComponent();
        
        await FollowupWrapperAsync("", embeds: new []{ embed }, components: component).ConfigureAwait(false);
    }

    /// <summary>
    /// TODO: Comment
    /// </summary>
    /// <returns></returns>
    [ExcludeFromCodeCoverage]
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
    [ExcludeFromCodeCoverage]
    private async Task FollowupWrapperAsync(string message, Embed[]? embeds = null, bool isTTS = false, 
        bool ephemeral = false, AllowedMentions? allowedMentions = null, RequestOptions? options = null, 
        MessageComponent? components = null)
    {
        try
        {
            await FollowupAsync(message, embeds, isTTS, ephemeral, allowedMentions, options, components);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to send followup message {message}", message);
            if (embeds != null) _logger.LogError(e, "Embed: {embedTitle}", embeds.First());
        }
    }

    /// <summary>
    /// Wrapper for DeferAsync().
    /// Created for mocking in unit tests.
    /// </summary>
    [ExcludeFromCodeCoverage]
    private async Task DeferWrapperAsync()
    {
        try
        {
            await DeferAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to defer response");
        }
    }
}