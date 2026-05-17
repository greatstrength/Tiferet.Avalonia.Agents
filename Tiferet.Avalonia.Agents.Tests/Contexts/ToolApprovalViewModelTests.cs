using Tiferet.Avalonia.Agents.Contexts;
using Tiferet.Avalonia.Agents.Domain;

namespace Tiferet.Avalonia.Agents.Tests.Contexts;

// *** tests

// ** test: tool_approval_view_model
/// <summary>
/// Tests for <see cref="ToolApprovalViewModel"/>.
/// </summary>
public class ToolApprovalViewModelTests
{
    // * method: create_sample_tool_call
    private static ToolCallRequest CreateSampleToolCall() => new()
    {
        Id = "tc-001",
        Name = "calc.add",
        Arguments = "{\"a\": 1, \"b\": 2}",
        RequiresApproval = true,
    };

    // * method: initial_state_is_pending
    [Fact]
    public void InitialState_IsPending()
    {
        var vm = new ToolApprovalViewModel(
            CreateSampleToolCall(), "agent-1", "thread-1");

        Assert.Equal("pending", vm.Status);
        Assert.Null(vm.ResultMessage);
    }

    // * method: constructor_sets_properties
    [Fact]
    public void Constructor_SetsProperties()
    {
        var toolCall = CreateSampleToolCall();
        var vm = new ToolApprovalViewModel(toolCall, "agent-1", "thread-1");

        Assert.Equal(toolCall, vm.ToolCall);
        Assert.Equal("agent-1", vm.AgentId);
        Assert.Equal("thread-1", vm.ThreadId);
    }

    // * method: approve_sets_status_to_approved
    [Fact]
    public async Task ApproveAsync_SetsStatusToApproved()
    {
        var vm = new ToolApprovalViewModel(
            CreateSampleToolCall(), "agent-1", "thread-1");

        await vm.ApproveCommand.ExecuteAsync(null);

        Assert.Equal("approved", vm.Status);
    }

    // * method: deny_sets_status_to_denied
    [Fact]
    public async Task DenyAsync_SetsStatusToDenied()
    {
        var vm = new ToolApprovalViewModel(
            CreateSampleToolCall(), "agent-1", "thread-1");

        await vm.DenyCommand.ExecuteAsync(null);

        Assert.Equal("denied", vm.Status);
    }

    // * method: approve_without_service_leaves_result_null
    [Fact]
    public async Task ApproveAsync_WithoutService_LeavesResultNull()
    {
        var vm = new ToolApprovalViewModel(
            CreateSampleToolCall(), "agent-1", "thread-1");

        await vm.ApproveCommand.ExecuteAsync(null);

        Assert.Null(vm.ResultMessage);
    }

    // * method: deny_without_service_leaves_result_null
    [Fact]
    public async Task DenyAsync_WithoutService_LeavesResultNull()
    {
        var vm = new ToolApprovalViewModel(
            CreateSampleToolCall(), "agent-1", "thread-1");

        await vm.DenyCommand.ExecuteAsync(null);

        Assert.Null(vm.ResultMessage);
    }

    // * method: approve_cannot_execute_after_approved
    [Fact]
    public async Task ApproveCommand_CannotExecute_AfterApproved()
    {
        var vm = new ToolApprovalViewModel(
            CreateSampleToolCall(), "agent-1", "thread-1");

        await vm.ApproveCommand.ExecuteAsync(null);

        Assert.False(vm.ApproveCommand.CanExecute(null));
    }

    // * method: deny_cannot_execute_after_denied
    [Fact]
    public async Task DenyCommand_CannotExecute_AfterDenied()
    {
        var vm = new ToolApprovalViewModel(
            CreateSampleToolCall(), "agent-1", "thread-1");

        await vm.DenyCommand.ExecuteAsync(null);

        Assert.False(vm.DenyCommand.CanExecute(null));
    }

    // * method: deny_cannot_execute_after_approved
    [Fact]
    public async Task DenyCommand_CannotExecute_AfterApproved()
    {
        var vm = new ToolApprovalViewModel(
            CreateSampleToolCall(), "agent-1", "thread-1");

        await vm.ApproveCommand.ExecuteAsync(null);

        Assert.False(vm.DenyCommand.CanExecute(null));
    }

    // * method: approve_can_execute_while_pending
    [Fact]
    public void ApproveCommand_CanExecute_WhilePending()
    {
        var vm = new ToolApprovalViewModel(
            CreateSampleToolCall(), "agent-1", "thread-1");

        Assert.True(vm.ApproveCommand.CanExecute(null));
    }

    // * method: deny_can_execute_while_pending
    [Fact]
    public void DenyCommand_CanExecute_WhilePending()
    {
        var vm = new ToolApprovalViewModel(
            CreateSampleToolCall(), "agent-1", "thread-1");

        Assert.True(vm.DenyCommand.CanExecute(null));
    }

    // * method: status_change_raises_property_changed
    [Fact]
    public async Task StatusChange_RaisesPropertyChanged()
    {
        var vm = new ToolApprovalViewModel(
            CreateSampleToolCall(), "agent-1", "thread-1");

        var changedProperties = new List<string>();
        vm.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName is not null)
                changedProperties.Add(args.PropertyName);
        };

        await vm.ApproveCommand.ExecuteAsync(null);

        Assert.Contains("Status", changedProperties);
    }
}
