using Avalonia.Controls;
using Avalonia.Input;
using Tiferet.Avalonia.Agents.Contexts;
using Tiferet.Avalonia.Agents.Domain;
using Tiferet.Avalonia.Agents.Interfaces;
using Tiferet.Avalonia.Agents.Services;

namespace Tiferet.Avalonia.Agents.Assets.Views;

// *** views

// ** view: agent_window
/// <summary>
/// Top-level shell window for the agent chat interface.
/// Contains a conversation sidebar with agent selection and the main ChatView panel.
/// Wires up ViewModels, mock services, and conversation selection callbacks.
/// </summary>
public partial class AgentWindow : Window
{
    // * attribute: chat_view_model
    private readonly ChatViewModel _chatViewModel;

    // * attribute: conversation_list_view_model
    private readonly ConversationListViewModel _conversationListViewModel;

    // * attribute: agent_service
    private readonly IAgentService _agentService;

    // * init
    /// <summary>
    /// Initializes the AgentWindow with mock services and wired ViewModels.
    /// </summary>
    public AgentWindow()
    {
        InitializeComponent();

        // Create mock services.
        var chatService = new MockAgentChatService();
        var conversationService = new MockConversationService();
        _agentService = new MockAgentService();

        // Create ViewModels.
        _chatViewModel = new ChatViewModel(chatService);
        _conversationListViewModel = new ConversationListViewModel(conversationService, _agentService);

        // Wire the conversation selection callback.
        _conversationListViewModel.ConversationSelected += OnConversationSelected;

        // Assign DataContexts.
        ChatPanel.DataContext = _chatViewModel;
        ConversationList.DataContext = _conversationListViewModel;

        // Load initial data when the window opens.
        Opened += OnWindowOpened;

        // Register global keyboard shortcuts.
        KeyDown += OnWindowKeyDown;
    }

    // * method: on_window_key_down
    /// <summary>
    /// Handle global keyboard shortcuts.
    /// Ctrl+N creates a new conversation. Escape deselects the current conversation.
    /// </summary>
    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        // Ctrl+N: New conversation.
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control) && e.Key == Key.N)
        {
            e.Handled = true;
            _conversationListViewModel.NewConversationCommand.Execute(null);
            return;
        }

        // Escape: Deselect conversation and clear chat.
        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            _conversationListViewModel.SelectedConversation = null;
            return;
        }
    }

    // * method: on_window_opened
    /// <summary>
    /// Load agents and conversations when the window first opens.
    /// </summary>
    private async void OnWindowOpened(object? sender, EventArgs e)
    {
        await _conversationListViewModel.LoadAgentsCommand.ExecuteAsync(null);
        await _conversationListViewModel.LoadConversationsCommand.ExecuteAsync(null);
    }

    // * method: on_conversation_selected
    /// <summary>
    /// Handle conversation selection changes from the sidebar.
    /// Updates the chat panel with the selected conversation's context.
    /// </summary>
    private async void OnConversationSelected(ConversationInfo? conversation)
    {
        if (conversation is null)
        {
            _chatViewModel.ClearMessages();
            return;
        }

        // Resolve the agent name for the header.
        var agent = await _agentService.GetAsync(conversation.AgentId);
        var agentName = agent?.Name ?? "Agent";

        // Load the conversation into the chat panel.
        _chatViewModel.LoadConversation(conversation, agentName);
    }
}
