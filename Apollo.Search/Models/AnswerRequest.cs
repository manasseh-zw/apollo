namespace Apollo.Search.Models;

public class AnswerRequest
{
    public string Query { get; set; } = string.Empty;
    public bool Stream { get; set; }
    public bool Text { get; set; }
    public string Model { get; set; } = "exa";
}