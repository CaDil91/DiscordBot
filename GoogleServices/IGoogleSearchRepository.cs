namespace GoogleService;

public interface IGoogleSearchRepository
{
    public Task<List<string>> GetCustomSearchResultsAsync(string sQuery, int iCount = 10);
}