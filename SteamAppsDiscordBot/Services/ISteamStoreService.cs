using SteamAppsDiscordBot.Services.DTO;

namespace SteamAppsDiscordBot.Services;

/// <summary>
/// Represents a service for retrieving Steam apps.
/// </summary>
public interface ISteamStoreService
{
    /// <summary>
    /// Retrieves a list of Steam apps asynchronously based on the provided search term and maximum return count.
    /// </summary>
    /// <param name="searchTerm">A string representing the search term to filter the apps.</param>
    /// <param name="iMaxReturnCount">An integer representing the maximum number of apps to be returned.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of SteamApp objects.</returns>
    Task<List<SteamApp>> GetAppsAsync(string searchTerm, int iMaxReturnCount);
}