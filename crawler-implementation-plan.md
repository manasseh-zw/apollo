# Implementing Apollo.Crawler Module

## 1. Project Setup
- Create Apollo.Crawler project
- Add necessary package references
- Add project reference to Apollo.Config

## 2. Models
Create the following model classes:
- ScrapeRequest
  - url: string
  - formats: string[]
  - onlyMainContent: bool
  - includeTags: string[]
  - excludeTags: string[]
  - waitFor: int
  - mobile: bool
  - timeout: int

- ScrapeResponse
  - success: bool
  - data: ScrapeData
    - markdown: string
    - html: string
    - rawHtml: string
    - screenshot: string
    - links: string[]
    - metadata: Metadata

- MapRequest
  - url: string
  - search: string
  - ignoreSitemap: bool
  - sitemapOnly: bool
  - includeSubdomains: bool
  - limit: int

- MapResponse
  - success: bool
  - links: string[]

- CrawlRequest/Response following API spec

## 3. Service Interface
```csharp
public interface ICrawlerService
{
    Task<ScrapeResponse> ScrapeAsync(ScrapeRequest request);
    Task<MapResponse> MapAsync(MapRequest request);
    Task<CrawlResponse> CrawlAsync(CrawlRequest request);
    Task<CrawlStatusResponse> GetCrawlStatusAsync(string id);
}
```

## 4. Implementation
1. Create FirecrawlService implementing ICrawlerService
2. Add logging support
3. Handle API authentication
4. Implement error handling
5. Add response deserialization

## 5. Tests
1. Create FirecrawlServiceTests
2. Mock HTTP responses
3. Test all main endpoints
4. Test error cases

## 6. Integration
1. Add service registration in ServiceExtensions
2. Update AppConfig with Firecrawl settings