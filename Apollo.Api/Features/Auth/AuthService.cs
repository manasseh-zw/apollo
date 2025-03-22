using System.Net.Http.Headers;
using System.Text.Json;
using Apollo.Data.Models;
using Apollo.Data.Repository;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace Apollo.Api.Features.Auth;

public interface IAuthService
{
    public Task<Result<AuthResult>> EmailSignUp(EmailSignUpRequest request);
    public Task<Result<AuthResult>> SignIn(SignInRequest request);
    public Task<Result<AuthResult>> GoogleSignUp(GoogleSignUpRequest request);
    public Task<Result<UserDataResponse>> GetUserData(Guid userId);
}

public class AuthService : IAuthService
{
    private readonly ApolloDbContext _repository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenManager _jwt;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthService(
        ApolloDbContext repository,
        IPasswordHasher<User> passwordHasher,
        IJwtTokenManager jwt,
        IHttpClientFactory httpClientFactory
    )
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _jwt = jwt;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Result<AuthResult>> EmailSignUp(EmailSignUpRequest request)
    {
        var isEmailTaken = await _repository.Users.AnyAsync(u => u.Email == request.Email);
        if (isEmailTaken)
            return Result.Fail("Email already exists");

        var validationResult = new AuthValidator().Validate(request);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            AuthProvider = AuthProvider.Email,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _repository.Users.AddAsync(user);
        await _repository.SaveChangesAsync();

        var token = _jwt.GenerateToken(user);
        var userData = new UserDataResponse(
            user.Id,
            user.Email,
            user.Username,
            user.AvatarUrl,
            user.IsEmailConfirmed,
            user.CreatedAt,
            user.AuthProvider
        );

        return Result.Ok(new AuthResult(token, userData));
    }

    public async Task<Result<AuthResult>> SignIn(SignInRequest request)
    {
        var validationResult = new SignInValidator().Validate(request);
        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        // Check if the identifier is an email or username
        var user = await _repository.Users.FirstOrDefaultAsync(u =>
            u.Email == request.UserIdentifier || u.Username == request.UserIdentifier
        );

        if (user == null)
            return Result.Fail("Invalid credentials");

        if (user.AuthProvider != AuthProvider.Email)
            return Result.Fail($"Please sign in with {user.AuthProvider.GetDisplayName()}");

        // Verify password
        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password
        );

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            return Result.Fail("Invalid credentials");

        var token = _jwt.GenerateToken(user);
        var userData = new UserDataResponse(
            user.Id,
            user.Email,
            user.Username,
            user.AvatarUrl,
            user.IsEmailConfirmed,
            user.CreatedAt,
            user.AuthProvider
        );

        return Result.Ok(new AuthResult(token, userData));
    }

    public async Task<Result<AuthResult>> GoogleSignUp(GoogleSignUpRequest request)
    {
        if (string.IsNullOrEmpty(request.AccessToken))
        {
            return Result.Fail("Invalid Google access token");
        }

        try
        {
            using var httpClient = _httpClientFactory.CreateClient("Google");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                request.AccessToken
            );

            var userInfoResponse = await httpClient.GetAsync(
                "https://www.googleapis.com/oauth2/v3/userinfo"
            );

            if (!userInfoResponse.IsSuccessStatusCode)
            {
                return Result.Fail("Failed to fetch user data from Google API");
            }

            var content = await userInfoResponse.Content.ReadAsStringAsync();
            var googleUser = JsonSerializer.Deserialize<GoogleUserInfo>(content);

            if (googleUser == null || string.IsNullOrEmpty(googleUser.Email))
            {
                return Result.Fail("Invalid user data received from Google");
            }

            var existingUser = await _repository.Users.FirstOrDefaultAsync(u =>
                u.Email == googleUser.Email
            );

            if (existingUser != null)
            {
                if (existingUser.AuthProvider != AuthProvider.Google)
                {
                    existingUser.AuthProvider = AuthProvider.Google;
                    await _repository.SaveChangesAsync();
                }

                var token = _jwt.GenerateToken(existingUser);
                var userData = new UserDataResponse(
                    existingUser.Id,
                    existingUser.Email,
                    existingUser.Username,
                    existingUser.AvatarUrl,
                    existingUser.IsEmailConfirmed,
                    existingUser.CreatedAt,
                    existingUser.AuthProvider
                );

                return Result.Ok(new AuthResult(token, userData));
            }

            var newUser = new User
            {
                Email = googleUser.Email,
                Username = googleUser.Username,
                AvatarUrl = googleUser.AvatarUrl,
                AuthProvider = AuthProvider.Google,
                IsEmailConfirmed = googleUser.IsEmailVerified,
            };

            await _repository.Users.AddAsync(newUser);
            await _repository.SaveChangesAsync();

            var newToken = _jwt.GenerateToken(newUser);
            var newUserData = new UserDataResponse(
                newUser.Id,
                newUser.Email,
                newUser.Username,
                newUser.AvatarUrl,
                newUser.IsEmailConfirmed,
                newUser.CreatedAt,
                newUser.AuthProvider
            );

            return Result.Ok(new AuthResult(newToken, newUserData));
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error processing Google sign-up: {ex.Message}");
        }
    }

    public async Task<Result<UserDataResponse>> GetUserData(Guid userId)
    {
        var user = await _repository.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return Result.Fail("User not found");
        }

        var response = new UserDataResponse(
            user.Id,
            user.Email,
            user.Username,
            user.AvatarUrl,
            user.IsEmailConfirmed,
            user.CreatedAt,
            user.AuthProvider
        );

        return Result.Ok(response);
    }
}
