using Discord;

namespace DiscordBot.DiscordBot.Commands;

public interface ICommand
{
    public Task RespondAsync(string sResponseMessage, Embed[]? embeds, bool isTTS, bool ephemeral,
        AllowedMentions allowedMentions, MessageComponent components, Embed embed, RequestOptions? options);
}