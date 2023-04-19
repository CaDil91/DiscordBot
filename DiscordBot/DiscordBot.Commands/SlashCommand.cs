using System.Text;
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

    /// <summary>
    /// Default construction. Creates invalid command.
    /// </summary>
    public SlashCommand()
    {
        WrappedSocketSlashCommand = null;
    }

    public SlashCommand(SocketSlashCommand slashCommand)
    {
        WrappedSocketSlashCommand = slashCommand;
    }

    public async Task RespondAsync(string sResponseMessage)
    {
        //if (WrappedSocketSlashCommand != null) await WrappedSocketSlashCommand.RespondAsync(sResponseMessage);
    }

    public override string ToString()
    {
        return new StringBuilder()
            .AppendLine($"WrappedSocketSlashCommand: {WrappedSocketSlashCommand?.ToString() ?? "null socket command"}")
            .AppendLine($"Data: {WrappedSocketSlashCommand?.Data.ToString() ?? "null data"}")
            .ToString();
    }

    /// <summary>
    /// After receiving an interaction, you must respond to acknowledge it. You can choose to respond with a message
    /// immediately using RespondAsync() or you can choose to send a deferred response with DeferAsync(). If choosing a
    /// deferred response, the user will see a loading state for the interaction, and you'll have up to 15 minutes to
    /// edit the original deferred response using ModifyOriginalResponseAsync(). You can read more about response types here.
    /// https://discord.com/developers/docs/interactions/slash-commands#interaction-response
    /// </summary>
    public async Task ExecuteCommand()
    {
        
        await Task.CompletedTask;
    }
}