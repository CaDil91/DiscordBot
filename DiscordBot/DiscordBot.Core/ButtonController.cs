using Discord.WebSocket;
using DiscordBot.DiscordBot.Commands;

namespace DiscordBot.DiscordBot.Core;

public class ButtonController
{
    public async Task FollowSteamAppAsync(SocketMessageComponentAdapter componentAdapter)
    {
        await componentAdapter.DeferAsync(); // Defer response to avoid timeout.
        
        var discordResponses = new List<DiscordResponse>();
        
        // Get title for role name.
        if (!TryGetEmbedTitle(out string roleName)) 
            return new List<DiscordResponse> { new() { Message = "Unable to get title from embed." } };
        
        // Add user to role. Create role if it doesn't exist.
        SocketRole? roleToFollow = GetGuildRole(roleName);
        if (roleToFollow == null)
        {
            if (!await TryCreateGuildRole(roleName)) return new List<DiscordResponse> { new() { Message = "Unable to create role." } };
            discordResponses.Add(new DiscordResponse { Message = $"Created new role {roleName}." });
        }
        
        // TODO: _context.User.Mention is the bot, not the user.
        await _context.Guild.GetUser(_context.User.Id).AddRoleAsync(roleToFollow).ConfigureAwait(false); // Add role to user.
        discordResponses.Add(new DiscordResponse { Message = $"{_context.User.Mention} added to role {roleName}" });

        return discordResponses;
    }

    public async Task UnfollowSteamAppAsync(SocketMessageComponentAdapter componentAdapter)
    {
        throw new NotImplementedException();
    }
}