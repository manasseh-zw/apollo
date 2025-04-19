using Apollo.Config;
using Firecrawl;

namespace Apollo.Crawler;

public class FirecrawlService : ICrawlerService
{
    private readonly FirecrawlApp _client;

    public FirecrawlService()
    {
        _client = new FirecrawlApp(AppConfig.FirecrawlAI.ApiKey);
    }

    public async Task<WebCrawlResponse> ScrapeAsync(string url)
    {
        var response = await _client.Scraping.ScrapeAndExtractFromUrlAsync(
            new ScrapeAndExtractFromUrlRequest { Url = url }
        );

        var webCrawlResponse = new WebCrawlResponse
        {
            Success = response.Success.GetValueOrDefault(),
            Content = response.Data?.Markdown ?? "No content for this source",
            Error = response.Success.GetValueOrDefault() ? string.Empty : "Unknown error occurred",
        };
        return webCrawlResponse;
    }

    public Task<WebCrawlBatchResponse> ScrapeBatchAsync(List<string> urls)
    {
        throw new NotImplementedException();
    }
}
