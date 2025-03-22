using System.Security.Claims;
using Apollo.Api.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Apollo.Api.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/signup", EmailSignUp);
        group.MapPost("/signin", SignIn);
        group.MapPost("/google-signup", GoogleSignUp);
        group.MapGet("/me", GetUserData).RequireAuthorization();
    }

    private static async Task<Results<Ok<UserDataResponse>, BadRequest<List<string>>>> EmailSignUp(
        [FromBody] EmailSignUpRequest request,
        [FromServices] IAuthService authService,
        HttpContext httpContext
    )
    {
        var result = await authService.EmailSignUp(request);

        if (result.IsFailed)
        {
            return TypedResults.BadRequest(result.Errors.Select(e => e.Message).ToList());
        }

        // Set the JWT token in an HTTP-only cookie
        SetAuthCookie(httpContext, result.Value.Token);

        // Return only the user data
        return TypedResults.Ok(result.Value.UserData);
    }

    private static async Task<Results<Ok<UserDataResponse>, BadRequest<List<string>>>> SignIn(
        [FromBody] SignInRequest request,
        [FromServices] IAuthService authService,
        HttpContext httpContext
    )
    {
        var result = await authService.SignIn(request);

        if (result.IsFailed)
        {
            return TypedResults.BadRequest(result.Errors.Select(e => e.Message).ToList());
        }

        // Set the JWT token in an HTTP-only cookie
        SetAuthCookie(httpContext, result.Value.Token);

        // Return only the user data
        return TypedResults.Ok(result.Value.UserData);
    }

    private static async Task<Results<Ok<UserDataResponse>, BadRequest<List<string>>>> GoogleSignUp(
        [FromBody] GoogleSignUpRequest request,
        [FromServices] IAuthService authService,
        HttpContext httpContext
    )
    {
        var result = await authService.GoogleSignUp(request);

        if (result.IsFailed)
        {
            return TypedResults.BadRequest(result.Errors.Select(e => e.Message).ToList());
        }

        SetAuthCookie(httpContext, result.Value.Token);

        return TypedResults.Ok(result.Value.UserData);
    }

    private static async Task<
        Results<Ok<UserDataResponse>, NotFound, UnauthorizedHttpResult>
    > GetUserData([FromServices] IAuthService authService, HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return TypedResults.Unauthorized();
        }

        var result = await authService.GetUserData(userId);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }

    private static void SetAuthCookie(HttpContext httpContext, string token)
    {
        httpContext.Response.Cookies.Append(
            Constants.AccessTokenCookieName,
            token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(14),
            }
        );
    }
}
