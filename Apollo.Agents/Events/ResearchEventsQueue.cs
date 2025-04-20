using System.Threading.Channels;

namespace Apollo.Agents.Events;

public interface IResearchEventsQueue
{
    ChannelReader<ResearchStartEvent> Reader { get; }
    ChannelWriter<ResearchStartEvent> Writer { get; }
}

public class ResearchEventsQueue : IResearchEventsQueue
{
    private readonly Channel<ResearchStartEvent> _channel;

    public ResearchEventsQueue()
    {
        _channel = Channel.CreateUnbounded<ResearchStartEvent>(
            new UnboundedChannelOptions { SingleReader = true }
        );
    }

    public ChannelReader<ResearchStartEvent> Reader => _channel.Reader;
    public ChannelWriter<ResearchStartEvent> Writer => _channel.Writer;
}
