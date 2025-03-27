public class Research
{
    public Guid Id { get; set; }
    public string? CoverImage { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Status Status { get; set; }
    public DateTime StartedAt { get; set; }
    public string? Content { get; set; }
}

public enum Status
{
    InProgress,
    Complete,
}
