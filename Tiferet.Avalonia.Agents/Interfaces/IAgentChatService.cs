using Tiferet.Avalonia.Agents.Domain;

namespace Tiferet.Avalonia.Agents.Interfaces;

// *** interfaces

// ** interface: agent_chat_service
/// <summary>
/// Service contract for sending messages to an agent and receiving responses.
/// Supports both one-shot and streaming interaction modes.
/// </summary>
public interface IAgentChatService
{
    // * method: send_message_async
    /// <summary>
    /// Send a message to an agent and receive the complete AI response.
    /// </summary>
    /// <param name="agentId">The agent configuration identifier.</param>
    /// <param name="message">The user message content.</param>
    /// <param name="conversationId">Optional existing conversation ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The complete AI response message.</returns>
    Task<ChatMessage> SendMessageAsync(
        string agentId,
        string message,
        string? conversationId = null,
        CancellationToken cancellationToken = default);

    // * method: stream_message_async
    /// <summary>
    /// Send a message to an agent and stream the response token-by-token.
    /// </summary>
    /// <param name="agentId">The agent configuration identifier.</param>
    /// <param name="message">The user message content.</param>
    /// <param name="conversationId">Optional existing conversation ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of stream tokens.</returns>
    IAsyncEnumerable<StreamToken> StreamMessageAsync(
        string agentId,
        string message,
        string? conversationId = null,
        CancellationToken cancellationToken = default);
}
