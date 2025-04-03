using System.Net.Http.Json;
using Apollo.Config;
using Apollo.Crawler.Models;
using Microsoft.Extensions.Logging;

namespace Apollo.Crawler;

public class FirecrawlService : ICrawlerService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FirecrawlService>? _logger;
    private const string BaseUrl = "https://api.firecrawl.dev/v1";

    public FirecrawlService(
        HttpClient httpClient,
        ILogger<FirecrawlService>? logger = null,
        string? apiKey = null
    )
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey ?? AppConfig.FirecrawlAI.ApiKey}");
    }

    public async Task<ScrapeResponse> ScrapeAsync(ScrapeRequest request)
    {
        _logger?.LogInformation(
            "Sending scrape request: {Request}",
            System.Text.Json.JsonSerializer.Serialize(request)
        );

        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/scrape", request);

        response.EnsureSuccessStatusCode();
        var scrapeResponse =
            await response.Content.ReadFromJsonAsync<ScrapeResponse>()
            ?? throw new Exception("Failed to deserialize scrape response");

        _logger?.LogInformation(
            "Received scrape response: {Response}",
            System.Text.Json.JsonSerializer.Serialize(
                scrapeResponse,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
            )
        );

        return scrapeResponse;
    }

    public async Task<MapResponse> MapAsync(MapRequest request)
    {
        _logger?.LogInformation(
            "Sending map request: {Request}",
            System.Text.Json.JsonSerializer.Serialize(request)
        );

        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/map", request);

        response.EnsureSuccessStatusCode();
        var mapResponse =
            await response.Content.ReadFromJsonAsync<MapResponse>()
            ?? throw new Exception("Failed to deserialize map response");

        _logger?.LogInformation(
            "Received map response: {Response}",
            System.Text.Json.JsonSerializer.Serialize(
                mapResponse,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
            )
        );

        return mapResponse;
    }
}