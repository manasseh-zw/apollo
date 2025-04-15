using Apollo.Agents.Helpers;
using Apollo.Agents.Plugins;
using Apollo.Agents.State;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;

namespace Apollo.Agents.Research;

#pragma warning disable SKEXP0050
public static class AgentFactory
{
    public const string ResearchCoordinatorAgentName = "ResearchCoordinator";
    public const string ResearchEngineAgentName = "ResearchEngine";
    public const string ResearchAnalyzerAgentName = "ResearchAnalyzer";
    public const string ReportSynthesizerAgentName = "ReportSynthesizer";

    public static ChatCompletionAgent CreateAgent(
        IKernelBuilder kernelBuilder,
        IStateManager state,
        IClientUpdateCallback streamingCallback,
        string researchId,
        string agentName,
        string instructions,
        List<KernelPlugin>? agentPlugins = null
    )
    {
        var kernel = kernelBuilder.Build();
        var statePluginInstance = new StatePlugin(
            state,
            kernel.LoggerFactory.CreateLogger<StatePlugin>(),
            researchId
        );
        var statePlugin = KernelPluginFactory.CreateFromObject(
            statePluginInstance,
            nameof(StatePlugin)
        );

        kernel.Plugins.Add(statePlugin);

        if (agentPlugins != null)
        {
            foreach (var plugin in agentPlugins)
                kernel.Plugins.Add(plugin);
        }
        kernel.Plugins.AddFromType<TimePlugin>();

        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        };

        //add research id as agent instruction param
        return new ChatCompletionAgent()
        {
            Kernel = kernel,
            Id = agentName,
            Name = agentName,
            Instructions = instructions,
        };
    }

    public static ChatCompletionAgent CreateResearchCoordinator(
        IKernelBuilder kb,
        IStateManager sm,
        IClientUpdateCallback cb,
        string researchId
    ) =>
        CreateAgent(
            kb,
            sm,
            cb,
            researchId,
            ResearchCoordinatorAgentName,
            Prompts.ResearchCoordinator
        );

    public static ChatCompletionAgent CreateResearchEngine(
        IKernelBuilder kb,
        IStateManager sm,
        IClientUpdateCallback cb,
        string researchId,
        KernelPlugin researchProcessorPlugin
    ) =>
        CreateAgent(
            kb,
            sm,
            cb,
            researchId,
            ResearchEngineAgentName,
            Prompts.ResearchEngine,
            [researchProcessorPlugin]
        );

    public static ChatCompletionAgent CreateResearchAnalyzer(
        IKernelBuilder kb,
        IStateManager sm,
        IClientUpdateCallback cb,
        string researchId,
        KernelPlugin kernelMemoryPlugin
    ) =>
        CreateAgent(
            kb,
            sm,
            cb,
            researchId,
            ResearchAnalyzerAgentName,
            Prompts.ResearchAnalyzer,
            [kernelMemoryPlugin]
        );

    public static ChatCompletionAgent CreateReportSynthesizer(
        IKernelBuilder kb,
        IStateManager sm,
        IClientUpdateCallback cb,
        string researchId,
        KernelPlugin kernelMemoryPlugin
    ) =>
        CreateAgent(
            kb,
            sm,
            cb,
            researchId,
            ReportSynthesizerAgentName,
            Prompts.ReportSynthesizer,
            [kernelMemoryPlugin]
        );
}
