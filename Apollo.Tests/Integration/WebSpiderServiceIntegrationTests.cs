using System.Text.Json;
using Apollo.Crawler;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Apollo.Tests.Integration;

[Trait("Category", "Integration")]
public class WebSpiderServiceIntegrationTests
{
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _output;
    private readonly WebSpiderService _webspider;

    public WebSpiderServiceIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddXUnit(_output);
        });
        _httpClient = new HttpClient();
        _webspider = new WebSpiderService(_httpClient);
    }

    public async Task ShouldScrapeAndReturnContent()
    {
        var url = "https://httpbin.org/html";

        var response = await _webspider.ScrapeAsync(url);
        _output.WriteLine($"Scrape Response: {JsonSerializer.Serialize(response)}");

        Assert.True(response.Success);
        Assert.NotNull(response.Content);
    }

    public async Task ShouldScrapeMultipleResultsAndReturnBatchResults()
    {
        var urls = new List<string>()
        {
            "https://httpbin.org/html",
            "https://httpbin.org/html",
            "https://httpbin.org/html",
            "https://httpbin.org/html",
            "https://httpbin.org/html",
        };

        var response = await _webspider.ScrapeBatchAsync(urls);

        _output.WriteLine($"Scraped : {JsonSerializer.Serialize(response.Results.Count)} urls");

        Assert.True(response.Success);
        Assert.True(response.Results.Count > 0);
    }
}
