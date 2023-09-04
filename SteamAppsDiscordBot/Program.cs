using DiscordBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, configuration) => configuration.AddEnvironmentVariables())
    .ConfigureServices((_, services) => { services.ComposeApplication(); })
    .Build();

var discordBot = host.Services.GetRequiredService<DiscordBot.DiscordBot>();

await discordBot.RunAsync(host.Services);