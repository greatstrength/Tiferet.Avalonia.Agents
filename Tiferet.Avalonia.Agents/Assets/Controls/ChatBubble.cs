using Avalonia;
using Avalonia.Controls.Primitives;

namespace Tiferet.Avalonia.Agents.Assets.Controls;

// *** controls

// ** control: chat_bubble
/// <summary>
/// A templated control that renders a single chat message bubble.
/// Supports role-based alignment and styling: human messages are right-aligned,
/// AI messages are left-aligned, and tool messages use a monospaced style.
/// </summary>
public class ChatBubble : TemplatedControl
{
    // * attribute: role_property
    /// <summary>The message role (system, human, ai, tool).</summary>
    public static readonly StyledProperty<string> RoleProperty =
        AvaloniaProperty.Register<ChatBubble, string>(nameof(Role), "ai");

    // * attribute: content_property
    /// <summary>The text content of the message.</summary>
    public static readonly StyledProperty<string> ContentProperty =
        AvaloniaProperty.Register<ChatBubble, string>(nameof(Content), string.Empty);

    // * attribute: is_streaming_property
    /// <summary>Whether this message is currently being streamed.</summary>
    public static readonly StyledProperty<bool> IsStreamingProperty =
        AvaloniaProperty.Register<ChatBubble, bool>(nameof(IsStreaming), false);

    // * attribute: timestamp_property
    /// <summary>The formatted timestamp to display.</summary>
    public static readonly StyledProperty<string> TimestampProperty =
        AvaloniaProperty.Register<ChatBubble, string>(nameof(Timestamp), string.Empty);

    // * property: role
    public string Role
    {
        get => GetValue(RoleProperty);
        set => SetValue(RoleProperty, value);
    }

    // * property: content
    public string Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    // * property: is_streaming
    public bool IsStreaming
    {
        get => GetValue(IsStreamingProperty);
        set => SetValue(IsStreamingProperty, value);
    }

    // * property: timestamp
    public string Timestamp
    {
        get => GetValue(TimestampProperty);
        set => SetValue(TimestampProperty, value);
    }

    // * init
    static ChatBubble()
    {
        RoleProperty.Changed.AddClassHandler<ChatBubble>((bubble, _) =>
            bubble.UpdateRolePseudoClasses());
        IsStreamingProperty.Changed.AddClassHandler<ChatBubble>((bubble, _) =>
            bubble.SetPseudoClass(":streaming", bubble.IsStreaming));
    }

    // * method: update_role_pseudo_classes
    private void UpdateRolePseudoClasses()
    {
        SetPseudoClass(":human", Role == "human");
        SetPseudoClass(":ai", Role == "ai");
        SetPseudoClass(":tool", Role == "tool");
        SetPseudoClass(":system", Role == "system");
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
        UpdateRolePseudoClasses();
    }
}
