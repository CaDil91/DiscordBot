using Microsoft.Extensions.DependencyInjection;

namespace SteamAppsDiscordBot;

public static class CompositionRoot
{
    public static IServiceCollection ComposeApplication(this IServiceCollection services)
    {
        services.RegisterDiscordBot();
        services.RegisterSteamServices();
        services.RegisterGoogleServices();
        
        return services;
    }
}