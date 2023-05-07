using System.Collections.Concurrent;
using FuzzySharp;
using FuzzySharp.PreProcess;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SteamServices;

public class SteamAppRepository : IAppRepository
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SteamAppRepository> _logger;
    
    public SteamAppRepository(IHttpClientFactory httpClientFactory, ILogger<SteamAppRepository> logger)
    {
        _httpClient = httpClientFactory.CreateClient("hardcodedsteam");
        _logger = logger;
    }

    public Task<List<SteamApp>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// TODO: Test and document.
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <param name="iMaxReturnCount"></param>
    /// <returns></returns>
    public async Task<List<SteamApp>> GetAsync(string searchTerm = "", int iMaxReturnCount = 3)
    {
        // Get Data.
        string jsonAppList = await HandleSteamApiQueryAsync("ISteamApps/GetAppList/v2");
        
        //Filter Data.
        List<string> jsonFilteredApps = FilterAppsJson(searchTerm, iMaxReturnCount, jsonAppList).ToList();

        // Convert Data.
        var steamApps = new List<SteamApp>();
        foreach (string json in jsonFilteredApps) if (JsonConvert.DeserializeObject<SteamApp>(json) is { } steamApp) steamApps.Add(steamApp);

        // Return.
        return steamApps;
    }

    /// <summary>
    /// TODO: Test and document.
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <param name="iMaxReturnCount"></param>
    /// <param name="jsonApps"></param>
    /// <returns></returns>
    private static IEnumerable<string> FilterAppsJson(string searchTerm, int iMaxReturnCount, string jsonApps)
    {
        if (string.IsNullOrEmpty(jsonApps)) return new List<string>();
        
        //Get apps from json.
        JObject jObject = JObject.Parse(jsonApps);
        JToken? jTokenAppList = jObject["applist"]?["apps"];
        var jTokenApps = new List<JToken>();
        if (jTokenAppList != null) jTokenApps = jTokenAppList.ToList();

        // Filter apps.
        ConcurrentQueue<JToken> primaryListApps = new();
        ConcurrentQueue<JToken> secondaryListApps = new();
        Parallel.ForEach(jTokenApps, (jToken, loopState) =>
        {
            var appName = jToken["name"]?.ToString();
            if (string.IsNullOrEmpty(appName)) return;

            // Skip if fuzzy ratio is less than 75.
            if (Fuzz.PartialRatio(appName, searchTerm, PreprocessMode.Full) < 75) return;

            // Check for great match (fuzzy > 90), and add to priorityList. 
            if (Fuzz.Ratio(appName.Split(' ')[0].ToLower(), searchTerm.Split(' ')[0].ToLower()) > 90)
            {
                primaryListApps.Enqueue(jToken);
                if (primaryListApps.Count >= iMaxReturnCount) loopState.Break();
            }
            secondaryListApps.Enqueue(jToken);
        });
        
        // Return up to iMaxReturnCount apps
        while (primaryListApps.Count < iMaxReturnCount && !secondaryListApps.IsEmpty)
            if (secondaryListApps.TryDequeue(out JToken? result)) primaryListApps.Enqueue(result);
        
        // take ane return the first iMaxReturnCount apps from primaryList.
        return primaryListApps.Take(iMaxReturnCount).Select(jToken => jToken.ToString());
    }

    /// <summary>
    /// // TODO: Test and document.
    /// </summary>
    /// <returns></returns>
    private async Task<string> HandleSteamApiQueryAsync(string sUrlPath = "", List<string>? listParameters = null)
    {
        HttpResponseMessage sResponse;
        try
        {
            sResponse = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, 
                new Uri(_httpClient.BaseAddress + sUrlPath)));
        }
        catch (Exception e)
        {
            _logger.LogWarning(string.Format("Failed to query Steam API. Url: {0}. {1}. {2}. {3}", 
                _httpClient.BaseAddress + sUrlPath, e.Message, e.InnerException?.Message, e.StackTrace));
            return "";
        }
        if (!sResponse.IsSuccessStatusCode) return "";
        string sResponseContent = await sResponse.Content.ReadAsStringAsync();
        return string.IsNullOrEmpty(sResponseContent) ? "" : sResponseContent;
    }
}