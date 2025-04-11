using Microsoft.Extensions.Caching.Memory;

namespace Apollo.Agents.State;

public interface IStateManager
{
    Task<ResearchState> GetState(string researchId);
    Task AddResearchQuestions(string researchId, List<string> questions);
    Task AddToProcessedQuestions(string researchId, string questionId);
    Task UpdateTableOfContents(string researchId, List<TocItem> toc);
}

public class StateManager : IStateManager
{
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan _cacheTimeout = TimeSpan.FromHours(1);

    private static class CacheKeys
    {
        public static string ResearchState(string researchId) => $"research_state_{researchId}";
    }

    public StateManager(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<ResearchState> GetState(string researchId)
    {
        var key = CacheKeys.ResearchState(researchId);
        return await _cache.GetOrCreate(
            key,
            entry =>
            {
                entry.SlidingExpiration = _cacheTimeout;
                return Task.FromResult(new ResearchState());
            }
        );
    }

    public async Task AddResearchQuestions(string researchId, List<string> questions)
    {
        var state = await GetState(researchId);
        questions.ForEach(q =>
        {
            state.ResearchQuestions.Enqueue(new(Guid.NewGuid().ToString(), q));
        });
        _cache.Set(CacheKeys.ResearchState(researchId), state, _cacheTimeout);
    }

    public async Task AddToProcessedQuestions(string researchId, string questionId)
    {
        var state = await GetState(researchId);
        var question = state.ResearchQuestions.First(q =>
            q.Id.Equals(questionId, StringComparison.OrdinalIgnoreCase)
        );
        state.ProcessedQuestions.Add(question);
        _cache.Set(CacheKeys.ResearchState(researchId), state, _cacheTimeout);
    }

    public async Task UpdateTableOfContents(string researchId, List<TocItem> toc)
    {
        var state = await GetState(researchId);
        state.TableOfContents = toc;
        _cache.Set(CacheKeys.ResearchState(researchId), state, _cacheTimeout);
    }
}

public class ResearchState
{
    public Queue<ResearchQuestion> ResearchQuestions { get; set; } = [];
    public List<ResearchQuestion> ProcessedQuestions { get; set; } = [];
    public List<TocItem>? TableOfContents { get; set; }
}

public record TocItem(string Title, string Description);

public record ResearchQuestion(string Id, string Question);
