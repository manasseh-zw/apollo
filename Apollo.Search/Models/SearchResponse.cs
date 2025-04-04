namespace Apollo.Search.Models;

public class SearchResponse
{
    public List<SearchResult> Results { get; set; } = [];
    public string SearchType { get; set; } = string.Empty;
}

public class SearchResult
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? PublishedDate { get; set; }
    public string? Author { get; set; }
    public double? Score { get; set; }
    public string? Text { get; set; }
    public List<string>? Highlights { get; set; }
    public string? Summary { get; set; }
}
