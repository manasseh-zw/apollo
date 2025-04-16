namespace Apollo.Agents.Research;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;

#pragma warning disable SKEXP0110 // Experimental AgentGroupChat
#pragma warning disable SKEXP0001 // Experimental Agent

// Custom Selection Strategy
public class ResearchSelectionStrategy : SelectionStrategy
{
    private readonly IResearchManager _researchManager;

    // Inject your manager here
    public ResearchSelectionStrategy(IResearchManager researchManager)
    {
        _researchManager = researchManager;
    }

    // Override the method the chat loop actually calls

    protected override Task<Agent> SelectAgentAsync(
        IReadOnlyList<Agent> agents,
        IReadOnlyList<ChatMessageContent> history,
        CancellationToken cancellationToken = default
    )
    {
        Agent? nextAgent = _researchManager.SelectNextAgent(agents, history);
        return Task.FromResult(nextAgent);
    }
}

// Custom Termination Strategy
public class ResearchTerminationStrategy : TerminationStrategy
{
    private readonly IResearchManager _researchManager;

    // Inject your manager here
    public ResearchTerminationStrategy(IResearchManager researchManager)
    {
        _researchManager = researchManager;
    }

    // Override the method the chat loop actually calls
    protected override Task<bool> ShouldAgentTerminateAsync(
        Agent agent,
        IReadOnlyList<ChatMessageContent> history,
        CancellationToken cancellationToken
    )
    {
        // Directly call your manager's logic
        bool shouldTerminate = _researchManager.CheckTermination(agent, history);
        return Task.FromResult(shouldTerminate); // Return as Task<bool>
    }
}
