using Apollo.Agents.Memory;

namespace Apollo.Agents.Events;

public record IngestEvent(Guid ResearchId, IngestRequest Request);

public interface IIngestEventHandler
{
    Task HandleIngest(IngestEvent @event);
}

public class IngestEventHandler : IIngestEventHandler
{
    public Task HandleIngest(IngestEvent @event)
    {
        throw new NotImplementedException();
    }
}
