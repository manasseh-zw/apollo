using Apollo.Crawler;
using Apollo.Crawler.Models;
using Apollo.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Apollo.Tests.Crawler;

public class FirecrawlServiceTests
{
    private readonly ITestOutputHelper _output;
    private const string TestApiKey = "test-api-key";

    public FirecrawlServiceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task ScrapeAsync_ValidRequest_ReturnsScrapeResponse()
    {
        // Arrange
        var expectedResponse = new ScrapeResponse
        {
            Success = true,
            Data = new ScrapeData
            {
                Markdown = "# Test Content",
                Html = "<h1>Test Content</h1>",
                Metadata = new Metadata
                {
                    Title = "Test Page",
                    SourceURL = "https://test.com",
                    StatusCode = 200,
                },
            },
        };

        var handler = MockHttpMessageHandler.CreateMockHandler(expectedResponse);
        var httpClient = new HttpClient(handler);
        
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddXUnit(_output);
        });
        var logger = loggerFactory.CreateLogger<FirecrawlService>();
        
        var crawlerService = new FirecrawlService(httpClient, logger, TestApiKey);

        var request = new ScrapeRequest
        {
            Url = "https://test.com",
            Formats = ["markdown", "html"],
            OnlyMainContent = true,
        };

        // Act
        var result = await crawlerService.ScrapeAsync(request);

        // Log the response
        _output.WriteLine($"Scrape Response: {System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(expectedResponse.Data.Markdown, result.Data.Markdown);
        Assert.Equal(expectedResponse.Data.Html, result.Data.Html);
    }

    [Fact]
    public async Task MapAsync_ValidRequest_ReturnsMapResponse()
    {
        // Arrange
        var expectedResponse = new MapResponse
        {
            Success = true,
            Links = 
            [
                "https://test.com/page1",
                "https://test.com/page2",
            ],
        };

        var handler = MockHttpMessageHandler.CreateMockHandler(expectedResponse);
        var httpClient = new HttpClient(handler);
        
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddXUnit(_output);
        });
        var logger = loggerFactory.CreateLogger<FirecrawlService>();
        
        var crawlerService = new FirecrawlService(httpClient, logger, TestApiKey);

        var request = new MapRequest
        {
            Url = "https://test.com",
            Search = "test",
            Limit = 2,
        };

        // Act
        var result = await crawlerService.MapAsync(request);

        // Log the response
        _output.WriteLine($"Map Response: {System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(2, result.Links.Count);
        Assert.Equal(expectedResponse.Links[0], result.Links[0]);
        Assert.Equal(expectedResponse.Links[1], result.Links[1]);
    }
}