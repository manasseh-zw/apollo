using Apollo.Api.Extensions;
using Apollo.Api.Features.Auth;
using Apollo.Api.Features.Research;
using Apollo.Config;
using Apollo.Data.Models;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

AppConfig.Initialize();

builder.Services.AddOpenApi();
builder.Services.ConfigureDatabase();
builder.Services.ConfigureAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "apollo",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    );
});

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IJwtTokenManager, JwtTokenManager>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IResearchService, ResearchService>();
builder.Services.ConfigureResearch();
builder.Services.AddHttpClient(
    "Google",
    client =>
    {
        client.BaseAddress = new Uri("https://www.googleapis.com/");
        client.DefaultRequestHeaders.Add("User-Agent", "Apollo/1.0");
    }
);

var app = builder.Build();

app.UseCors("apollo");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapAuthEndpoints();
app.MapResearchEndpoints();

app.MapHub<ResearchHub>("/hubs/research");

app.Run();
