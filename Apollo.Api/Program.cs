using Apollo.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    AppConfig.Initialize();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
