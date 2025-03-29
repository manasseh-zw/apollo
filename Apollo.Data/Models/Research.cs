using System.ComponentModel.DataAnnotations.Schema;

namespace Apollo.Data.Models;

public class Research
{
    public Guid Id { get; set; }
    public string? CoverImage { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public ResearchStatus Status { get; set; }
    public ResearchType Type { get; set; }
    public ResearchDepth Depth { get; set; }
    public DateTime StartedAt { get; set; }
    public string? Content { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public User User { get; set; }
}

public enum ResearchStatus
{
    InProgress,
    Complete,
}

public enum ResearchType
{
    Casual,
    Academic,
    Technical,
}

public enum ResearchDepth
{
    Brief,
    Standard,
    Comprehensive,
}
