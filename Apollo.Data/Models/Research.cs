using System.ComponentModel.DataAnnotations.Schema;

namespace Apollo.Data.Models;

public class Research
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required ResearchPlan Plan { get; set; }
    public ResearchReport? Report { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public ResearchStatus Status { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public User? User { get; set; }
}

public enum ResearchStatus
{
    InProgress,
    Complete,
}
