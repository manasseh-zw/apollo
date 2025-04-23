using Apollo.Agents.Events;
using Apollo.Search.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Apollo.Agents.Memory;

public class IngestProcessor : BackgroundService
{
    private readonly IIngestEventsQueue _queue;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<IngestProcessor> _logger;

    public IngestProcessor(
        IIngestEventsQueue queue,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<IngestProcessor> logger
    )
    {
        _queue = queue;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var ingestEvent in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation(
                    "Processing ingest event for research {ResearchId}",
                    ingestEvent.ResearchId
                );

                using var scope = _serviceScopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IIngestEventHandler>();
                handler.HandleIngest(ingestEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing ingest event for research {ResearchId}: {Message}",
                    ingestEvent.ResearchId,
                    ex.Message
                );
            }
        }
    }
}

