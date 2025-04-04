using Apollo.Search.Models;

namespace Apollo.Search;

public interface ISearchService
{
    Task<SearchResponse> SearchAsync(SearchRequest request);
    Task<SearchResponse> FindSimilarAsync(string url, int numResults = 10);
}
