namespace GoogleService;

public class SteamStoreRepository : IGoogleSearchRepository
{
    public Task<List<string>> GetCustomSearchResultsAsync(string sQuery, int iCount = 10)
    {
        throw new NotImplementedException();
    }
}