using Apollo.Agents.State;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Apollo.Agents.Research;

#pragma warning disable SKEXP0110
#pragma warning disable SKEXP0001

public interface IResearchManager
{
    void Initialize(string researchId);
    Agent? SelectNextAgent(IReadOnlyList<Agent> agents, IReadOnlyList<ChatMessageContent> history);
    bool CheckTermination(Agent agent, IReadOnlyList<ChatMessageContent> history);
}

public class ResearchManager : IResearchManager
{
    private readonly IStateManager _state;
    private readonly ILogger<ResearchManager> _logger;
    private string? _researchId;

    public ResearchManager(IStateManager state, ILogger<ResearchManager> logger)
    {
        _state = state;
        _logger = logger;
    }

    public void Initialize(string researchId)
    {
        _researchId = researchId;
    }

    public Agent? SelectNextAgent(
        IReadOnlyList<Agent> agents,
        IReadOnlyList<ChatMessageContent> history
    )
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

        // If analysis is in progress, keep the Analyzer working
        if (state.IsAnalyzing)
        {
            _logger.LogInformation(
                "[{ResearchId}] Selection: Analysis in progress. Continuing with ResearchAnalyzer.",
                _researchId
            );
            return agents.FirstOrDefault(a => a.Id == AgentFactory.ResearchAnalyzerAgentName);
        }

        // After ResearchAnalyzer, always go back to Coordinator
        if (
            lastAgentName?.Equals(
                AgentFactory.ResearchAnalyzerAgentName,
                StringComparison.OrdinalIgnoreCase
            ) ?? false
        )
        {
            _logger.LogInformation(
                "[{ResearchId}] Selection: After Analyzer, returning to Coordinator for next step evaluation.",
                _researchId
            );
            return agents.FirstOrDefault(a => a.Id == AgentFactory.ResearchCoordinatorAgentName);
        }

        // Handle initial state or after Coordinator
        if (
            string.IsNullOrEmpty(lastAgentName)
            || lastAgentName.Equals(
                AgentFactory.ResearchCoordinatorAgentName,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            // If analysis is needed, prioritize it
            if (state.NeedsAnalysis)
            {
                _logger.LogInformation(
                    "[{ResearchId}] Selection: Needs analysis. Selecting ResearchAnalyzer.",
                    _researchId
                );
                return agents.FirstOrDefault(a => a.Id == AgentFactory.ResearchAnalyzerAgentName);
            }

            // If we have an active question or pending questions, use ResearchEngine
            if (
                !string.IsNullOrEmpty(state.ActiveQuestionId)
                || state.PendingResearchQuestions.Any()
            )
            {
                _logger.LogInformation(
                    "[{ResearchId}] Selection: Questions pending. Selecting ResearchEngine.",
                    _researchId
                );
                return agents.FirstOrDefault(a => a.Id == AgentFactory.ResearchEngineAgentName);
            }

            // If ready for synthesis, stay with Coordinator (it will handle synthesis)
            if (!state.SynthesisComplete && state.TableOfContents?.Any() == true)
            {
                _logger.LogInformation(
                    "[{ResearchId}] Selection: Ready for synthesis. Staying with Coordinator.",
                    _researchId
                );
                return agents.FirstOrDefault(a =>
                    a.Id == AgentFactory.ResearchCoordinatorAgentName
                );
            }
        }
        // After ResearchEngine completes a question
        else if (
            lastAgentName.Equals(
                AgentFactory.ResearchEngineAgentName,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            _logger.LogInformation(
                "[{ResearchId}] Selection: After ResearchEngine. Selecting ResearchCoordinator.",
                _researchId
            );
            return agents.FirstOrDefault(a => a.Id == AgentFactory.ResearchCoordinatorAgentName);
        }

        // Default to Coordinator for unexpected states
        _logger.LogWarning(
            "[{ResearchId}] Selection: Unexpected state or last agent '{LastAgentName}'. Defaulting to ResearchCoordinator.",
            _researchId,
            lastAgentName
        );
        return agents.FirstOrDefault(a => a.Id == AgentFactory.ResearchCoordinatorAgentName);
    }

    public bool CheckTermination(Agent agent, IReadOnlyList<ChatMessageContent> history)
    {
        if (_researchId == null)
        {
            throw new InvalidOperationException(
                "ResearchManager not initialized. Call Initialize first."
            );
        }

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
