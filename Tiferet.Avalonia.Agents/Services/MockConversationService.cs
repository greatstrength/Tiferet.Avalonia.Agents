using Tiferet.Avalonia.Agents.Domain;
using Tiferet.Avalonia.Agents.Interfaces;

namespace Tiferet.Avalonia.Agents.Services;

// *** services

// ** service: mock_conversation_service
/// <summary>
/// In-memory mock implementation of <see cref="IConversationService"/>.
/// Provides sample conversation data for development, testing, and design-time use.
/// </summary>
public class MockConversationService : IConversationService
{
    // * attribute: conversations
    private readonly List<ConversationInfo> _conversations;

    // * init
    /// <summary>
    /// Initializes the mock service with sample conversation data.
    /// </summary>
    public MockConversationService()
    {
        _conversations =
        [
            new ConversationInfo
            {
                Id = "conv-001",
                AgentId = "agent-tiferet",
                ThreadId = "thread-001",
                Title = "Getting Started with Tiferet",
                Status = "active",
                CreatedAt = DateTimeOffset.UtcNow.AddHours(-2).ToString("o"),
                UpdatedAt = DateTimeOffset.UtcNow.AddMinutes(-30).ToString("o"),
            },
            new ConversationInfo
            {
                Id = "conv-002",
                AgentId = "agent-tiferet",
                ThreadId = "thread-002",
                Title = "Calculator Domain Events",
                Status = "active",
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-1).ToString("o"),
                UpdatedAt = DateTimeOffset.UtcNow.AddHours(-4).ToString("o"),
            },
            new ConversationInfo
            {
                Id = "conv-003",
                AgentId = "agent-general",
                ThreadId = "thread-003",
                Title = "Architecture Discussion",
                Status = "active",
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-3).ToString("o"),
                UpdatedAt = DateTimeOffset.UtcNow.AddDays(-1).ToString("o"),
            },
        ];
    }

    // * method: get_async
    /// <summary>
    /// Retrieve a conversation by ID.
    /// </summary>
    public Task<ConversationInfo?> GetAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var conversation = _conversations.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(conversation);
    }

    // * method: list_async
    /// <summary>
    /// List conversations with optional agent and status filters.
    /// </summary>
    public Task<IReadOnlyList<ConversationInfo>> ListAsync(
        string? agentId = null,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<ConversationInfo> result = _conversations;

        if (!string.IsNullOrEmpty(agentId))
            result = result.Where(c => c.AgentId == agentId);

        if (!string.IsNullOrEmpty(status))
            result = result.Where(c => c.Status == status);

        // Order by most recently updated first.
        var list = result
            .OrderByDescending(c => c.UpdatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<ConversationInfo>>(list);
    }

    // * method: delete_async
    /// <summary>
    /// Delete a conversation by ID.
    /// </summary>
    public Task DeleteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        _conversations.RemoveAll(c => c.Id == id);
        return Task.CompletedTask;
    }
}
