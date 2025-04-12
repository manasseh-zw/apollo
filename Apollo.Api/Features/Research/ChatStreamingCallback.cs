using Apollo.Agents.Helpers;
using Microsoft.AspNetCore.SignalR;

public class ChatStreamingCallback : IChatStreamingCallback
{
    private readonly IHubContext<ResearchHub, IResearchHubClient> _hubContext;

    public ChatStreamingCallback(IHubContext<ResearchHub, IResearchHubClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public void StreamPlannerResponse(string connectionId, string? message)
    {
        _hubContext.Clients.Client(connectionId).ReceiveResponse(message ?? string.Empty);
    }

    public void StreamAgentResponse(string researchId, string message)
    {
        _hubContext.Clients.Group(researchId).RecieveAgentChatUpdate(message);
    }

    public void SendCrawlProgressUpdate(string researchId, string update)
    {
        _hubContext.Clients.Group(researchId).RecieveCrawlProgressUpdate(update);
    }
}
