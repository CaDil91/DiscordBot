using Azure;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GoogleService;

public class GoogleCustomSearchService : IGoogleSearchRepository
{
    private readonly ILogger<GoogleCustomSearchService> _logger;
    private readonly HttpClient _httpClient;
    private readonly SecretClient _secretClient;
    private readonly string _sDefaultParameters;
    
    private static string? _key;
    private async Task<bool> SetKeyAsync(string sKeyName)
    {
        try
        {
            Response<KeyVaultSecret> secret = await _secretClient.GetSecretAsync(sKeyName).ConfigureAwait(false);
            _key = secret.Value.Value;
        }
        catch (ArgumentException e)
        {
            _logger.LogError(e, "Key name is empty.");
            throw;
        }
        catch (RequestFailedException e)
        {
            _logger.LogError(e, "Request to key vault failed. Key name: {KeyName}", sKeyName);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected exception.");
            throw;
        }

        return true;
    }

    /// <summary>
    /// Constructor for the GoogleCustomSearchService class.
    /// </summary>
    /// <param name="googleOptions">Options for Google Custom Search.</param>
    /// <param name="httpClientFactory">Factory for creating HttpClient instances.</param>
    /// <param name="secretClient">Client for accessing secrets.</param>
    /// <param name="logger">Logger for logging messages.</param>
    public GoogleCustomSearchService(IOptions<GoogleOptions> googleOptions, IHttpClientFactory httpClientFactory,
        SecretClient secretClient, ILogger<GoogleCustomSearchService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("hardcodedgoogle");
        _logger = logger;
        _secretClient = secretClient;
        
        // If the API key is null, retrieve it asynchronously from the googleOptions and set it.
        if (_key == null) SetKeyAsync(googleOptions.Value.Token).Wait();

        // Build the default query parameters using the retrieved API key and other options.
        _sDefaultParameters = $"key={_key}" +
                              $"&cx={googleOptions.Value.SteamStoreCx}" +
                              "&lr=lang_en";
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
        var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_httpClient.BaseAddress}?{_sDefaultParameters}&q={sQuery}"));
        
        HttpResponseMessage response;
        try
        {
            response = _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send http request. {request}", request);
            throw;
        }
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed to get results from google. Status code: {StatusCode}. Reason: {Reason}. Request message: {RequestMessage}", 
                response.StatusCode, response.ReasonPhrase, response.RequestMessage);
            return new List<Uri>();
        }

        // Deserialize the response.
        var googleResult = JsonConvert.DeserializeObject<GoogleResult>(await response.Content.ReadAsStringAsync());
        if (googleResult is not { Items: { } }) throw new Exception($"Failed to deserialize successfully retrieved google search. Response: {response}");

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
}

