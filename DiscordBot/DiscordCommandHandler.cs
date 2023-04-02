using Discord;
using Discord.WebSocket;

namespace DiscordBot;

public static class DiscordCommandHandler
{
    static DiscordCommandHandler()
    {
        
    }
    
    /// <summary>
    /// Re-routed entry point for all command types
    /// </summary>
    /// <param name="command"></param>
    public static async Task HandleCommand(IApplicationCommandInteraction? command)
    {
        if (command?.Data?.Name == null || string.IsNullOrEmpty(command?.Data?.Name)) return;
        string sCommandName = command.Data.Name;
        
        switch (sCommandName)
        {
            case "steam":
                //TODO: await _steamService.SteamStoreSearchHandler(command.Data?.Options?.FirstOrDefault()?.Value?.ToString() ?? "");
                break;
        }
    }

    /// <summary>
    /// Responds to Discord slash commands.
    /// Documentation: https://discordnet.dev/guides/int_basics/application-commands/slash-commands/responding-to-slash-commands.html
    /// </summary>
    public static async Task HandleSlashCommand(SocketSlashCommand socketSlashCommand) => await HandleCommand(socketSlashCommand);
}