using System.Windows.Input;
using DiscordBot.Controllers.Adapters;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Services.Commands.Factory;

public class DiscordCommandFactory : ICommandFactory
{
    private readonly ILogger<DiscordCommandFactory> _logger;
    private readonly IEnumerable<ICommand> _commandMapper;

    public DiscordCommandFactory(ILogger<DiscordCommandFactory> logger, IEnumerable<ICommand> commands)
    {
        _logger = logger;
        _commandMapper = commands;
    }

    /*public BaseDiscordCommand CreateCommand(string commandName)
    {
        // Get the command from the command mapper
        ICommand? command = _commandMapper.FirstOrDefault(c => c.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));
            //return discordCommand();

        _logger.LogError("Unable to find command {commandName}.", commandName);
        return new NullCommand();
    }*/
    public BaseDiscordCommand CreateCommand(string commandName)
    {
        throw new NotImplementedException();
    }
}