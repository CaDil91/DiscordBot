using Discord;
using Discord.WebSocket;

namespace DiscordBot.DiscordBot.Commands;

/// <summary>
/// SocketSlashCommand adapter.
/// </summary>
public class SocketSlashCommandAdapter : ISlashCommandInteractionAdapter
{
    private readonly SocketSlashCommand _socketSlashCommand;

    public SocketSlashCommandAdapter(SocketSlashCommand socketSlashCommand)
    {
        _socketSlashCommand = socketSlashCommand;
    }

    public SocketSlashCommandData Data => _socketSlashCommand.Data;
    public InteractionType Type { get; }
    public string? Token { get; }
    public int Version { get; }
    public bool HasResponded { get; }
    public IUser? User { get; }
    public string? UserLocale { get; }
    public string? GuildLocale { get; }
    public bool IsDMInteraction { get; }
    public ulong? ChannelId { get; }
    public ulong? GuildId { get; }
    public ulong ApplicationId { get; }
    ulong IDiscordInteraction.Id { get; }
    ulong IEntity<ulong>.Id { get; }
    public DateTimeOffset CreatedAt { get; }
    
    IApplicationCommandInteractionData ISlashCommandInteraction.Data => Data;
    IDiscordInteractionData IDiscordInteraction.Data => Data;
    IApplicationCommandInteractionData IApplicationCommandInteraction.Data => Data;
    
    /// <inheritdoc />
    public async Task RespondAsync(string? text = null, Embed[]? embeds = null, bool isTTS = false, bool ephemeral = false,
        AllowedMentions? allowedMentions = null, MessageComponent? components = null, Embed? embed = null, RequestOptions? options = null)
    {
        await _socketSlashCommand.RespondAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options).ConfigureAwait(false);
    }

    public Task RespondWithFilesAsync(IEnumerable<FileAttachment> attachments, string? text = null, Embed[]? embeds = null, bool isTTS = false,
        bool ephemeral = false, AllowedMentions? allowedMentions = null, MessageComponent? components = null,
        Embed? embed = null, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public async Task<IUserMessage> FollowupAsync(string? text = null, Embed[]? embeds = null, bool isTTS = false, bool ephemeral = false,
        AllowedMentions? allowedMentions = null, MessageComponent? components = null, Embed? embed = null,
        RequestOptions? options = null)
    {
        return await _socketSlashCommand.FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options);
    }

    public Task<IUserMessage> FollowupWithFilesAsync(IEnumerable<FileAttachment> attachments, string? text = null, Embed[]? embeds = null, bool isTTS = false,
        bool ephemeral = false, AllowedMentions? allowedMentions = null, MessageComponent? components = null,
        Embed? embed = null, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IUserMessage> GetOriginalResponseAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<IUserMessage> ModifyOriginalResponseAsync(Action<MessageProperties> func, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task DeleteOriginalResponseAsync(RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public async Task DeferAsync(bool ephemeral = false, RequestOptions? options = null)
    {
        await _socketSlashCommand.DeferAsync(ephemeral, options).ConfigureAwait(false);
    }

    public Task RespondWithModalAsync(Modal modal, RequestOptions? options = null)
    {
        throw new NotImplementedException();
    }
}

// Define the adapter interfaces
public interface ISlashCommandInteractionAdapter : ISlashCommandInteraction
{
}

public interface IApplicationCommandInteractionAdapter : IApplicationCommandInteraction
{
    // Any additional methods or properties specific to this adapter interface
}