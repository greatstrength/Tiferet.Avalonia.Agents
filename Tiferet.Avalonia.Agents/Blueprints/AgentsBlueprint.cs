using Microsoft.Extensions.DependencyInjection;
using Tiferet.Avalonia.Agents.Contexts;
using Tiferet.Avalonia.Agents.Interfaces;
using Tiferet.Avalonia.Agents.Services;

namespace Tiferet.Avalonia.Agents.Blueprints;

// *** blueprints

// ** blueprint: agents_blueprint
/// <summary>
/// Static blueprint for bootstrapping Tiferet.Avalonia.Agents services.
/// Registers service interfaces, mock implementations, and ViewModels
/// into an <see cref="IServiceCollection"/>.
/// </summary>
public static class AgentsBlueprint
{
    // * method: configure_services
    /// <summary>
    /// Register all agent services and ViewModels into the DI container.
    /// When <see cref="TiferetAgentsOptions.MockServices"/> is true, registers
    /// in-memory mock implementations for development and design-time use.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="options">The agent configuration options.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection ConfigureServices(
        IServiceCollection services,
        TiferetAgentsOptions options)
    {
        // Register the options for injection.
        services.AddSingleton(options);

        // Register mock service implementations when enabled.
        if (options.MockServices)
        {
            services.AddSingleton<IAgentChatService>(
                _ => new MockAgentChatService(options.TokenDelayMs));
            services.AddSingleton<IConversationService, MockConversationService>();
            services.AddSingleton<IAgentService, MockAgentService>();
        }

        // Register ViewModels as transient (new instance per resolve).
        services.AddTransient<ChatViewModel>();
        services.AddTransient<ConversationListViewModel>();

        return services;
    }
}
