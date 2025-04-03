using Apollo.Config;
using Apollo.Search;
using Apollo.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Apollo.Tests.Search;

public class ExaSearchServiceTests
{
    private readonly ITestOutputHelper _output;
    private const string TestApiKey = "test-api-key";

    public ExaSearchServiceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task SearchAsync_ValidRequest_ReturnsSearchResponse()
    {
        // Arrange
        var expectedResponse = new SearchResponse
        {
            Results = new List<SearchResult>
            {
                new()
                {
                    Title = "Test Result",
                    Url = "https://test.com",
                    Author = "Test Author",
                    Text = "Test content",
                },
            },
            SearchType = "neural",
        };

        var handler = MockHttpMessageHandler.CreateMockHandler(expectedResponse);
        var httpClient = new HttpClient(handler);
        
        // Create logger factory and logger
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddXUnit(_output);
        });
        var logger = loggerFactory.CreateLogger<ExaSearchService>();
        
        var searchService = new ExaSearchService(httpClient, logger, TestApiKey);

        var request = new SearchRequest
        {
            Query = "test query",
            Type = "neural",
            NumResults = 1,
        };

        // Act
        var result = await searchService.SearchAsync(request);

        // Log the response
        _output.WriteLine($"Search Response: {System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Results);
        Assert.Equal(expectedResponse.SearchType, result.SearchType);
        Assert.Equal(expectedResponse.Results[0].Title, result.Results[0].Title);
    }

    [Fact]
    public async Task FindSimilarAsync_ValidUrl_ReturnsSearchResponse()
    {
        // Arrange
        var expectedResponse = new SearchResponse
        {
            Results = new List<SearchResult>
            {
                new()
                {
                    Title = "Similar Result",
                    Url = "https://similar.com",
                    Score = 0.95,
                },
            },
        };

        var handler = MockHttpMessageHandler.CreateMockHandler(expectedResponse);
        var httpClient = new HttpClient(handler);
        
        // Create logger factory and logger
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddXUnit(_output);
        });
        var logger = loggerFactory.CreateLogger<ExaSearchService>();
        
        var searchService = new ExaSearchService(httpClient, logger, TestApiKey);

        // Act
        var result = await searchService.FindSimilarAsync("https://test.com", 1);

        // Log the response
        _output.WriteLine($"Find Similar Response: {System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Results);
        Assert.Equal(expectedResponse.Results[0].Title, result.Results[0].Title);
        Assert.Equal(expectedResponse.Results[0].Score, result.Results[0].Score);
    }
}
