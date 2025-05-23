using System.Collections.Concurrent;
using Apollo.Config;
using Microsoft.KernelMemory;
using OpenAI.Chat;

namespace Apollo.Agents.Memory;

public interface IMemoryContext
{
    Task Ingest(string researchId, string content, TagCollection tags);
    Task<SearchResult> SearchAsync(
        string researchId,
        string query,
        CancellationToken cancellationToken = default
    );
    Task<MemoryAnswer> AskAsync(
        string researchId,
        string question,
        CancellationToken cancellationToken = default
    );

    Task DeleteIndex(string researchId, CancellationToken cancellationToken = default);
    bool IsIngestionInProgress(string researchId);
    void SetIngestionInProgress(string researchId, bool inProgress);
}

public class MemoryContext : IMemoryContext
{
    private readonly MemoryServerless _memory;
    private readonly ConcurrentDictionary<string, bool> _activeIngestions = new();

    public MemoryContext()
    {
        _memory = new KernelMemoryBuilder()
            // .WithPostgresMemoryDb(AppConfig.DatabaseOptions.VectorConnectionString)
            .WithSimpleVectorDb()
            .WithSimpleQueuesPipeline()
            .With(new KernelMemoryConfig { DefaultIndexName = "apollo" })
            .WithAzureOpenAITextEmbeddingGeneration(
                new()
                {
                    Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                    APIKey = AppConfig.AzureAI.ApiKey,
                    APIType = AzureOpenAIConfig.APITypes.EmbeddingGeneration,
                    Deployment = AppConfig.Models.TextEmbeddingSmall,
                    Endpoint = AppConfig.AzureAI.Endpoint,
                    EmbeddingDimensions = 1536,
                }
            )
            .WithAzureOpenAITextGeneration(
                new()
                {
                    Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                    APIKey = AppConfig.AzureAI.ApiKey,
                    APIType = AzureOpenAIConfig.APITypes.TextCompletion,
                    Deployment = AppConfig.Models.Gpt41,
                    Endpoint = AppConfig.AzureAI.Endpoint,
                    MaxTokenTotal = 1047576,
                }
            )
            .WithSearchClientConfig(new() { AnswerTokens = 16768, MaxAskPromptSize = 32768 })
            .WithStructRagSearchClient()
            .Build<MemoryServerless>(
                //this is fine because i am not storing any documents at the moment
                new() { AllowMixingVolatileAndPersistentData = true }
            );
    }

    public async Task Ingest(string researchId, string content, TagCollection tags)
    {
        await _memory.ImportTextAsync(content, tags: tags, index: researchId);
    }

    public async Task<SearchResult> SearchAsync(
        string researchId,
        string query,
        CancellationToken cancellationToken = default
    )
    {
        return await _memory.SearchAsync(
            query,
            // filter: MemoryFilters.ByTag("researchId", researchId),
            index: researchId,
            cancellationToken: cancellationToken
        );
    }

    public async Task<MemoryAnswer> AskAsync(
        string researchId,
        string question,
        CancellationToken cancellationToken = default
    )
    {
        return await _memory.AskAsync(
            question,
            // filter: MemoryFilters.ByTag("researchId", researchId),
            index: researchId,
            cancellationToken: cancellationToken
        );
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

    public async Task DeleteIndex(string researchId, CancellationToken cancellationToken = default)
    {
        await _memory.DeleteIndexAsync(researchId, cancellationToken);
    }
}
