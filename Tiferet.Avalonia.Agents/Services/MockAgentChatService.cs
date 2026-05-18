using System.Runtime.CompilerServices;
using Tiferet.Avalonia.Agents.Domain;
using Tiferet.Avalonia.Agents.Interfaces;

namespace Tiferet.Avalonia.Agents.Services;

// *** services

// ** service: mock_agent_chat_service
/// <summary>
/// In-memory mock implementation of <see cref="IAgentChatService"/>.
/// Simulates LLM streaming by yielding tokens word-by-word with a configurable delay.
/// Intended for development, testing, and design-time use.
/// </summary>
public class MockAgentChatService : IAgentChatService
{
    // * attribute: token_delay
    private readonly TimeSpan _tokenDelay;

    // * init
    /// <summary>
    /// Initializes the mock service with a per-token delay.
    /// </summary>
    /// <param name="tokenDelayMs">Delay in milliseconds between each streamed token. Default: 50ms.</param>
    public MockAgentChatService(int tokenDelayMs = 50)
    {
        _tokenDelay = TimeSpan.FromMilliseconds(tokenDelayMs);
    }

    // * method: send_message_async
    /// <summary>
    /// Send a message and return the complete AI response (non-streaming).
    /// </summary>
    public async Task<ChatMessage> SendMessageAsync(
        string agentId,
        string message,
        string? conversationId = null,
        CancellationToken cancellationToken = default)
    {
        // Simulate a small processing delay.
        await Task.Delay(200, cancellationToken);

        var response = GenerateResponse(message);
        var convoId = conversationId ?? Guid.NewGuid().ToString();

        return new ChatMessage
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = convoId,
            Role = "ai",
            Content = response,
        };
    }

    // * method: stream_message_async
    /// <summary>
    /// Stream a response token-by-token, simulating LLM output.
    /// Splits the generated response into words and yields each as a token.
    /// </summary>
    public async IAsyncEnumerable<StreamToken> StreamMessageAsync(
        string agentId,
        string message,
        string? conversationId = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = GenerateResponse(message);
        var words = response.Split(' ');

        for (var i = 0; i < words.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Add a leading space for all words after the first.
            var content = i == 0 ? words[i] : $" {words[i]}";
            var isFinal = i == words.Length - 1;

            yield return new StreamToken
            {
                Content = content,
                IsFinal = isFinal,
            };

            if (!isFinal)
                await Task.Delay(_tokenDelay, cancellationToken);
        }
    }

    // * method: generate_response (static)
    /// <summary>
    /// Generate a canned response based on the user message.
    /// </summary>
    private static string GenerateResponse(string message)
    {
        var lower = message.ToLowerInvariant();

        if (lower.Contains("hello") || lower.Contains("hi"))
            return "Hello! I'm the Tiferet Assistant. How can I help you today?";

        if (lower.Contains("tiferet"))
            return "Tiferet is a framework for Domain-Driven Design that harmonizes code and concept, providing a graceful path to craft software that reflects its intended purpose with wisdom and precision.";

        if (lower.Contains("calculator") || lower.Contains("calc"))
            return "I can help with calculations! The calculator supports addition, subtraction, multiplication, division, and exponentiation. What would you like to compute?";

        if (lower.Contains("code") || lower.Contains("example") || lower.Contains("snippet"))
            return "Here's an example of a Tiferet domain event:\n\n```python\nfrom tiferet.events import DomainEvent\n\nclass AddNumber(DomainEvent):\n    def execute(self, a, b, **kwargs):\n        return a + b\n```\n\nThis event adds two numbers and returns the result.";

        if (lower.Contains("tool"))
            return "I have access to several tools that can help with your request. Would you like me to use one? I'll ask for your approval before executing any tool.";

        return $"Thank you for your message. You said: \"{message}\". I'm a mock agent running locally — in production, this response would come from a real LLM via the tiferet-agents backend.";
    }
}
