namespace Apollo.Crawler.Models;

public class ScrapeRequest
{
    public string Url { get; set; } = string.Empty;
    public List<string> Formats { get; set; } = ["markdown"];
    public bool OnlyMainContent { get; set; } = true;
    public List<string>? IncludeTags { get; set; }
    public List<string>? ExcludeTags { get; set; }
    public Dictionary<string, string>? Headers { get; set; }
    public int WaitFor { get; set; } = 0;
    public bool Mobile { get; set; } = false;
    public bool SkipTlsVerification { get; set; } = false;
    public int Timeout { get; set; } = 30000;
    public List<ScrapeAction>? Actions { get; set; }
    public LocationSettings? Location { get; set; }
    public bool RemoveBase64Images { get; set; } = true;
}

public class LocationSettings
{
    public string Country { get; set; } = "US";
    public List<string>? Languages { get; set; }
}

public class ScrapeAction
{
    public string Type { get; set; } = string.Empty;
    public int? Milliseconds { get; set; }
    public string? Selector { get; set; }
    public bool? FullPage { get; set; }
    public string? Text { get; set; }
    public string? Key { get; set; }
    public string? Direction { get; set; }
    public string? Script { get; set; }
}
