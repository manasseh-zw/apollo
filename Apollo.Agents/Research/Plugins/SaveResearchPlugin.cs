using System.ComponentModel;
using Apollo.Agents.Events;
using Apollo.Data.Models;
using Apollo.Data.Repository;
using Microsoft.SemanticKernel;

namespace Apollo.Agents.Research.Plugins;

public class SaveResearchPlugin
{
    private readonly ApolloDbContext _repository;
    private readonly IResearchEventHandler _eventHandler;

    public SaveResearchPlugin(ApolloDbContext repository, IResearchEventHandler eventHandler)
    {
        _repository = repository;
        _eventHandler = eventHandler;
    }

    [KernelFunction]
    [Description("Saves the research project.")]
    [return: Description("The id of the research project")]
    public async Task SaveResearch(
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

        await _eventHandler.HandleResearchSaved(
            new() { ResearchId = research.Id, UserId = userId }
        );
    }
}
