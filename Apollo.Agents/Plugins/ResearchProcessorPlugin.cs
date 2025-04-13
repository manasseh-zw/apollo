using System.ComponentModel;
using Apollo.Agents.Helpers;
using Apollo.Agents.Memory;
using Apollo.Agents.State;
using Apollo.Search;
using Apollo.Search.Models;
using Microsoft.SemanticKernel;

namespace Apollo.Agents.Plugins;

#pragma warning disable KMEXP01
public class ResearchProcessorPlugin
{
    private readonly IMemoryContext _memory;
    private readonly IStateManager _state;
    private readonly ISearchService _search;
    private readonly IChatStreamingCallback _streamingCallback;

    public ResearchProcessorPlugin(
        ISearchService search,
        IMemoryContext memory,
        IStateManager state,
        IChatStreamingCallback streamingCallback
    )
    {
        _memory = memory;
        _state = state;
        _search = search;
        _streamingCallback = streamingCallback;
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
            _streamingCallback.SendCrawlProgressUpdate(researchId, $"Searching web for: {query}");
            var searchResponse = await PerformWebSearch(query);

            foreach (var result in searchResponse.Results)
            {
                if (crawledUrlSet.Contains(result.Url))
                {
                    _streamingCallback.SendCrawlProgressUpdate(
                        researchId,
                        $"Skipping already processed URL: {result.Url}"
                    );
                    continue;
                }

                _streamingCallback.SendCrawlProgressUpdate(
                    researchId,
                    $"Processing: {result.Title} from {result.Url}"
                );

                await _memory.CrawlAndIngest(
                    new WebCrawlRequest(
                        searchContext: new WebSearchContext(
                            ResearchId: researchId,
                            ResearchQuestion: question.Text,
                            Query: query
                        ),
                        SearchResult: result
                    )
                );

                crawledUrlSet.Add(result.Url);
                _streamingCallback.SendCrawlProgressUpdate(
                    researchId,
                    $"Successfully processed: {result.Title}"
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
