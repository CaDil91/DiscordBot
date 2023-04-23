using Castle.Core.Configuration;
using DiscordBot.DiscordBot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;

namespace DiscordBot;

public class CompositionRootTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public CompositionRootTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void ComposeApplication_ReturnsServices()
    {
        // Arrange.
        IServiceCollection services = new ServiceCollection();

        // Act.
        services = services.ComposeApplication();

        // Assert.
        Assert.NotEmpty(services);
    }
    
    [Fact]
    public void ComposeApplication_AddsServices()
    {
        // Arrange.
        IServiceCollection services = new ServiceCollection();
        int iBefore = services.Count;

        // Act.
        services = services.ComposeApplication();
        int iAfter = services.Count;

        // Assert.
        Assert.True(iBefore < iAfter);
    }

    [Fact]
    public void ComposeApplication_AddsSteamHttpClient()
    {
        // Arrange.
        IServiceCollection services = new ServiceCollection();
        
        // Act.
        IServiceProvider serviceProvider = new ServiceCollection().ComposeApplication().BuildServiceProvider();
        HttpClient steamHttpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("hardcodedsteam");
        
        // Assert.
        Assert.NotNull(steamHttpClient);
    }
    
    [Fact]
    public void ComposeApplication_AddsDiscordBotOptions()
    {
        // Arrange.
        IServiceCollection services = new ServiceCollection();
        
        // Act.
        IServiceCollection serviceCollection = services.ComposeApplication();

        // Assert.
        Assert.Contains(serviceCollection, x => x.ServiceType == typeof(IConfigureOptions<DiscordBotOptions>));
    }
}