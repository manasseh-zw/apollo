using System.ComponentModel;
using Apollo.Agents.Contracts;
using Apollo.Agents.Helpers;
using Apollo.Agents.Memory;
using Apollo.Agents.Research;
using Apollo.Agents.State;
using Microsoft.SemanticKernel;

namespace Apollo.Agents.Plugins;

/// <summary>
/// Plugin providing functions to interact with Kernel Memory.
/// </summary>
public class KernelMemoryPlugin
{
    private readonly IMemoryContext _memory;
    private readonly IStateManager _state;
    private readonly IClientUpdateCallback _clientUpdate;

    public KernelMemoryPlugin(
        IMemoryContext memoryContext,
        IStateManager state,
        IClientUpdateCallback clientUpdate
    )
    {
        _memory = memoryContext;
        _state = state;
        _clientUpdate = clientUpdate;
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
        // Send initial message before starting analysis
        var startMessage = new AgentChatMessageEvent
        {
            ResearchId = researchId,
            Author = AgentFactory.ResearchAnalyzerAgentName,
            Message =
                "Okay Apollo, let me reflect on our findings so far and analyze the research data we've gathered...",
        };
        _state.AddChatMessage(researchId, startMessage);
        _clientUpdate.SendAgentChatMessage(startMessage);

        var response = await _memory.AskAsync(researchId, question, cancellationToken);

        return response.Result.ToLower();
    }
}
