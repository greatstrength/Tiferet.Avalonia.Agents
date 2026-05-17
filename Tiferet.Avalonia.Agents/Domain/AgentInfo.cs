namespace Tiferet.Avalonia.Agents.Domain;

// *** records

// ** record: agent_info
/// <summary>
/// View-layer DTO representing an agent configuration summary.
/// Mirrors the tiferet-agents <c>AgentConfiguration</c> Python domain object.
/// </summary>
public record AgentInfo
{
    // * attribute: id
    /// <summary>Unique agent configuration identifier.</summary>
    public required string Id { get; init; }

    // * attribute: name
    /// <summary>Human-readable agent name.</summary>
    public required string Name { get; init; }

    // * attribute: description
    /// <summary>Description of the agent's purpose.</summary>
    public string Description { get; init; } = string.Empty;

    // * attribute: provider
    /// <summary>LLM provider name (e.g., openai, anthropic).</summary>
    public string Provider { get; init; } = "openai";

    // * attribute: model
    /// <summary>Model identifier within the provider.</summary>
    public string Model { get; init; } = "gpt-4o-mini";
}
