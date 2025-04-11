using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Apollo.Agents.Research;

public class ResearchQueryProcessor
{
    public static ChatCompletionAgent Create(Kernel kernel)
    {
        return new ChatCompletionAgent
        {
            Kernel = kernel,
            Name = "QueryStrategist",
            Instructions = """
                Call the websearch plugin
                """,
        };
    }
}
