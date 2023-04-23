using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SteamServices;

namespace DiscordBot.SteamServices;

public class DIRegistrationTests
{
    // Test that StoreService is registered.
    [Fact]
    public void RegisterSteamServices_AddsStoreService()
    {
        // Arrange.
        IServiceProvider serviceProvider = new ServiceCollection().RegisterSteamServices().BuildServiceProvider();
        
        // Act.
        var storeService = serviceProvider.GetRequiredService<StoreService>();
        
        // Assert.
        Assert.NotNull(storeService);
    }
    
    // Test that SteamOptions is registered.
    [Fact]
    public void RegisterSteamServices_AddsSteamOptions()
    {
        // Arrange.
        IServiceCollection services = new ServiceCollection();
        
        // Act.
        IServiceCollection serviceCollection = services.RegisterSteamServices();
        
        // Assert.
        Assert.Contains(serviceCollection, x => x.ServiceType == typeof(IConfigureOptions<SteamOptions>));
    }
}