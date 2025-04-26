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
    private readonly IMemoryContext _memory;

    public KernelMemoryPlugin(IMemoryContext memoryContext)
    {
        _memory = memoryContext;
    }

    [KernelFunction]
    [Description(
        "Asks a question to the knowledge base (Kernel Memory) and gets a synthesized answer. Use this to get brief summaries or key findings about specific aspects of the research."
    )]
    public async Task<MemoryAnswer> AskMemoryAsync(
        [Description("The researchId")] string researchId,
        [Description(
            "The question to ask the memory. Frame questions to request brief summaries or key findings."
        )]
            string question,
        CancellationToken cancellationToken = default
    )
    {
        return await _memory.AskAsync(researchId, question, cancellationToken);
    }
}
