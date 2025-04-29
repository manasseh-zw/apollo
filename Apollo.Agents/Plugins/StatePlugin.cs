using System.ComponentModel;
using Apollo.Agents.State;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Apollo.Agents.Plugins;

public class StatePlugin
{
    private readonly IStateManager _state;
    private readonly ILogger<StatePlugin> _logger;
    private readonly string _researchId;

    public StatePlugin(IStateManager state, ILogger<StatePlugin> logger, string researchId)
    {
        _state = state;
        _logger = logger;
        _researchId = researchId;
    }

    [KernelFunction]
    [Description("Gets the text of the currently active research question.")]
    [return: Description(
        "The text of the active research question, or empty string if no active question exists."
    )]
    public string GetActiveResearchQuestion()
    {
        var state = _state.GetState(_researchId);
        var activeQuestion = state?.GetActiveQuestion();
        if (activeQuestion != null)
        {
            _logger.LogInformation(
                "[{ResearchId}] Getting active question: {QuestionId}",
                _researchId,
                activeQuestion.Id
            );
            return activeQuestion.Text;
        }
        _logger.LogWarning("[{ResearchId}] No active question found.", _researchId);
        return string.Empty;
    }

    [KernelFunction]
    [Description(
        "Marks the currently active research question as fully processed by the Research Engine (searched, ranked, crawled, ingested)."
    )]
    public void MarkActiveQuestionComplete()
    {
        _logger.LogInformation(
            "[{ResearchId}] Attempting to mark active question complete via StatePlugin.",
            _researchId
        );
        _state.CompleteActiveQuestion(_researchId);
    }

    //made this accept one gap question inorder to stop hitting the rate limit
    [KernelFunction]
    [Description(
        "Adds newly identified research question to the list of pending questions to address any identified knowledge gaps. Called by ResearchAnalyzer."
    )]
    public void AddGapAnalysisQuestions(
        [Description("The text of the new research question.")] string newQuestion
    )
    {
        if (string.IsNullOrEmpty(newQuestion))
            return;

        _logger.LogInformation($"for {_researchId} Adding new gap  question : {newQuestion}  ");
        _state.AddPendingQuestions(_researchId, [newQuestion]);
    }

    [KernelFunction]
    [Description(
        "Checks if there are any pending research questions left to process. Called by ResearchCoordinator."
    )]
    public bool AnyPendingQuestionsRemaining()
    {
        var state = _state.GetState(_researchId);
        bool remaining = state?.PendingResearchQuestions?.Any(q => !q.IsProcessed) ?? false;
        _logger.LogDebug(
            "[{ResearchId}] Checking pending questions remaining: {Remaining}",
            _researchId,
            remaining
        );
        return remaining;
    }

    [KernelFunction]
    [Description(
        "Checks if the research process requires analysis for knowledge gaps. Called by ResearchCoordinator after all initial questions are processed."
    )]
    public bool DoesResearchNeedAnalysis()
    {
        var state = _state.GetState(_researchId);
        bool needsAnalysis = state?.NeedsAnalysis ?? false;
        _logger.LogDebug(
            "[{ResearchId}] Checking if research needs analysis: {NeedsAnalysis}",
            _researchId,
            needsAnalysis
        );
        return needsAnalysis;
    }

    [KernelFunction]
    [Description(
        "Marks the final report synthesis as complete, effectively ending the research process. Called by ReportSynthesizer."
    )]
    public void MarkSynthesisCompleteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[{ResearchId}] Marking synthesis complete via StatePlugin.",
            _researchId
        );
        _state.MarkResearchComplete(_researchId);
    }

    [KernelFunction]
    [Description(
        "Gets the current table of contents for the research report. Used by both ResearchAnalyzer and ReportSynthesizer."
    )]
    public List<string> GetTableOfContents()
    {
        var state = _state.GetState(_researchId);
        return state.TableOfContents;
    }

    [KernelFunction]
    [Description(
        "Updates the table of contents for the research report. Called by ResearchAnalyzer to propose structure and ReportSynthesizer to finalize it."
    )]
    public void UpdateTableOfContents(
        [Description("The new table of contents sections in order")] List<string> sections
    )
    {
        _logger.LogInformation(
            "[{ResearchId}] Updating table of contents with {Count} sections",
            _researchId,
            sections.Count
        );
        _state.UpdateTableOfContents(_researchId, sections);
    }

    [KernelFunction]
    [Description(
        "Marks the analysis phase as started. Called by ResearchCoordinator before delegating to ResearchAnalyzer."
    )]
    public void MarkAnalysisStarted()
    {
        _logger.LogInformation(
            "[{ResearchId}] Marking analysis phase as started via StatePlugin.",
            _researchId
        );
        _state.MarkAnalysisStarted(_researchId);
    }

    [KernelFunction]
    [Description(
        "Marks the analysis phase as complete. Called by ResearchAnalyzer after finishing its analysis."
    )]
    public void MarkAnalysisComplete()
    {
        _logger.LogInformation(
            "[{ResearchId}] Marking analysis phase as complete via StatePlugin.",
            _researchId
        );
        _state.MarkAnalysisComplete(_researchId);
    }

    [KernelFunction]
    [Description(
        "Gets the research context (title and description) for the current research project."
    )]
    [return: Description("A string containing the research title and description.")]
    public string GetResearchContext()
    {
        var state = _state.GetState(_researchId);
        _logger.LogInformation(
            "[{ResearchId}] Getting research context via StatePlugin.",
            _researchId
        );
        return $"Title: {state.Title}\nDescription: {state.Description}";
    }

    [KernelFunction]
    [Description("Checks if the initial analysis phase has been performed.")]
    [return: Description("True if initial analysis has been performed, false otherwise.")]
    public bool HasInitialAnalysisBeenPerformed()
    {
        var hasPerformed = _state.HasInitialAnalysisBeenPerformed(_researchId);
        _logger.LogDebug(
            "[{ResearchId}] Checking if initial analysis has been performed: {HasPerformed}",
            _researchId,
            hasPerformed
        );
        return hasPerformed;
    }
}
