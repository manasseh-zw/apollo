using Apollo.Config;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.Postgres;
using Microsoft.SemanticKernel.Embeddings;
using Npgsql;

namespace Apollo.Agents.Memory;

#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0010

public interface IVectorStoreService
{
    Task IngestData(string collectionName, List<ResearchChunk> chunks);
    Task<List<ResearchChunk>> SearchSimilar(
        string collectionName,
        string searchText,
        int limit = 5
    );
}

public class VectorStoreService : IVectorStoreService
{
    private readonly IVectorStore _vectorStore;
    private readonly ITextEmbeddingGenerationService _embeddingGenerator;

    public VectorStoreService()
    {
        _embeddingGenerator = new AzureOpenAITextEmbeddingGenerationService(
            deploymentName: "text-embedding-3-small",
            endpoint: AppConfig.AzureAI.Endpoint,
            apiKey: AppConfig.AzureAI.ApiKey
        );

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(
            AppConfig.DatabaseOptions.VectorConnectionString
        );

        dataSourceBuilder.UseVector();

        var dataSource = dataSourceBuilder.Build();

        _vectorStore = new PostgresVectorStore(dataSource);
    }

    public async Task IngestData(string collectionName, List<ResearchChunk> chunks)
    {
        var collection = _vectorStore.GetCollection<string, ResearchChunk>(collectionName);
        await collection.CreateCollectionIfNotExistsAsync();

        // Generate embeddings for all chunks
        var tasks = chunks.Select(chunk =>
            Task.Run(async () =>
            {
                chunk.ContentVector = await _embeddingGenerator.GenerateEmbeddingAsync(
                    chunk.Content
                );
            })
        );
        await Task.WhenAll(tasks);

        // Upsert all chunks
        var upsertTasks = chunks.Select(x => collection.UpsertAsync(x));
        await Task.WhenAll(upsertTasks);
    }

    public async Task<List<ResearchChunk>> SearchSimilar(
        string collectionName,
        string searchText,
        int limit = 5
    )
    {
        var collection = _vectorStore.GetCollection<string, ResearchChunk>(collectionName);
        var searchVector = await _embeddingGenerator.GenerateEmbeddingAsync(searchText);

        var asyncSearchResult = await collection.VectorizedSearchAsync(
            searchVector,
            new() { Top = limit }
        );

        var result = new List<ResearchChunk>();

        await foreach (var chunk in asyncSearchResult.Results)
        {
            result.Add(chunk.Record);
        }

        return result;
    }
}

public class ResearchChunk
{
    [VectorStoreRecordKey]
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    [VectorStoreRecordData(IsFilterable = true)]
    public string ResearchId { get; set; } = string.Empty;

    [VectorStoreRecordData(IsFilterable = true)]
    public string SourceUrl { get; set; } = string.Empty;

    [VectorStoreRecordData]
    public string OriginalQuestion { get; set; } = string.Empty;

    [VectorStoreRecordData]
    public string Content { get; set; } = string.Empty;

    [VectorStoreRecordVector(Dimensions: 1536)]
    public ReadOnlyMemory<float>? ContentVector { get; set; }

    public override string ToString() => $"[{SourceUrl}] {Content}";
}

public record SaveResearchChunkDto(string SourceUrl, string OriginalQuestion, string Content);
