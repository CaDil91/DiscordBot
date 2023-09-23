using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SteamServices.Controllers;
using SteamServices.Repositories;

namespace SteamServices;

public static class DIRegistrations
{
    /// <summary>
    ///     DI container IServiceCollection extension method to register SteamStoreServices dependencies and settings.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterSteamServices(this IServiceCollection services)
    {
        services.AddHttpClient("hardcodedsteam");
        services.AddDbContext<SteamNewsDatabaseContext>(options => 
            options.UseSqlServer(Environment.GetEnvironmentVariable("SteamNewsDbConnectionString")!));
        services.AddSingleton<IController, SteamStoreController>();
        
        services.AddOptions<SteamOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection(SteamOptions.SECTION_NAME).Bind(options);
            });
        
        return services;
    }
}