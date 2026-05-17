using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tiferet.Avalonia.Agents.Domain;
using Tiferet.Avalonia.Agents.Interfaces;
using Tiferet.Avalonia.Contexts;

namespace Tiferet.Avalonia.Agents.Contexts;

// *** contexts

// ** context: chat_view_model
/// <summary>
/// ViewModel for the main chat panel.
/// Manages the message collection, user input, streaming, and send command.
/// </summary>
public partial class ChatViewModel : ViewModelBase
{
    // * attribute: chat_service
    private readonly IAgentChatService? _chatService;

    // * attribute: messages
    /// <summary>Observable collection of chat messages displayed in the conversation.</summary>
    public ObservableCollection<ChatMessage> Messages { get; } = [];

    // * attribute: current_input
    [ObservableProperty]
    private string _currentInput = string.Empty;

    // * attribute: is_sending
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
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

    // * init
    /// <summary>
    /// Initializes the ChatViewModel.
    /// </summary>
    /// <param name="chatService">Optional chat service for streaming. Null for design-time.</param>
    public ChatViewModel(IAgentChatService? chatService = null)
    {
        _chatService = chatService;
    }

    // * method: send_message
    /// <summary>
    /// Send a user message, then stream the AI response token-by-token.
    /// Falls back to echo mode when no IAgentChatService is available.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task SendMessageAsync(string text)
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

        // Clear the input and mark as sending.
        CurrentInput = string.Empty;
        IsSending = true;

        // Create a placeholder AI message for streaming.
        var aiMessage = new ChatMessage
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = ConversationId,
            Role = "ai",
            Content = string.Empty,
            IsStreaming = true,
        };
        Messages.Add(aiMessage);

        try
        {
            if (_chatService is not null)
            {
                // Stream tokens from the service.
                await foreach (var token in _chatService.StreamMessageAsync(
                    AgentId ?? "default", text, ConversationId))
                {
                    aiMessage.Content += token.Content;

                    // Notify the UI of the content change.
                    var index = Messages.IndexOf(aiMessage);
                    if (index >= 0)
                    {
                        Messages[index] = aiMessage;
                    }
                }
            }
            else
            {
                // Fallback echo mode (no service).
                aiMessage.Content = $"Echo: {text}";
                var index = Messages.IndexOf(aiMessage);
                if (index >= 0)
                    Messages[index] = aiMessage;
            }
        }
        finally
        {
            // Mark streaming complete.
            aiMessage.IsStreaming = false;
            var finalIndex = Messages.IndexOf(aiMessage);
            if (finalIndex >= 0)
                Messages[finalIndex] = aiMessage;

            IsSending = false;
        }
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
