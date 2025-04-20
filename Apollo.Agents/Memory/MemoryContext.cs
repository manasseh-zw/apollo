using System.Collections.Concurrent;
using Apollo.Config;
using Apollo.Crawler;
using Apollo.Search.Models;
using Microsoft.KernelMemory;

namespace Apollo.Agents.Memory;

public interface IMemoryContext
{
    Task Ingest(string content, TagCollection tags);
    Task<SearchResult> SearchAsync(string query, CancellationToken cancellationToken = default);
    Task<MemoryAnswer> AskAsync(string question, CancellationToken cancellationToken = default);
    bool IsIngestionInProgress(string researchId);
    void SetIngestionInProgress(string researchId, bool inProgress);
}

public class MemoryContext : IMemoryContext
{
    private readonly MemoryServerless _memory;
    private readonly ConcurrentDictionary<string, bool> _activeIngestions = new();

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

    public async Task<SearchResult> SearchAsync(
        string query,
        CancellationToken cancellationToken = default
    )
    {
        return await _memory.SearchAsync(query, cancellationToken: cancellationToken);
    }

    public async Task<MemoryAnswer> AskAsync(
        string question,
        CancellationToken cancellationToken = default
    )
    {
        return await _memory.AskAsync(question, cancellationToken: cancellationToken);
    }

    public bool IsIngestionInProgress(string researchId)
    {
        return _activeIngestions.GetValueOrDefault(researchId, false);
    }

    public void SetIngestionInProgress(string researchId, bool inProgress)
    {
        if (inProgress)
            _activeIngestions.TryAdd(researchId, true);
        else
            _activeIngestions.TryRemove(researchId, out _);
    }
}
