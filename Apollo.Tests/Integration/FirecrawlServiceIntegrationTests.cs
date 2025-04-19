using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Apollo.Crawler;
using Firecrawl;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Apollo.Tests.Integration;

[Trait("Category", "Integration")]
public class FirecrawlServiceIntegrationTests
{
    private readonly ITestOutputHelper _output;
    private readonly FirecrawlService _crawlerService;

    public FirecrawlServiceIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddXUnit(_output);
        });

        _crawlerService = new FirecrawlService();
    }

    [SkipIfNoApiKeyFact]
    public async Task ScrapeAsync_RealApi_ReturnsContent()
    {
        // Arrange
        var url = "www.example.com";
        // Act
        var result = await _crawlerService.ScrapeAsync(url);

        // Log the response
        _output.WriteLine(
            $"Scrape Response: {JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true })}"
        );

        Assert.NotNull(result);
        Assert.True(result.Success);
    }
}
