using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Apollo.Agents.Research;

public class ResearchSynthesizer
{
    public static ChatCompletionAgent Create(Kernel kernel)
    {
        return new ChatCompletionAgent
        {
            Kernel = kernel.Clone(), // Needs VectorDB query tool/plugin
            Name = "ReportSynthesizer",
            Instructions = """
                You are an AI report writer.
                INPUT: You will receive the final table of contents and access to a vector database containing research chunks via a query tool.
                TASK:
                1. For each section in the table of contents, query the vector database for relevant information chunks.
                2. Synthesize the retrieved information into a coherent, well-written report section.
                3. Combine all sections into a final report, following the table of contents structure.
                4. Ensure the report directly addresses the original research goals.
                5. Include citations based on the source URLs of the chunks used (if available in the query results).
                OUTPUT: The final, formatted research report in markdown.
                """,
        };
    }
}
