namespace Tiferet.Avalonia.Agents.Blueprints;

// *** blueprints

// ** blueprint: tiferet_agents_options
/// <summary>
/// Configuration POCO for bootstrapping Tiferet.Avalonia.Agents services.
/// Follows the <c>TiferetAvaloniaOptions</c> pattern from the parent framework.
/// </summary>
public class TiferetAgentsOptions
{
    // * property: agent_id
    /// <summary>Default agent configuration identifier.</summary>
    public string AgentId { get; set; } = "default";

    // * property: mock_services
    /// <summary>
    /// When true, registers mock service implementations for development and design-time.
    /// When false, consumers must register their own service implementations.
    /// </summary>
    public bool MockServices { get; set; } = true;

    // * property: token_delay_ms
    /// <summary>
    /// Delay in milliseconds between streamed tokens when using mock services.
    /// Only applies when <see cref="MockServices"/> is true.
    /// </summary>
    public int TokenDelayMs { get; set; } = 50;
}
