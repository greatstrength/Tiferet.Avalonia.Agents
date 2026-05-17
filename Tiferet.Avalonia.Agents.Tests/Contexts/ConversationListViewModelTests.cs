using Tiferet.Avalonia.Agents.Contexts;
using Tiferet.Avalonia.Agents.Domain;
using Tiferet.Avalonia.Agents.Services;

namespace Tiferet.Avalonia.Agents.Tests.Contexts;

// *** tests

// ** test: conversation_list_view_model
/// <summary>
/// Tests for <see cref="ConversationListViewModel"/>.
/// Uses mock services for deterministic behavior.
/// </summary>
public class ConversationListViewModelTests
{
    // * method: create_view_model
    private static ConversationListViewModel CreateViewModel()
    {
        return new ConversationListViewModel(
            new MockConversationService(),
            new MockAgentService());
    }

    // * method: load_conversations_populates_collection
    [Fact]
    public async Task LoadConversationsAsync_PopulatesCollection()
    {
        var vm = CreateViewModel();

        await vm.LoadConversationsCommand.ExecuteAsync(null);

        Assert.Equal(3, vm.Conversations.Count);
    }

    // * method: load_agents_populates_collection
    [Fact]
    public async Task LoadAgentsAsync_PopulatesCollection()
    {
        var vm = CreateViewModel();

        await vm.LoadAgentsCommand.ExecuteAsync(null);

        Assert.Equal(2, vm.Agents.Count);
    }

    // * method: new_conversation_adds_to_top
    [Fact]
    public async Task NewConversation_AddsToTopOfList()
    {
        var vm = CreateViewModel();
        await vm.LoadConversationsCommand.ExecuteAsync(null);
        var originalCount = vm.Conversations.Count;

        vm.NewConversationCommand.Execute(null);

        Assert.Equal(originalCount + 1, vm.Conversations.Count);
        Assert.Equal("New Conversation", vm.Conversations[0].Title);
    }

    // * method: new_conversation_selects_new_conversation
    [Fact]
    public void NewConversation_SelectsNewConversation()
    {
        var vm = CreateViewModel();

        vm.NewConversationCommand.Execute(null);

        Assert.NotNull(vm.SelectedConversation);
        Assert.Equal("New Conversation", vm.SelectedConversation.Title);
    }

    // * method: new_conversation_uses_selected_agent
    [Fact]
    public async Task NewConversation_UsesSelectedAgent()
    {
        var vm = CreateViewModel();
        await vm.LoadAgentsCommand.ExecuteAsync(null);

        // Select the second agent.
        vm.SelectedAgent = vm.Agents[1];

        // Clear conversations loaded by agent change, then create new.
        vm.NewConversationCommand.Execute(null);

        Assert.NotNull(vm.SelectedConversation);
        Assert.Equal(vm.Agents[1].Id, vm.SelectedConversation.AgentId);
    }

    // * method: delete_conversation_removes_from_collection
    [Fact]
    public async Task DeleteConversationAsync_RemovesFromCollection()
    {
        var vm = CreateViewModel();
        await vm.LoadConversationsCommand.ExecuteAsync(null);
        var target = vm.Conversations[0];

        await vm.DeleteConversationCommand.ExecuteAsync(target.Id);

        Assert.DoesNotContain(vm.Conversations, c => c.Id == target.Id);
    }

    // * method: delete_conversation_clears_selection_if_selected
    [Fact]
    public async Task DeleteConversationAsync_ClearsSelection_IfSelected()
    {
        var vm = CreateViewModel();
        await vm.LoadConversationsCommand.ExecuteAsync(null);
        var target = vm.Conversations[0];
        vm.SelectedConversation = target;

        await vm.DeleteConversationCommand.ExecuteAsync(target.Id);

        Assert.Null(vm.SelectedConversation);
    }

    // * method: delete_conversation_preserves_selection_if_other
    [Fact]
    public async Task DeleteConversationAsync_PreservesSelection_IfOtherSelected()
    {
        var vm = CreateViewModel();
        await vm.LoadConversationsCommand.ExecuteAsync(null);
        var selected = vm.Conversations[0];
        var toDelete = vm.Conversations[1];
        vm.SelectedConversation = selected;

        await vm.DeleteConversationCommand.ExecuteAsync(toDelete.Id);

        Assert.Equal(selected, vm.SelectedConversation);
    }

    // * method: selected_conversation_invokes_callback
    [Fact]
    public async Task SelectedConversation_InvokesCallback()
    {
        var vm = CreateViewModel();
        await vm.LoadConversationsCommand.ExecuteAsync(null);

        ConversationInfo? callbackValue = null;
        vm.ConversationSelected = c => callbackValue = c;

        vm.SelectedConversation = vm.Conversations[0];

        Assert.NotNull(callbackValue);
        Assert.Equal(vm.Conversations[0].Id, callbackValue.Id);
    }

    // * method: selected_conversation_callback_receives_null_on_clear
    [Fact]
    public async Task SelectedConversation_CallbackReceivesNull_OnClear()
    {
        var vm = CreateViewModel();
        await vm.LoadConversationsCommand.ExecuteAsync(null);
        vm.SelectedConversation = vm.Conversations[0];

        ConversationInfo? callbackValue = vm.Conversations[0]; // start non-null
        vm.ConversationSelected = c => callbackValue = c;

        vm.SelectedConversation = null;

        Assert.Null(callbackValue);
    }

    // * method: is_loading_set_during_load
    [Fact]
    public async Task IsLoading_SetDuringLoad()
    {
        var vm = CreateViewModel();

        // After load completes, IsLoading should be false.
        await vm.LoadConversationsCommand.ExecuteAsync(null);

        Assert.False(vm.IsLoading);
    }

    // * method: load_conversations_without_service_does_nothing
    [Fact]
    public async Task LoadConversationsAsync_WithoutService_DoesNothing()
    {
        var vm = new ConversationListViewModel();

        await vm.LoadConversationsCommand.ExecuteAsync(null);

        Assert.Empty(vm.Conversations);
    }

    // * method: load_agents_without_service_does_nothing
    [Fact]
    public async Task LoadAgentsAsync_WithoutService_DoesNothing()
    {
        var vm = new ConversationListViewModel();

        await vm.LoadAgentsCommand.ExecuteAsync(null);

        Assert.Empty(vm.Agents);
    }
}
