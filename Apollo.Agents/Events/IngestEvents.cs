using Apollo.Agents.Memory;
using Apollo.Search.Models;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;

namespace Apollo.Agents.Events;

public record IngestEvent(Guid ResearchId, List<WebSearchResult> SearchResults);

public interface IIngestEventHandler
{
    void HandleIngest(IngestEvent @event);
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

    public void HandleIngest(IngestEvent @event)
    {
        var researchId = @event.ResearchId.ToString();
        _memory.SetIngestionInProgress(researchId, true);

        try
        {
            @event.SearchResults.ForEach(async sr =>
            {
                var tags = new TagCollection
                {
                    { "researchId", @event.ResearchId.ToString() },
                    { "url", sr.Url },
                    { "title", sr.Title },
                    { "author", sr.Author },
                    { "published", sr.PublishedDate },
                };

                await _memory.Ingest(sr.Text, tags);
            });
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
