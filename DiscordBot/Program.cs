using DiscordBot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost? host = Host.CreateDefaultBuilder(args)
    .UseDefaultServiceProvider((context, options) => { options.ValidateScopes = true; })
    .ConfigureServices((context, services) => { services.ComposeApplication(); })
    .Build();

var discordBot = host.Services.GetRequiredService<DiscordBot.DiscordBot>();

await discordBot.RunAsync();