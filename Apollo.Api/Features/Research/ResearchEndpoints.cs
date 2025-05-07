using System.Security.Claims;
using Apollo.Notifications;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Apollo.Api.Features.Research;

public static class ResearchEndpoints
{
    public static void MapResearchEndpoints(this WebApplication app)
    {
        // Protected endpoints
        var group = app.MapGroup("/api/research").RequireAuthorization();
        group.MapGet("/{researchId}", GetResearch);
        group.MapGet("/", GetAllResearch);
        group.MapGet("/history", GetResearchHistory);
        group.MapPost("/", CreateResearch);
        group.MapGet("/{researchId}/updates", GetResearchUpdates);

        // Public endpoints
        app.MapGet("/api/research/share/{reportId}", GetSharedResearchReport).AllowAnonymous();
        app.MapPost("/api/research/email", TestEmail).AllowAnonymous();
    }

    private static async Task<
        Results<Ok<SharedResearchReportResponse>, NotFound>
    > GetSharedResearchReport(
        [FromRoute] Guid reportId,
        [FromServices] IResearchService researchService
    )
    {
        var result = await researchService.GetSharedResearchReport(reportId);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<
        Results<Ok<PaginatedResponse<ResearchHistoryItemResponse>>, UnauthorizedHttpResult>
    > GetResearchHistory(
        [FromServices] IResearchService researchService,
        HttpContext httpContext,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5
    )
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return TypedResults.Unauthorized();
        }

        var result = await researchService.GetResearchHistory(userId, page, pageSize);
        return TypedResults.Ok(result.Value);
    }

    private static async Task<
        Results<Ok<ResearchResponse>, NotFound, UnauthorizedHttpResult>
    > GetResearch(
        [FromRoute] Guid researchId,
        [FromServices] IResearchService researchService,
        HttpContext httpContext
    )
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return TypedResults.Unauthorized();
        }

        var result = await researchService.GetResearch(userId, researchId);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<
        Results<Ok<List<ResearchResponse>>, UnauthorizedHttpResult>
    > GetAllResearch([FromServices] IResearchService researchService, HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return TypedResults.Unauthorized();
        }

        var result = await researchService.GetAllResearch(userId);

        return TypedResults.Ok(result.Value);
    }

    private static async Task<
        Results<Ok<CreateResearchResponse>, UnauthorizedHttpResult>
    > CreateResearch(
        [FromServices] IResearchService researchService,
        HttpContext httpContext,
        [FromBody] CreateResearchRequest request
    )
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return TypedResults.Unauthorized();
        }

        var result = await researchService.CreateResearch(userId, request);

        return TypedResults.Ok(result.Value);
    }

    private static async Task<
        Results<Ok<ResearchUpdatesResponse>, NotFound, UnauthorizedHttpResult>
    > GetResearchUpdates(
        [FromRoute] Guid researchId,
        [FromServices] IResearchService researchService,
        HttpContext httpContext
    )
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return TypedResults.Unauthorized();
        }

        var result = await researchService.GetResearchUpdates(userId, researchId);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<string>, NotFound, UnauthorizedHttpResult>> TestEmail(
        [FromServices] IEmailService emailService,
        HttpContext httpContext
    )
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return TypedResults.Unauthorized();
        }

        var result = await emailService.SendResearchCompleteNotification(
            new Notifications.Models.Recipient("Manasseh", "dev.manasseh@gmail.com"),
            new Notifications.Models.ResearchCompleteContent(
                "0196a2d6-c8eb-779a-9acb-40b18b474868",
                "Amazing topic"
            )
        );

        if (!result)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok("sent successfully");
    }
}
