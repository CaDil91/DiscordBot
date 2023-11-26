using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot;

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