using Tiferet.Avalonia.Agents.Domain;

namespace Tiferet.Avalonia.Agents.Interfaces;

// *** interfaces

// ** interface: tool_approval_service
/// <summary>
/// Service contract for approving or denying pending tool calls
/// on a paused agent graph.
/// </summary>
public interface IToolApprovalService
{
    // * method: approve_async
    /// <summary>
    /// Approve a pending tool call and resume graph execution.
    /// </summary>
    /// <param name="agentId">The agent configuration identifier.</param>
    /// <param name="threadId">The thread ID of the paused graph.</param>
    /// <param name="toolCallId">The tool call identifier to approve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The AI response message after tool execution.</returns>
    Task<ChatMessage> ApproveAsync(
        string agentId,
        string threadId,
        string toolCallId,
        CancellationToken cancellationToken = default);

    // * method: deny_async
    /// <summary>
    /// Deny a pending tool call and resume graph execution with a rejection.
    /// </summary>
    /// <param name="agentId">The agent configuration identifier.</param>
    /// <param name="threadId">The thread ID of the paused graph.</param>
    /// <param name="toolCallId">The tool call identifier to deny.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The AI response message after denial.</returns>
    Task<ChatMessage> DenyAsync(
        string agentId,
        string threadId,
        string toolCallId,
        CancellationToken cancellationToken = default);
}
