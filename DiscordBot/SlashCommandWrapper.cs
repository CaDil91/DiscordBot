using Discord.WebSocket;

namespace DiscordBot;

public class SlashCommandWrapper
{
    public SocketSlashCommand SlashCommand { get; set; }
    public const string INVALID_COMMAND = "invalid_command"; 
    
    private bool _hasResponded;
    public bool HasResponded { get => SlashCommand.HasResponded;  set => _hasResponded = value; }

    private string _sCommandName = INVALID_COMMAND;
    public string CommandName { get => SlashCommand.CommandName; set => _sCommandName = value; }

    public SlashCommandWrapper(SocketSlashCommand slashCommand)
    {
        SlashCommand = slashCommand;
    }
}