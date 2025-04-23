namespace Apollo.Search.Models;

public class AnswerResponse
{
    public string Answer { get; set; } = string.Empty;
    public List<AnswerCitation> Citations { get; set; } = [];
}

public class AnswerCitation
{
    public string Id { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? PublishedDate { get; set; }
    public string? Text { get; set; }
    public string? Image { get; set; }
    public string? Favicon { get; set; }
}