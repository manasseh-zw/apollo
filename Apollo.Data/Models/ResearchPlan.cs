using System.ComponentModel.DataAnnotations.Schema;

namespace Apollo.Data.Models;

public class ResearchPlan
{
    public Guid Id { get; set; }
    public required List<string> Questions { get; set; }

    [ForeignKey(nameof(Research))]
    public Guid ResearchId { get; set; }
    public Research? Research { get; set; }
}
