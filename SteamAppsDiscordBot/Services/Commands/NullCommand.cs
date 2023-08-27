using DiscordBot.Controllers.Adapters;

namespace DiscordBot.Services.Commands;

public class NullCommand : BaseDiscordCommand
{
    public override Task ExecuteAsync()
    {
        throw new NotImplementedException();
    }

    public override Task RespondAsync()
    {
        throw new NotImplementedException();
    }
}