using System.Net.Http.Json;
using Apollo.Config;

namespace Apollo.Crawler;

public class WebSpiderService : ICrawlerService
{
    private readonly HttpClient _httpClient;

    public WebSpiderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WebCrawlResponse> ScrapeAsync(string url)
    {
        var result = await _httpClient.PostAsJsonAsync(AppConfig.WebSpider.ScrapeUrl, new { url });

        var response =
            await result.Content.ReadFromJsonAsync<WebCrawlResponse>() ?? new() { Success = false };

        return response;
    }

    public async Task<WebCrawlBatchResponse> ScrapeBatchAsync(List<string> urls)
    {
        var result = await _httpClient.PostAsJsonAsync(
            AppConfig.WebSpider.ScrapeBatchUrl,
            new { urls }
        );

        var response =
            await result.Content.ReadFromJsonAsync<WebCrawlBatchResponse>()
            ?? new() { Success = false };

        return response;
    }
}
