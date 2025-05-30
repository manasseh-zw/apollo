using System.Collections.Concurrent;
using System.Text;
using Apollo.Agents.Contracts;
using Apollo.Agents.Events;
using Apollo.Agents.Helpers;
using Apollo.Agents.Memory;
using Apollo.Agents.State;
using Apollo.Config;
using Apollo.Data.Models;
using Apollo.Data.Repository;
using Apollo.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Apollo.Agents.Research;

#pragma warning disable SKEXP0070
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
    private readonly IResearchEventHandler _eventHandler;
    private readonly IEmailService _email;
    private const int BatchSize = 3; // Process sections in batches for better efficiency

    public ResearchReportGenerator(
        IMemoryContext memory,
        ApolloDbContext repository,
        IClientUpdateCallback clientUpdate,
        IStateManager state,
        ILogger<ResearchReportGenerator> logger,
        IResearchEventHandler eventHandler,
        IEmailService email
    )
    {
        _memory = memory;
        _repository = repository;
        _clientUpdate = clientUpdate;
        _state = state;
        _logger = logger;
        _eventHandler = eventHandler;
        _email = email;

        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var kernel = Kernel
            .CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: AppConfig.Models.Gpt41,
                endpoint: AppConfig.AzureAI.Endpoint,
                apiKey: AppConfig.AzureAI.ApiKey,
                httpClient: httpClient
            )
            // .AddGoogleAIGeminiChatCompletion(
            //     modelId: AppConfig.Models.GeminiProFlash25,
            //     apiKey: AppConfig.Google.ApiKey,
            //     httpClient: httpClient
            // )
            .Build();
        _chat = kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<string> GenerateReportAsync(
        string researchId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("[{ResearchId}] Starting report generation", researchId);
        UpdateProgress(
            researchId,
            "Starting report creation process now... and please be patient this could take a while..."
        );

        try
        {
            var state = _state.GetState(researchId);
            var research =
                await _repository
                    .Research.Include(r => r.User)
                    .FirstOrDefaultAsync(r => r.Id == Guid.Parse(researchId))
                ?? throw new Exception($"Research not found for ID: {researchId}");

            var toc = state.TableOfContents;
            if (toc == null || !toc.Any())
            {
                throw new Exception("Table of contents is empty. Cannot generate report.");
            }

            // Process sections in batches to manage memory and improve throughput
            var allSectionContents =
                new ConcurrentBag<(string Section, string Content, List<SourceMetadata> Sources)>();

            for (int batchStart = 0; batchStart < toc.Count; batchStart += BatchSize)
            {
                var currentBatchSize = Math.Min(BatchSize, toc.Count - batchStart);
                var batchTasks =
                    new List<
                        Task<(string Section, string Content, List<SourceMetadata> Sources)>
                    >();

                for (int i = 0; i < currentBatchSize; i++)
                {
                    var sectionIndex = batchStart + i;
                    var section = toc[sectionIndex];

                    var sectionTask = ProcessSectionAsync(
                        researchId,
                        section,
                        sectionIndex,
                        toc.Count,
                        state,
                        cancellationToken
                    );
                    batchTasks.Add(sectionTask);
                }

                var batchResults = await Task.WhenAll(batchTasks);
                foreach (var result in batchResults)
                {
                    if (!string.IsNullOrEmpty(result.Content))
                    {
                        allSectionContents.Add(result);
                    }
                }
            }

            UpdateProgress(
                researchId,
                "Okay we're now synthesizing each section of  the final report... its cooking"
            );

            // Combine all sections into a single document
            var formattedSections = new StringBuilder();
            var allSources = new List<SourceMetadata>();

            // Add overview section first
            formattedSections.AppendLine($"# {state.Title}");
            formattedSections.AppendLine($"\n## Overview");
            formattedSections.AppendLine(state.Description);
            formattedSections.AppendLine("\n---\n");

            // Add each content section
            foreach (
                var (section, content, sources) in allSectionContents.OrderBy(s =>
                    toc.IndexOf(s.Section)
                )
            )
            {
                formattedSections.AppendLine($"## {section}");
                formattedSections.AppendLine(content);
                formattedSections.AppendLine("\n");

                allSources.AddRange(sources);
            }

            formattedSections.AppendLine("## Sources and References");
            foreach (
                var source in allSources
                    .DistinctBy(s => s.Url)
                    .Where(s => !string.IsNullOrEmpty(s.Url))
            )
            {
                formattedSections.AppendLine(
                    $"- {source.Title ?? "Untitled"} | {source.Author ?? "Unknown"} | {source.Url} | {source.PublishedDate ?? "Unknown date"}"
                );
            }

            // For debugging purposes
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

            UpdateProgress(
                researchId,
                "We're now putting each section all together this might take a while! ~ 2 (miniutes) "
            );

            // Generate final report using the chat service
            var finalReport = await SynthesizeFinalReportAsync(
                formattedSections.ToString(),
                cancellationToken
            );

            // Save the report
            var report = new ResearchReport { Content = finalReport, ResearchId = research.Id };

            research.Report = report;
            research.Status = ResearchStatus.Complete;

            await _repository.ResearchReports.AddAsync(report, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            var sendEmail = await _email.SendResearchCompleteNotification(
                new Notifications.Models.Recipient(research.User.Username, research.User.Email),
                new Notifications.Models.ResearchCompleteContent(researchId, research.Title)
            );

            if (sendEmail)
                _logger.LogInformation($"email sent to {research.User.Email}");

            await _eventHandler.HandleResearchCompletedWithReport(
                new ResearchCompletedWithReportEvent
                {
                    ResearchId = research.Id,
                    UserId = research.UserId.ToString(),
                    Report = new(report.Id.ToString(), research.Title, report.Content),
                }
            );

            await _memory.DeleteIndex(researchId, cancellationToken);

            UpdateProgress(researchId, "Report Synthesis complete!");
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

    private async Task<(
        string Section,
        string Content,
        List<SourceMetadata> Sources
    )> ProcessSectionAsync(
        string researchId,
        string section,
        int sectionIndex,
        int totalSections,
        ResearchState state,
        CancellationToken cancellationToken
    )
    {
        UpdateProgress(
            researchId,
            $"Processing section {sectionIndex + 1} of {totalSections}: {section}"
        );

        // More focused query to get facts, quotes, and key findings instead of full essay
        var memoryResults = await _memory.AskAsync(
            researchId,
            $"For the section '{section}' in research about '{state.Title}', provide research notes that contain the following: "
                + $"1. Key facts and data points (with numbers when available) "
                + $"2. Direct quotes from sources (with attribution) "
                + $"3. Main findings relevant to this specific section "
                + $"4. Cite all sources",
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

        // Extract source metadata
        var sources = ExtractSourceMetadata(memoryResults);

        var sectionContent = memoryResults.Result;

        return (section, sectionContent, sources);
    }

    private static List<SourceMetadata> ExtractSourceMetadata(MemoryAnswer answer)
    {
        var sources = new List<SourceMetadata>();

        foreach (var source in answer.RelevantSources)
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
                        !string.IsNullOrEmpty(metadata.Title) || !string.IsNullOrEmpty(metadata.Url)
                    )
                    {
                        sources.Add(metadata);
                    }
                }
            }
        }

        return sources;
    }

    private async Task<string> SynthesizeFinalReportAsync(
        string formattedSections,
        CancellationToken cancellationToken
    )
    {
        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage(Prompts.ReportSynthesizerPrompt);

        chatHistory.AddUserMessage(formattedSections);

        var finalResponse = await _chat.GetChatMessageContentAsync(
            chatHistory,
            executionSettings: new OpenAIPromptExecutionSettings() { MaxTokens = 32768 },
            cancellationToken: cancellationToken
        );

        return finalResponse.Content;
    }

    private void UpdateProgress(string researchId, string message)
    {
        var update = new ProgressMessageFeedUpdate
        {
            ResearchId = researchId,
            Type = ResearchFeedUpdateType.Message,
            Message = message,
        };

        _state.AddFeedUpdate(researchId, update);
        _clientUpdate.SendResearchFeedUpdate(update);
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
