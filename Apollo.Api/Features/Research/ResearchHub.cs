using Apollo.Agents.Contracts;
using Apollo.Agents.Events;
using Apollo.Agents.Research;
using Microsoft.AspNetCore.SignalR;

public interface IResearchHubClient
{
    Task ReceiveResponse(string response);
    Task ResearchSaved(Guid researchId);
    Task ResearchCompleted(Guid researchId);
    Task ResearchCompletedWithReport(Guid researchId, ResearchReportResponse report);

    Task ReceiveTimelineUpdate(TimelineUpdateEvent update);
    Task ReceiveResearchFeedUpdate(ResearchFeedUpdateEvent update);
    Task ReceiveAgentChatMessage(AgentChatMessageEvent message);
}

public class ResearchHub : Hub<IResearchHubClient>
{
    private readonly IResearchPlanner _assistant;

    public ResearchHub(IResearchPlanner assistant)
    {
        _assistant = assistant;
    }

    public async Task ReceiveMessage(string message)
    {
        await _assistant.ContinueChatSession(message, Context.ConnectionId);
    }

    public async Task StartResearchChat(string sessionId, string initialQuery)
    {
        var userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId))
            return;

        await _assistant.StartChatSession(sessionId, initialQuery, Context.ConnectionId, userId);
    }

    public async Task JoinResearchStream(string researchId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, researchId);
    }

    public async Task LeaveResearchStream(string researchId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, researchId);
    }
}
