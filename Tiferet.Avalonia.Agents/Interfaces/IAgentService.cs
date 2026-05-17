using Tiferet.Avalonia.Agents.Domain;

namespace Tiferet.Avalonia.Agents.Interfaces;

// *** interfaces

// ** interface: agent_service
/// <summary>
/// Service contract for retrieving agent configurations.
/// </summary>
public interface IAgentService
{
    // * method: get_async
    /// <summary>Retrieve an agent configuration by ID.</summary>
    Task<AgentInfo?> GetAsync(string id, CancellationToken cancellationToken = default);

    // * method: list_async
    /// <summary>List all available agent configurations.</summary>
    Task<IReadOnlyList<AgentInfo>> ListAsync(CancellationToken cancellationToken = default);
}
