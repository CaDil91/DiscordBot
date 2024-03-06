using Newtonsoft.Json;

namespace SteamAppsDiscordBot.Services;

/// <summary>
/// Represents an interface for retrieving search results from Google asynchronously.
/// </summary>
public interface IGoogleSearchService
{
    /// <summary>
    /// Retrieves custom search results from Google asynchronously.
    /// </summary>
    /// <param name="searchTerm">The search query string.</param>
    /// <param name="iCount">The number of search results to retrieve. Default is 10.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="HttpRequestException"></exception>
    /// <exception cref="JsonException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    /// <returns>Can return empty results</returns>
    public Task<List<Uri>> SearchAsync(string searchTerm, int iCount = 10);
}