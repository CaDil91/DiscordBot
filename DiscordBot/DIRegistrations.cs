using DiscordBot.DiscordBot.Core;
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
        services.AddSingleton<IDiscordCommandHandler, DiscordCommandHandler>();
        services.AddSingleton<DiscordBot.Core.DiscordBot>();
        services.AddLogging(x => x.AddConsole());

        services.AddOptions<DiscordBotOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection(DiscordBotOptions.SECTION_NAME).Bind(options);
            });

        return services;
    }
}