using Tiferet.Avalonia.Agents.Domain;

namespace Tiferet.Avalonia.Agents.Interfaces;

// *** interfaces

// ** interface: conversation_service
/// <summary>
/// Service contract for managing conversations.
/// </summary>
public interface IConversationService
{
    // * method: get_async
    /// <summary>Retrieve a conversation by ID.</summary>
    Task<ConversationInfo?> GetAsync(string id, CancellationToken cancellationToken = default);

    // * method: list_async
    /// <summary>List conversations with optional filters.</summary>
    Task<IReadOnlyList<ConversationInfo>> ListAsync(
        string? agentId = null,
        string? status = null,
        CancellationToken cancellationToken = default);

    // * method: delete_async
    /// <summary>Delete a conversation by ID.</summary>
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
