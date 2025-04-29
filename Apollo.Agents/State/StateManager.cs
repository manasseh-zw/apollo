using Apollo.Agents.Contracts;
using Apollo.Agents.Helpers;
using Apollo.Data.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Apollo.Agents.State;

public interface IStateManager
{
    Task<ResearchState> GetOrCreateState(string researchId, Func<Task<ResearchState>> factory);
    ResearchState GetState(string researchId);
    void UpdateState(string researchId, Action<ResearchState> updateAction);
    void CompleteActiveQuestion(string researchId);
    void MarkResearchComplete(string researchId);
    void AddPendingQuestions(string researchId, List<string> newQuestions);
    void UpdateTableOfContents(string researchId, List<string> sections);
    void MarkAnalysisStarted(string researchId);
    void MarkAnalysisComplete(string researchId);
    bool HasInitialAnalysisBeenPerformed(string researchId);
}

public class StateManager : IStateManager
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<StateManager> _logger;
    private readonly IClientUpdateCallback _clientUpdate;
    private static readonly TimeSpan _cacheTimeout = TimeSpan.FromHours(1);

    public StateManager(
        IMemoryCache cache,
        ILogger<StateManager> logger,
        IClientUpdateCallback clientUpdate
    )
    {
        _cache = cache;
        _logger = logger;
        _clientUpdate = clientUpdate;
    }

    public async Task<ResearchState> GetOrCreateState(
        string researchId,
        Func<Task<ResearchState>> factory
    )
    {
        return await _cache.GetOrCreateAsync(
                researchId,
                async entry =>
                {
                    entry.SlidingExpiration = _cacheTimeout; // Keep state for a while
                    _logger.LogInformation(
                        "Creating new research state for ID: {ResearchId}",
                        researchId
                    );
                    return await factory();
                }
            ) ?? throw new Exception("research state is null");
    }

    public ResearchState GetState(string researchId)
    {
        if (_cache.TryGetValue(researchId, out ResearchState? state))
        {
            return state ?? throw new Exception($"Research state not found for ID: {researchId}");
        }
        _logger.LogError("Research state not found for ID: {ResearchId}", researchId);
        throw new Exception($"Research state not found for ID: {researchId}");
    }

    public void UpdateState(string researchId, Action<ResearchState> updateAction)
    {
        if (_cache.TryGetValue(researchId, out ResearchState? state))
        {
            _ = state ?? throw new Exception($"Research state not found for ID: {researchId}");
            updateAction(state);

            _cache.Set(researchId, state, _cacheTimeout);
            _logger.LogDebug("Research state updated for ID: {ResearchId}", researchId);
        }
        else
        {
            _logger.LogError(
                "Attempted to update non-existent state for ID: {ResearchId}",
                researchId
            );
        }
    }

    private string SetNextPendingQuestionAsActive(string researchId)
    {
        string nextQuestionId = string.Empty;

        if (_cache.TryGetValue(researchId, out ResearchState? state))
        {
            _ = state ?? throw new Exception($"Research state not found for ID: {researchId}");
            var nextQuestion = state.PendingResearchQuestions.FirstOrDefault(q => !q.IsProcessed);
            if (nextQuestion != null)
            {
                state.ActiveQuestionId = nextQuestion.Id;
                nextQuestionId = nextQuestion.Id;
                _logger.LogInformation(
                    "Set active question for {ResearchId} to {QuestionId}: {QuestionText}",
                    researchId,
                    nextQuestionId,
                    nextQuestion.Text
                );
            }
            else
            {
                state.ActiveQuestionId = null; // No more pending questions
                state.NeedsAnalysis = true; // Signal to move to analysis phase
                _logger.LogInformation(
                    "No more pending questions for {ResearchId}. Setting NeedsAnalysis flag.",
                    researchId
                );
            }
            _cache.Set(researchId, state, _cacheTimeout); // Update cache with new timeout
        }
        else
        {
            _logger.LogWarning(
                "SetNextPendingQuestionAsActive: State not found for {ResearchId}",
                researchId
            );
        }

        return nextQuestionId;
    }

    private void SendTimelineUpdate(string researchId, ResearchState state)
    {
        // Build timeline items in original order
        var timelineItems = state
            .AllQuestionsInOrder.Select(q => new QuestionTimelineItem
            {
                Id = q.Id,
                Text = q.Text,
                Active = q.Id == state.ActiveQuestionId,
                Status =
                    state.CompletedResearchQuestions.Any(cq => cq.Id == q.Id)
                        ? QuestionStatus.Completed
                    : q.Id == state.ActiveQuestionId ? QuestionStatus.InProgress
                    : QuestionStatus.Pending,
            })
            .ToList();

        _clientUpdate.SendQuestionTimelineUpdate(
            new QuestionTimelineUpdateEvent { ResearchId = researchId, Questions = timelineItems }
        );
    }

    public void CompleteActiveQuestion(string researchId)
    {
        if (
            _cache.TryGetValue(researchId, out ResearchState? state)
            && !string.IsNullOrEmpty(state?.ActiveQuestionId)
        )
        {
            _ = state ?? throw new Exception($"Research state not found for ID: {researchId}");
            var activeQuestion = state.PendingResearchQuestions.FirstOrDefault(q =>
                q.Id == state.ActiveQuestionId
            );
            if (activeQuestion != null)
            {
                activeQuestion.IsProcessed = true;
                state.CompletedResearchQuestions.Add(activeQuestion);
                state.PendingResearchQuestions.Remove(activeQuestion);
                state.ActiveQuestionId = null; // Clear current before setting next
                _logger.LogInformation(
                    "Completed question {QuestionId} for {ResearchId}",
                    activeQuestion.Id,
                    researchId
                );
                _cache.Set(researchId, state, _cacheTimeout); // Update cache

                // Send timeline update
                SendTimelineUpdate(researchId, state);

                // Immediately set next question as active
                SetNextPendingQuestionAsActive(researchId);
            }
            else
            {
                _logger.LogWarning(
                    "CompleteActiveQuestionAsync: Active question {ActiveQuestionId} not found in pending list for {ResearchId}",
                    state.ActiveQuestionId,
                    researchId
                );
            }
        }
    }

    public void AddPendingQuestions(string researchId, List<string> newQuestions)
    {
        if (newQuestions == null || !newQuestions.Any())
            return;

        if (_cache.TryGetValue(researchId, out ResearchState? state))
        {
            _ = state ?? throw new Exception($"Research state not found for ID: {researchId}");

            // Create new question objects with IDs
            var newQuestionObjects = newQuestions
                .Select(q => new ResearchQuestion
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = q,
                    IsProcessed = false,
                })
                .ToList();

            // Add the new questions to pending list
            state.PendingResearchQuestions.AddRange(newQuestionObjects);
            _logger.LogInformation(
                "Added {Count} new pending questions to {ResearchId}",
                newQuestionObjects.Count,
                researchId
            );

            // If there's no active question, set the first new question as active
            if (string.IsNullOrEmpty(state.ActiveQuestionId) && newQuestionObjects.Any())
            {
                state.ActiveQuestionId = newQuestionObjects[0].Id;
                _logger.LogInformation(
                    "Set first gap question as active for {ResearchId}: '{QuestionText}'",
                    researchId,
                    newQuestionObjects[0].Text
                );
            }

            state.NeedsAnalysis = false;
            _cache.Set(researchId, state, _cacheTimeout);

            // Send timeline update after adding new questions
            SendTimelineUpdate(researchId, state);
        }
        else
        {
            _logger.LogWarning(
                "AddPendingQuestionsAsync: State not found for {ResearchId}",
                researchId
            );
        }
    }

    public void UpdateTableOfContents(string researchId, List<string> sections)
    {
        UpdateState(
            researchId,
            state =>
            {
                state.TableOfContents = sections;
                _logger.LogInformation(
                    "Updated table of contents for {ResearchId} with {Count} sections",
                    researchId,
                    sections.Count
                );
            }
        );
    }

    public void MarkResearchComplete(string researchId)
    {
        UpdateState(
            researchId,
            state =>
            {
                state.IsComplete = true;
                state.SynthesisComplete = true;
                _logger.LogInformation("Marking research {ResearchId} as complete.", researchId);
            }
        );
    }

    public void MarkAnalysisStarted(string researchId)
    {
        UpdateState(
            researchId,
            state =>
            {
                state.IsAnalyzing = true;
                _logger.LogInformation("[{ResearchId}] Analysis phase started.", researchId);
            }
        );
    }

    public void MarkAnalysisComplete(string researchId)
    {
        UpdateState(
            researchId,
            state =>
            {
                state.IsAnalyzing = false;
                if (!state.HasPerformedInitialAnalysis)
                {
                    state.HasPerformedInitialAnalysis = true;
                    _logger.LogInformation(
                        "[{ResearchId}] First analysis phase completed, marking initial analysis as performed.",
                        researchId
                    );
                }
                _logger.LogInformation("[{ResearchId}] Analysis phase completed.", researchId);
            }
        );
    }

    public bool HasInitialAnalysisBeenPerformed(string researchId)
    {
        var state = GetState(researchId);
        return state.HasPerformedInitialAnalysis;
    }
}

public class ResearchState
{
    public required string ResearchId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public List<ResearchQuestion> PendingResearchQuestions { get; set; } = [];
    public List<ResearchQuestion> CompletedResearchQuestions { get; set; } = [];
    public List<ResearchQuestion> AllQuestionsInOrder { get; set; } = [];
    public string? ActiveQuestionId { get; set; }
    public List<string> CrawledUrls { get; set; } = [];
    public List<string> TableOfContents { get; set; } = [];
    public bool IsComplete { get; set; } = false;
    public bool NeedsAnalysis { get; set; } = false;
    public bool IsAnalyzing { get; set; } = false;
    public bool SynthesisComplete { get; set; } = false;
    public bool HasPerformedInitialAnalysis { get; set; } = false;

    public ResearchQuestion? GetActiveQuestion()
    {
        return PendingResearchQuestions.FirstOrDefault(q => q.Id == ActiveQuestionId) ?? null;
    }
}

public class ResearchQuestion
{
    public required string Id { get; set; }
    public required string Text { get; set; }
    public bool IsProcessed { get; set; }
};
