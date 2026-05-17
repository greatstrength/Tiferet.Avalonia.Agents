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

    // * attribute: approval_service
    private readonly IToolApprovalService? _approvalService;

    // * attribute: messages
    /// <summary>Observable collection of chat messages displayed in the conversation.</summary>
    public ObservableCollection<ChatMessage> Messages { get; } = [];

    // * attribute: pending_approvals
    /// <summary>Active tool approval view models keyed by tool call ID.</summary>
    public ObservableCollection<ToolApprovalViewModel> PendingApprovals { get; } = [];

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
    /// <param name="approvalService">Optional tool approval service. Null for design-time.</param>
    public ChatViewModel(
        IAgentChatService? chatService = null,
        IToolApprovalService? approvalService = null)
    {
        _chatService = chatService;
        _approvalService = approvalService;
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

            // Check for tool calls requiring approval.
            if (aiMessage.ToolCalls.Count > 0)
            {
                foreach (var toolCall in aiMessage.ToolCalls.Where(tc => tc.RequiresApproval))
                {
                    var approval = new ToolApprovalViewModel(
                        toolCall,
                        AgentId ?? "default",
                        ConversationId ?? string.Empty,
                        _approvalService);

                    // Subscribe to status changes to insert result messages.
                    approval.PropertyChanged += async (sender, args) =>
                    {
                        if (args.PropertyName != nameof(ToolApprovalViewModel.Status)) return;
                        if (sender is not ToolApprovalViewModel vm) return;
                        await HandleToolApprovalResultAsync(vm);
                    };

                    PendingApprovals.Add(approval);
                }
            }

            IsSending = false;
        }
    }

    // * method: handle_tool_approval_result
    /// <summary>
    /// Insert a tool result or denial message after an approval decision.
    /// </summary>
    private async Task HandleToolApprovalResultAsync(ToolApprovalViewModel approval)
    {
        if (approval.Status == "approved")
        {
            // Wait for the result message to arrive.
            if (approval.ResultMessage is not null)
            {
                Messages.Add(approval.ResultMessage);
            }
            else
            {
                // Insert a placeholder tool result.
                Messages.Add(new ChatMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    ConversationId = ConversationId ?? string.Empty,
                    Role = "tool",
                    Content = $"Tool '{approval.ToolCall.Name}' executed.",
                    ToolCallId = approval.ToolCall.Id,
                });
            }
        }
        else if (approval.Status == "denied")
        {
            Messages.Add(new ChatMessage
            {
                Id = Guid.NewGuid().ToString(),
                ConversationId = ConversationId ?? string.Empty,
                Role = "tool",
                Content = $"Tool '{approval.ToolCall.Name}' was denied by the user.",
                ToolCallId = approval.ToolCall.Id,
            });
        }

        // Remove from pending.
        PendingApprovals.Remove(approval);

        await Task.CompletedTask;
    }

    // * method: can_send
    private bool CanSend(string text)
        => !IsSending && !string.IsNullOrWhiteSpace(text);

    // * method: load_conversation
    /// <summary>
    /// Load a conversation into the chat panel, resetting current state.
    /// Sets the conversation ID, agent ID, and agent name from the provided info.
    /// </summary>
    /// <param name="conversation">The conversation to load.</param>
    /// <param name="agentName">Display name for the agent. Defaults to "Agent".</param>
    public void LoadConversation(ConversationInfo conversation, string agentName = "Agent")
    {
        ClearMessages();
        ConversationId = conversation.Id;
        AgentId = conversation.AgentId;
        AgentName = agentName;
    }

    // * method: clear_messages
    /// <summary>
    /// Clear all messages and pending approvals, resetting the conversation state.
    /// </summary>
    public void ClearMessages()
    {
        Messages.Clear();
        PendingApprovals.Clear();
        ConversationId = null;
        AgentId = null;
        AgentName = "Agent";
        CurrentInput = string.Empty;
    }

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
