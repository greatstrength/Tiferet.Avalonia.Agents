using Tiferet.Avalonia.Agents.Domain;
using Tiferet.Avalonia.Agents.Interfaces;

namespace Tiferet.Avalonia.Agents.Services;

// *** services

// ** service: mock_agent_service
/// <summary>
/// In-memory mock implementation of <see cref="IAgentService"/>.
/// Provides sample agent configurations for development, testing, and design-time use.
/// </summary>
public class MockAgentService : IAgentService
{
    // * attribute: agents
    private static readonly List<AgentInfo> Agents =
    [
        new AgentInfo
        {
            Id = "agent-tiferet",
            Name = "Tiferet Assistant",
            Description = "A domain-driven design assistant powered by the Tiferet framework.",
            Provider = "openai",
            Model = "gpt-4o-mini",
        },
        new AgentInfo
        {
            Id = "agent-general",
            Name = "General Assistant",
            Description = "A general-purpose conversational assistant.",
            Provider = "anthropic",
            Model = "claude-sonnet-4-20250514",
        },
    ];

    // * method: get_async
    /// <summary>
    /// Retrieve an agent configuration by ID.
    /// </summary>
    public Task<AgentInfo?> GetAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var agent = Agents.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(agent);
    }

    // * method: list_async
    /// <summary>
    /// List all available agent configurations.
    /// </summary>
    public Task<IReadOnlyList<AgentInfo>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<AgentInfo>>(Agents);
    }
}
