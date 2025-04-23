namespace Apollo.Search.Models;

public class CommonRequest
{
    public int NumResults { get; set; } = 10;
    public List<string>? IncludeDomains { get; set; }
    public List<string>? ExcludeDomains { get; set; }
    public DateTime? StartCrawlDate { get; set; }
    public DateTime? EndCrawlDate { get; set; }
    public DateTime? StartPublishedDate { get; set; }
    public DateTime? EndPublishedDate { get; set; }
    public List<string>? IncludeText { get; set; }
    public List<string>? ExcludeText { get; set; }
    public ContentsRequest? Contents { get; set; }
}