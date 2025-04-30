using Apollo.Agents.Events;
using Apollo.Data.Models;
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

    public async Task NotifyResearchCompletedWithReport(string userId, Guid researchId, ResearchReport report)
    {
        await _hubContext.Clients.User(userId).ResearchCompletedWithReport(researchId, report);
    }
}
