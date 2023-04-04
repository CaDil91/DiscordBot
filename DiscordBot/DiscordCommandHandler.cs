using Discord.WebSocket;
using Microsoft.Extensions.Hosting.Internal;

namespace DiscordBot;

public class DiscordCommandHandler : IDiscordCommandHandler
{
    private readonly SteamService _steamService;

    public DiscordCommandHandler(SteamService steamService)
    {
        _steamService = steamService;
    }

    /// <summary>
    /// Responds to Discord slash commands.
    /// After receiving an interaction, you must respond to acknowledge it. You can choose to respond with a message immediately using RespondAsync()
    /// or you can choose to send a deferred response with DeferAsync(). If choosing a deferred response, the user will see a loading state for the interaction,
    /// and you'll have up to 15 minutes to edit the original deferred response using ModifyOriginalResponseAsync(). You can read more about response types here
    /// Documentation: https://discordnet.dev/guides/int_basics/application-commands/slash-commands/responding-to-slash-commands.html
    /// Response Types: https://discord.com/developers/docs/interactions/application-commands
    /// </summary>
    public async Task HandleSlashCommandAsync(SocketSlashCommand? socketSlashCommand)
    {
        if (socketSlashCommand == null) return;
        
        //Setup
        var sResponse = "Sorry, I don't know how to handle that slash command.";
        await socketSlashCommand.DeferAsync();
        
        string sCommandName = socketSlashCommand.Data?.Name ?? "";
        switch (sCommandName)
        {
            case "steam":
                string sSearchTerm = socketSlashCommand.Data?.Options?.FirstOrDefault()?.Value?.ToString() ?? "";
                if (string.IsNullOrEmpty(sSearchTerm))
                {
                    sResponse = "No search term found.";
                    break;
                }
                sResponse = await _steamService.SearchStoreAsync(sSearchTerm);
                break;
        }

        await socketSlashCommand.RespondAsync(sResponse);
    }
}