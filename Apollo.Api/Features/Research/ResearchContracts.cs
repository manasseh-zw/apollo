using Apollo.Agents.Contracts;
using Apollo.Data.Models;

namespace Apollo.Api.Features.Research;

// API-specific DTOs for research responses
public record ApiResearchResponse(
    string Id,
    string Title,
    string Description,
    ResearchStatus Status,
    string StartedAt,
    string CompletedAt,
    ApiResearchPlanResponse Plan,
    ApiResearchReportResponse? Report,
    ApiResearchMindMapResponse? MindMap
);

public record ApiResearchReportResponse(string Id, string Title, string Content);

public record ApiResearchPlanResponse(string Id, List<string> Questions);

public record ApiResearchMindMapResponse(string Id, MindMapNode? GraphData);

// Shared research report response (for public sharing)
public record SharedResearchReportResponse(string Id, string Title, string Content);

// Research history item for listing
public record ResearchHistoryItemResponse(string Id, string Title, string StartedAt);

// Paginated response wrapper
public record PaginatedResponse<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    bool HasMore
);

// Create research request/response
public record CreateResearchRequest(string Title, string Description, List<string> Questions);

public record CreateResearchResponse(string Id, string Title);

// Research updates response (for real-time updates)
public record ResearchUpdatesResponse(
    List<ResearchFeedUpdateEvent> FeedUpdates,
    List<AgentChatMessageEvent> ChatMessages
);
