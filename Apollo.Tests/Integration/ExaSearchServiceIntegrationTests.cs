using Apollo.Search;
using Apollo.Search.Models;
using Apollo.Config;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Apollo.Tests.Integration;

[Trait("Category", "Integration")]
public class ExaSearchServiceIntegrationTests : IDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly ExaSearchService _searchService;
    private readonly string? _apiKey;
    private readonly HttpClient _httpClient;

    public ExaSearchServiceIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
        _apiKey = Environment.GetEnvironmentVariable("EXA_API_KEY");
        _httpClient = new HttpClient();

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddXUnit(_output);
        });
        var logger = loggerFactory.CreateLogger<ExaSearchService>();

        _searchService = new ExaSearchService(_httpClient, logger, _apiKey);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    [SkipIfNoApiKeyFact]
    public async Task SearchAsync_RealApi_ReturnsResults()
    {
        // Arrange
        var request = new SearchRequest
        {
            Query = "artificial intelligence latest developments",
            Type = "neural",
            NumResults = 3,
        };

        // Act
        var result = await _searchService.SearchAsync(request);

        // Log the response
        _output.WriteLine(
            $"Search Response: {System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}"
        );

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Results);
        Assert.All(result.Results, item =>
        {
            Assert.NotNull(item.Title);
            Assert.NotNull(item.Url);
            Assert.True(Uri.IsWellFormedUriString(item.Url, UriKind.Absolute));
        });
    }

    [SkipIfNoApiKeyFact]
    public async Task FindSimilarAsync_RealApi_ReturnsResults()
    {
        // Arrange
        var url = "https://www.nature.com/articles/d41586-023-03266-1";

        // Act
        var result = await _searchService.FindSimilarAsync(url, 3);

        // Log the response
        _output.WriteLine(
            $"Similar Results: {System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}"
        );

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Results);
        Assert.All(result.Results, item =>
        {
            Assert.NotNull(item.Title);
            Assert.NotNull(item.Url);
            Assert.True(Uri.IsWellFormedUriString(item.Url, UriKind.Absolute));
            Assert.NotNull(item.Score);
            Assert.True(item.Score > 0);
        });
    }
}