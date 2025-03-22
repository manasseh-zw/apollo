using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Apollo.Config;
using Apollo.Data.Models;
using Microsoft.IdentityModel.Tokens;

namespace Apollo.Api.Features.Auth;

public interface IJwtTokenManager
{
    string GenerateToken(User user);

    string GenerateEmailConfirmationToken(User user);
}

public class JwtTokenManager : IJwtTokenManager
{
    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfig.JwtOptions.Secret));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: AppConfig.JwtOptions.Issuer,
            audience: AppConfig.JwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(14),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateEmailConfirmationToken(User user)
    {
        var claims = new[] { new Claim(ClaimTypes.Email, user.Email) };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfig.JwtOptions.Secret));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
