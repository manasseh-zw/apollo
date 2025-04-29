using System.ComponentModel;
using Apollo.Agents.Events;
using Apollo.Data.Models;
using Apollo.Data.Repository;
using Microsoft.SemanticKernel;

namespace Apollo.Agents.Plugins;

public class StartResearchPlugin
{
    private readonly ApolloDbContext _repository;
    private readonly IResearchEventHandler _eventHandler;

    public StartResearchPlugin(ApolloDbContext repository, IResearchEventHandler eventHandler)
    {
        _repository = repository;
        _eventHandler = eventHandler;
    }

    [KernelFunction("InitiateResearch")]
    [Description("Initiates and saves the research project plan.")]
    [return: Description("The id of the research project")]
    public async Task InitiateResearch(
        [Description("The userId ")] string userId,
        [Description("The title of the research project.")] string title,
        [Description("A brief description of the research project.")] string description,
        [Description("A list of 3-5 focused research questions.")] List<string> questions
    )
    {
        var researchPlan = new ResearchPlan() { Questions = questions };

        var research = new Data.Models.Research()
        {
            UserId = Guid.Parse(userId),
            Title = title,
            Description = description,
            Plan = researchPlan,
            StartedAt = DateTime.UtcNow,
            Status = ResearchStatus.InProgress,
        };

        await _repository.Research.AddAsync(research);
        await _repository.SaveChangesAsync();

        await _eventHandler.HandleResearchStart(
            new ResearchStartEvent { ResearchId = research.Id, UserId = userId }
        );
    }
}
