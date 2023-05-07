namespace SteamServices;

public class StoreService : IStoreService
{
    private readonly IAppRepository _steamAppRepository;

    /// <summary>
    /// TODO: Add documentation.
    /// </summary>
    /// <param name="steamAppRepository"></param>
    public StoreService(IAppRepository steamAppRepository)
    {
        _steamAppRepository = steamAppRepository;
    }

    /// <summary>
    ///     Search the Steam store by a given search term.
    /// </summary>
    /// <param name="searchTerm">The search parameter to query to the Steam Store by.</param>
    /// <param name="iAppReturnCountMax"></param>
    /// <returns>List of App Id's for the given search</returns>
    public async Task<List<SteamApp>> GetAppsAsync(string searchTerm = "", int iAppReturnCountMax = 3) => await _steamAppRepository.GetAsync(searchTerm, iAppReturnCountMax);

}