using FuzzySharp;
using FuzzySharp.PreProcess;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SteamServices;

public class StoreService
{
    public enum SortBy
    {
        MostPopular
    }

    private readonly HttpClient _httpClient;

    /// <summary>
    /// TODO: Add documentation.
    /// </summary>
    /// <param name="httpClientFactory"></param>
    public StoreService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("hardcodedsteam");
    }

    /// <summary>
    ///     Search the Steam store by a given search term.
    /// </summary>
    /// <param name="searchTerm">The search parameter to query to the Steam Store by.</param>
    /// <param name="iAppReturnCountMax"></param>
    /// <param name="listSortBy"></param>
    /// <returns>List of App Id's for the given search</returns>
    public async Task<List<SteamApp>> GetAppsFromStoreAsync(string searchTerm = "", int iAppReturnCountMax = 3,
        SortBy? listSortBy = SortBy.MostPopular)
    {
        List<SteamApp> steamApps = new();

        // Query the api.steampowered.com with the given search term.
        HttpResponseMessage sResponse = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
            new Uri("https://api.steampowered.com/ISteamApps/GetAppList/v2")));
        if (!sResponse.IsSuccessStatusCode) return steamApps;
        string sResponseContent = await sResponse.Content.ReadAsStringAsync();

        // Parse the response.
        JObject jResponse = JObject.Parse(sResponseContent);

        // Get the apps from the response.
        JToken? jApps = jResponse["applist"]?["apps"];
        
        // If the apps are null, return an empty list.
        if (jApps == null) return steamApps;
        
        // Convert each jApps to json string.
        var partialMatchList = new List<SteamApp>();
        foreach (JToken jToken in jApps)
        {
            // Convert jToken to json string.
            var json = jToken.ToString();
            if (string.IsNullOrEmpty(json)) continue;
            
            // Convert json to json object.
            JObject jObject = JObject.Parse(json);

            // Get jObject["name"]
            if (string.IsNullOrEmpty(jObject["name"]?.ToString())) continue;
            var sAppName = jObject["name"]!.ToString();

            // Fuzzy search filter on "name".
            if (Fuzz.PartialRatio(sAppName, searchTerm, PreprocessMode.Full) < 75) continue;
            
            // Add to list.
            if (JsonConvert.DeserializeObject<SteamApp>(json) is not { } steamApp) continue;
            
            // Check if "name" starts with the search term. 
            if (Fuzz.Ratio(sAppName.Split(' ')[0].ToLower(), searchTerm.ToLower()) > 90)
            {
                steamApps.Add(steamApp);
                if (steamApps.Count >= iAppReturnCountMax) break;
            }

            partialMatchList.Add(steamApp);
        }
        
        // If steamApps is less than iAppReturnCountMax, add partialMatchList.
        if (steamApps.Count < iAppReturnCountMax) steamApps.AddRange(partialMatchList);

        // return first iAppReturnCountMax apps.
        return steamApps.Take(iAppReturnCountMax).ToList();
    }

    /// <summary>
    /// TODO: Add documentation.
    /// </summary>
    /// <param name="steamApp"></param>
    /// <returns></returns>
    private async Task<int> GetAppPlayerCountAsync(SteamApp steamApp)
    {
        // Query the api.steampowered.com with the given steamApps AppId.
        HttpResponseMessage sResponse = await _httpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Get,
            new Uri($"https://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1/?appid={steamApp.AppId}")
        ));

        // Convert sResponse to a JObject.
        JObject jResponse = JObject.Parse(await sResponse.Content.ReadAsStringAsync());

        // Safely collect "playerCount" from the response.
        return int.TryParse(jResponse["response"]?["player_count"]?.ToString(), out int iPlayerCount) ? iPlayerCount : 0;
    }
    
    /// <summary>
    /// TODO: Add documentation.
    /// </summary>
    /// <param name="sAppId"></param>
    /// <returns></returns>
    private async Task<int> GetAppPlayerCountAsync(int sAppId)
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
    }
}