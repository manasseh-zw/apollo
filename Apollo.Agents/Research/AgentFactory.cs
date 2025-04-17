using Apollo.Agents.Helpers;
using Apollo.Agents.Plugins;
using Apollo.Agents.State;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
// using Microsoft.SemanticKernel.Connectors.OpenAI; // Keep if used elsewhere
using Microsoft.SemanticKernel.Plugins.Core; // For TimePlugin

namespace Apollo.Agents.Research;

#pragma warning disable SKEXP0050 // Experimental: ChatCompletionAgent

public static class AgentFactory
{
    public const string ResearchCoordinatorAgentName = "ResearchCoordinator";
    public const string ResearchEngineAgentName = "ResearchEngine";
    public const string ResearchAnalyzerAgentName = "ResearchAnalyzer";
    public const string ReportSynthesizerAgentName = "ReportSynthesizer";

    // Plugin Names (consistent with Orchestrator's original intent)
    private const string StatePluginName = nameof(StatePlugin); // Or keep as "StatePlugin" string literal
    private const string ResearchEnginePluginName = "Research.Engine";
    private const string KernelMemoryPluginName = "Research.Memory";
    private const string CompleteResearchPluginName = "Research.Complete";

    public static ChatCompletionAgent CreateResearchCoordinator(
        IKernelBuilder kernelBuilder,
        IStateManager state, // Pass the state manager service
        IClientUpdateCallback streamingCallback,
        string researchId
    )
    {
        var kernel = kernelBuilder.Build();

        // Create and add StatePlugin (unique instance per agent)
        var statePluginInstance = new StatePlugin(
            state, // Use the passed state manager
            kernel.LoggerFactory.CreateLogger<StatePlugin>(),
            researchId
        );
        var statePlugin = KernelPluginFactory.CreateFromObject(
            statePluginInstance,
            StatePluginName // Use consistent name
        );
        kernel.Plugins.Add(statePlugin);

        // Add Time Plugin
        kernel.Plugins.AddFromType<TimePlugin>();

        var executionSettings = new PromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };

        return new ChatCompletionAgent()
        {
            Kernel = kernel,
            Id = ResearchCoordinatorAgentName,
            Name = ResearchCoordinatorAgentName,
            Instructions = Prompts.ResearchCoordinator,
            Arguments = new KernelArguments(executionSettings),
        };
    }

    public static ChatCompletionAgent CreateResearchEngine(
        IKernelBuilder kernelBuilder,
        IStateManager state,
        IClientUpdateCallback streamingCallback,
        string researchId,
        ResearchEnginePlugin researchEnginePluginInstance
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
            StatePluginName
        );
        kernel.Plugins.Add(statePlugin);

        var researchEnginePlugin = KernelPluginFactory.CreateFromObject(
            researchEnginePluginInstance, // Use the passed instance
            ResearchEnginePluginName // Use the defined name
        );
        kernel.Plugins.Add(researchEnginePlugin);

        kernel.Plugins.AddFromType<TimePlugin>();

        var executionSettings = new PromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };

        return new ChatCompletionAgent()
        {
            Kernel = kernel,
            Id = ResearchEngineAgentName,
            Name = ResearchEngineAgentName,
            Instructions = Prompts.ResearchEngine,
            Arguments = new KernelArguments(executionSettings),
        };
    }

    public static ChatCompletionAgent CreateResearchAnalyzer(
        IKernelBuilder kernelBuilder,
        IStateManager state,
        IClientUpdateCallback streamingCallback,
        string researchId,
        KernelMemoryPlugin kernelMemoryPluginInstance // Pass the actual plugin instance
    )
    {
        var kernel = kernelBuilder.Build();

        // Create and add StatePlugin
        var statePluginInstance = new StatePlugin(
            state,
            kernel.LoggerFactory.CreateLogger<StatePlugin>(),
            researchId
        );
        var statePlugin = KernelPluginFactory.CreateFromObject(
            statePluginInstance,
            StatePluginName
        );
        kernel.Plugins.Add(statePlugin);

        var kernelMemoryPlugin = KernelPluginFactory.CreateFromObject(
            kernelMemoryPluginInstance,
            KernelMemoryPluginName
        );
        kernel.Plugins.Add(kernelMemoryPlugin);

        // Add Time Plugin
        kernel.Plugins.AddFromType<TimePlugin>();

        var executionSettings = new PromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };

        return new ChatCompletionAgent()
        {
            Kernel = kernel,
            Id = ResearchAnalyzerAgentName,
            Name = ResearchAnalyzerAgentName,
            Instructions = Prompts.ResearchAnalyzer,
            Arguments = new KernelArguments(executionSettings),
        };
    }

    public static ChatCompletionAgent CreateReportSynthesizer(
        IKernelBuilder kernelBuilder,
        IStateManager state,
        IClientUpdateCallback streamingCallback,
        string researchId,
        KernelMemoryPlugin kernelMemoryPluginInstance, // Pass the actual plugin instance
        CompleteResearchPlugin completeResearchPluginInstance // Pass the actual plugin instance
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
            StatePluginName
        );
        kernel.Plugins.Add(statePlugin);

        var kernelMemoryPlugin = KernelPluginFactory.CreateFromObject(
            kernelMemoryPluginInstance,
            KernelMemoryPluginName
        );
        kernel.Plugins.Add(kernelMemoryPlugin);

        var completeResearchPlugin = KernelPluginFactory.CreateFromObject(
            completeResearchPluginInstance,
            CompleteResearchPluginName
        );
        kernel.Plugins.Add(completeResearchPlugin);

        kernel.Plugins.AddFromType<TimePlugin>();

        var executionSettings = new PromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };

        return new ChatCompletionAgent()
        {
            Kernel = kernel,
            Id = ReportSynthesizerAgentName,
            Name = ReportSynthesizerAgentName,
            Instructions = Prompts.ReportSynthesizer,
            Arguments = new KernelArguments(executionSettings),
        };
    }
}
