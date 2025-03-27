using Apollo.Agents.Planner;
using Microsoft.AspNetCore.SignalR;

public interface IResearchHubClient
{
    Task ReceiveResponse(string response);
}

public class ResearchHub : Hub<IResearchHubClient>
{
    private readonly IResearchAssistant _assistant;

    public ResearchHub(IResearchAssistant assistant)
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
}
