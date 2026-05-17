using Tiferet.Avalonia.Agents.Services;

namespace Tiferet.Avalonia.Agents.Tests.Services;

// *** tests

// ** test: mock_agent_chat_service
/// <summary>
/// Tests for <see cref="MockAgentChatService"/>.
/// </summary>
public class MockAgentChatServiceTests
{
    // * method: send_message_returns_ai_role
    [Fact]
    public async Task SendMessageAsync_ReturnsAiRole()
    {
        var service = new MockAgentChatService(tokenDelayMs: 0);

        var result = await service.SendMessageAsync("agent-1", "hello");

        Assert.Equal("ai", result.Role);
    }

    // * method: send_message_uses_provided_conversation_id
    [Fact]
    public async Task SendMessageAsync_UsesProvidedConversationId()
    {
        var service = new MockAgentChatService(tokenDelayMs: 0);

        var result = await service.SendMessageAsync("agent-1", "hello", conversationId: "conv-123");

        Assert.Equal("conv-123", result.ConversationId);
    }

    // * method: send_message_generates_conversation_id_when_null
    [Fact]
    public async Task SendMessageAsync_GeneratesConversationId_WhenNull()
    {
        var service = new MockAgentChatService(tokenDelayMs: 0);

        var result = await service.SendMessageAsync("agent-1", "hello");

        Assert.False(string.IsNullOrEmpty(result.ConversationId));
    }

    // * method: send_message_hello_response
    [Fact]
    public async Task SendMessageAsync_HelloResponse()
    {
        var service = new MockAgentChatService(tokenDelayMs: 0);

        var result = await service.SendMessageAsync("agent-1", "hello there");

        Assert.Contains("Tiferet Assistant", result.Content);
    }

    // * method: send_message_tiferet_response
    [Fact]
    public async Task SendMessageAsync_TiferetResponse()
    {
        var service = new MockAgentChatService(tokenDelayMs: 0);

        var result = await service.SendMessageAsync("agent-1", "What is Tiferet?");

        Assert.Contains("Domain-Driven Design", result.Content);
    }

    // * method: send_message_calculator_response
    [Fact]
    public async Task SendMessageAsync_CalculatorResponse()
    {
        var service = new MockAgentChatService(tokenDelayMs: 0);

        var result = await service.SendMessageAsync("agent-1", "use the calculator");

        Assert.Contains("calculations", result.Content);
    }

    // * method: send_message_tool_response
    [Fact]
    public async Task SendMessageAsync_ToolResponse()
    {
        var service = new MockAgentChatService(tokenDelayMs: 0);

        var result = await service.SendMessageAsync("agent-1", "use a tool please");

        Assert.Contains("tools", result.Content);
    }

    // * method: send_message_default_response
    [Fact]
    public async Task SendMessageAsync_DefaultResponse()
    {
        var service = new MockAgentChatService(tokenDelayMs: 0);

        var result = await service.SendMessageAsync("agent-1", "random message xyz");

        Assert.Contains("random message xyz", result.Content);
        Assert.Contains("mock agent", result.Content);
    }

    // * method: stream_message_reassembles_to_full_response
    [Fact]
    public async Task StreamMessageAsync_ReassemblesToFullResponse()
    {
        var service = new MockAgentChatService(tokenDelayMs: 0);

        var content = string.Empty;
        await foreach (var token in service.StreamMessageAsync("agent-1", "hello"))
        {
            content += token.Content;
        }

        Assert.Contains("Tiferet Assistant", content);
    }

    // * method: stream_message_last_token_is_final
    [Fact]
    public async Task StreamMessageAsync_LastTokenIsFinal()
    {
        var service = new MockAgentChatService(tokenDelayMs: 0);

        var tokens = new List<Domain.StreamToken>();
        await foreach (var token in service.StreamMessageAsync("agent-1", "hello"))
        {
            tokens.Add(token);
        }

        Assert.True(tokens.Count > 0);
        Assert.True(tokens[^1].IsFinal);

        // All tokens except the last should not be final.
        foreach (var token in tokens.Take(tokens.Count - 1))
        {
            Assert.False(token.IsFinal);
        }
    }

    // * method: stream_message_first_token_has_no_leading_space
    [Fact]
    public async Task StreamMessageAsync_FirstTokenHasNoLeadingSpace()
    {
        var service = new MockAgentChatService(tokenDelayMs: 0);

        var first = true;
        await foreach (var token in service.StreamMessageAsync("agent-1", "hello"))
        {
            if (first)
            {
                Assert.False(token.Content.StartsWith(' '));
                first = false;
            }
        }
    }

    // * method: stream_message_yields_multiple_tokens
    [Fact]
    public async Task StreamMessageAsync_YieldsMultipleTokens()
    {
        var service = new MockAgentChatService(tokenDelayMs: 0);

        var count = 0;
        await foreach (var _ in service.StreamMessageAsync("agent-1", "hello"))
        {
            count++;
        }

        // "Hello! I'm the Tiferet Assistant. How can I help you today?" has multiple words.
        Assert.True(count > 1);
    }
}
