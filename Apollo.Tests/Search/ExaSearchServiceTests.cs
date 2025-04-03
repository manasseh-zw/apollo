using Apollo.Search;
using Apollo.Tests.Helpers;
using Xunit;

namespace Apollo.Tests.Search;

public class ExaSearchServiceTests
{
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
        var searchService = new ExaSearchService(httpClient);

        var request = new SearchRequest
        {
            Query = "test query",
            Type = "neural",
            NumResults = 1,
        };

        // Act
        var result = await searchService.SearchAsync(request);

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
        var searchService = new ExaSearchService(httpClient);

        // Act
        var result = await searchService.FindSimilarAsync("https://test.com", 1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Results);
        Assert.Equal(expectedResponse.Results[0].Title, result.Results[0].Title);
        Assert.Equal(expectedResponse.Results[0].Score, result.Results[0].Score);
    }
}
