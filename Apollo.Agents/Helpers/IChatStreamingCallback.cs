namespace Apollo.Agents.Helpers;

public interface IChatStreamingCallback
{
    void StreamPlannerResponse(string connectionId, string message);
    void StreamAgentResponse(string researchId, string message);
    void SendResearchProgressUpdate(string researchId, string update);
}
