using System.Text.RegularExpressions;
using FuzzySharp;
using GoogleService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace SteamServices;

public class SteamAppRepository : IAppRepository
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SteamAppRepository> _logger;
    private readonly IGoogleSearchRepository _steamStoreRepository;

    public SteamAppRepository(IHttpClientFactory httpClientFactory, ILogger<SteamAppRepository> logger, IGoogleSearchRepository steamStoreRepository)
    {
        _httpClient = httpClientFactory.CreateClient("hardcodedsteam");
        _logger = logger;
        _steamStoreRepository = steamStoreRepository;
    }

    public Task<List<SteamApp>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get a list of SteamApps from the Steam API ISteamApps.
    /// </summary>
    /// <param name="searchTerm">Term to search for.</param>
    /// <param name="iMaxReturnCount">Maximum SteamApps returns. Default: 3</param>
    /// <returns></returns>
    public async Task<List<SteamApp>> GetAsync(string searchTerm, int iMaxReturnCount = 3)
    {
        var steamApps = new List<SteamApp>();
        // Get Data.
        List<string> jsonAppList = await _steamStoreRepository.GetCustomSearchResultsAsync("steam app list");
        
        /*//Filter Data.
        List<string> jsonFilteredApps = FilterAppsJson(jsonAppList, searchTerm, iMaxReturnCount).ToList();

        // Convert Data.
        var steamApps = new List<SteamApp>();
        foreach (string json in jsonFilteredApps) if (JsonConvert.DeserializeObject<SteamApp>(json) is { } steamApp) steamApps.Add(steamApp);*/

        // Return.
        return steamApps;
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
    /// TODO: Test and document.
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <param name="iMaxReturnCount"></param>
    /// <param name="jsonApps"></param>
    /// <returns></returns>
    public IEnumerable<string> FilterAppsJson(string jsonApps, string searchTerm, int iMaxReturnCount)
    {
        if (string.IsNullOrWhiteSpace(jsonApps)) return new List<string>();
        List<JToken> returnList = new();

        // Get app list.
        List<JToken>? jTokenAppList = JToken.Parse(jsonApps)["applist"]?["apps"]?.ToList();
        if (jTokenAppList is not { Count: > 0 }) return new List<string>();
        
        // Remove apps that don't contain the whole searchTerm. Ignore case. Ignore punctuation. Ignore whitespace.
        jTokenAppList.RemoveAll(jTokenApp => !Regex.IsMatch(
            jTokenApp["name"]?.ToString() ?? "",
            $@"\b{Regex.Escape(searchTerm)}\b", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)
        );
        if (jTokenAppList.Count <= iMaxReturnCount) return jTokenAppList.Select(jToken => jToken.ToString());
        
        // Get apps that start with searchTerm. Ignore case. Ignore punctuation. Ignore whitespace.
        IEnumerable<JToken> startsWithList = jTokenAppList.Where(jTokenApp => Regex.IsMatch(jTokenApp["name"]?.ToString() ?? "",
            $@"^{Regex.Escape(searchTerm)}", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)).ToList();
        returnList.AddRange(startsWithList);
        
        if (returnList.Count >= iMaxReturnCount) return returnList.Take(iMaxReturnCount).Select(jToken => jToken.ToString());
        
        // Apply FuzzySharp to get the best matches of remaining app names.
        Dictionary<JToken, int> appNameRatios = jTokenAppList.ToDictionary
        (
            jTokenApp => jTokenApp, 
            jTokenApp => Fuzz.PartialRatio(jTokenApp["name"]?.ToString() ?? "", searchTerm)
        );
        appNameRatios = appNameRatios.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        
        return appNameRatios.Keys.Take(iMaxReturnCount).Select(jToken => jToken.ToString());
    }

    /// <summary>
    /// Handles the http request to the Steam API.
    /// </summary>
    /// <returns>Steams http response as ReadAsStringAsync()</returns>
    private async Task<string> HandleApiQueryAsync(string sUrlPath = "")
    {
        HttpResponseMessage sResponse;
        string sResponseContent;
        try
        {
            sResponse = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, 
                new Uri(_httpClient.BaseAddress + sUrlPath)));
            sResponseContent = await sResponse.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            _logger.LogWarning(string.Format("Failed to query Steam API. Url: {0}. {1}. {2}. {3}", 
                _httpClient.BaseAddress + sUrlPath, e.Message, e.InnerException?.Message, e.StackTrace));
            return string.Empty;
        }
        
        // Log if the response was unsuccessful.
        if (!sResponse.IsSuccessStatusCode)
            _logger.LogWarning(string.Format("Failed to query Steam API. Url: {0}. Status Code: {1}. Reason: {2}.",
                _httpClient.BaseAddress + sUrlPath, sResponse.StatusCode, sResponse.ReasonPhrase));

        return sResponseContent;
    }
}