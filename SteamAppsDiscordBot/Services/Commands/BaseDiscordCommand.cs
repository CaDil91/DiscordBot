using DiscordBot.Controllers.Adapters;

namespace DiscordBot.Services.Commands;

public abstract class BaseDiscordCommand
{
    public IEnumerable<DiscordResponse> DiscordResponses = new List<DiscordResponse>();
    public ICommandAdapter CommandAdapter = new NullCommandAdapter();
    public abstract void SetAdapter(ICommandAdapter commandCommandAdapter);
    public abstract Task ExecuteAsync();
    public abstract Task RespondAsync();
}