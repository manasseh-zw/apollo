using System.Text;
using Apollo.Agents.Events;
using Apollo.Agents.Research;
using Apollo.Agents.Research.Plugins;
using Apollo.Api.Features.Research;
using Apollo.Api.Utils;
using Apollo.Config;
using Apollo.Data.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Apollo.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services)
    {
        services.AddDbContext<ApolloDbContext>(options =>
            options.UseNpgsql(AppConfig.DatabaseOptions.ConnectionString)
        );
        return services;
    }

    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(
                "Bearer",
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        SaveSigninToken = true,

                        ValidateIssuer = true,

                        ValidateAudience = true,

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,

                        ValidIssuer = AppConfig.JwtOptions.Issuer,

                        ValidAudience = AppConfig.JwtOptions.Audience,

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(AppConfig.JwtOptions.Secret)
                        ),
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            ctx.Request.Cookies.TryGetValue(
                                Constants.AccessTokenCookieName,
                                out var accessToken
                            );

                            if (!string.IsNullOrEmpty(accessToken))
                                ctx.Token = accessToken;

                            return Task.CompletedTask;
                        },
                    };
                }
            );

        return services;
    }

    public static IServiceCollection ConfigurePlugins(this IServiceCollection services)
    {
        services.AddScoped<SaveResearchPlugin>();
        services.AddScoped<IResearchEventHandler, ResearchEventHandler>();
        return services;
    }

    public static IServiceCollection ConfigureResearch(this IServiceCollection services)
    {
        services.AddScoped<IChatStreamingCallback, ChatStreamingCallback>();
        services.AddScoped<IResearchAssistant, ResearchAssistant>();
        services.AddScoped<IResearchNotifier, ResearchNotifier>();
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });
        return services;
    }
}
