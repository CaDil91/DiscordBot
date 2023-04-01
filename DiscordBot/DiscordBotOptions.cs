namespace DiscordBot;

public class DiscordBotOptions
{
    public const string SectionName = "DiscordBot";

    
    public string? DiscordToken { get; set; } 
    public bool RegisterSlashCommands { get; set; } 
}