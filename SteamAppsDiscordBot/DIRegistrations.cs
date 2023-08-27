using Discord.WebSocket;
using DiscordBot.Controllers;
using DiscordBot.Core;
using DiscordBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordBot;

public static class DIRegistrations
{
    /// <summary>
    ///     DI container IServiceCollection extension method to register DiscordBot dependencies and settings.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterDiscordBot(this IServiceCollection services)
    {
        //services.AddSingleton<ICommandController, DiscordCommandController>();
        services.AddSingleton<DiscordGuildServices>();
        services.AddSingleton<DiscordButtonController>();
        services.AddSingleton<DiscordBot>();
        
        /*//When working with events that have Cacheable<IMessage, ulong> parameters,
        //you must enable the message cache in your config settings if you plan to use the cached message entity.
        var discordSocketConfig = new DiscordSocketConfig { MessageCacheSize = 100 };
        var client = new DiscordSocketClient(discordSocketConfig);*/
        services.AddSingleton<DiscordSocketClient>();
        
        services.AddLogging(x => x.AddConsole());

        services.AddOptions<DiscordBotOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection(DiscordBotOptions.SECTION_NAME).Bind(options);
            });

        return services;
    }
}