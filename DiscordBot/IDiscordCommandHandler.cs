using Discord;

namespace DiscordBot;

public interface IDiscordCommandHandler
{
    public Task<string?> HandleCommand(IApplicationCommandInteraction? command);
}