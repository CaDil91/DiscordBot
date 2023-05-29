using GoogleService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace SteamServices;

public class SteamStoreService : IStoreService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SteamStoreService> _logger;
    private readonly IGoogleSearchRepository _steamStoreRepository;

    /// <summary>
    /// TODO: Add documentation.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="httpClientFactory"></param>
    /// <param name="steamStoreRepository"></param>S
    public SteamStoreService(IHttpClientFactory httpClientFactory, ILogger<SteamStoreService> logger, IGoogleSearchRepository steamStoreRepository)
    {
        _httpClient = httpClientFactory.CreateClient("hardcodedsteam");
        _logger = logger;
        _steamStoreRepository = steamStoreRepository;
    }

    /// <summary>
    /// Search the Steam store by a given search term.
    /// </summary>
    /// <param name="searchTerm">The search parameter to query to the Steam Store by.</param>
    /// <param name="iAppReturnCountMax"></param>
    /// <returns>List of App Id's for the given search</returns>
    public async Task<List<SteamApp>> GetAppsAsync(string searchTerm = "", int iAppReturnCountMax = 3)
    {
        // Get top results from custom google search api.
        List<Uri> searchResults = await _steamStoreRepository.GetCustomSearchResultsAsync(searchTerm, iAppReturnCountMax);
        if (searchResults.Count == 0) return new List<SteamApp>();
        
        // Get the app id's from the search results.
        List<string> appIds = searchResults.Select(result => result.AbsoluteUri.Split("/")[4]).ToList();
        appIds = appIds.Distinct().Where(appId => !string.IsNullOrEmpty(appId)).ToList();
        
        // Get the SteamApp's from the app id's.
        List<SteamApp> steamApps = new();
        foreach (string appId in appIds)
        {
            SteamApp? steamApp = await GetSteamAppAsync(appId);
            if (steamApp != null) steamApps.Add(steamApp);
        }
        
        return steamApps;
    }
    
    /// <summary>
    /// Get a SteamApp from Steam's store.steampowered.com/api.
    /// </summary>
    /// <param name="appId">App to get.</param>
    /// <returns></returns>
    private async Task<SteamApp?> GetSteamAppAsync(string appId)
    {
        // Query the Steam API with the given appId.
        string? response = await HandleApiQueryAsync(new Uri($"https://store.steampowered.com/api/appdetails?appids={appId}"));
        if (string.IsNullOrEmpty(response)) return null;
        
        // Validate response is json, and convert to SteamApp.
        if (!JObject.Parse(response).TryGetValue(appId, out _)) return null;
        JToken? jToken = JObject.Parse(response)[appId]?["data"] ?? null;
        var steamApp = jToken?.ToObject<SteamApp>();

        return steamApp;
    }

    /// <summary>
    /// Handles the http request to the Steam API.
    /// </summary>
    /// <returns>Steams http response as ReadAsStringAsync()</returns>
    private async Task<string?> HandleApiQueryAsync(Uri uri)
    {
        HttpResponseMessage sResponse;
        try
        {
            sResponse = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), HttpCompletionOption.ResponseContentRead);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to query Steam API.");
            return string.Empty;
        }

        if (sResponse.IsSuccessStatusCode) return await sResponse.Content.ReadAsStringAsync();
        
        // Log if the response was unsuccessful.
        _logger.LogWarning(string.Format("Failed to query Steam API. Url: {0}. Status Code: {1}. Reason: {2}.",
            uri.AbsoluteUri, sResponse.StatusCode, sResponse.ReasonPhrase));
        return null;


    }

    /*public async Task<int> GetAppPlayerCountAsync(int sAppId)
    {
        // Query the api.steampowered.com with the given steamApps AppId.
        HttpResponseMessage sResponse = await _httpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Get,
            new Uri($"https://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1/?appid={sAppId.ToString()}")
        ));

        // Safely convert sResponse to a JObject.
        JObject jResponse = JObject.Parse(await sResponse.Content.ReadAsStringAsync());

        // Safely collect "playerCount" from the response.
        return int.TryParse(jResponse["response"]?["player_count"]?.ToString(), out int iPlayerCount) ? iPlayerCount : 0;
    }*/

}