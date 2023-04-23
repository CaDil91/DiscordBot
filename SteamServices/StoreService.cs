using Azure.Security.KeyVault.Secrets;
using AzureServices;
using Microsoft.Extensions.Options;

namespace SteamServices;

public class StoreService
{
    public enum SortBy
    {
        MostPopular
    }
    
    private readonly HttpClient _httpClient;
    private readonly SecretClient _secretClient;
    private readonly IOptions<AzureOptions> _azureOptions;

    public StoreService()
    {
    }

    /// <summary>
    /// TODO: Add documentation.
    /// </summary>
    /// <param name="httpClientFactory"></param>
    /// <param name="secretClient"></param>
    /// <param name="azureOptions"></param>
    public StoreService(IHttpClientFactory httpClientFactory, SecretClient secretClient, IOptions<AzureOptions> azureOptions)
    {
        _httpClient = httpClientFactory.CreateClient("hardcodedsteam");
        _secretClient = secretClient;
        _azureOptions = azureOptions;
    }

    /// <summary>
    ///     Search the Steam store by a given search term.
    /// </summary>
    /// <param name="searchTerm">The search parameter to query to the Steam Store by.</param>
    /// <param name="iAppReturnCountMax"></param>
    /// <param name="listSortBy"></param>
    /// <returns>List of App Id's for the given search</returns>
    public async Task<string> GetAppsFromStoreAsync(string? searchTerm, int iAppReturnCountMax = 10, List<SortBy>? listSortBy = null)
    {
        //List<SteamApp> steamApps = new();

        HttpResponseMessage sResponse = await _httpClient
            .GetAsync($"https://api.steampowered.com/ISteamApps/GetAppList/v2/?key={_secretClient.GetSecretAsync(_azureOptions.Value.SteamSecret)}");
        
        return "Not yet implemented";
    }
}