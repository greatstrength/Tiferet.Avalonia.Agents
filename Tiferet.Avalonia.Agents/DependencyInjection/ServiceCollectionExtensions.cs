using Microsoft.Extensions.DependencyInjection;
using Tiferet.Avalonia.Agents.Blueprints;

namespace Tiferet.Avalonia.Agents.DependencyInjection;

// *** extensions

// ** extension: service_collection_extensions
/// <summary>
/// Extension methods for integrating Tiferet.Avalonia.Agents with
/// <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    // * method: add_tiferet_agents
    /// <summary>
    /// Add Tiferet agent chat services, ViewModels, and mock implementations
    /// to the DI container, using an explicit configuration callback.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configure">Callback to configure <see cref="TiferetAgentsOptions"/>.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddTiferetAgents(
        this IServiceCollection services,
        Action<TiferetAgentsOptions> configure)
    {
        var options = new TiferetAgentsOptions();
        configure(options);
        return AgentsBlueprint.ConfigureServices(services, options);
    }

    // * method: add_tiferet_agents_default
    /// <summary>
    /// Add Tiferet agent chat services with default options (mock services enabled).
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddTiferetAgents(
        this IServiceCollection services)
    {
        return AgentsBlueprint.ConfigureServices(services, new TiferetAgentsOptions());
    }
}
