using Apollo.Agents.Contracts;
using Apollo.Agents.Helpers;
using Microsoft.AspNetCore.SignalR;

public class ClientUpdateCallback : IClientUpdateCallback
{
    private readonly IHubContext<ResearchHub, IResearchHubClient> _hubContext;

    public ClientUpdateCallback(IHubContext<ResearchHub, IResearchHubClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public void StreamPlannerResponse(string connectionId, string? message)
    {
        _hubContext.Clients.Client(connectionId).ReceiveResponse(message ?? string.Empty);
    }

    public void SendTimelineUpdate(TimelineUpdateEvent update)
    {
        _hubContext.Clients.Group(update.ResearchId).ReceiveTimelineUpdate(update);
    }

    public void SendResearchFeedUpdate(ResearchFeedUpdateEvent update)
    {
        _hubContext.Clients.Group(update.ResearchId).ReceiveResearchFeedUpdate(update);
    }

    public void SendAgentChatMessage(AgentChatMessageEvent message)
    {
        _hubContext.Clients.Group(message.ResearchId).ReceiveAgentChatMessage(message);
    }
}
