using Discord.WebSocket;

namespace DiscordBot;

public interface ISocketInteractionContextAdapter
{
    DiscordSocketClient Client { get; }
    SocketInteraction Interaction { get; }
}