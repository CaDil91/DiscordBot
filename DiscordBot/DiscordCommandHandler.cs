using Discord;
using Discord.WebSocket;

namespace DiscordBot;

public class DiscordCommandHandler : IDiscordCommandHandler
{
    private readonly SteamService _steamService;

    public DiscordCommandHandler(SteamService steamService)
    {
        _steamService = steamService;
    }
    
    /// <summary>
    /// Primary entry point for all command types.
    /// </summary>
    /// <param name="command">returns null if no response message</param>
    public async Task<string?> HandleCommand(IApplicationCommandInteraction? command)
    {
        if (command?.Data?.Name == null || string.IsNullOrEmpty(command.Data?.Name)) return null;
        
        string sCommandName = command.Data.Name;
        string? sResponse = null;

        //Handle command types. Cleaner way than an expanding switch statement?
        switch (command)
        {
            case SocketSlashCommand socketSlashCommand:
                await HandleSlashCommand(socketSlashCommand);
                
                break;
        }

        return sResponse;
    }

    /// <summary>
    /// Responds to Discord slash commands.
    /// Documentation: https://discordnet.dev/guides/int_basics/application-commands/slash-commands/responding-to-slash-commands.html
    /// </summary>
    private async Task<string?> HandleSlashCommand(SocketSlashCommand socketSlashCommand)
    { 
        return await _steamService.SearchStore(socketSlashCommand.Data?.Options?.FirstOrDefault()?.Value?.ToString() ?? "");
    }
}