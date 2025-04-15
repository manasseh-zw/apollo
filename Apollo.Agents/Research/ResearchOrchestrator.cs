using Apollo.Agents.Helpers;
using Apollo.Agents.Plugins;
using Apollo.Agents.State;
using Apollo.Config;
using Apollo.Data.Models;
using Apollo.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Apollo.Agents.Research;

public class ResearchOrchestrator
{
    private readonly ApolloDbContext _repository;
    private readonly ILogger<ResearchOrchestrator> _logger;
    private readonly IClientUpdateCallback _clientUpdate;
    private readonly IStateManager _state;
    private readonly IResearchManager _manager;
    private readonly ResearchProcessorPlugin _processor;
    private readonly KernelMemoryPlugin _memory;

    public ResearchOrchestrator(
        ApolloDbContext repository,
        ILogger<ResearchOrchestrator> logger,
        IClientUpdateCallback clientUpdate,
        IStateManager state,
        IResearchManager manager,
        ResearchProcessorPlugin processor,
        KernelMemoryPlugin memory
    )
    {
        _repository = repository;
        _logger = logger;
        _clientUpdate = clientUpdate;
        _state = state;
        _manager = manager;
        _processor = processor;
        _memory = memory;
    }

    public async Task StartReserchProcessAsync(string researchId)
    {
        _logger.LogInformation("Starting research for ID:{researchId}", researchId);

        //intialize state
        await InitializeResearch(Guid.Parse(researchId));

        var initialState = _state.GetState(researchId);

        if (initialState == null || initialState.PendingResearchQuestions.Any())
        {
            _logger.LogError(
                "Cannot start research, there is no initial state or pending questions for id {researchId}",
                researchId
            );
            return;
        }

        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.Services.AddLogging(c =>
            c.AddConsole().SetMinimumLevel(LogLevel.Information)
        );

        kernelBuilder.AddAzureOpenAIChatCompletion(
            deploymentName: AppConfig.Models.Gpt4o,
            endpoint: AppConfig.AzureAI.Endpoint,
            apiKey: AppConfig.AzureAI.ApiKey
        );
    }

    private async Task InitializeResearch(Guid researchId)
    {
        var research =
            await _repository
                .Research.Where(r => r.Id == researchId)
                .Include(x => x.Plan)
                .FirstOrDefaultAsync() ?? throw new Exception("Research instance does not exist");

        await _state.GetOrCreateState(
            researchId.ToString(),
            async () =>
            {
                await Task.CompletedTask;
                return new ResearchState
                {
                    ResearchId = researchId.ToString(),
                    Title = research.Title,
                    Description = research.Description,
                    Type = research.Plan.Type,
                    Depth = research.Plan.Depth,
                    PendingResearchQuestions = research
                        .Plan.Questions.Select(q => new ResearchQuestion
                        {
                            Id = Guid.NewGuid().ToString(),
                            Text = q,
                            IsProcessed = false,
                        })
                        .ToList(),
                };
            }
        );
    }
}
