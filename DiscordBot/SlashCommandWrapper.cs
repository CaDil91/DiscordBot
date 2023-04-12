using Discord.WebSocket;

namespace DiscordBot;

public class SlashCommandWrapper
{
    public bool IsValidCommand { get; set; } = false;
    public SocketSlashCommand? SlashCommand { get; set; }
    
    public bool HasResponded { get; set; }

    public string CommandName { get; set; } = string.Empty;

    /// <summary>
    /// Default construction. Creates invalid command.
    /// </summary>
    public SlashCommandWrapper()
    {
        SlashCommand = null;
    }

    public SlashCommandWrapper(SocketSlashCommand? slashCommand)
    {
        if (slashCommand != null) SlashCommand = slashCommand;
    }

    public async Task RespondAsync(string sResponseMessage)
    {
        if (SlashCommand != null) await SlashCommand.RespondAsync(sResponseMessage);
    }
}