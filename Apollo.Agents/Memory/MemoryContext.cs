using Apollo.Config;
using Apollo.Crawler;
using Apollo.Search.Models;
using Microsoft.KernelMemory;

namespace Apollo.Agents.Memory;

public interface IMemoryContext
{
    Task Ingest(string content, TagCollection tags);
    MemoryServerless GetMemoryContextInstance();
}

public class MemoryContext : IMemoryContext
{
    private readonly MemoryServerless _memory;

    public MemoryContext(ICrawlerService crawler)
    {
        _memory = new KernelMemoryBuilder()
            .WithPostgresMemoryDb(AppConfig.DatabaseOptions.VectorConnectionString)
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
            .WithCustomWebScraper(new WebScraperService(crawler))
            .Build<MemoryServerless>();
    }

    public async Task Ingest(string content, TagCollection tags)
    {
        await _memory.ImportTextAsync(content, tags: tags);
    }

    public MemoryServerless GetMemoryContextInstance()
    {
        return _memory;
    }
}
