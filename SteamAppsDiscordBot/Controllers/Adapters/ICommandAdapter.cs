using Discord.WebSocket;

namespace DiscordBot.Controllers.Adapters;

public interface ICommandAdapter<T>
{
    T? CommandData { get; }
    string CommandName { get; }
}