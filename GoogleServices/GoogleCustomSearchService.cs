using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GoogleService;

public class GoogleCustomSearchService : IGoogleSearchRepository, IDisposable
{
    private readonly ILogger<GoogleCustomSearchService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _cx;
    private readonly string _googleApiKey;

    /// <summary>
    /// Constructor for the GoogleCustomSearchService class.
    /// </summary>
    /// <param name="googleOptions">Options for Google Custom Search.</param>
    /// <param name="httpClientFactory">Factory for creating HttpClient instances.</param>
    /// <param name="logger">Logger for logging messages.</param>
    public GoogleCustomSearchService(IOptions<GoogleOptions> googleOptions, IHttpClientFactory httpClientFactory,
        ILogger<GoogleCustomSearchService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("hardcodedgoogle");
        _logger = logger;
        _cx = googleOptions.Value.SteamStoreCx;
        _googleApiKey = googleOptions.Value.Key;

        // Build the default query parameters using the retrieved API key and other options.;
    }

    /// <summary>
    /// Retrieves custom search results from Google asynchronously.
    /// </summary>
    /// <param name="sQuery">The search query string.</param>
    /// <param name="iCount">The number of search results to retrieve. Default is 10.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of URIs representing the search results.</returns>
    public async Task<List<Uri>> GetCustomSearchResultsAsync(string sQuery, int iCount = 10)
    {
        // Send HTTP request to Google and get response.
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, new Uri($"{_httpClient.BaseAddress}?&q={sQuery}&key={_googleApiKey}&cx={_cx}&lr=lang_en")));
        }
        catch (HttpRequestException e)   
        {
            _logger.LogError(e, "Request to Google Custom Search API failed.");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Failed to send http request.");
            throw;
        }
        
        // If the response is not successful, log a warning and return an empty list.
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed to get results from google. Status code: {StatusCode}. Reason: {Reason}. Request message: {RequestMessage}", 
                response.StatusCode, response.ReasonPhrase, response.RequestMessage);
            return new List<Uri>();
        }

        // Deserialize the response.
        var googleResult = JsonConvert.DeserializeObject<GoogleResult>(await response.Content.ReadAsStringAsync());
        if (googleResult is not { Items: { } })
        {
            _logger.LogError("Failed to get items from google search. Response: {Response}", response.Content);
            throw new ArgumentException("Failed to get items from google search.");
        }

        // Get the first iCount results.
        List<Uri> uriList = GetUriList(googleResult.Items, iCount);
        
        return uriList;
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
        [JsonProperty("items")]
        public List<GoogleItem> Items { get; set; } = new();

        internal class GoogleItem
        {
            [JsonProperty("link")]
            public string Link { get; set; } = "";
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}

