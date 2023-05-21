using Azure;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace GoogleService;

public class SteamStoreRepository : IGoogleSearchRepository
{
    private readonly ILogger<SteamStoreRepository> _logger;
    private readonly HttpClient _httpClient;
    private readonly SecretClient _secretClient;
    private static string? _key;
    private readonly string _sDefaultParameters;

    public async Task<bool> SetKeyAsync(string sKeyName)
    {
        try
        {
            Response<KeyVaultSecret> secret = await _secretClient.GetSecretAsync(sKeyName).ConfigureAwait(false);
            _key = secret.Value.Value;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to get secret from vault.");
            return false;
        }

        return true;
    }


    /// <summary>
    /// TODO: Document.
    /// </summary>
    /// <param name="googleOptions"></param>
    /// <param name="httpClientFactory"></param>
    /// <param name="secretClient"></param>
    /// <param name="logger"></param>
    public SteamStoreRepository(IOptions<GoogleOptions> googleOptions, IHttpClientFactory httpClientFactory,
        SecretClient secretClient, ILogger<SteamStoreRepository> logger)
    {
        _httpClient = httpClientFactory.CreateClient("hardcodedgoogle");
        _logger = logger;
        _secretClient = secretClient;
        if (_key == null) SetKeyAsync(googleOptions.Value.Token).Wait();

        _sDefaultParameters = $"?key={_key}" +
                              $"&cx={googleOptions.Value.SteamStoreCx}" +
                              "&lr=lang_en";
    }

    /// <summary>
    /// TODO: Document.
    /// </summary>
    /// <param name="sQuery"></param>
    /// <param name="iCount"></param>
    /// <returns></returns>
    public async Task<List<string>> GetCustomSearchResultsAsync(string sQuery, int iCount = 10)
    {
        HttpResponseMessage response;
        try
        {
            response = _httpClient.SendAsync(new HttpRequestMessage(
                    HttpMethod.Get, new Uri($"{_httpClient.BaseAddress}{_sDefaultParameters}&q={sQuery}")),
                    HttpCompletionOption.ResponseContentRead).Result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send http request.");
            return new List<string>();
        }

        List<string> results;
        if (response.IsSuccessStatusCode)
        {
            results = await GetLinksFromResponseAsync(response).ConfigureAwait(false);
        }
        else
        {
            _logger.LogWarning(string.Format("Failed to get results from google. Status code: {0}. Reason: {1}. Request message: {2}", 
                response.StatusCode, response.ReasonPhrase, response.RequestMessage));
            return new List<string>();
        }
        
        return results.Take(iCount).ToList();
    }

    /// <summary>
    /// TODO: Document.
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    public async Task<List<string>> GetLinksFromResponseAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return new List<string>();
        
        string sContent = await response.Content.ReadAsStringAsync();
        
        // Verify that the response is a valid json.
        if (!sContent.StartsWith("{") || !sContent.EndsWith("}")) return new List<string>();

        // Parse the json.
        JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
        List<string> results = json["items"]?.Select(item => item["link"]?.ToString() ?? "").ToList() ?? new List<string>();
        results.RemoveAll(string.IsNullOrEmpty);
        
        return results;
    }
}