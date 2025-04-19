namespace Apollo.Search.Models;

public class WebSearchRequest
{
    public string Query { get; set; } = string.Empty;
    public bool UseAutoprompt { get; set; } = true;
    public string Type { get; set; } = "auto";
    public string? Category { get; set; }
    public int NumResults { get; set; } = 2;
    public bool IncludeText { get; set; } = false;
}
