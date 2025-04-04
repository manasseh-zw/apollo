namespace Apollo.Search.Models;

public class SearchRequest
{
    public string Query { get; set; } = string.Empty;
    public bool UseAutoprompt { get; set; } = false;
    public string Type { get; set; } = "auto";
    public string? Category { get; set; }
    public int NumResults { get; set; } = 10;
    public bool IncludeText { get; set; } = false;
}
