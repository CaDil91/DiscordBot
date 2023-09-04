using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Services.Commands;

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
public class GetSteamAppCommand : InteractionModuleBase //Command modules are transient objects.
{
    private readonly ILogger<GetSteamAppCommand> _logger;

    public GetSteamAppCommand(ILogger<GetSteamAppCommand> logger)
    {
        _logger = logger;
    }

    [SlashCommand("steam", "Search the steam store")]
    public async Task GetSteamApp()
    {
        _logger.LogInformation("GetSteamAppCommand called");
        await RespondAsync("Hello world!");
    }
    
}