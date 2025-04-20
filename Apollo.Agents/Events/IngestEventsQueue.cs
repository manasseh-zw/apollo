using System.Threading.Channels;

namespace Apollo.Agents.Events;

public interface IIngestEventsQueue
{
    ChannelReader<IngestEvent> Reader { get; }
    ChannelWriter<IngestEvent> Writer { get; }
}

public class IngestEventsQueue : IIngestEventsQueue
{
    private readonly Channel<IngestEvent> _channel;

    public IngestEventsQueue()
    {
        _channel = Channel.CreateUnbounded<IngestEvent>(
            new UnboundedChannelOptions { SingleReader = true }
        );
    }

    public ChannelReader<IngestEvent> Reader => _channel.Reader;
    public ChannelWriter<IngestEvent> Writer => _channel.Writer;
}
