namespace Apollo.Crawler.Models;

public class ScrapeResponse
{
    public bool Success { get; set; }
    public ScrapeData? Data { get; set; }
}

public class ScrapeData
{
    public string? Markdown { get; set; }
    public string? Html { get; set; }
    public string? RawHtml { get; set; }
    public string? Screenshot { get; set; }
    public List<string>? Links { get; set; }
    public ActionResults? Actions { get; set; }
    public Metadata? Metadata { get; set; }
    public object? LlmExtraction { get; set; }
    public string? Warning { get; set; }
}

public class ActionResults
{
    public List<string>? Screenshots { get; set; }
}

public class Metadata
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Language { get; set; }
    public string? SourceURL { get; set; }
    public int StatusCode { get; set; }
    public string? Error { get; set; }
}