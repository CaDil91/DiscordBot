namespace DiscordBot.DiscordBot.Commands;

public interface ICommand
{
    public Task RespondAsync(string sResponseMessage);
    public Task ExecuteCommand();
}