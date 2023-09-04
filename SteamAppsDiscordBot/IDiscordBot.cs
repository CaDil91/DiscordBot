namespace DiscordBot;

public interface IDiscordBot
{
    public Task RunAsync(IServiceProvider serviceProvider);
}