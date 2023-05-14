using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using GoogleService;
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

        // Create _host.
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder()
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
    
    // Test that SteamStoreRepository is registered.
    [Fact]
    public void RegisterGoogleServices_AddsStoreService()
    {
        // Arrange.
        
        // Act.

        // Assert.
        Assert.Contains(_subjectUnderTest, serviceDescriptor => serviceDescriptor.ServiceType == typeof(IGoogleSearchRepository));
    }
    
    // Test that SteamOptions is registered.
    [Fact]
    public async Task RegisterGoogleServices_AddsGoogleOptions()
    {
        // Arrange.
        var secretClient = new SecretClient(new Uri("https://justabotvault.vault.azure.net/"), new DefaultAzureCredential());
        var googleOptions = _host.Services.GetRequiredService<IOptions<GoogleOptions>>();
        
        // Act.
        Response<KeyVaultSecret>? test = await secretClient.GetSecretAsync(googleOptions.Value.Token);
        

        // Assert.
        Assert.NotNull(googleOptions);
        Assert.Equal("Google", googleOptions.Value.Token);
        Assert.Equal("3533c3e3e23024252", googleOptions.Value.SteamStoreCx);
        Assert.True(test.HasValue);
    }
    
}