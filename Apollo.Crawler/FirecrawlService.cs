using System.Text.Json;
using Apollo.Config;
using Firecrawl;
using Microsoft.Extensions.Logging;

namespace Apollo.Crawler;

public class FirecrawlService : ICrawlerService
{
    private readonly FirecrawlApp _client;

    public FirecrawlService()
    {
        _client = new FirecrawlApp(AppConfig.FirecrawlAI.ApiKey);
    }

    public async Task<MapResponse> MapAsync(MapUrlsRequest request)
    {
        var response = await _client.Mapping.MapUrlsAsync(request);
        return response;
    }

    public async Task<ScrapeResponse> ScrapeAsync(ScrapeAndExtractFromUrlRequest request)
    {
        var response = await _client.Scraping.ScrapeAndExtractFromUrlAsync(request);
        return response;
    }
}
