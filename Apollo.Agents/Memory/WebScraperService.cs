using Apollo.Crawler;
using Firecrawl;
using Microsoft.KernelMemory.DataFormats.WebPages;

namespace Apollo.Agents.Memory;

public class WebScraperService : IWebScraper
{
    private readonly ICrawlerService _crawler;

    public WebScraperService(ICrawlerService crawler)
    {
        _crawler = crawler;
    }

    public async Task<WebScraperResult> GetContentAsync(
        string url,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _crawler.ScrapeAsync(
            new()
            {
                Url = url,
                Formats = [ScrapeAndExtractFromUrlRequestFormat.Markdown],
                RemoveBase64Images = true,
                OnlyMainContent = true,
            }
        );

        if (!result.Success.HasValue || !result.Success.Value)
        {
            return new WebScraperResult()
            {
                Success = false,
                Error = $"Something went wrong, failed to scrape: {url}",
            };
        }

        return new WebScraperResult()
        {
            Content = BinaryData.FromString(result.Data.Markdown),
            ContentType = "text/markdown",
            Success = true,
        };
    }
}
