using Discord.WebSocket;
using SteamServices;

namespace DiscordBot;

public class DiscordCommandHandler : IDiscordCommandHandler
{
    private readonly StoreService _storeService;

    public DiscordCommandHandler(StoreService storeService)
    {
        _storeService = storeService;
    }

    /// <summary>
    /// Verify SocketSlashCommand and send to handler.
    /// SocketSlashCommand is difficult to mock for unit testing. All private.
    /// </summary>
    /// <param name="socketSlashCommand">Discord.WebSocket.SocketSlashCommand</param>
    public async Task HandleSlashCommandAsync(SocketSlashCommand? socketSlashCommand)
    {
        if(socketSlashCommand != null) await HandleSlashCommandAsync(new SlashCommandWrapper(socketSlashCommand));
    } 


    public async Task HandleSlashCommandAsync(SlashCommandWrapper slashCommand)
    {
        if (slashCommand.CommandName == SlashCommandWrapper.INVALID_COMMAND) await Task.CompletedTask; //TODO: log
    }
    

}