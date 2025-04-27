using Apollo.Agents.Events;
using Microsoft.AspNetCore.SignalR;

namespace Apollo.Api.Features.Research;

public class ResearchNotifier : IResearchNotifier
{
    private readonly IHubContext<ResearchHub, IResearchHubClient> _hubContext;

    public ResearchNotifier(IHubContext<ResearchHub, IResearchHubClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyResearchSaved(string userId, Guid researchId)
    {
        await _hubContext.Clients.User(userId).ResearchSaved(researchId);
    }

    public async Task NotifyResearchCompleted(string userId, Guid researchId)
    {
        await _hubContext.Clients.User(userId).ResearchCompleted(researchId);
    }
}
