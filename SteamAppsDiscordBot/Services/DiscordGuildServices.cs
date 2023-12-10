using Discord;
using Discord.WebSocket;

namespace SteamAppsDiscordBot.Services;

public class DiscordGuildServices
{
    private readonly DiscordSocketClient _client;
    
    public DiscordGuildServices(DiscordSocketClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Get message from embed attacked to a SocketMessageComponentAdapter.
    /// </summary>
    /// <param name="message"></param>
    /// <returns>Null on failure</returns>
    public static string? GetEmbedTitleAsync(IMessage? message)
    {
        //IMessage message = await socketMessageComponentAdapter.Channel.GetMessageAsync(socketMessageComponentAdapter.Message.Id);
        return message == null && message?.Embeds == null ? string.Empty : message.Embeds.FirstOrDefault()?.Title;
    }

    /*/// <summary>
    /// Get role from socketMessageComponentAdapter's guild.
    /// </summary>
    /// <param name="socketMessageComponentAdapter">Used in some discord commands, like buttons presses</param>
    /// <param name="roleTitle">Title of role</param>
    /// <returns></returns>
    public SocketRole? GetGuildRoleAsync(SocketMessageComponentAdapter socketMessageComponentAdapter, string roleTitle)
    {
        ulong? guildId = socketMessageComponentAdapter.GuildId;
        if (guildId == null) return null;
        
        SocketGuild? guild = _client.GetGuild(guildId.Value);
        if (guild == null) return null;
        
        IReadOnlyCollection<SocketRole>? roles = guild.Roles;
        SocketRole? guildRole = roles?.FirstOrDefault(r => r.Name == roleTitle);
        return guildRole ?? null;
    }

    /// <summary>
    /// Create a new role in socketMessageComponentAdapter's guild.
    /// </summary>
    /// <param name="socketMessageComponentAdapter">Used in some discord commands, like buttons presses</param>
    /// <param name="roleTitle">Title of role</param>
    /// <returns></returns>
    public async Task<bool> CreateGuildRoleAsync(SocketMessageComponentAdapter socketMessageComponentAdapter, string roleTitle)
    {
        ulong? guildId = socketMessageComponentAdapter.GuildId;
        if (guildId == null) return false;
        
        SocketGuild? guild = _client.GetGuild(guildId.Value);
        if (guild == null) return false;
        
        RestRole? role = await guild.CreateRoleAsync(roleTitle);
        return role != null;
    }

    /// <summary>
    /// Add user to a role.
    /// </summary>
    /// <param name="socketMessageComponentAdapter"></param>
    /// <param name="roleToFollow"></param>
    /// <returns></returns>
    public async Task<bool> AddUserToRoleAsync(SocketMessageComponentAdapter socketMessageComponentAdapter, string roleToFollow)
    {
        SocketGuild? guild = GetGuild(socketMessageComponentAdapter);
        SocketRole? role = guild?.Roles.FirstOrDefault(r => r.Name == roleToFollow);
        if (role == null || guild == null) return false;

        SocketGuildUser? user = guild.GetUser(socketMessageComponentAdapter.User.Id);
        if (user == null) return false;
        
        await user.AddRoleAsync(role);
        return true;
    }
    
    /// <summary>
    /// Remove user from a role.
    /// </summary>
    /// <param name="socketMessageComponentAdapter"></param>
    /// <param name="roleTitle"></param>
    /// <returns></returns>
    public async Task<bool> RemoveUserFromRoleAsync(SocketMessageComponentAdapter socketMessageComponentAdapter, string roleTitle)
    {
        SocketGuild? guild = GetGuild(socketMessageComponentAdapter);
        SocketRole? role = guild?.Roles.FirstOrDefault(r => r.Name == roleTitle);
        if (role == null || guild == null) return false;

        SocketGuildUser? user = guild.GetUser(socketMessageComponentAdapter.User.Id);
        if (user == null) return false;
        
        await user.RemoveRoleAsync(role);
        return true;
    }

    /// <summary>
    /// Get guild from socketMessageComponentAdapter.
    /// </summary>
    /// <param name="socketMessageComponentAdapter">DiscordBot.DiscordBot.Commands.SocketMessageComponentAdapter</param>
    /// <returns></returns>
    private SocketGuild? GetGuild(SocketMessageComponentAdapter socketMessageComponentAdapter)
    {
        ulong? guildId = socketMessageComponentAdapter.GuildId;
        if (guildId == null) return null;

        SocketGuild? guild = _client.GetGuild(guildId.Value);
        return (guild == null) ? guild : null;
    }*/

    
}