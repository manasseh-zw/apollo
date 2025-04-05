using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;

namespace Apollo.Agents.Memory;

public class VectorRepository
{
    private readonly InMemoryVectorStoreRecordCollection<string, ResearchChunk> _vectorStore;

    public VectorRepository()
    {
        _vectorStore = new InMemoryVectorStoreRecordCollection<string, ResearchChunk>(
            "research_data"
        );
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

    [VectorStoreRecordVector(Dimensions: 1024)]
    public ReadOnlyMemory<float>? ContentVector { get; set; }

    public override string ToString() => $"[{SourceUrl}] {Content}";
}
