namespace DiscordBot;

public interface IDiscordBot
{
    public Task RunAsync(int timeout = Timeout.Infinite);
}