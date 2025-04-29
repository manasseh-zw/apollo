using System.Text;
using Apollo.Agents.Contracts;
using Apollo.Agents.Helpers;
using Apollo.Agents.Memory;
using Apollo.Agents.State;
using Apollo.Config;
using Apollo.Data.Models;
using Apollo.Data.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Apollo.Agents.Research;

public interface IResearchReportGenerator
{
    Task<string> GenerateReportAsync(
        string researchId,
        CancellationToken cancellationToken = default
    );
}

public class ResearchReportGenerator : IResearchReportGenerator
{
    private readonly IMemoryContext _memory;
    private readonly ApolloDbContext _repository;
    private readonly IClientUpdateCallback _clientUpdate;
    private readonly IStateManager _state;
    private readonly ILogger<ResearchReportGenerator> _logger;
    private readonly IChatCompletionService _chat;

    public ResearchReportGenerator(
        IMemoryContext memory,
        ApolloDbContext repository,
        IClientUpdateCallback clientUpdate,
        IStateManager state,
        ILogger<ResearchReportGenerator> logger
    )
    {
        _memory = memory;
        _repository = repository;
        _clientUpdate = clientUpdate;
        _state = state;
        _logger = logger;

        var kernel = Kernel
            .CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: AppConfig.Models.Gpt41,
                endpoint: AppConfig.AzureAI.Endpoint,
                apiKey: AppConfig.AzureAI.ApiKey
            )
            .Build();
        _chat = kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<string> GenerateReportAsync(
        string researchId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("[{ResearchId}] Starting report generation", researchId);
        _clientUpdate.SendResearchFeedUpdate(
            new ProgressMessageFeedUpdate
            {
                ResearchId = researchId,
                Type = ResearchFeedUpdateType.Message,
                Message = "Starting report generation...",
            }
        );

        try
        {
            var state = _state.GetState(researchId);
            var research =
                await _repository.Research.FindAsync(Guid.Parse(researchId))
                ?? throw new Exception($"Research not found for ID: {researchId}");

            var toc = state.TableOfContents;
            if (toc == null || !toc.Any())
            {
                throw new Exception("Table of contents is empty. Cannot generate report.");
            }

            // Create tasks for concurrent processing of all sections
            var sectionTasks =
                new List<Task<(string Section, string Content, List<SourceMetadata> Sources)>>();

            for (int i = 0; i < toc.Count; i++)
            {
                var section = toc[i];
                var sectionIndex = i; // Capture for progress updates

                var sectionTask = Task.Run(
                    async () =>
                    {
                        _clientUpdate.SendResearchFeedUpdate(
                            new ProgressMessageFeedUpdate
                            {
                                ResearchId = researchId,
                                Type = ResearchFeedUpdateType.Message,
                                Message =
                                    $"Gathering information for section {sectionIndex + 1} of {toc.Count}: {section}",
                            }
                        );

                        var memoryResults = await _memory.AskAsync(
                            researchId,
                            $"Hie do a comprehensive report essay analysis of the '{section}' for {state.Title} , {state.Description}, including all relevant facts, data, and findings. Include specific details and ensure all information is properly attributed to sources.",
                            cancellationToken
                        );

                        if (memoryResults.RelevantSources.Count < 1)
                        {
                            _logger.LogWarning(
                                "[{ResearchId}] No memory results found for section: {Section}",
                                researchId,
                                section
                            );
                            return (section, string.Empty, new List<SourceMetadata>());
                        }

                        var sources = new List<SourceMetadata>();
                        foreach (var source in memoryResults.RelevantSources)
                        {
                            foreach (var partition in source.Partitions)
                            {
                                if (partition.Tags != null)
                                {
                                    var metadata = new SourceMetadata
                                    {
                                        Url = partition
                                            .Tags.FirstOrDefault(t => t.Key == "url")
                                            .Value?.FirstOrDefault(),
                                        Title = partition
                                            .Tags.FirstOrDefault(t => t.Key == "title")
                                            .Value?.FirstOrDefault(),
                                        Author = partition
                                            .Tags.FirstOrDefault(t => t.Key == "author")
                                            .Value?.FirstOrDefault(),
                                        PublishedDate = partition
                                            .Tags.FirstOrDefault(t => t.Key == "published")
                                            .Value?.FirstOrDefault(),
                                    };

                                    if (
                                        !string.IsNullOrEmpty(metadata.Title)
                                        || !string.IsNullOrEmpty(metadata.Url)
                                    )
                                    {
                                        sources.Add(metadata);
                                    }
                                }
                            }
                        }

                        return (section, memoryResults.Result, sources);
                    },
                    cancellationToken
                );

                sectionTasks.Add(sectionTask);
            }

            var sectionResults = await Task.WhenAll(sectionTasks);

            var sectionContents = sectionResults
                .Where(result => !string.IsNullOrEmpty(result.Content))
                .ToList();

            _clientUpdate.SendResearchFeedUpdate(
                new ProgressMessageFeedUpdate
                {
                    ResearchId = researchId,
                    Type = ResearchFeedUpdateType.Message,
                    Message = "Synthesizing final report...",
                }
            );

            var formattedSections = new StringBuilder();
            foreach (var (section, content, sources) in sectionContents)
            {
                formattedSections.AppendLine($"## {section}");
                formattedSections.AppendLine("Content:");
                formattedSections.AppendLine(content);
                formattedSections.AppendLine("\nSources for this section:");
                foreach (var source in sources.DistinctBy(s => s.Url))
                {
                    formattedSections.AppendLine(
                        $"- {source.Title} | {source.Author} | {source.Url} | {source.PublishedDate}"
                    );
                }
                formattedSections.AppendLine("\n---\n");
            }

            var promptFileName =
                $"synthesis_prompt_{researchId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.txt";
            await File.WriteAllTextAsync(
                promptFileName,
                formattedSections.ToString(),
                cancellationToken
            );
            _logger.LogInformation(
                "[{ResearchId}] Saved synthesis sections to file: {FileName}",
                researchId,
                promptFileName
            );

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(Prompts.ReportSynthesizerPrompt);
            chatHistory.AddUserMessage(formattedSections.ToString());

            var finalReport = await _chat.GetChatMessageContentAsync(
                chatHistory,
                executionSettings: new OpenAIPromptExecutionSettings() { MaxTokens = 1047576 },
                cancellationToken: cancellationToken
            );

            var report = new ResearchReport
            {
                Content = finalReport.Content,
                ResearchId = research.Id,
            };

            research.Report = report;
            research.Status = ResearchStatus.Complete;

            await _repository.ResearchReports.AddAsync(report, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            _clientUpdate.SendResearchFeedUpdate(
                new ProgressMessageFeedUpdate
                {
                    ResearchId = researchId,
                    Type = ResearchFeedUpdateType.Message,
                    Message = "Report generation complete!",
                }
            );
            _logger.LogInformation(
                "[{ResearchId}] Report generation completed successfully",
                researchId
            );

            return "Report generation completed successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[{ResearchId}] Error generating report: {Message}",
                researchId,
                ex.Message
            );
            throw;
        }
    }
}

public class SourceMetadata
{
    public string? ResearchId { get; set; }
    public string? Url { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? PublishedDate { get; set; }
}
