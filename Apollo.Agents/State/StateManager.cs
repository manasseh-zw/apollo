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
    void AddFeedUpdate(string researchId, ResearchFeedUpdateEvent update);
    void AddChatMessage(string researchId, AgentChatMessageEvent message);
    void AddSearchQueryToMindMap(
        string researchId,
        string questionNodeId,
        SearchQueryMindMapNode queryNode
    );
    void AddSearchResultToMindMap(
        string researchId,
        string queryNodeId,
        SearchResultMindMapNode resultNode
    );
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
                    var state = await factory();
                    state.FeedUpdates = new List<ResearchFeedUpdateEvent>();
                    state.ChatMessages = new List<AgentChatMessageEvent>();

                    // Initialize the mind map root node
                    state.MindMapRoot = new RootMindMapNode
                    {
                        Id = "root",
                        Label = state.Title,
                        Type = MindMapNodeType.Root,
                        ResearchTitle = state.Title,
                        ResearchDescription = state.Description,
                        Children = [],
                    };

                    return state;
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

                // Create a mind map node for this question
                var questionNode = new QuestionMindMapNode
                {
                    Id = $"q-{nextQuestion.Id}",
                    Label = nextQuestion.Text,
                    Type = MindMapNodeType.Question,
                    QuestionText = nextQuestion.Text,
                    IsGapQuestion = false,
                    Children = [],
                };

                // Add it to the mind map root
                state.MindMapRoot?.Children.Add(questionNode);
                state.ActiveQuestionMindMapNodeId = questionNode.Id;

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
                state.ActiveQuestionMindMapNodeId = null; // Clear active mind map node ID
                state.NeedsAnalysis = true; // Signal to move to analysis phase
                state.NeedsSynthesis = false; // Ensure synthesis is not triggered prematurely
                _logger.LogInformation(
                    "No more pending questions for {ResearchId}. Setting NeedsAnalysis flag.",
                    researchId
                );
            }
            _cache.Set(researchId, state, _cacheTimeout); // Update cache with new timeout
            SendTimelineUpdate(researchId, state);
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
        var timelineItems = new List<TimelineItem>();

        // Add all questions in their original order
        timelineItems.AddRange(
            state.AllQuestionsInOrder.Select(q => new TimelineItem
            {
                Id = q.Id,
                Text = q.Text,
                Type = TimelineItemType.Question,
                Active = q.Id == state.ActiveQuestionId,
                Status =
                    state.CompletedResearchQuestions.Any(cq => cq.Id == q.Id)
                        ? TimelineItemStatus.Completed
                    : q.Id == state.ActiveQuestionId ? TimelineItemStatus.InProgress
                    : TimelineItemStatus.Pending,
            })
        );

        // Add Analysis phase if it's in progress or has been completed
        if (state.IsAnalyzing || state.NeedsAnalysis)
        {
            timelineItems.Add(
                new TimelineItem
                {
                    Id = $"{researchId}-analysis",
                    Text = "Analyzing Research Findings",
                    Type = TimelineItemType.Analysis,
                    Active = state.IsAnalyzing,
                    Status =
                        !state.IsAnalyzing && !state.NeedsAnalysis ? TimelineItemStatus.Completed
                        : state.IsAnalyzing ? TimelineItemStatus.InProgress
                        : TimelineItemStatus.Pending,
                }
            );
        }

        // Add Synthesis phase
        // It's shown if synthesis is needed, in progress, or complete
        bool canStartSynthesis =
            state.NeedsSynthesis
            || (
                !state.IsAnalyzing
                && !state.NeedsAnalysis
                && state.PendingResearchQuestions.Count == 0
                && string.IsNullOrEmpty(state.ActiveQuestionId)
            );

        if (canStartSynthesis || state.SynthesisComplete)
        {
            timelineItems.Add(
                new TimelineItem
                {
                    Id = $"{researchId}-synthesis",
                    Text = "Synthesizing Final Report",
                    Type = TimelineItemType.Synthesis,
                    Active = state.NeedsSynthesis && !state.SynthesisComplete,
                    Status =
                        state.SynthesisComplete ? TimelineItemStatus.Completed
                        : state.NeedsSynthesis ? TimelineItemStatus.InProgress
                        : TimelineItemStatus.Pending,
                }
            );
        }

        _clientUpdate.SendTimelineUpdate(
            new TimelineUpdateEvent { ResearchId = researchId, Items = timelineItems }
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

            // Add the new questions to pending list and all questions list
            state.PendingResearchQuestions.AddRange(newQuestionObjects);
            state.AllQuestionsInOrder.AddRange(newQuestionObjects);

            // Create mind map nodes for the gap questions
            foreach (var question in newQuestionObjects)
            {
                var questionNode = new QuestionMindMapNode
                {
                    Id = $"q-{question.Id}",
                    Label = question.Text,
                    Type = MindMapNodeType.Question,
                    QuestionText = question.Text,
                    IsGapQuestion = true, // These are gap questions
                    Children = [],
                };

                state.MindMapRoot?.Children.Add(questionNode);
            }

            _logger.LogInformation(
                "Added {Count} new pending questions to {ResearchId}",
                newQuestionObjects.Count,
                researchId
            );

            // If there's no active question, set the first new question as active
            if (string.IsNullOrEmpty(state.ActiveQuestionId) && newQuestionObjects.Any())
            {
                state.ActiveQuestionId = newQuestionObjects[0].Id;
                state.ActiveQuestionMindMapNodeId = $"q-{newQuestionObjects[0].Id}";
                _logger.LogInformation(
                    "Set first gap question as active for {ResearchId}: '{QuestionText}'",
                    researchId,
                    newQuestionObjects[0].Text
                );
            }

            // Reset analysis and synthesis flags since we have new questions
            state.NeedsAnalysis = false;
            state.NeedsSynthesis = false;
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
                SendTimelineUpdate(researchId, state);
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
                SendTimelineUpdate(researchId, state);
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
                SendTimelineUpdate(researchId, state);
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
                state.NeedsAnalysis = false; // Explicitly mark analysis as no longer needed

                // If no pending questions and no active question, it's time for synthesis
                if (
                    state.PendingResearchQuestions.Count == 0
                    && string.IsNullOrEmpty(state.ActiveQuestionId)
                )
                {
                    state.NeedsSynthesis = true;
                    _logger.LogInformation(
                        "[{ResearchId}] Analysis complete, no new questions. Flagging for synthesis.",
                        researchId
                    );
                }
                else
                {
                    state.NeedsSynthesis = false;
                    _logger.LogInformation(
                        "[{ResearchId}] Analysis complete, but there are pending questions to process.",
                        researchId
                    );
                }

                _logger.LogInformation("[{ResearchId}] Analysis phase completed.", researchId);
                SendTimelineUpdate(researchId, state);
            }
        );
    }

    public void AddFeedUpdate(string researchId, ResearchFeedUpdateEvent update)
    {
        UpdateState(
            researchId,
            state =>
            {
                state.FeedUpdates.Add(update);
                _logger.LogDebug(
                    "[{ResearchId}] Added feed update of type {UpdateType}",
                    researchId,
                    update.Type
                );
            }
        );
    }

    public void AddChatMessage(string researchId, AgentChatMessageEvent message)
    {
        UpdateState(
            researchId,
            state =>
            {
                state.ChatMessages.Add(message);
                _logger.LogDebug(
                    "[{ResearchId}] Added chat message from {Author}",
                    researchId,
                    message.Author
                );
            }
        );
    }

    public void AddSearchQueryToMindMap(
        string researchId,
        string questionNodeId,
        SearchQueryMindMapNode queryNode
    )
    {
        UpdateState(
            researchId,
            state =>
            {
                var questionNode =
                    state.MindMapRoot?.Children.FirstOrDefault(n => n.Id == questionNodeId)
                    as QuestionMindMapNode;

                if (questionNode != null)
                {
                    questionNode.Children.Add(queryNode);
                    _logger.LogInformation(
                        "[{ResearchId}] Added search query node {QueryId} to question {QuestionId}",
                        researchId,
                        queryNode.Id,
                        questionNodeId
                    );
                }
                else
                {
                    _logger.LogWarning(
                        "[{ResearchId}] Question node {QuestionId} not found for adding search query",
                        researchId,
                        questionNodeId
                    );
                }
            }
        );
    }

    public void AddSearchResultToMindMap(
        string researchId,
        string queryNodeId,
        SearchResultMindMapNode resultNode
    )
    {
        UpdateState(
            researchId,
            state =>
            {
                var queryNode =
                    state
                        .MindMapRoot?.Children.SelectMany(q => q.Children)
                        .FirstOrDefault(n => n.Id == queryNodeId) as SearchQueryMindMapNode;

                if (queryNode != null)
                {
                    queryNode.Children.Add(resultNode);
                    _logger.LogInformation(
                        "[{ResearchId}] Added search result node {ResultId} to query {QueryId}",
                        researchId,
                        resultNode.Id,
                        queryNodeId
                    );
                }
                else
                {
                    _logger.LogWarning(
                        "[{ResearchId}] Query node {QueryId} not found for adding search result",
                        researchId,
                        queryNodeId
                    );
                }
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
    public List<ResearchQuestion> AllQuestionsInOrder { get; set; } = [];
    public string? ActiveQuestionId { get; set; }
    public string? ActiveQuestionMindMapNodeId { get; set; }
    public RootMindMapNode? MindMapRoot { get; set; }
    public List<string> CrawledUrls { get; set; } = [];
    public List<string> TableOfContents { get; set; } = [];
    public bool IsComplete { get; set; } = false;
    public bool NeedsAnalysis { get; set; } = false;
    public bool IsAnalyzing { get; set; } = false;
    public bool NeedsSynthesis { get; set; } = false; // New flag to control synthesis transition
    public bool SynthesisComplete { get; set; } = false;
    public List<ResearchFeedUpdateEvent> FeedUpdates { get; set; } = [];
    public List<AgentChatMessageEvent> ChatMessages { get; set; } = [];

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
}
