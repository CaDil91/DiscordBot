using DiscordBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DiscordBot.GoogleService;

public class DIRegistrationTests
{
    private readonly IHost _host;
    private IServiceCollection _subjectUnderTest;

    public DIRegistrationTests()
    {
        _subjectUnderTest = new ServiceCollection();

        // Create _host, and Act.
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, configuration) => configuration.AddEnvironmentVariables())
            .ConfigureServices((_, services) =>
            {
                services.RegisterGoogleServices();
                _subjectUnderTest = services;
            });
        
        _host = hostBuilder.Build();
    }
    
    [Fact]
    public void RegisterGoogleServices_AddsHttpClient()
    {
        // Arrange.
        
        // Act. Get the named HttpClient.
        HttpClient httpClient = _host.Services.GetRequiredService<IHttpClientFactory>().CreateClient("hardcodedgoogle");
        
        // Assert.
        Assert.NotNull(httpClient);
        Assert.Contains("googleapis.com", httpClient.BaseAddress?.ToString());

    }
    
    [Fact]
    public void RegisterGoogleServices_AddsStoreService()
    {
        // Arrange.
        // Act.
        // Assert.
        Assert.Contains(_subjectUnderTest, serviceDescriptor => serviceDescriptor.ServiceType == typeof(IGoogleSearchRepository));
    }
    
    [Fact]
    public void RegisterGoogleServices_AddsGoogleOptions()
    {
        // Arrange.
        var googleOptions = _host.Services.GetRequiredService<IOptions<GoogleOptions>>();
        
        // Act.
        // Assert.
        Assert.NotNull(googleOptions);
        Assert.NotNull(googleOptions.Value.Key);
        Assert.NotNull(googleOptions.Value.SteamStoreCx);
    }
    
    [Fact]
    public void RegisterGoogleServices_AddsGoogleOptions_LoadsKeyFromConfigurationManagerAppSettings()
    {
        // Arrange.

        // Act.

        // Assert.
    }
    
}