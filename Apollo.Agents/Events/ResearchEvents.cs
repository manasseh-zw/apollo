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

    public ResearchEventHandler(IResearchNotifier notifier)
    {
        _notifier = notifier;
    }

    public async Task HandleResearchSaved(ResearchSavedEvent @event)
    {
        await _notifier.NotifyResearchSaved(@event.UserId, @event.ResearchId);
    }
}

public interface IResearchNotifier
{
    Task NotifyResearchSaved(string userId, Guid researchId);
}
