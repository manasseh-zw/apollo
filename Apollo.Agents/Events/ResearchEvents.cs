using Apollo.Agents.Research;
using Apollo.Data.Models;

namespace Apollo.Agents.Events;

public class ResearchStartEvent
{
    public Guid ResearchId { get; set; }
    public string UserId { get; set; }
}

public class ResearchCompletedEvent
{
    public Guid ResearchId { get; set; }
    public string UserId { get; set; }
}

public class ResearchCompletedWithReportEvent
{
    public Guid ResearchId { get; set; }
    public string UserId { get; set; }
    public ResearchReport Report { get; set; }
}

public interface IResearchEventHandler
{
    Task HandleResearchStart(ResearchStartEvent @event);
    Task HandleResearchCompleted(ResearchCompletedEvent @event);
    Task HandleResearchCompletedWithReport(ResearchCompletedWithReportEvent @event);
}

public class ResearchEventHandler : IResearchEventHandler
{
    private readonly IResearchNotifier _notifier;
    private readonly IResearchEventsQueue _queue;

    public ResearchEventHandler(IResearchNotifier notifier, IResearchEventsQueue queue)
    {
        _notifier = notifier;
        _queue = queue;
    }

    public async Task HandleResearchStart(ResearchStartEvent @event)
    {
        await _notifier.NotifyResearchSaved(@event.UserId, @event.ResearchId);
        await _queue.Writer.WriteAsync(@event);
    }

    public async Task HandleResearchCompleted(ResearchCompletedEvent @event)
    {
        await _notifier.NotifyResearchCompleted(@event.UserId, @event.ResearchId);
    }

    public async Task HandleResearchCompletedWithReport(ResearchCompletedWithReportEvent @event)
    {
        await _notifier.NotifyResearchCompletedWithReport(@event.UserId, @event.ResearchId, @event.Report);
    }
}

public interface IResearchNotifier
{
    Task NotifyResearchSaved(string userId, Guid researchId);
    Task NotifyResearchCompleted(string userId, Guid researchId);
    Task NotifyResearchCompletedWithReport(string userId, Guid researchId, ResearchReport report);
}
