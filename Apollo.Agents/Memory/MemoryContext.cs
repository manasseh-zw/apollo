using Apollo.Config;
using Apollo.Crawler;
using Apollo.Search.Models;
using Microsoft.KernelMemory;

namespace Apollo.Agents.Memory;

public interface IMemoryContext
{
    Task CrawlAndIngest(WebCrawlRequest request);
    MemoryServerless GetMemoryContextInstance();
}

public class MemoryContext : IMemoryContext
{
    private readonly MemoryServerless _memory;

    public MemoryContext(ICrawlerService crawler)
    {
        _memory = new KernelMemoryBuilder()
            .WithAzureOpenAITextEmbeddingGeneration(
                new()
                {
                    Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                    APIKey = AppConfig.AzureAI.ApiKey,
                    APIType = AzureOpenAIConfig.APITypes.EmbeddingGeneration,
                    Deployment = AppConfig.Models.TextEmbeddingSmall,
                    Endpoint = AppConfig.AzureAI.Endpoint,
                }
            )
            .WithAzureOpenAITextGeneration(
                new()
                {
                    Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                    APIKey = AppConfig.AzureAI.ApiKey,
                    APIType = AzureOpenAIConfig.APITypes.TextCompletion,
                    Deployment = AppConfig.Models.Gpt4o,
                    Endpoint = AppConfig.AzureAI.Endpoint,
                }
            )
            .WithSimpleQueuesPipeline()
            .WithCustomWebScraper(new WebScraperService(crawler))
            .Build<MemoryServerless>();
    }

    public async Task CrawlAndIngest(WebCrawlRequest request)
    {
        var tags = new TagCollection
        {
            { "ResearchId", request.searchContext.ResearchId },
            { "ResearchQuestion", request.searchContext.ResearchQuestion },
            { "SearchQuery", request.searchContext.Query },
            { "Title", request.SearchResult.Title },
            { "PublicationDate", request.SearchResult.PublishedDate },
            { "Author", request.SearchResult.Author },
            { "Highlights", request.SearchResult.Highlights },
        };

        await _memory.ImportWebPageAsync(request.SearchResult.Url, tags: tags);
    }

    public MemoryServerless GetMemoryContextInstance()
    {
        return _memory;
    }
}

public record WebCrawlRequest(WebSearchContext searchContext, WebSearchResult SearchResult);

public record WebSearchContext(string ResearchId, string ResearchQuestion, string Query);
