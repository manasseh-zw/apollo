namespace Apollo.Agents.Contracts;

public abstract record ClientUpdateEvent
{
    public string ResearchId { get; init; } = null!;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record TimelineUpdateEvent : ClientUpdateEvent
{
    public List<TimelineItem> Items { get; init; } = new();
}

public record TimelineItem
{
    public string Id { get; init; } = null!;
    public string Text { get; init; } = null!;
    public string Type { get; init; } = null!;
    public bool Active { get; init; }
    public TimelineItemStatus Status { get; init; }
}

public enum TimelineItemStatus
{
    Pending,
    InProgress,
    Completed,
}

public enum TimelineItemType
{
    Question,
    Analysis,
    Synthesis,
}

public abstract record ResearchFeedUpdateEvent : ClientUpdateEvent
{
    public ResearchFeedUpdateType Type { get; init; }
}

public enum ResearchFeedUpdateType
{
    Message,
    Searching,
    SearchResults,
    Snippet,
    TableOfContents,
}

public record ProgressMessageFeedUpdate : ResearchFeedUpdateEvent
{
    public string Message { get; init; } = null!;
}

public record WebSearchFeedUpdate : ResearchFeedUpdateEvent
{
    public string Query { get; init; } = null!;
}

public record SearchResultsFeedUpdate : ResearchFeedUpdateEvent
{
    public List<SearchResultItemContract> Results { get; init; } = new();
}

public record SearchResultItemContract
{
    public string Id { get; init; } = null!;
    public string Icon { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string Url { get; init; } = null!;
    public string? Snippet { get; init; }
    public List<string>? Highlights { get; init; }
}

public record SnippetFeedUpdate : ResearchFeedUpdateEvent
{
    public string Content { get; init; } = null!;
    public List<string>? Highlights { get; init; }
}

public record TableOfContentsFeedUpdate : ResearchFeedUpdateEvent
{
    public List<string> Sections { get; init; } = new();
}

public record AgentChatMessageEvent : ClientUpdateEvent
{
    public string Author { get; init; } = null!;
    public string Message { get; init; } = null!;
}
