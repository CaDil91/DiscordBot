using Discord.WebSocket;

namespace DiscordBot.Controllers.Adapters;

public class NullCommandAdapter : ICommandAdapter<object>
{
    public object? CommandData => null;
    public string CommandName { get; set; } = "Null command";
}