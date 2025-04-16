using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Apollo.Api.Features.Research;

public static class ResearchEndpoints
{
    public static void MapResearchEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/research").RequireAuthorization();

        group.MapGet("/{researchId}", GetResearch);
        group.MapGet("/", GetAllResearch);
        group.MapPost("/", CreateResearch);
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
}
