using Firecrawl;

namespace Apollo.Crawler;

public interface ICrawlerService
{
    Task<ScrapeResponse> ScrapeAsync(ScrapeAndExtractFromUrlRequest request);
    Task<MapResponse> MapAsync(MapUrlsRequest request);
}
