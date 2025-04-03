using System.Net.Http.Json;
using Apollo.Config;

namespace Apollo.Search;

public class ExaSearchService : ISearchService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.exa.ai";

    public ExaSearchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("x-api-key", AppConfig.ExaAI.ApiKey);
    }

    public async Task<SearchResponse> SearchAsync(SearchRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{BaseUrl}/search",
            new
            {
                query = request.Query,
                useAutoprompt = request.UseAutoprompt,
                type = request.Type,
                category = request.Category,
                numResults = request.NumResults,
                contents = new { text = request.IncludeText },
            }
        );

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SearchResponse>()
            ?? throw new Exception("Failed to deserialize search response");
    }

    public async Task<SearchResponse> FindSimilarAsync(string url, int numResults = 10)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{BaseUrl}/findSimilar",
            new
            {
                url = url,
                numResults = numResults,
                contents = new { text = true },
            }
        );

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SearchResponse>()
            ?? throw new Exception("Failed to deserialize find similar response");
    }
}
