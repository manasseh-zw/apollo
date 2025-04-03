using Apollo.Crawler.Models;

namespace Apollo.Crawler;

public interface ICrawlerService
{
    Task<ScrapeResponse> ScrapeAsync(ScrapeRequest request);
    Task<MapResponse> MapAsync(MapRequest request);
}