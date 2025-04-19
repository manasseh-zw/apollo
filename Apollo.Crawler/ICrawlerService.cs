using Firecrawl;

namespace Apollo.Crawler;

public interface ICrawlerService
{
    Task<WebCrawlResponse> ScrapeAsync(string url);
    Task<WebCrawlBatchResponse> ScrapeBatchAsync(List<string> urls);
}
