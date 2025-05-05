using System.ComponentModel;
using Apollo.Agents.Memory;
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
        "Asks a question to the knowledge base (Kernel Memory) and gets a synthesized answer. Use this to get an internal critic of any significant knowledge gaps and a possible table of contents."
    )]
    public async Task<string> AskMemoryAsync(
        [Description("The researchId")] string researchId,
        [Description(
            "The SINGLE self reflective question to ask the knowledge base to get knowledge gap analysis results and a draft toc."
        )]
            string question,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _memory.AskAsync(researchId, question, cancellationToken);

        return response.Result.ToLower();
    }
}
