namespace Apollo.Search.Models;

public class ContentsRequest
{
    public TextOptions? Text { get; set; }
    public HighlightsOptions? Highlights { get; set; }
    public bool? Summary { get; set; } = true;
    public string? Livecrawl { get; set; }
    public int? LivecrawlTimeout { get; set; }
    public int? Subpages { get; set; }
    public object? SubpageTarget { get; set; }
    public ExtrasOptions? Extras { get; set; }
}

public class TextOptions
{
    public int? MaxCharacters { get; set; }
    public bool IncludeHtmlTags { get; set; } = false;
}

public class HighlightsOptions
{
    public int NumSentences { get; set; } = 5;
    public int HighlightsPerUrl { get; set; } = 1;
    public string? Query { get; set; }
}

public class ExtrasOptions
{
    public int Links { get; set; }
    public int ImageLinks { get; set; }
}
