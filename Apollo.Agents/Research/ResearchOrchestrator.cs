using System.Text;
using Apollo.Agents.Helpers;
using Apollo.Agents.Plugins;
using Apollo.Agents.State;
using Apollo.Config;
using Apollo.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Apollo.Agents.Research;

#pragma warning disable SKEXP0110
#pragma warning disable SKEXP0001

public class ResearchOrchestrator
{
    private readonly ApolloDbContext _repository;
    private readonly ILogger<ResearchOrchestrator> _logger;
    private readonly IClientUpdateCallback _clientUpdate;
    private readonly IStateManager _state;
    private readonly IResearchManager _manager;
    private readonly KernelPlugin _engine;
    private readonly KernelPlugin _memory;
    private const string UnknownAgentName = "Unknown Agent";

    public ResearchOrchestrator(
        ApolloDbContext repository,
        ILogger<ResearchOrchestrator> logger,
        IClientUpdateCallback clientUpdate,
        IStateManager state,
        IResearchManager manager
    )
    {
        _repository = repository;
        _logger = logger;
        _clientUpdate = clientUpdate;
        _state = state;
        _manager = manager;
        _engine = KernelPluginFactory.CreateFromType<ResearchEnginePlugin>("ResearchEnginePlugin");
        _memory = KernelPluginFactory.CreateFromType<KernelMemoryPlugin>("KernelMemoryPlugin");
    }

    public async Task StartResearchProcessAsync(string researchId)
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

        var kernel = kernelBuilder.Build();

        var agents = new Dictionary<string, Agent>()
        {
            {
                AgentFactory.ResearchCoordinatorAgentName,
                AgentFactory.CreateResearchCoordinator(
                    kernelBuilder,
                    _state,
                    _clientUpdate,
                    researchId
                )
            },
            {
                AgentFactory.ResearchEngineAgentName,
                AgentFactory.CreateResearchEngine(
                    kernelBuilder,
                    _state,
                    _clientUpdate,
                    researchId,
                    _engine
                )
            },
            {
                AgentFactory.ResearchAnalyzerAgentName,
                AgentFactory.CreateResearchAnalyzer(
                    kernelBuilder,
                    _state,
                    _clientUpdate,
                    researchId,
                    _memory
                )
            },
            {
                AgentFactory.ReportSynthesizerAgentName,
                AgentFactory.CreateReportSynthesizer(
                    kernelBuilder,
                    _state,
                    _clientUpdate,
                    researchId,
                    _memory
                )
            },
        };

        var chat = new AgentGroupChat([.. agents.Values])
        {
            ExecutionSettings = new()
            {
                SelectionStrategy = new KernelFunctionSelectionStrategy(
                    KernelFunctionFactory.CreateFromMethod(_manager.SelectNextAgent),
                    kernel
                ),
                TerminationStrategy = new KernelFunctionTerminationStrategy(
                    KernelFunctionFactory.CreateFromMethod(_manager.CheckTermination),
                    kernel
                ),
            },
        };

        _logger.LogInformation(
            "Initiating agent group chat streaming for {ResearchId}...",
            researchId
        );

        _clientUpdate.SendResearchProgressUpdate(researchId, "research is now initiating!");

        var intialPrompt = new ChatMessageContent(
            AuthorRole.User,
            $"Start the research process for '{initialState.Title}'. Coordinator, please begin."
        );

        var currentMessageBuffer = new StringBuilder();
        string currentAuthor = string.Empty;
        chat.AddChatMessage(intialPrompt);
        try
        {
            await foreach (var chunk in chat.InvokeStreamingAsync())
            {
                if (currentAuthor == null)
                {
                    currentAuthor = chunk.AuthorName ?? UnknownAgentName;
                }
                else if (currentAuthor != (chunk.AuthorName ?? UnknownAgentName))
                {
                    if (currentMessageBuffer.Length > 0)
                    {
                        _clientUpdate.StreamAgentMessage(
                            researchId,
                            currentAuthor,
                            currentMessageBuffer.ToString()
                        );
                        _logger.LogInformation(
                            "[{ResearchId}] >> Sent buffered message from {AgentName}",
                            researchId,
                            currentAuthor
                        );
                    }
                    currentAuthor = chunk.AuthorName ?? UnknownAgentName;
                    currentMessageBuffer.Clear();
                }
                currentMessageBuffer.Append(chunk.Content);
            }
            if (currentMessageBuffer.Length > 0 && currentAuthor != null)
            {
                _clientUpdate.StreamAgentMessage(
                    researchId,
                    currentAuthor,
                    currentMessageBuffer.ToString()
                );
                _logger.LogInformation(
                    "[{ResearchId}] >> Sent final buffered message after loop from {AgentName}",
                    researchId,
                    currentAuthor
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred during the research process for {ResearchId}",
                researchId
            );
        }
        finally
        {
            _logger.LogInformation(
                "Research orchestration completed for {ResearchId}.",
                researchId
            );
        }
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
