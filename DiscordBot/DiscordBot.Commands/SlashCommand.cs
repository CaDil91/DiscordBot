using System.Text;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.DiscordBot.Commands;

public class SlashCommand : ICommand
{
    private SocketSlashCommand? WrappedSocketSlashCommand { get; set; }

    /// <summary>Gets the name of the invoked command.</summary>
    private string? _commandName;

    public string? CommandName
    {
        get => WrappedSocketSlashCommand?.CommandName ?? null;
        set => _commandName = value;
    }

    /// <summary>Gets the data associated with this interaction.</summary>
    private SocketSlashCommandData? _data;

    public SocketSlashCommandData? Data
    {
        get => WrappedSocketSlashCommand?.Data ?? null;
        set => _data = value;
    }

    public SlashCommand()
    {
    }

    /// <summary>
    /// Default construction. Creates invalid command.
    /// </summary>
    /// <param name="socketSlashCommand"></param>
    public SlashCommand(SocketSlashCommand socketSlashCommand)
    {
        WrappedSocketSlashCommand = socketSlashCommand;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sResponseMessage">string text = null</param>
    /// <param name="embeds">Discord.Embed[] embeds = null</param>
    /// <param name="isTTS">bool isTTS = false</param>
    /// <param name="ephemeral">false</param>
    /// <param name="allowedMentions">null</param>
    /// <param name="components">null</param>
    /// <param name="embed">null</param>
    /// <param name="options">null</param>
    public async Task RespondAsync(string? sResponseMessage = null, Embed[]? embeds = null, bool isTTS = false,
        bool ephemeral = false, AllowedMentions? allowedMentions = null, MessageComponent? components = null,
        Embed? embed = null, RequestOptions? options = null)
    {
        if (WrappedSocketSlashCommand != null)
            await WrappedSocketSlashCommand.RespondAsync(sResponseMessage,
                embeds: embeds, isTTS: isTTS, ephemeral: ephemeral, allowedMentions: allowedMentions,
                components: components, embed: embed, options: options);
    }

    public async Task DeferAsync()
    {
        if (WrappedSocketSlashCommand != null) await WrappedSocketSlashCommand.DeferAsync();
    }

    public async Task FollowupAsync(string? sResponseMessage = null, Embed[]? embeds = null, bool isTTS = false,
        bool ephemeral = false, AllowedMentions? allowedMentions = null, MessageComponent? components = null,
        Embed? embed = null, RequestOptions? options = null)
    {
        if (WrappedSocketSlashCommand != null) await WrappedSocketSlashCommand.FollowupAsync(sResponseMessage,
            embeds: embeds, isTTS: isTTS, ephemeral: ephemeral, allowedMentions: allowedMentions,
            components: components, embed: embed, options: options);
    }

    public override string ToString()
    {
        return new StringBuilder()
            .AppendLine($"WrappedSocketSlashCommand: {WrappedSocketSlashCommand?.ToString() ?? "null socket command"}")
            .AppendLine($"Data: {WrappedSocketSlashCommand?.Data.ToString() ?? "null data"}")
            .ToString();
    }

    /// <summary>
    /// TODO: 
    /// </summary>
    /// <returns></returns>
    public bool ValidateCommand()
    {
        return true;
    }
}