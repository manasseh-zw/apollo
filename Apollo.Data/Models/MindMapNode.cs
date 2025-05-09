using System.Text.Json.Serialization;

namespace Apollo.Data.Models;

[JsonPolymorphic]
[JsonDerivedType(typeof(RootMindMapNode), nameof(RootMindMapNode))]
[JsonDerivedType(typeof(QuestionMindMapNode), nameof(QuestionMindMapNode))]
[JsonDerivedType(typeof(SearchQueryMindMapNode), nameof(SearchQueryMindMapNode))]
[JsonDerivedType(typeof(SearchResultMindMapNode), nameof(SearchResultMindMapNode))]
public abstract class MindMapNode
{
    public required string Id { get; set; }
    public required string Label { get; set; }
    public required MindMapNodeType Type { get; set; }
    public List<MindMapNode> Children { get; set; } = [];
}

public class RootMindMapNode : MindMapNode
{
    public required string ResearchTitle { get; set; }
    public required string ResearchDescription { get; set; }
}

public class QuestionMindMapNode : MindMapNode
{
    public required string QuestionText { get; set; }
    public required bool IsGapQuestion { get; set; }
}

public class SearchQueryMindMapNode : MindMapNode
{
    public required string QueryText { get; set; }
    public required DateTime ExecutedAt { get; set; }
}

public class SearchResultMindMapNode : MindMapNode
{
    public required string Url { get; set; }
    public required string Title { get; set; }
    public required string Favicon { get; set; }
    public string? ImageUrl { get; set; }
    public required string Summary { get; set; }
}

public enum MindMapNodeType
{
    Root = 0,
    Question = 1,
    SearchQuery = 2,
    SearchResult = 3,
}
