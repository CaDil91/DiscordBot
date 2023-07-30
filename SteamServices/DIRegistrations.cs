using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SteamServices.MySteamDatabase;

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
        services.AddHttpClient("hardcodedsteam");
        services.AddDbContext<SteamNewsDatabaseContext>(options => 
            options.UseSqlServer(Environment.GetEnvironmentVariable("SteamNewsDbConnectionString")!));
        services.AddSingleton<IStoreService, SteamStoreService>();
        
        services.AddOptions<SteamOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection(SteamOptions.SECTION_NAME).Bind(options);
            });
        
        return services;
    }
}