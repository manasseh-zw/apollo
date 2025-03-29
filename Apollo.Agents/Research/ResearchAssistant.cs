using System.Text;
using Apollo.Agents.Helpers;
using Apollo.Agents.Research.Plugins;
using Apollo.Config;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Apollo.Agents.Research;

public interface IResearchAssistant
{
    Task StartChatSession(
        string sessionId,
        string initialQuery,
        string connectionId,
        string userId
    );

    Task ContinueChatSession(string message, string connectionId);
}

public interface IChatStreamingCallback
{
    void OnStreamResponse(string connectionId, string? message);
}

public class ResearchAssistant : IResearchAssistant
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chat;
    private readonly IMemoryCache _cache;
    private readonly IChatStreamingCallback _streamingCallback;
    private static readonly TimeSpan _cacheTimeout = TimeSpan.FromHours(1);
    private readonly KernelPlugin _saveResearchPlugin;

    public ResearchAssistant(IMemoryCache cache, IChatStreamingCallback streamingCallback)
    {
        _kernel = Kernel
            .CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                Models.Gpt4o,
                AppConfig.AzureAI.Endpoint,
                AppConfig.AzureAI.ApiKey
            )
            .Build();

        _chat = _kernel.GetRequiredService<IChatCompletionService>();
        _saveResearchPlugin = _kernel.ImportPluginFromType<SaveResearchPlugin>();
        _cache = cache;
        _streamingCallback = streamingCallback;
    }

    public async Task StartChatSession(
        string sessionId,
        string initialQuery,
        string connectionId,
        string userId
    )
    {
        var chatState = new ChatState
        {
            SessionId = sessionId,
            ConnectionId = connectionId,
            UserId = userId,
            ChatHistory =
            [
                new ChatMessageContent(AuthorRole.System, Prompts.ResearchAssistant),
                new ChatMessageContent(AuthorRole.User, initialQuery),
            ],
        };

        _cache.Set(connectionId, chatState, _cacheTimeout);

        await StreamResponse(connectionId, chatState.ChatHistory);
    }

    public async Task ContinueChatSession(string message, string connectionId)
    {
        if (!_cache.TryGetValue(connectionId, out ChatState? chatState) || chatState == null)
        {
            return;
        }

        chatState.ChatHistory.Add(new ChatMessageContent(AuthorRole.User, content: message));

        _cache.Set(connectionId, chatState, _cacheTimeout);
        await StreamResponse(connectionId, chatState.ChatHistory);
    }

    private async Task StreamResponse(string connectionId, ChatHistory chatHistory)
    {
        var responseBuffer = new StringBuilder();

        if (!_cache.TryGetValue(connectionId, out ChatState? chatState) || chatState == null)
        {
            return;
        }

        var settings = new PromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            ExtensionData = new Dictionary<string, object> { { "userId", chatState.UserId } },
        };

        await foreach (
            var chunk in _chat.GetStreamingChatMessageContentsAsync(
                chatHistory,
                executionSettings: settings,
                kernel: _kernel
            )
        )
        {
            _streamingCallback.OnStreamResponse(connectionId, chunk.Content);
            responseBuffer.Append(chunk.Content);
        }

        chatState.ChatHistory.Add(
            new ChatMessageContent(AuthorRole.Assistant, content: responseBuffer.ToString())
        );

        _cache.Set(connectionId, chatState, _cacheTimeout);
    }
}

public class ChatState
{
    public string SessionId { get; set; }

    public string ConnectionId { get; set; }

    public string UserId { get; set; }

    public ChatHistory ChatHistory { get; set; } = [];
}

internal class ResearchPlanResponse
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string Depth { get; set; }
}
