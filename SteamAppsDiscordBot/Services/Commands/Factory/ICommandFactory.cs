namespace DiscordBot.Services.Commands.Factory;

public interface ICommandFactory
{
    public BaseDiscordCommand CreateCommand(string commandName);
}