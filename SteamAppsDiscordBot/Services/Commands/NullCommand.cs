using DiscordBot.Controllers.Adapters;

namespace DiscordBot.Services.Commands;

public class NullCommand : BaseDiscordCommand
{
    public override void SetAdapter(ICommandAdapter commandCommandAdapter)
    {
        throw new NotImplementedException();
    }

    public override Task ExecuteAsync()
    {
        throw new NotImplementedException();
    }

    public override Task RespondAsync()
    {
        throw new NotImplementedException();
    }
}