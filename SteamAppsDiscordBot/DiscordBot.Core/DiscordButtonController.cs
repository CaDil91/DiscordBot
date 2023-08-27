using Discord.WebSocket;
using DiscordBot.Controllers.Adapters;
using DiscordBot.Services;

namespace DiscordBot.Core;

/// <summary>
/// Handles incoming button presses.
/// </summary>
public class DiscordButtonController
{
    private readonly DiscordGuildServices _guildServices;

    public DiscordButtonController(DiscordGuildServices guildServices)
    {
        _guildServices = guildServices;
    }

    /// <summary>
    /// Follow a Steam app.
    /// </summary>
    /// <param name="socketMessageComponentAdapter">Incoming SocketMessageComponentAdapter from button</param>
    public async Task<List<DiscordResponse>> FollowSteamAppAsync(SocketMessageComponentAdapter socketMessageComponentAdapter)
    {
        var discordResponses = new List<DiscordResponse>();

        // Get title for role name.
        string? appTitle = await DiscordGuildServices.GetEmbedTitleAsync(socketMessageComponentAdapter);
        if (appTitle == null)
        {
            await socketMessageComponentAdapter.FollowupAsync("Unable to get title from embed."); // TODO: Return this?
            return discordResponses;
        }
        
        // Get guild role, or create the role if it doesn't exist.
        SocketRole? roleToFollow = _guildServices.GetGuildRoleAsync(socketMessageComponentAdapter, appTitle);
        if (roleToFollow == null)
        {
            if (!await _guildServices.CreateGuildRoleAsync(socketMessageComponentAdapter, appTitle))
            {
                await socketMessageComponentAdapter.FollowupAsync("Unable to create role.");
                return discordResponses;
            }
            discordResponses.Add(new DiscordResponse { Message = $"Created new role {appTitle}." });
        }

        // Add user to role.
        if (!await _guildServices.AddUserToRoleAsync(socketMessageComponentAdapter, appTitle))
            discordResponses.Add(new DiscordResponse { Message = $"Unable to add user to role {appTitle}." });
        
        return discordResponses;
    }

    public async Task<List<DiscordResponse>> UnfollowSteamAppAsync(SocketMessageComponentAdapter socketMessageComponentAdapter)
    {
        var discordResponses = new List<DiscordResponse>();
        
        // Get title for role name.
        string? appTitle = await DiscordGuildServices.GetEmbedTitleAsync(socketMessageComponentAdapter);
        if (appTitle == null)
        {
            discordResponses.Add(new DiscordResponse { Message = "Unable to get title."} );
            return discordResponses;
        }
        
        // Get guild role.
        SocketRole? roleToUnfollow = _guildServices.GetGuildRoleAsync(socketMessageComponentAdapter, appTitle);
        if (roleToUnfollow == null)
        {
            discordResponses.Add(new DiscordResponse { Message = $"Unable to find guild role {appTitle}." });
            return discordResponses;
        }
        
        // Remove user from role.
        if (!await _guildServices.RemoveUserFromRoleAsync(socketMessageComponentAdapter, appTitle))
            discordResponses.Add(new DiscordResponse { Message = $"Unable to remove user from role {appTitle}." });
            
        return discordResponses;
            
    }
}