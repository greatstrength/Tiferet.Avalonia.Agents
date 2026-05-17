namespace Tiferet.Avalonia.Agents.Domain;

// *** records

// ** record: conversation_info
/// <summary>
/// View-layer DTO representing a conversation session summary.
/// Mirrors the tiferet-agents <c>Conversation</c> Python domain object.
/// </summary>
public record ConversationInfo
{
    // * attribute: id
    /// <summary>UUID string uniquely identifying this conversation.</summary>
    public required string Id { get; init; }

    // * attribute: agent_id
    /// <summary>ID of the agent configuration used in this conversation.</summary>
    public required string AgentId { get; init; }

    // * attribute: thread_id
    /// <summary>LangGraph thread identifier for checkpointer state.</summary>
    public required string ThreadId { get; init; }

    // * attribute: title
    /// <summary>Human-readable conversation title.</summary>
    public string Title { get; init; } = string.Empty;

    // * attribute: status
    /// <summary>Conversation status: active or archived.</summary>
    public string Status { get; init; } = "active";

    // * attribute: created_at
    /// <summary>ISO 8601 creation timestamp.</summary>
    public string CreatedAt { get; init; } = DateTimeOffset.UtcNow.ToString("o");

    // * attribute: updated_at
    /// <summary>ISO 8601 last-updated timestamp.</summary>
    public string UpdatedAt { get; init; } = DateTimeOffset.UtcNow.ToString("o");
}
