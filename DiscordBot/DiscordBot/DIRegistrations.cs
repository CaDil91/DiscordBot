using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SteamServices;

namespace DiscordBot.DiscordBot;

public static class DIRegistrations
{
    /// <summary>
    ///     DI container IServiceCollection extension method to register DiscordBot dependencies and settings.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterDiscordBot(this IServiceCollection services)
    {
        services.AddHttpClient("hardcodedsteam", client => { client.BaseAddress = new Uri("https://store.steampowered.com"); });

        services.AddTransient<StoreService>();
        services.AddSingleton<IDiscordCommandHandler, DiscordCommandHandler>();
        services.AddSingleton<DiscordBot>();
        services.AddLogging(x => x.AddConsole());

        services.AddOptions<DiscordBotOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection(DiscordBotOptions.SECTION_NAME).Bind(options);
            });

        return services;
    }
}