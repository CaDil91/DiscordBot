using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace AzureServices;

public static class DIRegistrations
{
    public static IServiceCollection RegisterAzureServices(this IServiceCollection services)
    {
        // Add AzureOptions
        services.AddOptions<AzureOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection(AzureOptions.SECTION_NAME).Bind(options);
            });
        
        return services;
    }
}