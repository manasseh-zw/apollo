using System.ComponentModel;
using Apollo.Data.Models;
using Apollo.Data.Repository;
using Microsoft.SemanticKernel;

namespace Apollo.Agents.Research.Plugins;

public class SaveResearchPlugin
{
    private readonly ApolloDbContext _repository;

    public SaveResearchPlugin(ApolloDbContext repository)
    {
        _repository = repository;
    }

    [KernelFunction]
    [Description("Saves the research project.")]
    [return: Description("The id of the research project")]
    public async Task<Guid> SaveResearch(
        [Description("The userId ")] string userId,
        [Description("The title of the research project.")] string title,
        [Description("A brief description of the research project.")] string description,
        [Description("The type of research : Casual | Academic | Technical).")] string type,
        [Description("The depth of the research : Brief | Standard | Comprehensive.")] string depth
    )
    {
        var research = new Data.Models.Research()
        {
            Title = title,
            Description = description,
            Type = Enum.Parse<ResearchType>(type),
            Depth = Enum.Parse<ResearchDepth>(depth),
            Status = ResearchStatus.InProgress,
            StartedAt = DateTime.UtcNow,
            UserId = Guid.Parse(userId),
        };

        await _repository.AddAsync(research);

        await _repository.SaveChangesAsync();

        return research.Id;
    }
}
