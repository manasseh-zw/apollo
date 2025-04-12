using System.Text;
using Apollo.Agents.Helpers;
using Apollo.Agents.Plugins;
using Apollo.Config;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Apollo.Agents.Research;

public interface IResearchPlanner
{
    Task StartChatSession(
        string sessionId,
        string initialQuery,
        string connectionId,
        string userId
    );

    Task ContinueChatSession(string message, string connectionId);
}

public class ResearchPlanner : IResearchPlanner
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chat;
    private readonly IMemoryCache _cache;
    private readonly IChatStreamingCallback _streamingCallback;
    private readonly ILogger<ResearchPlanner> _logger;
    private readonly SaveResearchPlugin _saveResearchPlugin;
    private static readonly TimeSpan _cacheTimeout = TimeSpan.FromHours(1);

    public ResearchPlanner(
        IMemoryCache cache,
        IChatStreamingCallback streamingCallback,
        ILogger<ResearchPlanner> logger,
        SaveResearchPlugin saveResearchPlugin
    )
    {
        _logger = logger;
        _cache = cache;
        _streamingCallback = streamingCallback;
        _saveResearchPlugin = saveResearchPlugin;

        _logger.LogInformation("Initializing ResearchAssistant");

        _kernel = Kernel
            .CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: AppConfig.Models.Gpt4o,
                endpoint: AppConfig.AzureAI.Endpoint,
                apiKey: AppConfig.AzureAI.ApiKey
            )
            .Build();

        _chat = _kernel.GetRequiredService<IChatCompletionService>();

        _kernel.Plugins.AddFromObject(_saveResearchPlugin, "SaveResearch");

        _logger.LogInformation("ResearchAssistant initialized successfully");
    }

    public async Task StartChatSession(
        string sessionId,
        string initialQuery,
        string connectionId,
        string userId
    )
    {
        _logger.LogInformation(
            "Starting new chat session. SessionId: {SessionId}, UserId: {UserId}, Query: {Query}",
            sessionId,
            userId,
            initialQuery
        );

        var chatState = new ChatState
        {
            SessionId = sessionId,
            ConnectionId = connectionId,
            UserId = userId,
            ChatHistory =
            [
                new ChatMessageContent(AuthorRole.System, Prompts.ResearchPlanner(userId)),
                new ChatMessageContent(AuthorRole.User, initialQuery),
            ],
        };

        _cache.Set(connectionId, chatState, _cacheTimeout);
        _logger.LogDebug(
            "Chat state initialized and cached for connection {ConnectionId}",
            connectionId
        );

        await StreamResponse(connectionId, chatState.ChatHistory);
    }

    public async Task ContinueChatSession(string message, string connectionId)
    {
        if (!_cache.TryGetValue(connectionId, out ChatState? chatState) || chatState == null)
        {
            _logger.LogWarning(
                "Failed to continue chat session - no state found for connection {ConnectionId}",
                connectionId
            );
            return;
        }

        _logger.LogInformation(
            "Continuing chat session. SessionId: {SessionId}, Message: {Message}",
            chatState.SessionId,
            message
        );

        chatState.ChatHistory.Add(new ChatMessageContent(AuthorRole.User, content: message));

        _cache.Set(connectionId, chatState, _cacheTimeout);
        await StreamResponse(connectionId, chatState.ChatHistory);
    }

    private async Task StreamResponse(string connectionId, ChatHistory chatHistory)
    {
        var responseBuffer = new StringBuilder();

        if (!_cache.TryGetValue(connectionId, out ChatState? chatState) || chatState == null)
        {
            _logger.LogWarning(
                "Failed to stream response - no state found for connection {ConnectionId}",
                connectionId
            );
            return;
        }

        _logger.LogDebug("Starting stream response for session {SessionId}", chatState.SessionId);

        var settings = new PromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            ExtensionData = new Dictionary<string, object> { { "userId", chatState.UserId } },
        };

        try
        {
            await foreach (
                var chunk in _chat.GetStreamingChatMessageContentsAsync(
                    chatHistory,
                    executionSettings: settings,
                    kernel: _kernel
                )
            )
            {
                _streamingCallback.StreamPlannerResponse(connectionId, chunk.Content ?? "");
                responseBuffer.Append(chunk.Content);
            }

            chatState.ChatHistory.Add(
                new ChatMessageContent(AuthorRole.Assistant, content: responseBuffer.ToString())
            );

            _cache.Set(connectionId, chatState, _cacheTimeout);

            _logger.LogInformation(
                "Stream response completed for session {SessionId}. Response length: {Length} characters",
                chatState.SessionId,
                responseBuffer.Length
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error streaming response for session {SessionId}: {Message}",
                chatState.SessionId,
                ex.Message
            );
            throw;
        }
    }
}

public class ChatState
{
    public required string SessionId { get; set; }

    public required string ConnectionId { get; set; }

    public required string UserId { get; set; }

    public ChatHistory ChatHistory { get; set; } = [];
}
