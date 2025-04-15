namespace Apollo.Agents.Helpers;

public interface IClientUpdateCallback
{
    void StreamPlannerResponse(string connectionId, string message);
    void StreamAgentMessage(string researchId, string author, string message);
    void SendResearchProgressUpdate(string researchId, string update);
}
