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

    public string ResponseMessage { get; set; } = "";

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

    public async Task RespondAsync(string sResponseMessage)
    {
        if (WrappedSocketSlashCommand != null) await WrappedSocketSlashCommand.RespondAsync(sResponseMessage);
    }
    
    public async Task DeferAsync()
    {
        if (WrappedSocketSlashCommand != null) await WrappedSocketSlashCommand.DeferAsync();
    }
    
    public async Task FollowupAsync()
    {
        if (WrappedSocketSlashCommand != null) await WrappedSocketSlashCommand.FollowupAsync(ResponseMessage);
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