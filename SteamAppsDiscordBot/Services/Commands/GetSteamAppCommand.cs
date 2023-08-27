using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Services.Commands;

public class GetSteamAppCommand : InteractionModuleBase
{
    private readonly ILogger<GetSteamAppCommand> _logger;

    [SlashCommand("command", "description")]
    public async Task Something()
    {
        
    }
    
}