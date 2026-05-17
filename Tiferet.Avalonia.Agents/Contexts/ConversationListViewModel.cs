using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tiferet.Avalonia.Agents.Domain;
using Tiferet.Avalonia.Agents.Interfaces;
using Tiferet.Avalonia.Contexts;

namespace Tiferet.Avalonia.Agents.Contexts;

// *** contexts

// ** context: conversation_list_view_model
/// <summary>
/// ViewModel for the conversation sidebar.
/// Manages the conversation list, agent selection, and conversation lifecycle commands.
/// </summary>
public partial class ConversationListViewModel : ViewModelBase
{
    // * attribute: conversation_service
    private readonly IConversationService? _conversationService;

    // * attribute: agent_service
    private readonly IAgentService? _agentService;

    // * attribute: conversations
    /// <summary>Observable collection of conversations displayed in the sidebar.</summary>
    public ObservableCollection<ConversationInfo> Conversations { get; } = [];

    // * attribute: agents
    /// <summary>Observable collection of available agent configurations.</summary>
    public ObservableCollection<AgentInfo> Agents { get; } = [];

    // * attribute: selected_conversation
    [ObservableProperty]
    private ConversationInfo? _selectedConversation;

    // * attribute: selected_agent
    [ObservableProperty]
    private AgentInfo? _selectedAgent;

    // * attribute: is_loading
    [ObservableProperty]
    private bool _isLoading;

    // * attribute: conversation_selected
    /// <summary>
    /// Callback invoked when the selected conversation changes.
    /// Used by the parent window to update the chat panel.
    /// </summary>
    public Action<ConversationInfo?>? ConversationSelected { get; set; }

    // * init
    /// <summary>
    /// Initializes the ConversationListViewModel.
    /// </summary>
    /// <param name="conversationService">Optional conversation service. Null for design-time.</param>
    /// <param name="agentService">Optional agent service. Null for design-time.</param>
    public ConversationListViewModel(
        IConversationService? conversationService = null,
        IAgentService? agentService = null)
    {
        _conversationService = conversationService;
        _agentService = agentService;
    }

    // * method: on_selected_conversation_changed
    /// <summary>
    /// Invoked automatically when <see cref="SelectedConversation"/> changes.
    /// Notifies the parent via the <see cref="ConversationSelected"/> callback.
    /// </summary>
    partial void OnSelectedConversationChanged(ConversationInfo? value)
    {
        ConversationSelected?.Invoke(value);
    }

    // * method: on_selected_agent_changed
    /// <summary>
    /// Invoked automatically when <see cref="SelectedAgent"/> changes.
    /// Reloads the conversation list filtered by the selected agent.
    /// </summary>
    partial void OnSelectedAgentChanged(AgentInfo? value)
    {
        if (_conversationService is not null)
            LoadConversationsCommand.Execute(null);
    }

    // * method: load_conversations
    /// <summary>
    /// Load conversations from the service, optionally filtered by selected agent.
    /// </summary>
    [RelayCommand]
    private async Task LoadConversationsAsync()
    {
        if (_conversationService is null) return;

        IsLoading = true;

        try
        {
            var agentId = SelectedAgent?.Id;
            var conversations = await _conversationService.ListAsync(agentId: agentId);

            Conversations.Clear();
            foreach (var conversation in conversations)
                Conversations.Add(conversation);
        }
        finally
        {
            IsLoading = false;
        }
    }

    // * method: load_agents
    /// <summary>
    /// Load available agent configurations from the service.
    /// </summary>
    [RelayCommand]
    private async Task LoadAgentsAsync()
    {
        if (_agentService is null) return;

        var agents = await _agentService.ListAsync();

        Agents.Clear();
        foreach (var agent in agents)
            Agents.Add(agent);
    }

    // * method: new_conversation
    /// <summary>
    /// Create a new conversation placeholder and select it.
    /// Uses the currently selected agent, or the first available agent.
    /// </summary>
    [RelayCommand]
    private void NewConversation()
    {
        var agent = SelectedAgent ?? (Agents.Count > 0 ? Agents[0] : null);
        var agentId = agent?.Id ?? "default";

        var conversation = new ConversationInfo
        {
            Id = Guid.NewGuid().ToString(),
            AgentId = agentId,
            ThreadId = Guid.NewGuid().ToString(),
            Title = "New Conversation",
            Status = "active",
        };

        // Insert at the top of the list.
        Conversations.Insert(0, conversation);

        // Select the new conversation.
        SelectedConversation = conversation;
    }

    // * method: delete_conversation
    /// <summary>
    /// Delete a conversation by ID and remove it from the collection.
    /// </summary>
    [RelayCommand]
    private async Task DeleteConversationAsync(string id)
    {
        if (_conversationService is not null)
            await _conversationService.DeleteAsync(id);

        var conversation = Conversations.FirstOrDefault(c => c.Id == id);
        if (conversation is not null)
        {
            Conversations.Remove(conversation);

            // Clear selection if the deleted conversation was selected.
            if (SelectedConversation?.Id == id)
                SelectedConversation = null;
        }
    }
}
