namespace SteamServices;

public interface IStoreService
{
    public Task<List<SteamApp>> GetAppsAsync(string searchTerm = "", int iAppReturnCountMax = 0);
}