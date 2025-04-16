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
