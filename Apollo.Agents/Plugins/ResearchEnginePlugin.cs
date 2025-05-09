using System.ComponentModel;
using Apollo.Agents.Contracts;
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
        IClientUpdateCallback clientUpdate,
        IIngestEventsQueue ingestQueue
    )
    {
        _memory = memory;
        _state = state;
        _search = search;
        _clientUpdate = clientUpdate;
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
            // Send searching update
            var searchingUpdate = new WebSearchFeedUpdate
            {
                ResearchId = researchId,
                Type = ResearchFeedUpdateType.Searching,
                Query = query,
            };
            _state.AddFeedUpdate(researchId, searchingUpdate);
            _clientUpdate.SendResearchFeedUpdate(searchingUpdate);

            var searchResponse = await PerformWebSearch(query);
            var newResults = searchResponse
                .Results.Where(result => !crawledUrlSet.Contains(result.Url))
                .ToList();

            if (newResults.Count > 0)
            {
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

                // Now send individual results with delay
                foreach (var result in newResults)
                {
                    var searchResultItem = new SearchResultItemContract
                    {
                        Id = Guid.NewGuid().ToString(),
                        Icon = result.Favicon ?? "https://www.google.com/favicon.ico",
                        Title = result.Title,
                        Url = result.Url,
                        Snippet = result.Summary ?? null,
                        Highlights = result.Highlights ?? [],
                    };

                    var searchResultsUpdate = new SearchResultsFeedUpdate
                    {
                        ResearchId = researchId,
                        Type = ResearchFeedUpdateType.SearchResults,
                        Results = [searchResultItem],
                    };
                    _state.AddFeedUpdate(researchId, searchResultsUpdate);
                    _clientUpdate.SendResearchFeedUpdate(searchResultsUpdate);
                    await Task.Delay(1000);
                }
            }
            else
            {
                var noResultsUpdate = new ProgressMessageFeedUpdate
                {
                    ResearchId = researchId,
                    Type = ResearchFeedUpdateType.Message,
                    Message = $"No new results found for query: {query}",
                };
                _state.AddFeedUpdate(researchId, noResultsUpdate);
                _clientUpdate.SendResearchFeedUpdate(noResultsUpdate);
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
