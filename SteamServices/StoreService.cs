using Newtonsoft.Json.Linq;

namespace SteamServices;

public class StoreService
{
    /// <summary>
    /// 
    /// </summary>
    public enum SortBy
    {
        MostPopular
    }

    /// <summary>
    /// 
    /// </summary>
    private readonly HttpClient _httpClient;
    
    private readonly AzureEncryptionService _azureEncryptionService;

    /// <summary>
    /// 
    /// </summary>
    public StoreService()
    {
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpClientFactory"></param>
    public StoreService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("hardcodedsteam");
        //_azureEncryptionService = azureEncryptionService;
    }

    /// <summary>
    ///     Search the Steam store by a given search term.
    /// </summary>
    /// <param name="searchTerm">The search parameter to query to the Steam Store by.</param>
    /// <param name="iAppReturnCountMax"></param>
    /// <param name="listSortBy"></param>
    /// <returns>List of App Id's for the given search</returns>
    public async Task<List<SteamApp>> GetAppsFromStoreAsync(string? searchTerm, int iAppReturnCountMax = 10, List<SortBy>? listSortBy = null)
    {
        List<SteamApp> steamApps = new();

        /*HttpResponseMessage sResponse = await _httpClient
            .GetAsync($"https://api.steampowered.com/ISteamApps/GetAppList/v2/?key={_azureEncryptionService.DecryptAsync(Convert.FromBase64String(steamSettings.Value.Token)).Result}");
            */

        //throw new Exception("Not yet implemented");
        return steamApps;
    }
}

public class AzureEncryptionService
{
    /// <summary>
    /// Decryption using Azure.Security.KeyVault.Keys.Cryptography.CryptographyClient
    /// </summary>
    /// <param name="sEncryptedBytes">value to decrypt</param>
    /// <returns>Task<string></returns>
    public async Task<string> DecryptAsync(byte[] sEncryptedBytes)
    {
        return "Not yet implemented";
    }

}