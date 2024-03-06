using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace SteamAppsDiscordBot.Services;

/// <summary>
/// Represents a service for searching using a Google Programmable Search setup to search the Steam Store.
/// Implements the <see cref="IGoogleSearchService"/> interface.
/// </summary>
public class SteamStoreSearchService : IGoogleSearchService
{
    private readonly ILogger<SteamStoreSearchService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _cx;
    private readonly string _googleApiKey;

    /// <summary>
    /// Constructor for the GoogleCustomSearchService class.
    /// </summary>
    /// <param name="googleOptions">Options for Google Custom Search.</param>
    /// <param name="httpClientFactory">Factory for creating HttpClient instances.</param>
    /// <param name="logger">Logger for logging messages.</param>
    public SteamStoreSearchService(IOptions<GoogleOptions> googleOptions, IHttpClientFactory httpClientFactory,
        ILogger<SteamStoreSearchService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("hardcodedgoogle");
        _logger = logger;
        _cx = googleOptions.Value.SteamStoreCx;
        _googleApiKey = googleOptions.Value.Key;
    }


    /// <summary>
    /// Searches Steam Store for the given query and retrieves a list of search results.
    /// </summary>
    /// <param name="searchTerm">The search query.</param>
    /// <param name="iCount">The number of search results to retrieve. Default is 3.</param>
    /// <returns>A list of URIs representing the search results, or an empty list if the search fails or no results are found.</returns>
    public async Task<List<Uri>> SearchAsync(string searchTerm, int iCount = 3)
    {
        HttpResponseMessage response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
            new Uri($"{_httpClient.BaseAddress}?&q={searchTerm}&key={_googleApiKey}&cx={_cx}&lr=lang_en")));
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed to get results from google. Status Code: {StatusCode}. Reason: {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
            return new List<Uri>();
        }
        
        var googleResult = JsonConvert.DeserializeObject<GoogleResult>(await response.Content.ReadAsStringAsync());
        return googleResult?.Items == null ? new List<Uri>() : GetUriList(googleResult.Items, iCount);
    }

    /// <summary>
    /// Retrieves a list of URIs from the provided list of Google search items, limited to a specified count.
    /// </summary>
    /// <param name="googleItems">The list of Google search items.</param>
    /// <param name="iCount">The maximum number of URIs to retrieve.</param>
    /// <returns>A list of URIs.</returns>
    private static List<Uri> GetUriList(List<GoogleResult.GoogleItem> googleItems, int iCount)
    {
        var uriList = new List<Uri>();
        foreach (GoogleResult.GoogleItem googleItem in googleItems)
        {
            if (!Uri.TryCreate(googleItem.Link, UriKind.Absolute, out Uri? uri))
                continue; // Skip invalid URIs.

            uriList.Add(uri);

            if (uriList.Count == iCount)
                break; // Stop if the desired count is reached.
        }

        return uriList;
    }

    [JsonObject]
    private class GoogleResult
    {
        [JsonProperty("items")] public List<GoogleItem> Items { get; set; } = new();

        internal class GoogleItem
        {
            [JsonProperty("link")] public string Link { get; set; } = "";
        }
    }
}