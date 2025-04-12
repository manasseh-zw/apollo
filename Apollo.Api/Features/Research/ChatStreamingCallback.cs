using Apollo.Agents.Research;
using Microsoft.AspNetCore.SignalR;

public class ChatStreamingCallback : IChatStreamingCallback
{
    private readonly IHubContext<ResearchHub, IResearchHubClient> _hubContext;

    public ChatStreamingCallback(IHubContext<ResearchHub, IResearchHubClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public void OnStreamResponse(string connectionId, string? message)
    {
        _hubContext.Clients.Client(connectionId).ReceiveResponse(message ?? string.Empty);
    }
}
