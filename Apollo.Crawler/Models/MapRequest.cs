namespace Apollo.Crawler.Models;

public class MapRequest
{
    public string Url { get; set; } = string.Empty;
    public string? Search { get; set; }
    public bool IgnoreSitemap { get; set; } = true;
    public bool SitemapOnly { get; set; }
    public bool IncludeSubdomains { get; set; }
    public int Limit { get; set; } = 5000;
}