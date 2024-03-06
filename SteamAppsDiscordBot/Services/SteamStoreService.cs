using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SteamAppsDiscordBot.Services.DTO;

namespace SteamAppsDiscordBot.Services;

/// <summary>
/// Represents a service that interacts with the Steam store.
/// </summary>
public class SteamStoreService : ISteamStoreService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SteamStoreService> _logger;
    private readonly IGoogleSearchService _steamStoreGoogleSearch;

    /// <summary>
    /// The SteamStoreServices class is responsible for fetching data from the Steam Store API.
    /// </summary>
    /// <param name="httpClientFactory">An instance of IHttpClientFactory used to create an HttpClient for making HTTP requests.</param>
    /// <param name="logger">An instance of ILogger used for logging.</param>
    /// <param name="steamStoreGoogleSearch">An instance of IGoogleSearchRepository used for interacting with the Steam store repository.</param>
    public SteamStoreService(IHttpClientFactory httpClientFactory, ILogger<SteamStoreService> logger,
        IGoogleSearchService steamStoreGoogleSearch)
    {
        _httpClient = httpClientFactory.CreateClient("hardcodedsteam");
        _logger = logger;
        _steamStoreGoogleSearch = steamStoreGoogleSearch;
    }

    /// <summary>
    /// Search the Steam store by a given search term.
    /// </summary>
    /// <param name="searchTerm">The search parameter to query to the Steam Store by.</param>
    /// <param name="iMaxReturnCount"></param>
    /// <returns>List of App Id's for the given search</returns>
    public async Task<List<SteamApp>> GetAppsAsync(string searchTerm = "", int iMaxReturnCount = 3)
    {
        // Google search the Steam store for the given search term.
        List<Uri> searchResults = await _steamStoreGoogleSearch.SearchAsync(searchTerm, iMaxReturnCount).ConfigureAwait(false);
        if (searchResults.Count == 0) return new List<SteamApp>();
        
        // Get the app id's from the search results.
        IEnumerable<string> appIds = ExtractAppIds(searchResults);

        // Asynchronously get the SteamApp for each appId.
        List<Task<SteamApp?>> getAppTasks = appIds.Select(GetSteamAppAsync).ToList();
        SteamApp?[] apps = await Task.WhenAll(getAppTasks);
        
        return apps
            .Where(app => app != null)
            .Select(app => app!) // We know from the above check that app is not null
            .ToList();
    }

    private static IEnumerable<string> ExtractAppIds(IEnumerable<Uri> searchResults)
    {
        List<string> appIds = searchResults.Select(result => result.AbsoluteUri.Split("/")[4]).ToList();
        appIds = appIds.Distinct().Where(appId => !string.IsNullOrEmpty(appId)).ToList();
        return appIds;
    }

    /// <summary>
    /// Get a SteamApp from Steam's store.steampowered.com/api.
    /// </summary>
    /// <param name="appId">App to get.</param>
    /// <returns></returns>
    public async Task<SteamApp?> GetSteamAppAsync(string appId)
    {
        // Query the Steam API with the given appId.
        string? response = await QueryApiAsync(new Uri($"https://store.steampowered.com/api/appdetails?appids={appId}"), true);
        if (string.IsNullOrEmpty(response) || !response.StartsWith("{") || !response.EndsWith("}")) return null;

        // Safely convert response to a JObject.
        JObject jResponse = JObject.Parse(response);
        
        // Safely collect the data node of the first child of the response.
        JToken? jToken = jResponse[appId]?["data"];
        var steamApp = jToken?.ToObject<SteamApp>();
        
        return steamApp;
    }

    /// <summary>
    /// Handles the http request to the Steam API.
    /// </summary>
    /// <param name="uri">Uri to query.</param>
    /// <param name="bValidateResponseIsJson">Validate the response is json.</param>
    /// <returns>Steams http response as ReadAsStringAsync()</returns>
    public async Task<string?> QueryApiAsync(Uri uri, bool bValidateResponseIsJson = false)
    {
        HttpResponseMessage sResponse;
        try
        {
            sResponse = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri), HttpCompletionOption.ResponseContentRead);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to query Steam API.");
            return null;
        }

        // Log if the response was unsuccessful.
        if (!sResponse.IsSuccessStatusCode) 
            _logger.LogWarning(string.Format("Failed to query Steam API. Url: {0}. Status Code: {1}. Reason: {2}.",
                uri.AbsoluteUri, sResponse.StatusCode, sResponse.ReasonPhrase));
        
        // Validate response is json.
        if (bValidateResponseIsJson 
            && sResponse.Content.Headers.ContentType?.MediaType is { } && sResponse.Content.Headers.ContentType != null 
            && !sResponse.Content.Headers.ContentType.MediaType.Equals("application/json"))
        {
            _logger.LogWarning(string.Format("Failed to query Steam API. Url: {0}. Status Code: {1}. Reason: {2}.",
                uri.AbsoluteUri, sResponse.StatusCode, sResponse.ReasonPhrase));
            return string.Empty;
        }
        
        return await sResponse.Content.ReadAsStringAsync();
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

    /// <summary>
    /// Get the news for a given app.
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="lastNewsCheckDate"></param>
    /// <returns></returns>
    public IEnumerable<AppNews.NewsItem> GetNewsForApp(int appId, DateTime? lastNewsCheckDate)
    {
        
        string? response = QueryApiAsync(new Uri($"https://api.steampowered.com/ISteamNews/GetNewsForApp/v2/" +
                                                 $"?appid={appId}&feeds=steam_community_announcements"), true).Result;
        
        if (string.IsNullOrEmpty(response) || !response.StartsWith("{") || !response.EndsWith("}")) 
            return new List<AppNews.NewsItem>();
        
        JObject jResponse = JObject.Parse(response);
        AppNews appNews = jResponse["appnews"]?.ToObject<AppNews>() ?? new AppNews();
        
        if (appNews.NewsItems is not { Count: > 0 }) return new List<AppNews.NewsItem>();

        return lastNewsCheckDate != null ? 
            appNews.NewsItems.Where(newsItem => newsItem.Date > lastNewsCheckDate).ToList() 
            : appNews.NewsItems.Take(3).ToList();
    }
}