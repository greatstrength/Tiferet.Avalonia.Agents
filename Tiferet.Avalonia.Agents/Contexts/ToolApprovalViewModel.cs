using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tiferet.Avalonia.Agents.Domain;
using Tiferet.Avalonia.Agents.Interfaces;
using Tiferet.Avalonia.Contexts;

namespace Tiferet.Avalonia.Agents.Contexts;

// *** contexts

// ** context: tool_approval_view_model
/// <summary>
/// ViewModel for managing a single pending tool call approval flow.
/// Wraps a <see cref="ToolCallRequest"/> and exposes Approve/Deny commands
/// that delegate to <see cref="IToolApprovalService"/>.
/// </summary>
public partial class ToolApprovalViewModel : ViewModelBase
{
    // * attribute: approval_service
    private readonly IToolApprovalService? _approvalService;

    // * attribute: tool_call
    /// <summary>The tool call request being managed.</summary>
    [ObservableProperty]
    private ToolCallRequest _toolCall;

    // * attribute: status
    /// <summary>Current approval status: pending, approved, or denied.</summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApproveCommand))]
    [NotifyCanExecuteChangedFor(nameof(DenyCommand))]
    private string _status = "pending";

    // * attribute: result_message
    /// <summary>The response message after approval/denial, if available.</summary>
    [ObservableProperty]
    private ChatMessage? _resultMessage;

    // * attribute: agent_id
    /// <summary>The agent configuration identifier.</summary>
    public string AgentId { get; }

    // * attribute: thread_id
    /// <summary>The conversation thread ID for graph resumption.</summary>
    public string ThreadId { get; }

    // * init
    /// <summary>
    /// Initializes the ToolApprovalViewModel.
    /// </summary>
    /// <param name="toolCall">The pending tool call request.</param>
    /// <param name="agentId">The agent identifier.</param>
    /// <param name="threadId">The thread identifier.</param>
    /// <param name="approvalService">Optional approval service. Null for design-time.</param>
    public ToolApprovalViewModel(
        ToolCallRequest toolCall,
        string agentId,
        string threadId,
        IToolApprovalService? approvalService = null)
    {
        _toolCall = toolCall;
        AgentId = agentId;
        ThreadId = threadId;
        _approvalService = approvalService;
    }

    // * method: approve
    /// <summary>
    /// Approve the pending tool call and resume graph execution.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsPending))]
    private async Task ApproveAsync()
    {
        Status = "approved";

        if (_approvalService is not null)
        {
            ResultMessage = await _approvalService.ApproveAsync(
                AgentId, ThreadId, ToolCall.Id);
        }
    }

    // * method: deny
    /// <summary>
    /// Deny the pending tool call and resume graph execution with a rejection.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsPending))]
    private async Task DenyAsync()
    {
        Status = "denied";

        if (_approvalService is not null)
        {
            ResultMessage = await _approvalService.DenyAsync(
                AgentId, ThreadId, ToolCall.Id);
        }
    }

    // * method: is_pending
    /// <summary>Whether the tool call is still pending approval.</summary>
    private bool IsPending() => Status == "pending";
}
