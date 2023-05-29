namespace SteamServices;

public interface IAppRepository
{
    public Task<List<SteamApp>> GetAsync(string searchTerm = "", int iMaxReturnCount = 3);
}