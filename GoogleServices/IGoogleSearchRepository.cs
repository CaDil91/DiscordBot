namespace GoogleService;

public interface IGoogleSearchRepository
{
    public Task<List<Uri>> GetCustomSearchResultsAsync(string sQuery, int iCount = 10);
}