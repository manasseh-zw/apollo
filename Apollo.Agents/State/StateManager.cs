using System.Collections.Concurrent;
using Apollo.Search.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Apollo.Agents.State;

public interface IStateManager
{
    Task<ResearchState> GetOrCreateState(string researchId, Func<Task<ResearchState>> factory);
    ResearchState GetState(string researchId);
    void UpdateState(string researchId, Action<ResearchState> updateAction);
    string SetNextPendingQuestionAsActive(string researchId);
    void CompleteActiveQuestion(string researchId);
    void MarkResearchComplete(string researchId);
    void AddPendingQuestions(string researchId, List<string> newQuestions);
}

public class StateManager : IStateManager
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<StateManager> _logger;
    private static readonly TimeSpan _cacheTimeout = TimeSpan.FromHours(1);

    public StateManager(IMemoryCache cache, ILogger<StateManager> logger)
    {
        _cache = cache;
        _logger = logger;
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

    //plugin functions
    public string SetNextPendingQuestionAsActive(string researchId)
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
                    "Set active question for {ResearchId} to {QuestionId}",
                    researchId,
                    nextQuestionId
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
            _cache.Set(researchId, state, TimeSpan.FromHours(2)); // Update cache
        }
        else
        {
            _logger.LogWarning(
                "SetNextPendingQuestionAsActiveAsync: State not found for {ResearchId}",
                researchId
            );
        }

        return nextQuestionId;
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
                state.ActiveQuestionId = null; // Ready for the next one
                _logger.LogInformation(
                    "Completed question {QuestionId} for {ResearchId}",
                    activeQuestion.Id,
                    researchId
                );
                _cache.Set(researchId, state, _cacheTimeout); // Update cache
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
            foreach (var q in newQuestions)
            {
                state.PendingResearchQuestions.Add(
                    new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Text = q,
                        IsProcessed = false,
                    }
                );
                _logger.LogInformation(
                    "Added new pending question to {ResearchId}: '{QuestionText}'",
                    researchId,
                    q
                );
            }
            state.NeedsAnalysis = false;
            _cache.Set(researchId, state, _cacheTimeout);
        }
        else
        {
            _logger.LogWarning(
                "AddPendingQuestionsAsync: State not found for {ResearchId}",
                researchId
            );
        }
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
}

public class ResearchState
{
    public required string ResearchId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public List<ResearchQuestion> PendingResearchQuestions { get; set; } = [];
    public List<ResearchQuestion> CompletedResearchQuestions { get; set; } = [];
    public string? ActiveQuestionId { get; set; }
    public ConcurrentBag<string> CrawledUrls { get; set; } = [];
    public List<string> TableOfContents { get; set; } = [];
    public bool IsComplete { get; set; } = false;
    public bool NeedsAnalysis { get; set; } = false;
    public bool SynthesisComplete { get; set; } = false;

    public ResearchQuestion GetActiveQuestion()
    {
        return PendingResearchQuestions.FirstOrDefault(q => q.Id == ActiveQuestionId)
            ?? CompletedResearchQuestions.First(q => q.Id == ActiveQuestionId);
    }
}

public class ResearchQuestion
{
    public required string Id { get; set; }
    public required string Text { get; set; }
    public bool IsProcessed { get; set; }
};
