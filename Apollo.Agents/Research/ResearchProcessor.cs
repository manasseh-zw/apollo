using System.Threading.Channels;
using Apollo.Agents.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Apollo.Agents.Research;

public interface IResearchProcessor
{
    Task EnqueueResearch(ResearchStartEvent researchEvent);
    Task ProcessResearch(
        ResearchStartEvent researchEvent,
        CancellationToken cancellationToken = default
    );
}

public class ResearchProcessor : BackgroundService
{
    private readonly IResearchMessageQueue _queue;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ResearchProcessor> _logger;

    public ResearchProcessor(
        IResearchMessageQueue queue,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ResearchProcessor> logger
    )
    {
        _queue = queue;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task EnqueueResearch(ResearchStartEvent researchEvent)
    {
        await _queue.Writer.WriteAsync(researchEvent);
        _logger.LogInformation(
            "Enqueued research {ResearchId} for processing",
            researchEvent.ResearchId
        );
    }

    public async Task ProcessResearch(
        ResearchStartEvent researchEvent,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("Processing research {ResearchId}", researchEvent.ResearchId);

        using var scope = _serviceScopeFactory.CreateScope();
        var orchestrator = scope.ServiceProvider.GetRequiredService<ResearchOrchestrator>();
        await orchestrator.StartResearchProcessAsync(researchEvent.ResearchId.ToString());
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var researchEvent in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation(
                    "Starting research process for {ResearchId}",
                    researchEvent.ResearchId
                );

                await ProcessResearch(researchEvent, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing research {ResearchId}: {Message}",
                    researchEvent.ResearchId,
                    ex.Message
                );
            }
        }

        
    }
}
