using System.Text.Json;
using Apollo.Agents.Contracts;
using Apollo.Agents.State;
using Apollo.Data.Models;
using Apollo.Data.Repository;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Apollo.Api.Features.Research;

public interface IResearchService
{
    Task<ApiResearchResponse?> GetResearch(Guid id);
    Task<List<ApiResearchResponse>> GetAllResearch(Guid userId);
    Task<Result<SharedResearchReportResponse>> GetSharedResearchReport(Guid reportId);
    Task<Result<PaginatedResponse<ResearchHistoryItemResponse>>> GetResearchHistory(
        Guid userId,
        int page,
        int pageSize
    );
    Task<Result<CreateResearchResponse>> CreateResearch(Guid userId, CreateResearchRequest request);
    Task<Result<ResearchUpdatesResponse>> GetResearchUpdates(Guid userId, Guid researchId);
}

public class ResearchService : IResearchService
{
    private readonly ApolloDbContext _repository;
    private readonly IStateManager _state;

    public ResearchService(ApolloDbContext repository, IStateManager state)
    {
        _repository = repository;
        _state = state;
    }

    public async Task<ApiResearchResponse?> GetResearch(Guid id)
    {
        var researchData = await _repository
            .Research.Where(r => r.Id == id)
            .Select(r => new
            {
                r.Id,
                r.Title,
                r.Description,
                r.Status,
                r.StartedAt,
                r.CompletedAt,
                PlanId = r.Plan.Id,
                PlanQuestions = r.Plan.Questions,
                ReportId = r.Report != null ? r.Report.Id : (Guid?)null,
                ReportContent = r.Report != null ? r.Report.Content : null,
                MindMapId = r.MindMap != null ? r.MindMap.Id : (Guid?)null,
                MindMapGraphData = r.MindMap != null ? r.MindMap.GraphData : null,
            })
            .FirstOrDefaultAsync();

        if (researchData == null)
            return null;

        MindMapNode? mindMapNode = null;
        if (
            !string.IsNullOrEmpty(researchData.MindMapGraphData)
            && researchData.MindMapGraphData != "{}"
        )
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    AllowOutOfOrderMetadataProperties = true,
                };
                mindMapNode = JsonSerializer.Deserialize<MindMapNode>(
                    researchData.MindMapGraphData,
                    options
                );
            }
            catch (NotSupportedException ex)
            {
                // TODO: Use proper logging (ILogger) in production
                Console.WriteLine(
                    $"Failed to deserialize MindMapGraphData for research ID {researchData.Id}: {ex.Message}"
                );
                mindMapNode = null;
            }
            catch (JsonException ex)
            {
                // TODO: Use proper logging (ILogger) in production
                Console.WriteLine(
                    $"JSON parsing error for research ID {researchData.Id}: {ex.Message}"
                );
                mindMapNode = null;
            }
        }

        return new ApiResearchResponse(
            researchData.Id.ToString(),
            researchData.Title,
            researchData.Description,
            researchData.Status,
            researchData.StartedAt.ToString("O"),
            researchData.CompletedAt.ToString("O"),
            new ApiResearchPlanResponse(researchData.PlanId.ToString(), researchData.PlanQuestions),
            researchData.ReportId.HasValue
                ? new ApiResearchReportResponse(
                    researchData.ReportId.Value.ToString(),
                    researchData.Title,
                    researchData.ReportContent!
                )
                : null,
            researchData.MindMapId.HasValue
                ? new ApiResearchMindMapResponse(
                    researchData.MindMapId.Value.ToString(),
                    mindMapNode
                )
                : null
        );
    }

    public async Task<List<ApiResearchResponse>> GetAllResearch(Guid userId)
    {
        var researchList = await _repository
            .Research.Where(r => r.UserId == userId)
            .OrderByDescending(r => r.StartedAt)
            .Select(r => new
            {
                r.Id,
                r.Title,
                r.Description,
                r.Status,
                r.StartedAt,
                r.CompletedAt,
                PlanId = r.Plan.Id,
                PlanQuestions = r.Plan.Questions,
                ReportId = r.Report != null ? r.Report.Id : (Guid?)null,
                ReportContent = r.Report != null ? r.Report.Content : null,
                MindMapId = r.MindMap != null ? r.MindMap.Id : (Guid?)null,
                MindMapGraphData = r.MindMap != null ? r.MindMap.GraphData : null,
            })
            .ToListAsync();

        return researchList
            .Select(r =>
            {
                // Deserialize mind map data if present
                MindMapNode? mindMapNode = null;
                if (!string.IsNullOrEmpty(r.MindMapGraphData) && r.MindMapGraphData != "{}")
                {
                    try
                    {
                        var options = new JsonSerializerOptions
                        {
                            WriteIndented = true,
                            AllowOutOfOrderMetadataProperties = true,
                        };
                        mindMapNode = JsonSerializer.Deserialize<MindMapNode>(
                            r.MindMapGraphData,
                            options
                        );
                    }
                    catch (NotSupportedException ex)
                    {
                        // TODO: Use proper logging (ILogger) in production
                        Console.WriteLine(
                            $"Failed to deserialize MindMapGraphData for research ID {r.Id}: {ex.Message}"
                        );
                        mindMapNode = null;
                    }
                    catch (JsonException ex)
                    {
                        // TODO: Use proper logging (ILogger) in production
                        Console.WriteLine(
                            $"JSON parsing error for research ID {r.Id}: {ex.Message}"
                        );
                        mindMapNode = null;
                    }
                }

                return new ApiResearchResponse(
                    r.Id.ToString(),
                    r.Title,
                    r.Description,
                    r.Status,
                    r.StartedAt.ToString("O"),
                    r.CompletedAt.ToString("O"),
                    new ApiResearchPlanResponse(r.PlanId.ToString(), r.PlanQuestions),
                    r.ReportId.HasValue
                        ? new ApiResearchReportResponse(
                            r.ReportId.Value.ToString(),
                            r.Title,
                            r.ReportContent!
                        )
                        : null,
                    r.MindMapId.HasValue
                        ? new ApiResearchMindMapResponse(r.MindMapId.Value.ToString(), mindMapNode)
                        : null
                );
            })
            .ToList();
    }

    public async Task<Result<SharedResearchReportResponse>> GetSharedResearchReport(Guid reportId)
    {
        var reportData = await _repository
            .ResearchReports.Where(r => r.Id == reportId)
            .Select(r => new
            {
                r.Id,
                ResearchTitle = r.Research.Title,
                r.Content,
            })
            .FirstOrDefaultAsync();

        if (reportData == null)
            return Result.Fail("Report not found");

        return Result.Ok(
            new SharedResearchReportResponse(
                reportData.Id.ToString(),
                reportData.ResearchTitle,
                reportData.Content
            )
        );
    }

    public async Task<Result<PaginatedResponse<ResearchHistoryItemResponse>>> GetResearchHistory(
        Guid userId,
        int page,
        int pageSize
    )
    {
        var query = _repository
            .Research.Where(r => r.UserId == userId)
            .OrderByDescending(r => r.StartedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ResearchHistoryItemResponse(
                r.Id.ToString(),
                r.Title,
                r.StartedAt.ToString("O")
            ))
            .ToListAsync();

        return Result.Ok(
            new PaginatedResponse<ResearchHistoryItemResponse>(
                items,
                totalCount,
                page,
                pageSize,
                (page * pageSize) < totalCount
            )
        );
    }

    public async Task<Result<CreateResearchResponse>> CreateResearch(
        Guid userId,
        CreateResearchRequest request
    )
    {
        var research = new Data.Models.Research
        {
            Title = request.Title,
            Description = request.Description,
            UserId = userId,
            Status = ResearchStatus.InProgress,
            StartedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow, // Will be updated when complete
            Plan = new ResearchPlan() { Questions = [] },
        };

        await _repository.Research.AddAsync(research);
        await _repository.SaveChangesAsync();

        return Result.Ok(new CreateResearchResponse(research.Id.ToString(), research.Title));
    }

    public async Task<Result<ResearchUpdatesResponse>> GetResearchUpdates(
        Guid userId,
        Guid researchId
    )
    {
        var research = await _repository.Research.FirstOrDefaultAsync(r =>
            r.Id == researchId && r.UserId == userId
        );

        // Only return updates if research is in progress
        if (research.Status != ResearchStatus.InProgress)
        {
            return Result.Ok(new ResearchUpdatesResponse(new List<ResearchFeedUpdateEvent>(), []));
        }

        try
        {
            var state = _state.GetState(researchId.ToString());
            return Result.Ok(
                new ResearchUpdatesResponse(state.FeedUpdates ?? [], state.ChatMessages ?? [])
            );
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to retrieve research updates: {ex.Message}");
        }
    }
}
