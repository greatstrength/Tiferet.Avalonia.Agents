namespace Tiferet.Avalonia.Agents.Domain;

// *** records

// ** record: tool_call_request
/// <summary>
/// A tool call request issued by the AI during graph execution.
/// Mirrors the tool_calls entries in tiferet-agents <c>Message</c>.
/// </summary>
public record ToolCallRequest
{
    // * attribute: id
    /// <summary>Unique identifier for this tool call.</summary>
    public required string Id { get; init; }

    // * attribute: name
    /// <summary>The name of the tool being invoked.</summary>
    public required string Name { get; init; }

    // * attribute: arguments
    /// <summary>JSON-serialized arguments for the tool call.</summary>
    public string Arguments { get; init; } = "{}";

    // * attribute: requires_approval
    /// <summary>Whether this tool call requires human approval before execution.</summary>
    public bool RequiresApproval { get; init; }
}
