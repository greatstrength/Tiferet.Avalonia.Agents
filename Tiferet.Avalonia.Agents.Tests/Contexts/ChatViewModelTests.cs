using Tiferet.Avalonia.Agents.Contexts;
using Tiferet.Avalonia.Agents.Domain;
using Tiferet.Avalonia.Agents.Services;

namespace Tiferet.Avalonia.Agents.Tests.Contexts;

// *** tests

// ** test: chat_view_model
/// <summary>
/// Tests for <see cref="ChatViewModel"/>.
/// </summary>
public class ChatViewModelTests
{
    // * method: create_sample_conversation
    private static ConversationInfo CreateSampleConversation() => new()
    {
        Id = "conv-test",
        AgentId = "agent-test",
        ThreadId = "thread-test",
        Title = "Test Conversation",
    };

    // * method: initial_state_is_empty
    [Fact]
    public void InitialState_IsEmpty()
    {
        var vm = new ChatViewModel();

        Assert.Empty(vm.Messages);
        Assert.Empty(vm.PendingApprovals);
        Assert.Null(vm.ConversationId);
        Assert.Null(vm.AgentId);
        Assert.Equal("Agent", vm.AgentName);
        Assert.Equal(string.Empty, vm.CurrentInput);
        Assert.False(vm.IsSending);
    }

    // * method: send_message_echo_mode_adds_two_messages
    [Fact]
    public async Task SendMessageAsync_EchoMode_AddsTwoMessages()
    {
        var vm = new ChatViewModel();

        await vm.SendMessageCommand.ExecuteAsync("test input");

        // Should have human + ai echo messages.
        Assert.Equal(2, vm.Messages.Count);
        Assert.Equal("human", vm.Messages[0].Role);
        Assert.Equal("test input", vm.Messages[0].Content);
        Assert.Equal("ai", vm.Messages[1].Role);
        Assert.Contains("Echo: test input", vm.Messages[1].Content);
    }

    // * method: send_message_echo_mode_generates_conversation_id
    [Fact]
    public async Task SendMessageAsync_EchoMode_GeneratesConversationId()
    {
        var vm = new ChatViewModel();

        await vm.SendMessageCommand.ExecuteAsync("test");

        Assert.NotNull(vm.ConversationId);
    }

    // * method: send_message_clears_input
    [Fact]
    public async Task SendMessageAsync_ClearsInput()
    {
        var vm = new ChatViewModel();
        vm.CurrentInput = "some text";

        await vm.SendMessageCommand.ExecuteAsync("some text");

        Assert.Equal(string.Empty, vm.CurrentInput);
    }

    // * method: send_message_resets_is_sending
    [Fact]
    public async Task SendMessageAsync_ResetIsSending()
    {
        var vm = new ChatViewModel();

        await vm.SendMessageCommand.ExecuteAsync("test");

        Assert.False(vm.IsSending);
    }

    // * method: send_message_with_service_streams_response
    [Fact]
    public async Task SendMessageAsync_WithService_StreamsResponse()
    {
        var service = new MockAgentChatService(tokenDelayMs: 0);
        var vm = new ChatViewModel(service);

        await vm.SendMessageCommand.ExecuteAsync("hello");

        Assert.Equal(2, vm.Messages.Count);
        Assert.Equal("ai", vm.Messages[1].Role);
        Assert.Contains("Tiferet Assistant", vm.Messages[1].Content);
    }

    // * method: send_message_marks_streaming_complete
    [Fact]
    public async Task SendMessageAsync_MarksStreamingComplete()
    {
        var service = new MockAgentChatService(tokenDelayMs: 0);
        var vm = new ChatViewModel(service);

        await vm.SendMessageCommand.ExecuteAsync("hello");

        Assert.False(vm.Messages[1].IsStreaming);
    }

    // * method: send_message_preserves_conversation_id
    [Fact]
    public async Task SendMessageAsync_PreservesConversationId()
    {
        var vm = new ChatViewModel();
        vm.ConversationId = "existing-convo";

        await vm.SendMessageCommand.ExecuteAsync("test");

        Assert.Equal("existing-convo", vm.ConversationId);
        Assert.Equal("existing-convo", vm.Messages[0].ConversationId);
    }

