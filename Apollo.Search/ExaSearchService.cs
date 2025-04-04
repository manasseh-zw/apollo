using System.Net.Http.Json;
using Apollo.Config;
using Apollo.Search.Models;
using Microsoft.Extensions.Logging;

namespace Apollo.Search;

public class ExaSearchService : ISearchService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExaSearchService>? _logger;
    private const string BaseUrl = "https://api.exa.ai";

    public ExaSearchService(
        HttpClient httpClient,
        ILogger<ExaSearchService>? logger = null,
        string? apiKey = null
    )
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey ?? AppConfig.ExaAI.ApiKey);
    }

    public async Task<SearchResponse> SearchAsync(SearchRequest request)
    {
        _logger?.LogInformation(
            "Sending search request: {Request}",
            System.Text.Json.JsonSerializer.Serialize(request)
        );

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
        var searchResponse =
            await response.Content.ReadFromJsonAsync<SearchResponse>()
            ?? throw new Exception("Failed to deserialize search response");

        _logger?.LogInformation(
            "Received search response: {Response}",
            System.Text.Json.JsonSerializer.Serialize(
                searchResponse,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
            )
        );

        return searchResponse;
    }

    public async Task<SearchResponse> FindSimilarAsync(string url, int numResults = 10)
    {
        _logger?.LogInformation("Sending find similar request for URL: {Url}", url);

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
        var searchResponse =
            await response.Content.ReadFromJsonAsync<SearchResponse>()
            ?? throw new Exception("Failed to deserialize find similar response");

        _logger?.LogInformation(
            "Received find similar response: {Response}",
            System.Text.Json.JsonSerializer.Serialize(
                searchResponse,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
            )
        );

        return searchResponse;
    }
}
