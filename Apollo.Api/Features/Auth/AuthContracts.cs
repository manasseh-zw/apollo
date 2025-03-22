using System.Text.Json.Serialization;
using Apollo.Data.Models;

namespace Apollo.Api.Features.Auth;

public record EmailSignUpRequest(string Username, string Email, string Password);

public record SignInRequest(string UserIdentifier, string Password);

public record GoogleSignUpRequest(string AccessToken);

public record AuthResult(string Token, UserDataResponse UserData);

public record UserDataResponse(
    Guid Id,
    string Email,
    string? Username,
    string? AvatarUrl,
    bool IsEmailConfirmed,
    DateTime CreatedAt,
    AuthProvider AuthProvider
);

public class GoogleUserInfo
{
    [JsonPropertyName("sub")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Username { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("email_verified")]
    public bool IsEmailVerified { get; set; }

    [JsonPropertyName("picture")]
    public string AvatarUrl { get; set; }
}
