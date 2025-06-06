using System.Text;
using Apollo.Agents.Events;
using Apollo.Agents.Helpers;
using Apollo.Agents.Memory;
using Apollo.Agents.Plugins;
using Apollo.Agents.Research;
using Apollo.Agents.State;
using Apollo.Api.Features.Research;
using Apollo.Api.Utils;
using Apollo.Config;
using Apollo.Data.Repository;
using Apollo.Notifications;
using Apollo.Search;
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

    public static IServiceCollection ConfigureResearch(this IServiceCollection services)
    {
        services.AddScoped<IClientUpdateCallback, ClientUpdateCallback>();
        services.AddScoped<IResearchEventHandler, ResearchEventHandler>();
        services.AddScoped<IResearchPlanner, ResearchPlanner>();
        services.AddScoped<IResearchNotifier, ResearchNotifier>();
        services.AddScoped<ISearchService, ExaSearchService>();
        services.AddScoped<IStateManager, StateManager>();

        services.AddScoped<IMemoryContext, MemoryContext>();
        services.AddScoped<KernelMemoryPlugin>();
        services.AddScoped<ResearchEnginePlugin>();
        services.AddScoped<StartResearchPlugin>();
        services.AddScoped<SynthesizeResearchPlugin>();

        services.AddScoped<IResearchManager, ResearchManager>();
        services.AddScoped<ResearchOrchestrator>();
        services.AddScoped<IResearchReportGenerator, ResearchReportGenerator>();

        services.AddSingleton<IResearchEventsQueue, ResearchEventsQueue>();
        services.AddHostedService<ResearchProcessor>();

        services.AddSingleton<IIngestEventsQueue, IngestEventsQueue>();
        services.AddHostedService<IngestProcessor>();
        services.AddScoped<IIngestEventHandler, IngestEventHandler>();

        services.AddMemoryCache();
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });
        return services;
    }

    public static IServiceCollection ConfigureNotifications(this IServiceCollection services)
    {
        services.AddSingleton<IEmailService, ResendEmailService>();
        return services;
    }
}
