namespace Tiferet.Avalonia.Agents.Domain;

// *** records

// ** record: stream_token
/// <summary>
/// A single streamed token chunk from an LLM response.
/// </summary>
public record StreamToken
{
    // * attribute: content
    /// <summary>The text content of this token chunk.</summary>
    public required string Content { get; init; }

    // * attribute: is_final
    /// <summary>Whether this is the final token in the stream.</summary>
    public bool IsFinal { get; init; }

    // * attribute: metadata
    /// <summary>Optional metadata associated with this token (e.g., token usage).</summary>
    public Dictionary<string, object>? Metadata { get; init; }
}
