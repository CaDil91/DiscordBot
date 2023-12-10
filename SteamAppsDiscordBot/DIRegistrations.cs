using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SteamAppsDiscordBot.Adapters;
using SteamAppsDiscordBot.Repositories;
using SteamAppsDiscordBot.Services;

// ReSharper disable RedundantTypeArgumentsOfMethod

namespace SteamAppsDiscordBot;

public static class DIRegistrations
{
    /// <summary>
    ///     DI container IServiceCollection extension method to register DiscordBot dependencies and settings.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterDiscordBot(this IServiceCollection services)
    {
        services.AddLogging(x => x.AddConsole());
        
        // When working with events that have Cacheable<IMessage, ulong> parameters,
        // you must enable the message cache in your config settings if you plan to
        // use the cached message entity. 
        var client = new DiscordSocketClient(new DiscordSocketConfig { MessageCacheSize = 100 });
        services.AddSingleton<DiscordSocketClient>(client);
        services.AddSingleton<IDiscordSocketClientAdapter, DiscordSocketClientAdapter>();
        
        var interactionService = new InteractionService(client.Rest);
        services.AddSingleton<InteractionService>(interactionService);
        services.AddSingleton<IInteractionServiceAdapter, InteractionServiceAdapter>();
        
        services.AddSingleton<DiscordBot>();
        services.AddOptions<DiscordBotOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection(DiscordBotOptions.SECTION_NAME).Bind(options);
            });
        
        services.AddSingleton<DiscordGuildServices>();

        return services;
    }
    
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
        services.AddSingleton<ISteamStoreService, SteamStoreServices>();
        
        services.AddOptions<SteamOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection(SteamOptions.SECTION_NAME).Bind(options);
            });
        
        return services;
    }
    
    public static IServiceCollection RegisterGoogleServices(this IServiceCollection services)
    {
        //change
        services.AddHttpClient("hardcodedgoogle", client => { client.BaseAddress = new Uri("https://www.googleapis.com/customsearch/v1"); });
        services.AddSingleton<IGoogleSearchRepository, GoogleCustomSearchService>();
        services.AddOptions<GoogleOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection(GoogleOptions.SECTION_NAME).Bind(options);
            });
        return services;
    }
}