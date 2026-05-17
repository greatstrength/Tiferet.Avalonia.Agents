using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tiferet.Avalonia.Agents.Domain;
using Tiferet.Avalonia.Contexts;

namespace Tiferet.Avalonia.Agents.Contexts;

// *** contexts

// ** context: chat_view_model
/// <summary>
/// ViewModel for the main chat panel.
/// Manages the message collection, user input, and send command.
/// </summary>
public partial class ChatViewModel : ViewModelBase
{
    // * attribute: messages
    /// <summary>Observable collection of chat messages displayed in the conversation.</summary>
    public ObservableCollection<ChatMessage> Messages { get; } = [];

    // * attribute: current_input
    [ObservableProperty]
    private string _currentInput = string.Empty;

    // * attribute: is_sending
    [ObservableProperty]
    private bool _isSending;

    // * attribute: conversation_id
    [ObservableProperty]
    private string? _conversationId;

    // * attribute: agent_id
    [ObservableProperty]
    private string? _agentId;

    // * attribute: agent_name
    [ObservableProperty]
    private string _agentName = "Agent";

    // * method: send_message
    /// <summary>
    /// Send a user message and add it to the conversation.
    /// In this static prototype, adds a hardcoded AI echo response.
    /// Will be wired to IAgentChatService in Milestone 2.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSend))]
    private void SendMessage(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        // Generate a conversation ID if none exists.
        ConversationId ??= Guid.NewGuid().ToString();

        // Add the user message.
        Messages.Add(new ChatMessage
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = ConversationId,
            Role = "human",
            Content = text,
        });

        // Static prototype: add a placeholder AI response.
        Messages.Add(new ChatMessage
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = ConversationId,
            Role = "ai",
            Content = $"Echo: {text}",
        });

        // Clear the input.
        CurrentInput = string.Empty;
    }

    // * method: can_send
    private bool CanSend(string text)
        => !IsSending && !string.IsNullOrWhiteSpace(text);

    // * method: load_design_time_messages
    /// <summary>
    /// Populate the message list with sample data for design-time preview.
    /// </summary>
    public void LoadDesignTimeMessages()
    {
        var convoId = Guid.NewGuid().ToString();
        ConversationId = convoId;
        AgentName = "Tiferet Assistant";

        Messages.Add(new ChatMessage
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = convoId,
            Role = "system",
            Content = "You are a helpful assistant.",
        });
        Messages.Add(new ChatMessage
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = convoId,
            Role = "human",
            Content = "What is Tiferet?",
        });
        Messages.Add(new ChatMessage
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = convoId,
            Role = "ai",
            Content = "Tiferet is a framework for Domain-Driven Design that harmonizes code and concept, providing a graceful path to craft software that reflects its intended purpose.",
        });
        Messages.Add(new ChatMessage
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = convoId,
            Role = "human",
            Content = "Can you run the calculator?",
        });
        Messages.Add(new ChatMessage
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = convoId,
            Role = "ai",
            Content = "I'll invoke the calculator tool for you.",
            ToolCalls =
            [
                new ToolCallRequest
                {
                    Id = "tc_001",
                    Name = "calc.add",
                    Arguments = "{\"a\": 2, \"b\": 3}",
                    RequiresApproval = true,
                }
            ],
        });
        Messages.Add(new ChatMessage
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = convoId,
            Role = "tool",
            Content = "5",
            ToolCallId = "tc_001",
        });
    }
}
