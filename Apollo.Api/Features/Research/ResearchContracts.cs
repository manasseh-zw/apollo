using Apollo.Data.Models;

namespace Apollo.Api.Features.Research;

public record ResearchResponse(
    Guid Id,
    string Title,
    string Description,
    ResearchPlan Plan,
    ResearchReport? Report,
    DateTime StartedAt,
    ResearchStatus Status
);

public record CreateResearchResponse(
    Guid Id,
    string Title,
    string Description,
    DateTime StartedAt,
    ResearchStatus Status
);

public record CreateResearchRequest(
    string Title,
    string Description,
    List<string> Questions,
    string Type,
    string Depth
);

public record ResearchHistoryItemResponse(Guid Id, string Title, DateTime StartedAt);

public record PaginatedResponse<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    bool HasMore
);
