using Discord.WebSocket;

namespace SteamAppsDiscordBot.Adapters;

public interface ISocketInteractionContextAdapter
{
    DiscordSocketClient Client { get; }
    SocketInteraction Interaction { get; }
}