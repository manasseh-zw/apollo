using Apollo.Agents.Helpers;
using Apollo.Agents.Plugins;
using Apollo.Agents.State;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Apollo.Agents.Research;

#pragma warning disable SKEXP0050 // Experimental: ChatCompletionAgent

public static class AgentFactory
{
    public const string ResearchCoordinatorAgentName = "ResearchCoordinator";
    public const string ResearchEngineAgentName = "ResearchEngine";
    public const string ResearchAnalyzerAgentName = "ResearchAnalyzer";

    private const string StatePluginName = nameof(StatePlugin);
    private const string ResearchEnginePluginName = "Research_Engine";
    private const string KernelMemoryPluginName = "Research_Memory";
    private const string SynthesizeResearchPluginName = "Research_Synthesis";

    public static ChatCompletionAgent CreateResearchCoordinator(
        IKernelBuilder kernelBuilder,
        IStateManager state,
        IClientUpdateCallback streamingCallback,
        string researchId,
        SynthesizeResearchPlugin synthesizeResearchPluginInstance
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

        var synthesizeResearchPlugin = KernelPluginFactory.CreateFromObject(
            synthesizeResearchPluginInstance,
            SynthesizeResearchPluginName
        );
        kernel.Plugins.Add(synthesizeResearchPlugin);

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
            researchEnginePluginInstance,
            ResearchEnginePluginName
        );
        kernel.Plugins.Add(researchEnginePlugin);

        var executionSettings = new PromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Required(),
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
        KernelMemoryPlugin kernelMemoryPluginInstance
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

        var executionSettings = new PromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Required(),
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
}
