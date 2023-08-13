using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace DiscordBot.DiscordBot.Commands;

/// <summary>
/// SocketSlashCommand adapter.
/// </summary>
public class DiscordSocketSlashCommandAdapter
{
    private readonly SocketSlashCommand _socketSlashCommand;

    public DiscordSocketSlashCommandAdapter(SocketSlashCommand wrappedCommand)
    {
        _socketSlashCommand = wrappedCommand;
    }

    public SocketSlashCommandData Data => _socketSlashCommand.Data;
    
    public Task RespondAsync(
        string? text = null,
        Embed[]? embeds = null,
        bool isTTS = false,
        bool ephemeral = false,
        AllowedMentions? allowedMentions = null,
        MessageComponent? components = null,
        Embed? embed = null,
        RequestOptions? options = null)
    {
        return _socketSlashCommand.RespondAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options);
    }
    
    public Task<RestFollowupMessage> FollowupAsync(
        string? text = null,
        Embed[]? embeds = null,
        bool isTTS = false,
        bool ephemeral = false,
        AllowedMentions? allowedMentions = null,
        MessageComponent? components = null,
        Embed? embed = null,
        RequestOptions? options = null)
    {
        return _socketSlashCommand.FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options);
    }
    
    public Task DeferAsync(bool ephemeral = false, RequestOptions? options = null)
    {
        return _socketSlashCommand.DeferAsync(ephemeral, options);
    }
}