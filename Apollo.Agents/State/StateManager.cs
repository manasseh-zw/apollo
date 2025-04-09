using Microsoft.Extensions.Caching.Memory;

namespace Apollo.Agents.State;

public interface IStateManager
{
    Task<ResearchState> GetStateAsync(string researchId);
    Task AddResearchQuestionAsync(string researchId, string question);
    Task AddProcessedQuestionAsync(string researchId, string question);
    Task UpdateTableOfContentsAsync(string researchId, List<TocItem> toc);
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

    public async Task<ResearchState> GetStateAsync(string researchId)
    {
        var key = CacheKeys.ResearchState(researchId);
        return await _cache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SlidingExpiration = _cacheTimeout;
                return Task.FromResult(new ResearchState());
            }
        );
    }

    public async Task AddResearchQuestionAsync(string researchId, string question)
    {
        var state = await GetStateAsync(researchId);
        state.ResearchQuestions.Enqueue(question);
        _cache.Set(CacheKeys.ResearchState(researchId), state, _cacheTimeout);
    }

    public async Task AddProcessedQuestionAsync(string researchId, string question)
    {
        var state = await GetStateAsync(researchId);
        state.ProcessedQuestions.Add(question);
        _cache.Set(CacheKeys.ResearchState(researchId), state, _cacheTimeout);
    }

    public async Task UpdateTableOfContentsAsync(string researchId, List<TocItem> toc)
    {
        var state = await GetStateAsync(researchId);
        state.TableOfContents = toc;
        _cache.Set(CacheKeys.ResearchState(researchId), state, _cacheTimeout);
    }
}

public class ResearchState
{
    public Queue<string> ResearchQuestions { get; set; } = [];
    public List<string> ProcessedQuestions { get; set; } = [];
    public List<TocItem>? TableOfContents { get; set; }
}

public record TocItem(string Title, string Description);