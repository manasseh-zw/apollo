using System.ComponentModel;
using Apollo.Agents.Events;
using Apollo.Data.Models;
using Apollo.Data.Repository;
using Microsoft.SemanticKernel;

namespace Apollo.Agents.Plugins;

public class CompleteResearchPlugin
{
    private readonly ApolloDbContext _repository;
    private readonly IResearchEventHandler _eventHandler;

    public CompleteResearchPlugin(ApolloDbContext repository, IResearchEventHandler eventHandler)
    {
        _repository = repository;
        _eventHandler = eventHandler;
    }

    [KernelFunction("CompleteResearch")]
    [Description("Saves the final report of the research topic and marks it as complete.")]
    public async Task CompleteResearch(
        [Description("The research Id")] string researchId,
        [Description("The comprehensive research content in markdown format")] string content
    )
    {
        var research =
            await _repository.Research.FindAsync(Guid.Parse(researchId))
            ?? throw new Exception("Research not found");

        var report = new ResearchReport { Content = content, ResearchId = research.Id };

        research.Report = report;
        research.Status = ResearchStatus.Complete;

        await _repository.ResearchReports.AddAsync(report);
        await _repository.SaveChangesAsync();

        await _eventHandler.HandleResearchCompleted(
            new ResearchCompletedEvent
            {
                ResearchId = research.Id,
                UserId = research.UserId.ToString(),
            }
        );
    }
}
