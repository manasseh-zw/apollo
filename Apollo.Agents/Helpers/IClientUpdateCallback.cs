using Apollo.Agents.Contracts;

namespace Apollo.Agents.Helpers;

public interface IClientUpdateCallback
{
    void StreamPlannerResponse(string connectionId, string message);
    void SendQuestionTimelineUpdate(QuestionTimelineUpdateEvent update);
    void SendResearchFeedUpdate(ResearchFeedUpdateEvent update);
    void SendAgentChatMessage(AgentChatMessageEvent message);
}
