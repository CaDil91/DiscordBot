using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SteamServices;
using SteamServices.Controllers;

namespace DiscordBot.SteamServices;

public class DIRegistrationTests
{
    private readonly IHost _host;
    private IServiceCollection _subjectUnderTest;

    public DIRegistrationTests()
    {
        _subjectUnderTest = new ServiceCollection();
        
        // Create _host.
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.RegisterSteamServices();
                _subjectUnderTest = services;
            });
        
        _host = hostBuilder.Build();
    }
    
    [Fact]
    public void RegisterSteamServices_AddsHttpClient()
    {
        // Arrange.
        
        // Act. Get the named HttpClient.
        HttpClient httpClient = _host.Services.GetRequiredService<IHttpClientFactory>().CreateClient("hardcodedsteam");

        // Assert.
        Assert.NotNull(httpClient);
    }

    // Test that StoreService is registered.
    [Fact]
    public void RegisterSteamServices_AddsStoreService()
    {
        // Arrange.
        
        // Act.

        // Assert.
        Assert.Contains(_subjectUnderTest, x => x.ServiceType == typeof(IController));
    }
    
    // Test that SteamOptions is registered.
    [Fact]
    public void RegisterSteamServices_AddsSteamOptions()
    {
        // Arrange.
        var steamOptions = _host.Services.GetRequiredService<IOptions<SteamOptions>>();
        
        // Act.
        // Nothing to do here.

        // Assert.
        Assert.NotNull(steamOptions);
        Assert.Equal("SteamToken", steamOptions.Value.Token);
    }
}