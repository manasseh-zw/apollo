using Apollo.Agents.Contracts;
using Apollo.Agents.Events;
using Apollo.Agents.State;
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
    Task<Result<PaginatedResponse<ResearchHistoryItemResponse>>> GetResearchHistory(
        Guid userId,
        int page = 1,
        int pageSize = 5
    );
    Task<Result<ResearchUpdatesResponse>> GetResearchUpdates(Guid userId, Guid researchId);
    Task<Result<SharedResearchReportResponse>> GetSharedResearchReport(Guid reportId);
}

public class ResearchService : IResearchService
{
    private readonly ApolloDbContext _repository;
    private readonly IResearchEventHandler _eventHandler;
    private readonly IStateManager _stateManager;

    public ResearchService(
        ApolloDbContext repository,
        IResearchEventHandler eventHandler,
        IStateManager stateManager
    )
    {
        _repository = repository;
        _eventHandler = eventHandler;
        _stateManager = stateManager;
    }

    public async Task<Result<SharedResearchReportResponse>> GetSharedResearchReport(Guid reportId)
    {
        var report = await _repository
            .ResearchReports.Include(r => r.Research) // Include Research to get the title
            .Where(r => r.Id == reportId)
            .Select(r => new SharedResearchReportResponse(r.Id, r.Research.Title, r.Content))
            .FirstOrDefaultAsync();

        if (report == null)
        {
            return Result.Fail("Research report not found");
        }

        return Result.Ok(report);
    }

    public async Task<Result<PaginatedResponse<ResearchHistoryItemResponse>>> GetResearchHistory(
        Guid userId,
        int page = 1,
        int pageSize = 5
    )
    {
        var query = _repository.Research.Where(r => r.UserId == userId);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(r => r.StartedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ResearchHistoryItemResponse(r.Id, r.Title, r.StartedAt))
            .ToListAsync();

        var hasMore = (page * pageSize) < totalCount;

        var response = new PaginatedResponse<ResearchHistoryItemResponse>(
            items,
            totalCount,
            page,
            pageSize,
            hasMore
        );

        return Result.Ok(response);
    }

    public async Task<Result<ResearchResponse>> GetResearch(Guid userId, Guid researchId)
    {
        var research = await _repository
            .Research.Where(r => r.Id == researchId && r.UserId == userId)
            .Select(r => new ResearchResponse(
                r.Id,
                r.Title,
                r.Description,
                new ResearchPlan
                {
                    Id = r.Plan.Id,
                    Questions = r.Plan.Questions,
                    ResearchId = r.Plan.ResearchId,
                },
                r.Report == null
                    ? null
                    : new ResearchReport
                    {
                        Id = r.Report.Id,
                        Content = r.Report.Content,
                        ResearchId = r.Report.ResearchId,
                    },
                r.StartedAt,
                r.Status
            ))
            .FirstOrDefaultAsync();

        if (research == null)
        {
            return Result.Fail("Research not found");
        }

        return Result.Ok(research);
    }

    public async Task<Result<List<ResearchResponse>>> GetAllResearch(Guid userId)
    {
        var research = await _repository
            .Research.Where(r => r.UserId == userId)
            .OrderByDescending(r => r.StartedAt)
            .Select(r => new ResearchResponse(
                r.Id,
                r.Title,
                r.Description,
                new ResearchPlan
                {
                    Id = r.Plan.Id,
                    Questions = r.Plan.Questions,
                    ResearchId = r.Plan.ResearchId,
                },
                r.Report == null
                    ? null
                    : new ResearchReport
                    {
                        Id = r.Report.Id,
                        Content = r.Report.Content,
                        ResearchId = r.Report.ResearchId,
                    },
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
        var researchPlan = new ResearchPlan() { Questions = request.Questions };

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

    public async Task<Result<ResearchUpdatesResponse>> GetResearchUpdates(
        Guid userId,
        Guid researchId
    )
    {
        // First verify the user has access to this research
        var research = await _repository.Research.FirstOrDefaultAsync(r =>
            r.Id == researchId && r.UserId == userId
        );

        if (research == null)
        {
            return Result.Fail("Research not found");
        }

        // Only return updates if research is in progress
        if (research.Status != ResearchStatus.InProgress)
        {
            return Result.Ok(new ResearchUpdatesResponse(new List<ResearchFeedUpdateEvent>(), []));
        }

        try
        {
            var state = _stateManager.GetState(researchId.ToString());
            return Result.Ok(
                new ResearchUpdatesResponse(
                    state.FeedUpdates ?? new List<ResearchFeedUpdateEvent>(),
                    state.ChatMessages ?? new List<AgentChatMessageEvent>()
                )
            );
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to retrieve research updates: {ex.Message}");
        }
    }
}
