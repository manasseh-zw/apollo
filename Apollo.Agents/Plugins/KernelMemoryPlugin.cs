using System.ComponentModel;
using Apollo.Agents.Memory;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;

namespace Apollo.Agents.Plugins;

/// <summary>
/// Plugin providing functions to interact with Kernel Memory.
/// </summary>
public class KernelMemoryPlugin
{
    private readonly MemoryServerless _memory;

    public KernelMemoryPlugin(IMemoryContext memoryContext)
    {
        _memory = memoryContext.GetMemoryContextInstance();
    }

    [KernelFunction]
    [Description(
        "Searches the knowledge base (Kernel Memory) for relevant information based on a query."
    )]
    public async Task<SearchResult> SearchMemoryAsync(
        [Description("The search query")] string query,
        CancellationToken cancellationToken = default
    )
    {
        return await _memory.SearchAsync(query, cancellationToken: cancellationToken);
    }

    [KernelFunction]
    [Description(
        "Asks a question to the knowledge base (Kernel Memory) and gets a synthesized answer."
    )]
    public async Task<MemoryAnswer> AskMemoryAsync(
        [Description("The question to ask the memory")] string question,
        CancellationToken cancellationToken = default
    )
    {
        return await _memory.AskAsync(question, cancellationToken: cancellationToken);
    }
}
