using Apollo.Search.Models;

namespace Apollo.Search;

public interface ISearchService
{
    Task<WebSearchResponse> SearchAsync(WebSearchRequest request);
    Task<WebSearchResponse> FindSimilarAsync(string url, CommonRequest? options = null);
    Task<WebSearchResponse> GetContentsAsync(List<string> urls, ContentsRequest? options = null);
    Task<AnswerResponse> AnswerAsync(AnswerRequest request);
}
