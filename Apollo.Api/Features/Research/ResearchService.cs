using Apollo.Agents.Events;
using Apollo.Data.Models;
using Apollo.Data.Repository;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Expressions;

namespace Apollo.Api.Features.Research;

public interface IResearchService
{
    Task<Result<CreateResearchResponse>> CreateResearch(Guid userId, CreateResearchRequest request);
    Task<Result<ResearchResponse>> GetResearch(Guid userId, Guid researchId);
    Task<Result<List<ResearchResponse>>> GetAllResearch(Guid userId);
}

public class ResearchService : IResearchService
{
    private readonly ApolloDbContext _repository;
    private readonly IResearchEventHandler _eventHandler;

    public ResearchService(ApolloDbContext repository, IResearchEventHandler eventHandler)
    {
        _repository = repository;
        _eventHandler = eventHandler;
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

    public async Task<Result<CreateResearchResponse>> CreateResearch(
        Guid userId,
        CreateResearchRequest request
    )
    {
        var researchPlan = new ResearchPlan()
        {
            Questions = request.Questions,
            Type = Enum.Parse<ResearchType>(request.Type),
            Depth = Enum.Parse<ResearchDepth>(request.Depth),
        };

        var research = new Data.Models.Research()
        {
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            Plan = researchPlan,
            StartedAt = DateTime.UtcNow,
            Status = ResearchStatus.InProgress,
        };

        await _repository.Research.AddAsync(research);
        await _repository.SaveChangesAsync();

        await _eventHandler.HandleResearchStart(
            new ResearchStartEvent { ResearchId = research.Id, UserId = userId.ToString() }
        );
        var response = new CreateResearchResponse(
            research.Id,
            research.Title,
            research.Description,
            research.StartedAt,
            research.Status
        );
        return Result.Ok(response);
    }
}
