using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SteamServices;

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
        services.AddHttpClient("hardcodedsteam",
            client => { client.BaseAddress = new Uri("https://store.steampowered.com"); });

        services.AddTransient<StoreService>();
        services.AddSingleton<IDiscordCommandHandler, DiscordCommandHandler>();
        services.AddSingleton<DiscordBot>();

        services.AddOptions<DiscordBotOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection(DiscordBotOptions.SectionName).Bind(options);
            });

        return services;
    }
}