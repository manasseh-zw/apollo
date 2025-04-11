using System.ComponentModel.DataAnnotations.Schema;

namespace Apollo.Data.Models;

public class ResearchReport
{
    public Guid Id { get; set; }
    public required string Content { get; set; }

    [ForeignKey(nameof(Research))]
    public Guid ResearchId { get; set; }
    public Research? Research { get; set; }
}
