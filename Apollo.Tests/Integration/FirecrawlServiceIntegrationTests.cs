using Apollo.Crawler;
using Apollo.Crawler.Models;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Apollo.Tests.Integration;

[Trait("Category", "Integration")]
public class FirecrawlServiceIntegrationTests : IDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly FirecrawlService _crawlerService;
    private readonly string? _apiKey;
    private readonly HttpClient _httpClient;

    public FirecrawlServiceIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
        _httpClient = new HttpClient();

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddXUnit(_output);
        });
        var logger = loggerFactory.CreateLogger<FirecrawlService>();

        _crawlerService = new FirecrawlService(_httpClient, logger, _apiKey);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    [SkipIfNoApiKeyFact]
    public async Task ScrapeAsync_RealApi_ReturnsContent()
    {
        // Arrange
        var request = new ScrapeRequest
        {
            Url = "https://www.example.com",
            Formats = ["markdown", "html"],
            OnlyMainContent = true,
        };

        // Act
        var result = await _crawlerService.ScrapeAsync(request);

        // Log the response
        _output.WriteLine(
            $"Scrape Response: {System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}"
        );

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data.Markdown);
        Assert.NotEmpty(result.Data.Html);
        Assert.NotNull(result.Data.Metadata);
        Assert.Equal(200, result.Data.Metadata.StatusCode);
    }

    [SkipIfNoApiKeyFact]
    public async Task MapAsync_RealApi_ReturnsLinks()
    {
        // Arrange
        var request = new MapRequest { Url = "https://www.example.com", Limit = 5 };

        // Act
        var result = await _crawlerService.MapAsync(request);

        // Log the response
        _output.WriteLine(
            $"Map Response: {System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}"
        );

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotEmpty(result.Links);
        Assert.All(
            result.Links,
            link => Assert.True(Uri.IsWellFormedUriString(link, UriKind.Absolute))
        );
    }
}
