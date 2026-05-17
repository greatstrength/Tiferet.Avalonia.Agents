using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace Tiferet.Avalonia.Agents.Assets.Controls;

// *** controls

// ** control: tool_call_card
/// <summary>
/// An inline card that displays a pending tool call with Approve and Deny buttons.
/// Renders the tool name and arguments, and transitions through
/// :pending → :approved / :denied pseudo-class states.
/// </summary>
public class ToolCallCard : TemplatedControl
{
    // * attribute: tool_name_property
    /// <summary>The name of the tool being invoked.</summary>
    public static readonly StyledProperty<string> ToolNameProperty =
        AvaloniaProperty.Register<ToolCallCard, string>(nameof(ToolName), string.Empty);

    // * attribute: arguments_property
    /// <summary>JSON-serialized arguments for the tool call.</summary>
    public static readonly StyledProperty<string> ArgumentsProperty =
        AvaloniaProperty.Register<ToolCallCard, string>(nameof(Arguments), "{}");

    // * attribute: tool_call_id_property
    /// <summary>The tool call identifier.</summary>
    public static readonly StyledProperty<string> ToolCallIdProperty =
        AvaloniaProperty.Register<ToolCallCard, string>(nameof(ToolCallId), string.Empty);

    // * attribute: status_property
    /// <summary>Current status: pending, approved, or denied.</summary>
    public static readonly StyledProperty<string> StatusProperty =
        AvaloniaProperty.Register<ToolCallCard, string>(nameof(Status), "pending");

    // * attribute: approve_command_property
    /// <summary>Command invoked when the user approves the tool call.</summary>
    public static readonly StyledProperty<ICommand?> ApproveCommandProperty =
        AvaloniaProperty.Register<ToolCallCard, ICommand?>(nameof(ApproveCommand));

    // * attribute: deny_command_property
    /// <summary>Command invoked when the user denies the tool call.</summary>
    public static readonly StyledProperty<ICommand?> DenyCommandProperty =
        AvaloniaProperty.Register<ToolCallCard, ICommand?>(nameof(DenyCommand));

    // * property: tool_name
    public string ToolName
    {
        get => GetValue(ToolNameProperty);
        set => SetValue(ToolNameProperty, value);
    }

    // * property: arguments
    public string Arguments
    {
        get => GetValue(ArgumentsProperty);
        set => SetValue(ArgumentsProperty, value);
    }

    // * property: tool_call_id
    public string ToolCallId
    {
        get => GetValue(ToolCallIdProperty);
        set => SetValue(ToolCallIdProperty, value);
    }

    // * property: status
    public string Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    // * property: approve_command
    public ICommand? ApproveCommand
    {
        get => GetValue(ApproveCommandProperty);
        set => SetValue(ApproveCommandProperty, value);
    }

    // * property: deny_command
    public ICommand? DenyCommand
    {
        get => GetValue(DenyCommandProperty);
        set => SetValue(DenyCommandProperty, value);
    }

    // * init
    static ToolCallCard()
    {
        StatusProperty.Changed.AddClassHandler<ToolCallCard>((card, _) =>
            card.UpdateStatusPseudoClasses());
    }

    // * method: update_status_pseudo_classes
    private void UpdateStatusPseudoClasses()
    {
        SetPseudoClass(":pending", Status == "pending");
        SetPseudoClass(":approved", Status == "approved");
        SetPseudoClass(":denied", Status == "denied");
    }

    // * method: set_pseudo_class
    private void SetPseudoClass(string name, bool active)
    {
        if (active)
            PseudoClasses.Add(name);
        else
            PseudoClasses.Remove(name);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdateStatusPseudoClasses();
    }
}
