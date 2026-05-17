namespace Tiferet.Avalonia.Agents.Domain;

// *** records

// ** record: chat_message
/// <summary>
/// View-layer DTO representing a single message within a conversation.
/// Mirrors the tiferet-agents <c>Message</c> Python domain object.
/// </summary>
public record ChatMessage
{
    // * attribute: id
    /// <summary>UUID string uniquely identifying this message.</summary>
    public required string Id { get; init; }

    // * attribute: conversation_id
    /// <summary>UUID of the parent conversation.</summary>
    public required string ConversationId { get; init; }

    // * attribute: role
    /// <summary>Message role: system, human, ai, or tool.</summary>
    public required string Role { get; init; }

    // * attribute: content
    /// <summary>Text content of the message.</summary>
    public string Content { get; set; } = string.Empty;

    // * attribute: tool_calls
    /// <summary>Tool call requests from the AI (for ai role messages).</summary>
    public List<ToolCallRequest> ToolCalls { get; init; } = [];

    // * attribute: tool_call_id
    /// <summary>ID of the tool call this message responds to (for tool role).</summary>
    public string? ToolCallId { get; init; }

    // * attribute: created_at
    /// <summary>ISO 8601 creation timestamp.</summary>
    public string CreatedAt { get; init; } = DateTimeOffset.UtcNow.ToString("o");

    // * attribute: is_streaming
    /// <summary>Whether this message is currently being streamed (UI-only state).</summary>
    public bool IsStreaming { get; set; }
}
