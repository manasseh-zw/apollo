namespace Apollo.Crawler;

public class WebCrawlResponse
{
    public bool Success { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
}

public class WebCrawlBatchResponse
{
    public bool Success { get; set; }
    public List<WebCrawlBatchResultItem> Results { get; set; } = [];
    public string Error { get; set; } = string.Empty;
}

public class WebCrawlBatchResultItem
{
    public string Url { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool Success { get; set; }
}
