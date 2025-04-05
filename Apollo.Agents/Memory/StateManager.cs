using Microsoft.Extensions.VectorData;

namespace Apollo.Agents.Memory;

public class StateManager { }

public class ResearchState
{
    public Queue<string> ResearchQuestions = [];
    public List<string> ProcessedQuestions = [];
    public List<TocItem>? TableOfContents;
}


public record TocItem(string Title, string Description);
