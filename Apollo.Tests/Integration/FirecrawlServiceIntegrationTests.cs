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
    public async Task ShouldScrapeAndReturnContent()
    {
        var url = "https://httpbin.org/html";

        var result = await _crawlerService.ScrapeAsync(url);

        _output.WriteLine($"Scrape Response: {JsonSerializer.Serialize(result)}");

        Assert.NotNull(result);
        Assert.True(result.Success);
    }
}
