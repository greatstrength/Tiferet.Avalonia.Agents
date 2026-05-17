namespace Tiferet.Avalonia.Agents.Domain;

// *** records

// ** record: tool_call_result
/// <summary>
/// The result of a tool call execution.
/// </summary>
public record ToolCallResult
{
    // * attribute: tool_call_id
    /// <summary>ID of the tool call this result responds to.</summary>
    public required string ToolCallId { get; init; }

    // * attribute: content
    /// <summary>Text content of the tool result.</summary>
    public string Content { get; init; } = string.Empty;

    // * attribute: is_error
    /// <summary>Whether the tool execution resulted in an error.</summary>
    public bool IsError { get; init; }
}
