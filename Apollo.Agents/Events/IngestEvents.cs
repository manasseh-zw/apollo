using Apollo.Agents.Memory;
using Apollo.Crawler;
using Apollo.Search.Models;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;

namespace Apollo.Agents.Events;

public record IngestEvent(Guid ResearchId, IngestRequest Request);

public interface IIngestEventHandler
{
    Task HandleIngest(IngestEvent @event);
}

public class IngestEventHandler : IIngestEventHandler
{
    private readonly IMemoryContext _memory;
    private readonly ICrawlerService _crawler;
    private readonly ILogger<IngestEventHandler> _logger;

    public IngestEventHandler(
        IMemoryContext memory,
        ICrawlerService crawler,
        ILogger<IngestEventHandler> logger
    )
    {
        _memory = memory;
        _crawler = crawler;
        _logger = logger;
    }

    public async Task HandleIngest(IngestEvent @event)
    {
        try
        {
            var urls = @event.Request.SearchResults.Select(r => r.Url).ToList();

            var batchResponse = await _crawler.ScrapeBatchAsync(urls);

            if (!batchResponse.Success)
            {
                _logger.LogError(
                    "Failed to crawl batch of URLs for research {ResearchId}",
                    @event.ResearchId
                );
                return;
            }

            // Process results in parallel
            var tasks = batchResponse.Results.Select(async result =>
            {
                if (!result.Success)
                    return;

                var searchResult = @event.Request.SearchResults.First(r => r.Url == result.Url);
                var tags = new TagCollection
                {
                    { "researchId", @event.ResearchId.ToString() },
                    { "question", @event.Request.SearchContext.ResearchQuestion },
                    { "query", @event.Request.SearchContext.Query },
                    { "url", result.Url },
                    { "title", searchResult.Title },
                };

                await _memory.Ingest(result.Content, tags);
            });

            await Task.WhenAll(tasks);
            _logger.LogInformation(
                "Successfully processed batch ingest for research {ResearchId}",
                @event.ResearchId
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing ingest event for research {ResearchId}",
                @event.ResearchId
            );
            throw;
        }
    }
}
