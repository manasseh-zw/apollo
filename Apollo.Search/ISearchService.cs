using Apollo.Search.Models;

namespace Apollo.Search;

public interface ISearchService
{
    Task<WebSearchResponse> SearchAsync(WebSearchRequest request);
    Task<WebSearchResponse> FindSimilarAsync(string url, int numResults = 10);
}
