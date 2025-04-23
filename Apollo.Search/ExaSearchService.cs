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

    public async Task<WebSearchResponse> SearchAsync(WebSearchRequest request)
    {
        _logger?.LogInformation(
            "Sending search request: {Request}",
            System.Text.Json.JsonSerializer.Serialize(request)
        );

        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/search", request);

        response.EnsureSuccessStatusCode();
        var searchResponse =
            await response.Content.ReadFromJsonAsync<WebSearchResponse>()
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

    public async Task<WebSearchResponse> FindSimilarAsync(string url, CommonRequest? options = null)
    {
        _logger?.LogInformation("Sending find similar request for URL: {Url}", url);

        var request = new
        {
            url,
            options?.NumResults,
            options?.IncludeDomains,
            options?.ExcludeDomains,
            options?.StartCrawlDate,
            options?.EndCrawlDate,
            options?.StartPublishedDate,
            options?.EndPublishedDate,
            options?.IncludeText,
            options?.ExcludeText,
            options?.Contents,
        };

        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/findSimilar", request);

        response.EnsureSuccessStatusCode();
        var searchResponse =
            await response.Content.ReadFromJsonAsync<WebSearchResponse>()
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

    public async Task<WebSearchResponse> GetContentsAsync(
        List<string> urls,
        ContentsRequest? options = null
    )
    {
        _logger?.LogInformation("Sending get contents request for URLs: {Urls}", urls);

        var request = new
        {
            urls,
            options?.Text,
            options?.Highlights,
            options?.Summary,
            options?.Livecrawl,
            options?.LivecrawlTimeout,
            options?.Subpages,
            options?.SubpageTarget,
            options?.Extras,
        };

        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/contents", request);

        response.EnsureSuccessStatusCode();
        var contentsResponse =
            await response.Content.ReadFromJsonAsync<WebSearchResponse>()
            ?? throw new Exception("Failed to deserialize contents response");

        _logger?.LogInformation(
            "Received contents response: {Response}",
            System.Text.Json.JsonSerializer.Serialize(
                contentsResponse,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
            )
        );

        return contentsResponse;
    }

    public async Task<AnswerResponse> AnswerAsync(AnswerRequest request)
    {
        _logger?.LogInformation(
            "Sending answer request: {Request}",
            System.Text.Json.JsonSerializer.Serialize(request)
        );

        if (request.Stream)
        {
            throw new NotImplementedException("Streaming is not yet implemented");
        }

        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/answer", request);

        response.EnsureSuccessStatusCode();
        var answerResponse =
            await response.Content.ReadFromJsonAsync<AnswerResponse>()
            ?? throw new Exception("Failed to deserialize answer response");

        _logger?.LogInformation(
            "Received answer response: {Response}",
            System.Text.Json.JsonSerializer.Serialize(
                answerResponse,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
            )
        );

        return answerResponse;
    }
}
