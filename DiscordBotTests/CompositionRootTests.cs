using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordBot;

public class CompositionRootTests
{
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
}