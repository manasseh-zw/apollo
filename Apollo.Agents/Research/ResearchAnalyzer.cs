using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Apollo.Agents.Research;

public class ResearchAnalyzer
{
    public static ChatCompletionAgent Create(Kernel kernel)
    {
        return new ChatCompletionAgent
        {
            Kernel = kernel,
            Name = "Inquisitor",
            Instructions = """
                You are an AI assistant that generates initial research questions based on a topic and description.
                Analyze the provided research topic and description.
                Generate a list of 3-5 broad, open-ended questions that cover the core aspects of the topic.
                Output *only* the list of questions, each on a new line, starting with '- '.
                """,
        };
    }
}
