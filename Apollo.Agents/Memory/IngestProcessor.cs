using Apollo.Agents.Events;
using Apollo.Crawler;
using Apollo.Search.Models;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;

namespace Apollo.Agents.Memory;

public class IngestProcessor : BackgroundService
{
    private readonly IIngestEventsQueue _queue;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<IngestProcessor> _logger;
    private readonly IMemoryContext _memoryContext;
    private readonly ICrawlerService _crawler;

    public IngestProcessor(
        IIngestEventsQueue queue,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<IngestProcessor> logger,
        IMemoryContext memoryContext,
        ICrawlerService crawler
    )
    {
        _queue = queue;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _memoryContext = memoryContext;
        _crawler = crawler;
    }

    public async Task EnqueueJob(IngestEvent @event)
    {
        await _queue.Writer.WriteAsync(@event);
        _logger.LogInformation(
            "Enqueued ingest event for search query : {Query} of researchId: {researchId}, and question: {question} for processing",
            @event.Request.searchContext.Query,
            @event.ResearchId,
            @event.Request.searchContext.ResearchQuestion
        );
    }

    public async Task ProcessIngest(IngestEvent @event)
    {
        var memory = _memoryContext.GetMemoryContextInstance();
        var urls = @event.Request.SearchResult.Results.Select(r => r.Url).ToList();
        var crawlResponse = await _crawler.ScrapeBatchAsync(urls);
        // imporove performance here
        var IngestTasks = new List<Task>();
        if (crawlResponse.Success)
        {
            crawlResponse.Results.ForEach(r =>
            {
                var tags = @event
                    .Request.SearchResult.Results.Where(sr => sr.Url == r.Url)
                    .Select(x => new TagCollection()
                    {
                        { "ResearchId", @event.ResearchId.ToString() },
                        { "ResearchQuestion", @event.Request.searchContext.ResearchQuestion },
                        { "Source", x.Url },
                        { "SearchQuery", @event.Request.searchContext.Query },
                        { "Title", x.Title },
                        { "PublicationDate", x.PublishedDate },
                        { "Author", x.Author },
                        { "Highlights", x.Highlights ?? [] },
                    })
                    .FirstOrDefault();

                _memoryContext.Ingest(r.Content, tags: tags ?? []);
                // use await.all etc to init the ingestion concurently all at once
            });
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}

public record IngestRequest(WebSearchContext searchContext, WebSearchResponse SearchResult);

public record WebSearchContext(string ResearchId, string ResearchQuestion, string Query);
