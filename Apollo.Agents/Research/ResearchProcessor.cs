using System.Threading.Channels;
using Apollo.Agents.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Apollo.Agents.Research;

public interface IResearchProcessor
{
    Task EnqueueResearch(ResearchSavedEvent researchEvent);
}

public class ResearchProcessor : BackgroundService, IResearchProcessor
{
    private readonly Channel<ResearchSavedEvent> _channel;
    private readonly ResearchOrchestrator _orchestrator;
    private readonly ILogger<ResearchProcessor> _logger;

    public ResearchProcessor(ResearchOrchestrator orchestrator, ILogger<ResearchProcessor> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
        _channel = Channel.CreateUnbounded<ResearchSavedEvent>(
            new UnboundedChannelOptions { SingleReader = true }
        );
    }

    public async Task EnqueueResearch(ResearchSavedEvent researchEvent)
    {
        await _channel.Writer.WriteAsync(researchEvent);
        _logger.LogInformation(
            "Enqueued research {ResearchId} for processing",
            researchEvent.ResearchId
        );
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var researchEvent in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation(
                    "Starting research process for {ResearchId}",
                    researchEvent.ResearchId
                );
                await _orchestrator.StartResearchProcessAsync(researchEvent.ResearchId.ToString());
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
