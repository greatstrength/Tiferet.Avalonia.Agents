using Tiferet.Avalonia.Agents.Services;

namespace Tiferet.Avalonia.Agents.Tests.Services;

// *** tests

// ** test: mock_agent_service
/// <summary>
/// Tests for <see cref="MockAgentService"/>.
/// </summary>
public class MockAgentServiceTests
{
    // * method: list_async_returns_all_agents
    [Fact]
    public async Task ListAsync_ReturnsAllAgents()
    {
        var service = new MockAgentService();

        var result = await service.ListAsync();

        Assert.Equal(2, result.Count);
    }

    // * method: list_async_returns_expected_agent_names
    [Fact]
    public async Task ListAsync_ReturnsExpectedAgentNames()
    {
        var service = new MockAgentService();

        var result = await service.ListAsync();

        Assert.Contains(result, a => a.Name == "Tiferet Assistant");
        Assert.Contains(result, a => a.Name == "General Assistant");
    }

    // * method: get_async_returns_agent_by_id
    [Fact]
    public async Task GetAsync_ReturnsAgentById()
    {
        var service = new MockAgentService();

        var result = await service.GetAsync("agent-tiferet");

        Assert.NotNull(result);
        Assert.Equal("agent-tiferet", result.Id);
        Assert.Equal("Tiferet Assistant", result.Name);
    }

    // * method: get_async_returns_null_for_unknown_id
    [Fact]
    public async Task GetAsync_ReturnsNull_ForUnknownId()
    {
        var service = new MockAgentService();

        var result = await service.GetAsync("nonexistent");

        Assert.Null(result);
    }
}
