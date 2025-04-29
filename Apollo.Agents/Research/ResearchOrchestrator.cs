using System.Text;
using Apollo.Agents.Contracts;
using Apollo.Agents.Helpers;
using Apollo.Agents.Plugins;
using Apollo.Agents.State;
using Apollo.Config;
using Apollo.Data.Models;
using Apollo.Data.Repository;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
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

    private readonly ResearchEnginePlugin _engineInstance;
    private readonly KernelMemoryPlugin _memoryInstance;
    private readonly SynthesizeResearchPlugin _synthesizeResearchInstance;
    private const string UnknownAgentName = "Unknown Agent";

    public ResearchOrchestrator(
        ApolloDbContext repository,
        ILogger<ResearchOrchestrator> logger,
        IClientUpdateCallback clientUpdate,
        IStateManager state,
        IResearchManager manager,
        ResearchEnginePlugin engine,
        KernelMemoryPlugin memory,
        SynthesizeResearchPlugin synthesizeResearch
    )
    {
        _repository = repository;
        _logger = logger;
        _clientUpdate = clientUpdate;
        _state = state;
        _manager = manager;

        _engineInstance = engine;
        _memoryInstance = memory;
        _synthesizeResearchInstance = synthesizeResearch;
    }

    public async Task StartResearchProcessAsync(string researchId)
    {
        _logger.LogInformation("Starting research for ID:{researchId}", researchId);

        //intialize state
        await InitializeResearch(Guid.Parse(researchId));

        var initialState = _state.GetState(researchId);

        if (initialState == null || !initialState.PendingResearchQuestions.Any())
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
            deploymentName: AppConfig.Models.Gpt41,
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
                    researchId,
                    _synthesizeResearchInstance
                )
            },
            {
                AgentFactory.ResearchEngineAgentName,
                AgentFactory.CreateResearchEngine(
                    kernelBuilder,
                    _state,
                    _clientUpdate,
                    researchId,
                    _engineInstance
                )
            },
            {
                AgentFactory.ResearchAnalyzerAgentName,
                AgentFactory.CreateResearchAnalyzer(
                    kernelBuilder,
                    _state,
                    _clientUpdate,
                    researchId,
                    _memoryInstance
                )
            },
        };

        (_manager as ResearchManager)?.Initialize(researchId);

        var chat = new AgentGroupChat([.. agents.Values])
        {
            ExecutionSettings = new()
            {
                SelectionStrategy = new ResearchSelectionStrategy(_manager),
                TerminationStrategy = new ResearchTerminationStrategy(_manager),
            },
        };

        _logger.LogInformation(
            "Initiating agent group chat streaming for {ResearchId}...",
            researchId
        );

        _clientUpdate.SendResearchFeedUpdate(
            new ProgressMessageFeedUpdate
            {
                ResearchId = researchId,
                Type = ResearchFeedUpdateType.Message,
                Message = "Research is now initiating!",
            }
        );

        var intialPrompt = new ChatMessageContent(
            AuthorRole.User,
            $"Start the research process for '{initialState.Title}' of resarchId: '{initialState.ResearchId}'. Coordinator, please begin."
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
                        _clientUpdate.SendAgentChatMessage(
                            new AgentChatMessageEvent
                            {
                                ResearchId = researchId,
                                Author = currentAuthor,
                                Message = currentMessageBuffer.ToString(),
                            }
                        );
                        _logger.LogInformation(
                            "[{ResearchId}] >> {message} from {AgentName}",
                            researchId,
                            currentMessageBuffer.ToString(),
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
                _clientUpdate.SendAgentChatMessage(
                    new AgentChatMessageEvent
                    {
                        ResearchId = researchId,
                        Author = currentAuthor,
                        Message = currentMessageBuffer.ToString(),
                    }
                );
                _logger.LogInformation(
                    "[{ResearchId}] >> Sent final message {message}  after loop from {AgentName}",
                    researchId,
                    currentMessageBuffer.ToString(),
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
                .Research.Where(x => x.Id == researchId)
                .Select(x => new Data.Models.Research
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Plan = new ResearchPlan { Questions = x.Plan.Questions },
                })
                .FirstOrDefaultAsync() ?? throw new Exception("Research instance does not exist");

        await _state.GetOrCreateState(
            researchId.ToString(),
            async () =>
            {
                await Task.CompletedTask;
                // Create initial questions list with IDs
                var questionsWithIds = research
                    .Plan.Questions.Select(q => new ResearchQuestion
                    {
                        Id = Guid.NewGuid().ToString(),
                        Text = q,
                        IsProcessed = false,
                    })
                    .ToList();

                var state = new ResearchState
                {
                    ResearchId = researchId.ToString(),
                    Title = research.Title,
                    Description = research.Description,
                    PendingResearchQuestions = [.. questionsWithIds], // Copy the list
                    AllQuestionsInOrder = [.. questionsWithIds], // Store original order
                };

                // Set the first question as active immediately
                if (state.PendingResearchQuestions.Any())
                {
                    state.ActiveQuestionId = state.PendingResearchQuestions.First().Id;
                }

                return state;
            }
        );
    }
}
