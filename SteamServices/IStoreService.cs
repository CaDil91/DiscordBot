namespace SteamServices;

public interface IStoreService
{
    public Task<List<SteamApp>> GetAppsAsync(string searchTerm = "", int iAppReturnCountMax = 0);
    public IEnumerable<AppNews.NewsItem> GetNewsForApp(int appId, DateTime? lastNewsCheckDate);
}