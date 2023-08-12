using AzureServices;
using GoogleService;
using Microsoft.Extensions.DependencyInjection;
using SteamServices;

namespace DiscordBot;

public static class CompositionRoot
{
    public static IServiceCollection ComposeApplication(this IServiceCollection services)
    {
        services.RegisterDiscordBot();
        services.RegisterSteamServices();
        services.RegisterAzureServices();
        services.RegisterGoogleServices();
        
        return services;
    }
}