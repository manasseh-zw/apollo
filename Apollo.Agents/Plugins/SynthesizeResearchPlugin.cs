using System.ComponentModel;
using Apollo.Agents.Research;
using Apollo.Agents.State;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Apollo.Agents.Plugins;

public class SynthesizeResearchPlugin
{
    private readonly IResearchReportGenerator _reportGenerator;
    private readonly IStateManager _state;
    private readonly ILogger<SynthesizeResearchPlugin> _logger;

    public SynthesizeResearchPlugin(
        IResearchReportGenerator reportGenerator,
        IStateManager state,
        ILogger<SynthesizeResearchPlugin> logger
    )
    {
        _reportGenerator = reportGenerator;
        _state = state;
        _logger = logger;
    }

    [KernelFunction]
    [Description("Synthesizes all gathered research information into a final report.")]
    public async Task<string> SynthesizeFinalReportAsync(
        [Description("The research ID")] string researchId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _reportGenerator.GenerateReportAsync(researchId, cancellationToken);

            _state.MarkResearchComplete(researchId);

            return "Research synthesis completed successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[{ResearchId}] Error synthesizing research: {Message}",
                researchId,
                ex.Message
            );
            throw;
        }
    }
}