    // * method: send_message_ignores_whitespace
    [Fact]
    public async Task SendMessageAsync_IgnoresWhitespace()
    {
        var vm = new ChatViewModel();

        await vm.SendMessageCommand.ExecuteAsync("   ");

        Assert.Empty(vm.Messages);
    }

    // * method: load_conversation_sets_fields
    [Fact]
    public void LoadConversation_SetsFields()
    {
        var vm = new ChatViewModel();
        var conversation = CreateSampleConversation();

        vm.LoadConversation(conversation, "Test Agent");

        Assert.Equal("conv-test", vm.ConversationId);
        Assert.Equal("agent-test", vm.AgentId);
        Assert.Equal("Test Agent", vm.AgentName);
    }

    // * method: load_conversation_clears_existing_messages
    [Fact]
    public async Task LoadConversation_ClearsExistingMessages()
    {
        var vm = new ChatViewModel();
        await vm.SendMessageCommand.ExecuteAsync("old message");
        Assert.NotEmpty(vm.Messages);

        vm.LoadConversation(CreateSampleConversation());

        Assert.Empty(vm.Messages);
    }

    // * method: clear_messages_resets_all_state
    [Fact]
    public async Task ClearMessages_ResetsAllState()
    {
        var vm = new ChatViewModel();
        vm.CurrentInput = "some text";
        await vm.SendMessageCommand.ExecuteAsync("test");

        vm.ClearMessages();

        Assert.Empty(vm.Messages);
        Assert.Empty(vm.PendingApprovals);
        Assert.Null(vm.ConversationId);
        Assert.Null(vm.AgentId);
        Assert.Equal("Agent", vm.AgentName);
        Assert.Equal(string.Empty, vm.CurrentInput);
    }

    // * method: load_design_time_messages_populates
    [Fact]
    public void LoadDesignTimeMessages_Populates()
    {
        var vm = new ChatViewModel();

        vm.LoadDesignTimeMessages();

        Assert.True(vm.Messages.Count >= 4);
        Assert.NotNull(vm.ConversationId);
        Assert.Equal("Tiferet Assistant", vm.AgentName);
    }

    // * method: generate_markdown_export_empty
    [Fact]
    public void GenerateMarkdownExport_Empty_ContainsHeader()
    {
        var vm = new ChatViewModel();

        var md = vm.GenerateMarkdownExport();

        Assert.Contains("# Agent Conversation", md);
        Assert.Contains("Exported:", md);
    }

    // * method: generate_markdown_export_with_messages
    [Fact]
    public async Task GenerateMarkdownExport_WithMessages_ContainsRoles()
    {
        var vm = new ChatViewModel();
        await vm.SendMessageCommand.ExecuteAsync("hello world");

        var md = vm.GenerateMarkdownExport();

        Assert.Contains("### Human", md);
        Assert.Contains("hello world", md);
        Assert.Contains("### Agent", md);
    }

    // * method: generate_json_export_empty
    [Fact]
    public void GenerateJsonExport_Empty_IsValidJson()
    {
        var vm = new ChatViewModel();

        var json = vm.GenerateJsonExport();

        Assert.Contains("\"agentName\": \"Agent\"", json);
        Assert.Contains("\"messages\": []", json);
    }

    // * method: generate_json_export_with_messages
    [Fact]
    public async Task GenerateJsonExport_WithMessages_ContainsMessageData()
    {
        var vm = new ChatViewModel();
        await vm.SendMessageCommand.ExecuteAsync("test export");

        var json = vm.GenerateJsonExport();

        Assert.Contains("\"role\": \"human\"", json);
        Assert.Contains("test export", json);
        Assert.Contains("\"role\": \"ai\"", json);
    }

    // * method: generate_markdown_export_uses_agent_name
    [Fact]
    public async Task GenerateMarkdownExport_UsesAgentName()
    {
        var vm = new ChatViewModel();
        var conversation = CreateSampleConversation();
        vm.LoadConversation(conversation, "Tiferet Bot");
        await vm.SendMessageCommand.ExecuteAsync("hello");

        var md = vm.GenerateMarkdownExport();

        Assert.Contains("# Tiferet Bot Conversation", md);
        Assert.Contains("### Tiferet Bot", md);
    }
}
