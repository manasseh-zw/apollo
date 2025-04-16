using Apollo.Data.Models;
using Apollo.Data.Repository;
using FluentResults;
using Microsoft.EntityFrameworkCore;

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

public interface IResearchService
{
    Task<Result<ResearchResponse>> GetResearch(Guid userId, Guid researchId);
    Task<Result<List<ResearchResponse>>> GetAllResearch(Guid userId);
}

public class ResearchService : IResearchService
{
    private readonly ApolloDbContext _repository;

    public ResearchService(ApolloDbContext repository)
    {
        _repository = repository;
    }

    public async Task<Result<ResearchResponse>> GetResearch(Guid userId, Guid researchId)
    {
        var research = await _repository
            .Research.Include(r => r.Plan)
            .Include(r => r.Report)
            .FirstOrDefaultAsync(r => r.Id == researchId && r.UserId == userId);

        if (research == null)
        {
            return Result.Fail("Research not found");
        }

        var response = new ResearchResponse(
            research.Id,
            research.Title,
            research.Description,
            research.Plan,
            research.Report,
            research.StartedAt,
            research.Status
        );

        return Result.Ok(response);
    }

    public async Task<Result<List<ResearchResponse>>> GetAllResearch(Guid userId)
    {
        var research = await _repository
            .Research.Include(r => r.Plan)
            .Include(r => r.Report)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.StartedAt)
            .Select(r => new ResearchResponse(
                r.Id,
                r.Title,
                r.Description,
                r.Plan,
                r.Report,
                r.StartedAt,
                r.Status
            ))
            .ToListAsync();

        return Result.Ok(research);
    }
}
