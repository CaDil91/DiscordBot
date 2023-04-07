using Newtonsoft.Json.Linq;

namespace SteamServices;

public class StoreService
{
    private readonly IHttpClientFactory  _httpClientFactory;
    private readonly HttpClient _httpClient;
    public enum SortBy 
    {
        MostPopular,
    }

    public StoreService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient("hardcodedsteam");
    }

    /// <summary>
    /// Search the Steam store by a given search term.
    /// </summary>
    /// <param name="searchTerm">The search parameter to query to the Steam Store by.</param>
    /// <returns>List of App Id's for the given search</returns>
    public async Task<List<JObject>> GetAppFromStoreAsync(string? searchTerm, int iAppReturnCountMax = 10, List<SortBy> listSortBy = null)
    {
        List<JObject> retListAppIds = new() { new JObject() };

        HttpResponseMessage responseMessage = await _httpClient.GetAsync(new Uri($"https://store.steampowered.com/app/{searchTerm}"));
        
        //throw new Exception("Not yet implemented");
        return retListAppIds;
    }

    public async Task<HttpResponseMessage> RequestSteamStore() => await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://store.steampowered.com/app/"));

}