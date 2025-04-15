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

    public void StreamAgentMessage(string researchId, string author, string message)
    {
        _hubContext.Clients.Group(researchId).RecieveAgentMessage(author, message);
    }

    public void SendResearchProgressUpdate(string researchId, string update)
    {
        _hubContext.Clients.Group(researchId).RecieveResearchProgressUpdate(update);
    }
}
