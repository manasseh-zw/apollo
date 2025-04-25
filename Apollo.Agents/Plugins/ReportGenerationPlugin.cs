using System.ComponentModel;
using System.Text;
using Apollo.Agents.Helpers;
using Apollo.Agents.Memory;
using Apollo.Agents.State;
using Apollo.Config;
using Apollo.Data.Models;
using Apollo.Data.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;

namespace Apollo.Agents.Plugins;

#pragma warning disable SKEXP0070
public class ReportGenerationPlugin
{
    private readonly IMemoryContext _memory;
    private readonly ApolloDbContext _repository;
    private readonly IClientUpdateCallback _clientUpdate;
    private readonly IStateManager _state;
    private readonly ILogger<ReportGenerationPlugin> _logger;
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chat;

    public ReportGenerationPlugin(
        IMemoryContext memory,
        ApolloDbContext repository,
        IClientUpdateCallback clientUpdate,
        IStateManager state,
        ILogger<ReportGenerationPlugin> logger
    )
    {
        _memory = memory;
        _repository = repository;
        _clientUpdate = clientUpdate;
        _state = state;
        _logger = logger;
        _kernel = Kernel
            .CreateBuilder()
            .AddGoogleAIGeminiChatCompletion(
                modelId: AppConfig.Models.GeminiPro25,
                apiKey: AppConfig.Google.ApiKey
            )
            .Build();
        _chat = _kernel.GetRequiredService<IChatCompletionService>();
    }

    [KernelFunction]
    [Description("Generates the final research report by synthesizing all gathered information.")]
    public async Task<string> GenerateReportAsync(
        [Description("The research ID")] string researchId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("[{ResearchId}] Starting report generation", researchId);
        _clientUpdate.SendResearchProgressUpdate(researchId, "Starting report generation...");

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

            // Initialize structures to hold section data
            var sectionContents =
                new List<(string Section, string Content, List<SourceMetadata> Sources)>();

            // Process each section
            for (int i = 0; i < toc.Count; i++)
            {
                var section = toc[i];
                _clientUpdate.SendResearchProgressUpdate(
                    researchId,
                    $"Gathering information for section {i + 1} of {toc.Count}: {section}"
                );

                // Query memory for comprehensive information about this section
                var memoryResults = await _memory.AskAsync(
                    researchId,
                    $"Provide a comprehensive analysis of '{section}', including all relevant facts, data, and findings. Include specific details and ensure all information is properly attributed to sources.",
                    cancellationToken
                );

                if (memoryResults.RelevantSources.Count < 1)
                {
                    _logger.LogWarning(
                        "[{ResearchId}] No memory results found for section: {Section}",
                        researchId,
                        section
                    );
                    continue;
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

                // Store the section content and its sources
                sectionContents.Add((section, memoryResults.Result, sources));
            }

            _clientUpdate.SendResearchProgressUpdate(researchId, "Synthesizing final report...");

            // Prepare the synthesis prompt
            var synthesisPrompt = new StringBuilder();
            synthesisPrompt.AppendLine(
                $"Generate a comprehensive exhaustive research report on '{research.Title}'."
            );
            synthesisPrompt.AppendLine(
                "\nYou will be provided with section-by-section content and their associated sources. Your task is to:"
            );
            synthesisPrompt.AppendLine("1. Synthesize the information into a cohesive report");
            synthesisPrompt.AppendLine(
                "2. Remove redundancies and overlapping information between sections"
            );
            synthesisPrompt.AppendLine("3. Ensure proper flow and transitions between sections");
            synthesisPrompt.AppendLine(
                "4. Include all relevant citations using [Author, Year] format"
            );
            synthesisPrompt.AppendLine("5. Format the output in Markdown");
            synthesisPrompt.AppendLine("\nHere is the section-by-section content:\n");

            foreach (var (section, content, sources) in sectionContents)
            {
                synthesisPrompt.AppendLine($"## {section}");
                synthesisPrompt.AppendLine("Content:");
                synthesisPrompt.AppendLine(content);
                synthesisPrompt.AppendLine("\nSources for this section:");
                foreach (var source in sources.DistinctBy(s => s.Url))
                {
                    synthesisPrompt.AppendLine(
                        $"- {source.Title} | {source.Author} | {source.Url} | {source.PublishedDate}"
                    );
                }
                synthesisPrompt.AppendLine("\n---\n");
            }

            synthesisPrompt.AppendLine(
                "\nPlease generate the final report following these guidelines:"
            );
            synthesisPrompt.AppendLine(
                "1. Use proper Markdown formatting (headers, lists, emphasis)"
            );
            synthesisPrompt.AppendLine("2. Include a table of contents at the start");
            synthesisPrompt.AppendLine(
                "3. End with a References section listing all cited sources"
            );
            synthesisPrompt.AppendLine("4. Ensure all factual claims have appropriate citations");

            // Generate the final report using Gemini
            var chatHistory = new ChatHistory();
            chatHistory.AddUserMessage(synthesisPrompt.ToString());

            var finalReport = await _chat.GetChatMessageContentAsync(
                chatHistory,
                cancellationToken: cancellationToken
            );

            // Save the report
            var report = new ResearchReport
            {
                Content = finalReport.Content,
                ResearchId = research.Id,
            };
            research.Report = report;
            research.Status = ResearchStatus.Complete;

            await _repository.ResearchReports.AddAsync(report, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            _clientUpdate.SendResearchProgressUpdate(researchId, "Report generation complete!");
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
