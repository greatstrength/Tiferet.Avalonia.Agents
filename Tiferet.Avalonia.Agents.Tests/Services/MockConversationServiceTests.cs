using Tiferet.Avalonia.Agents.Services;

namespace Tiferet.Avalonia.Agents.Tests.Services;

// *** tests

// ** test: mock_conversation_service
/// <summary>
/// Tests for <see cref="MockConversationService"/>.
/// </summary>
public class MockConversationServiceTests
{
    // * method: list_async_returns_all_conversations
    [Fact]
    public async Task ListAsync_ReturnsAllConversations()
    {
        var service = new MockConversationService();

        var result = await service.ListAsync();

        Assert.Equal(3, result.Count);
    }

    // * method: list_async_filters_by_agent_id
    [Fact]
    public async Task ListAsync_FiltersByAgentId()
    {
        var service = new MockConversationService();

        var result = await service.ListAsync(agentId: "agent-tiferet");

        Assert.Equal(2, result.Count);
        Assert.All(result, c => Assert.Equal("agent-tiferet", c.AgentId));
    }

    // * method: list_async_filters_by_status
    [Fact]
    public async Task ListAsync_FiltersByStatus()
    {
        var service = new MockConversationService();

        var result = await service.ListAsync(status: "active");

        Assert.Equal(3, result.Count);
        Assert.All(result, c => Assert.Equal("active", c.Status));
    }

    // * method: list_async_filters_by_agent_id_no_match
    [Fact]
    public async Task ListAsync_FiltersByAgentId_NoMatch_ReturnsEmpty()
    {
        var service = new MockConversationService();

        var result = await service.ListAsync(agentId: "nonexistent");

        Assert.Empty(result);
    }

    // * method: list_async_ordered_by_updated_at_descending
    [Fact]
    public async Task ListAsync_OrderedByUpdatedAtDescending()
    {
        var service = new MockConversationService();

        var result = await service.ListAsync();

        for (var i = 0; i < result.Count - 1; i++)
        {
            Assert.True(
                string.Compare(result[i].UpdatedAt, result[i + 1].UpdatedAt, StringComparison.Ordinal) >= 0,
                "Conversations should be ordered by UpdatedAt descending.");
        }
    }

    // * method: get_async_returns_conversation_by_id
    [Fact]
    public async Task GetAsync_ReturnsConversationById()
    {
        var service = new MockConversationService();

        var result = await service.GetAsync("conv-001");

        Assert.NotNull(result);
        Assert.Equal("conv-001", result.Id);
        Assert.Equal("Getting Started with Tiferet", result.Title);
    }

    // * method: get_async_returns_null_for_unknown_id
    [Fact]
    public async Task GetAsync_ReturnsNull_ForUnknownId()
    {
        var service = new MockConversationService();

        var result = await service.GetAsync("nonexistent");

        Assert.Null(result);
    }

    // * method: delete_async_removes_conversation
    [Fact]
    public async Task DeleteAsync_RemovesConversation()
    {
        var service = new MockConversationService();

        await service.DeleteAsync("conv-001");

        var result = await service.ListAsync();
        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, c => c.Id == "conv-001");
    }

    // * method: delete_async_idempotent_for_unknown_id
    [Fact]
    public async Task DeleteAsync_Idempotent_ForUnknownId()
    {
        var service = new MockConversationService();

        // Should not throw.
        await service.DeleteAsync("nonexistent");

        var result = await service.ListAsync();
        Assert.Equal(3, result.Count);
    }
}
