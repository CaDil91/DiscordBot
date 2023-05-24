using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SteamServices;

public static class DIRegistrations
{
    /// <summary>
    ///     DI container IServiceCollection extension method to register SteamServices dependencies and settings.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterSteamServices(this IServiceCollection services)
    {
        services.AddHttpClient("hardcodedsteam", client => { client.BaseAddress = new Uri("https://api.steampowered.com"); });
        services.AddSingleton<IAppRepository, SteamAppRepository>();
        services.AddSingleton<IStoreService, StoreService>();
        services.AddOptions<SteamOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection(SteamOptions.SECTION_NAME).Bind(options);
            });

        return services;
    }
}