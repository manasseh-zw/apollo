using System.ComponentModel;
using Apollo.Agents.Events;
using Apollo.Agents.Helpers;
using Apollo.Agents.Memory;
using Apollo.Agents.State;
using Apollo.Search;
using Apollo.Search.Models;
using Microsoft.SemanticKernel;

namespace Apollo.Agents.Plugins;

#pragma warning disable KMEXP01
public class ResearchEnginePlugin
{
    private readonly IMemoryContext _memory;
    private readonly IStateManager _state;
    private readonly ISearchService _search;
    private readonly IClientUpdateCallback _clientUpdate;
    private readonly IIngestEventsQueue _ingestQueue;

    public ResearchEnginePlugin(
        ISearchService search,
        IMemoryContext memory,
        IStateManager state,
        IClientUpdateCallback streamingCallback,
        IIngestEventsQueue ingestQueue
    )
    {
        _memory = memory;
        _state = state;
        _search = search;
        _clientUpdate = streamingCallback;
        _ingestQueue = ingestQueue;
    }

    [KernelFunction]
    [Description(
        "Processes a list of research queries by performing web searches, filtering out already visited URLs, and ingesting new content into memory."
    )]
    public async Task<bool> ProcessResarchQueries(
        [Description("The unique identifier for the research project")] string researchId,
        [Description("A list of search queries to process for the research")] List<string> queries
    )
    {
        var state = _state.GetState(researchId);
        var question = state.GetActiveQuestion();
        var crawledUrls = state.CrawledUrls;
        var crawledUrlSet = new HashSet<string>(crawledUrls, StringComparer.OrdinalIgnoreCase);

        foreach (var query in queries)
        {
            _clientUpdate.SendResearchProgressUpdate(researchId, $"Searching web for: {query}");
            var searchResponse = await PerformWebSearch(query);

            var newResults = searchResponse
                .Results.Where(result => !crawledUrlSet.Contains(result.Url))
                .ToList();

            if (newResults.Count > 0)
            {
                _clientUpdate.SendResearchProgressUpdate(
                    researchId,
                    $"Processing {newResults.Count} new results for query: {query}"
                );

                await _ingestQueue.Writer.WriteAsync(
                    new IngestEvent(Guid.Parse(researchId), newResults)
                );

                foreach (var result in newResults)
                {
                    crawledUrlSet.Add(result.Url);
                }
                _state.UpdateState(
                    researchId,
                    (s) =>
                    {
                        s.CrawledUrls.AddRange(crawledUrlSet);
                    }
                );
                crawledUrlSet.Clear();
            }
            else
            {
                _clientUpdate.SendResearchProgressUpdate(
                    researchId,
                    $"No new URLs to process for query: {query}"
                );
            }
        }

        return true;
    }

    private async Task<WebSearchResponse> PerformWebSearch(string query)
    {
        var result = await _search.SearchAsync(new WebSearchRequest { Query = query });
        return result;
    }
}
