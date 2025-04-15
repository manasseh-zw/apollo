using Apollo.Agents.Research;

namespace Apollo.Agents.Events;

public class ResearchSavedEvent
{
    public Guid ResearchId { get; set; }
    public string UserId { get; set; }
}

public interface IResearchEventHandler
{
    Task HandleResearchSaved(ResearchSavedEvent @event);
}

public class ResearchEventHandler : IResearchEventHandler
{
    private readonly IResearchNotifier _notifier;
    private readonly IResearchProcessor _processor;

    public ResearchEventHandler(IResearchNotifier notifier, IResearchProcessor processor)
    {
        _notifier = notifier;
        _processor = processor;
    }

    public async Task HandleResearchSaved(ResearchSavedEvent @event)
    {
        await _notifier.NotifyResearchSaved(@event.UserId, @event.ResearchId);
        await _processor.EnqueueResearch(@event);
    }
}

public interface IResearchNotifier
{
    Task NotifyResearchSaved(string userId, Guid researchId);
}
