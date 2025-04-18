using Apollo.Crawler;
using Firecrawl;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Apollo.Tests.Unit;

public class FirecrawlServiceTests
{
    private readonly FirecrawlService _service;

    public FirecrawlServiceTests()
    {
        _service = new FirecrawlService();
    }

    [Fact]
    public async Task ScrapeAsync_ValidRequest_ReturnsResponse()
    {
        // Arrange
        var request = new ScrapeAndExtractFromUrlRequest
        {
            Url = "https://www.example.com",
            Formats = [ScrapeAndExtractFromUrlRequestFormat.Markdown],
            RemoveBase64Images = true,
        };

        // Act
        var result = await _service.ScrapeAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Markdown);
    }

    [Fact]
    public async Task MapAsync_ValidRequest_ReturnsResponse()
    {
        // Arrange
        var request = new MapUrlsRequest { Url = "https://www.example.com", Limit = 5 };

        // Act
        var result = await _service.MapAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Links);
    }
}
