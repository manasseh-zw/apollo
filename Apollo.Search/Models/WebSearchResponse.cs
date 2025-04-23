namespace Apollo.Search.Models;

public class WebSearchResponse
{
    public List<WebSearchResult> Results { get; set; } = [];
    public string SearchType { get; set; } = string.Empty;
}

public class WebSearchResult
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? PublishedDate { get; set; }
    public string? Author { get; set; }
    public double? Score { get; set; }
    public string? Text { get; set; }
    public List<string>? Highlights { get; set; }
    public List<double>? HighlightScores { get; set; }
    public string? Summary { get; set; }
    public List<WebSearchResult>? Subpages { get; set; }
    public string? Image { get; set; }
    public string? Favicon { get; set; }
    public ExtrasResult? Extras { get; set; }
}

public class ExtrasResult
{
    public List<string>? Links { get; set; }
    public List<string>? ImageLinks { get; set; }
}
