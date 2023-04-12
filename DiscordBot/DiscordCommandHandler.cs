using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SteamServices;

namespace DiscordBot;

public class DiscordCommandHandler : IDiscordCommandHandler
{
    private readonly StoreService _storeService;
    private readonly ILogger<DiscordCommandHandler> _logger;

    public DiscordCommandHandler(StoreService storeService, ILogger<DiscordCommandHandler> logger)
    {
        _storeService = storeService;
        _logger = logger;
    }

    /// <summary>
    /// Verify SocketSlashCommand and send to handler.
    /// SocketSlashCommand is difficult to mock for unit testing. All private.
    /// </summary>
    /// <param name="socketSlashCommand">Discord.WebSocket.SocketSlashCommand</param>
    public async Task HandleSlashCommandAsync(SocketSlashCommand? socketSlashCommand) => await HandleSlashCommandAsync(new SlashCommandWrapper(socketSlashCommand));


    public async Task HandleSlashCommandAsync(SlashCommandWrapper slashCommand)
    {
        if (!slashCommand.IsValidCommand)
        {
            _logger.LogInformation("Invalid slash command received.");
            return;
        }
        
        _logger.LogInformation("Invalid slash command received.");

        var sResponseMessage = "";

        switch (slashCommand.CommandName)
        {
            
        }

        await slashCommand.RespondAsync(sResponseMessage);
    }
    

}