using Newtonsoft.Json;

namespace SteamAppsDiscordBot.Services;

public interface IGoogleSearchRepository
{
    /// <summary>
    /// Retrieves custom search results from Google asynchronously.
    /// </summary>
    /// <param name="sQuery">The search query string.</param>
    /// <param name="iCount">The number of search results to retrieve. Default is 10.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="HttpRequestException"></exception>
    /// <exception cref="JsonException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    /// <returns>Can return empty results</returns>
    public Task<List<Uri>> GetCustomSearchResultsAsync(string sQuery, int iCount = 10);
}