namespace Apollo.Search.Models;

public class WebSearchRequest : CommonRequest
{
    public string Query { get; set; } = string.Empty;
    public bool UseAutoprompt { get; set; } = true;
    public string Type { get; set; } = "auto";
    public string? Category { get; set; }
}
