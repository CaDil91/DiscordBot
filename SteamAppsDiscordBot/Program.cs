using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordBot;

[ExcludeFromCodeCoverage]
internal abstract class Program
{
    public static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, configuration) => configuration.AddEnvironmentVariables())
            .ConfigureServices((_, services) => { services.ComposeApplication(); })
            .Build();

        var discordBot = host.Services.GetRequiredService<IDiscordSocketClientAdapter>();

        discordBot.Initialize();
        await discordBot.RunAsync();
    }
}