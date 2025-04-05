using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Apollo.Agents.Research;

public class QueryStrategist
{
    public static ChatCompletionAgent Create(Kernel kernel)
    {
        return new ChatCompletionAgent
        {
            Kernel = kernel,
            Name = "QueryStrategist",
            Instructions = """
                You are an AI assistant that generates effective search engine queries for a specific research question.
                Analyze the input research question.
                Generate a list of 3 distinct, concise search queries likely to yield relevant results. Use different keywords and phrasings.
                Output *only* the list of queries, each on a new line.
                """,
        };
    }
}
