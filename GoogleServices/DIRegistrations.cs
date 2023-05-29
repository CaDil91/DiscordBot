using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoogleService;

public static class DIRegistrations
{
    public static IServiceCollection RegisterGoogleServices(this IServiceCollection services)
    {
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