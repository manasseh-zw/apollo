using Apollo.Agents.Memory;
using Apollo.Search.Models;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;

namespace Apollo.Agents.Events;

public record IngestEvent(Guid ResearchId, List<WebSearchResult> SearchResults);

public interface IIngestEventHandler
{
    Task HandleIngestAsync(IngestEvent @event);
}

public class IngestEventHandler : IIngestEventHandler
{
    private readonly IMemoryContext _memory;
    private readonly ILogger<IngestEventHandler> _logger;

    public IngestEventHandler(IMemoryContext memory, ILogger<IngestEventHandler> logger)
    {
        _memory = memory;
        _logger = logger;
    }

    public async Task HandleIngestAsync(IngestEvent @event)
    {
        var researchId = @event.ResearchId.ToString();
        _memory.SetIngestionInProgress(researchId, true);

        try
        {
            foreach (var result in @event.SearchResults)
            {
                var tags = new TagCollection
                {
                    { "researchId", @event.ResearchId.ToString() },
                    { "url", result.Url },
                    { "title", result.Title },
                    { "author", result.Author },
                    { "published", result.PublishedDate },
                };

                await _memory.Ingest(researchId, result.Text, tags);
            }
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
        finally
        {
            _memory.SetIngestionInProgress(researchId, false);
        }
    }
}
