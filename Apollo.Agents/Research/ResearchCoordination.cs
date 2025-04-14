using Apollo.Agents.State;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Apollo.Agents.Research;

#pragma warning disable SKEXP0110
#pragma warning disable SKEXP0001

public class ResearchCoordination
{
    private readonly IStateManager _state;
    private readonly ILogger<ResearchCoordination> _logger;
    private readonly string _researchId;
    private readonly Dictionary<string, Agent> _agents;

    public ResearchCoordination(
        IStateManager state,
        ILogger<ResearchCoordination> logger,
        string researchId,
        Dictionary<string, Agent> agents
    )
    {
        _state = state;
        _logger = logger;
        _researchId = researchId;
        _agents = agents;
    }

    public Agent? SelectNextAgent(AgentGroupChat chat, IReadOnlyList<ChatMessageContent> history)
    {
        var lastMessage = history.LastOrDefault(m => m.Role == AuthorRole.Assistant);
        var lastAgentName = lastMessage?.AuthorName;

        var state = _state.GetState(_researchId);
        if (state == null || state.IsComplete)
        {
            _logger.LogWarning(
                "[{ResearchId}] Selection: State not found or research complete. Terminating selection.",
                _researchId
            );
            return null;
        }

        _logger.LogInformation(
            "[{ResearchId}] Selection: Last agent was '{LastAgentName}'. Evaluating next step.",
            _researchId,
            lastAgentName ?? "N/A"
        );

        if (
            string.IsNullOrEmpty(lastAgentName)
            || lastAgentName.Equals(
                AgentFactory.ResearchCoordinatorAgentName,
                StringComparison.OrdinalIgnoreCase
            )
            || lastAgentName.Equals(
                AgentFactory.ResearchAnalyzerAgentName,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            if (state.NeedsAnalysis)
            {
                _logger.LogInformation(
                    "[{ResearchId}] Selection: Needs analysis. Selecting ResearchAnalyzer.",
                    _researchId
                );

                return _agents[AgentFactory.ResearchAnalyzerAgentName];
            }

            // Check if there are pending questions *after* potentially setting the active one
            if (string.IsNullOrEmpty(state.ActiveQuestionId))
            {
                _state.SetNextPendingQuestionAsActive(_researchId);
                state = _state.GetState(_researchId); // Re-fetch state
            }

            // If no pending questions and analysis not needed (Analyzer decided no gaps or already done)
            if (
                !state.NeedsAnalysis
                && !state.PendingResearchQuestions.Any(q => !q.IsProcessed)
                && !state.SynthesisComplete
            )
            {
                _logger.LogInformation(
                    "[{ResearchId}] Selection: No pending questions, analysis done/not needed. Selecting ReportSynthesizer.",
                    _researchId
                );
                return _agents[AgentFactory.ReportSynthesizerAgentName];
            }
        }
        else if (
            lastAgentName.Equals(
                AgentFactory.ResearchEngineAgentName,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            // ResearchEngine calls MarkActiveQuestionComplete. Coordinator takes over.
            _logger.LogInformation(
                "[{ResearchId}] Selection: After ResearchEngine. Selecting ResearchCoordinator.",
                _researchId
            );
            return _agents[AgentFactory.ResearchCoordinatorAgentName];
        }
        else if (
            lastAgentName.Equals(
                AgentFactory.ReportSynthesizerAgentName,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            // Synthesizer marks completion. End of flow.
            _logger.LogInformation(
                "[{ResearchId}] Selection: After ReportSynthesizer. Research complete. No agent selected.",
                _researchId
            );
            return null;
        }
        // Default fallback
        _logger.LogWarning(
            "[{ResearchId}] Selection: Unexpected state or last agent '{LastAgentName}'. Defaulting to ResearchCoordinator.",
            _researchId,
            lastAgentName
        );
        return _agents[AgentFactory.ResearchCoordinatorAgentName];
    }

    public bool CheckTermination(AgentGroupChat chat, IReadOnlyList<ChatMessageContent> history)
    {
        var state = _state.GetState(_researchId);
        bool shouldTerminate = state?.IsComplete ?? false;
        if (shouldTerminate)
        {
            _logger.LogInformation(
                "[{ResearchId}] Termination check: Research marked as complete. Terminating.",
                _researchId
            );
        }
        else
        {
            _logger.LogDebug(
                "[{ResearchId}] Termination check: Research not complete. Continuing.",
                _researchId
            );
        }
        return shouldTerminate;
    }
}
