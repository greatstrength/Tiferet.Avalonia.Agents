using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Tiferet.Avalonia.Agents.Assets.Controls;

// *** controls

// ** control: message_input
/// <summary>
/// A composable message input area with a multi-line text box and send button.
/// Supports Enter to send and Shift+Enter for newline.
/// </summary>
public class MessageInput : TemplatedControl
{
    // * attribute: text_property
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<MessageInput, string>(nameof(Text), string.Empty);

    // * attribute: send_command_property
    public static readonly StyledProperty<ICommand?> SendCommandProperty =
        AvaloniaProperty.Register<MessageInput, ICommand?>(nameof(SendCommand));

    // * attribute: placeholder_property
    public static readonly StyledProperty<string> PlaceholderProperty =
        AvaloniaProperty.Register<MessageInput, string>(nameof(Placeholder), "Type a message...");

    // * attribute: is_input_enabled_property
    public static readonly StyledProperty<bool> IsInputEnabledProperty =
        AvaloniaProperty.Register<MessageInput, bool>(nameof(IsInputEnabled), true);

    // * property: text
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    // * property: send_command
    public ICommand? SendCommand
    {
        get => GetValue(SendCommandProperty);
        set => SetValue(SendCommandProperty, value);
    }

    // * property: placeholder
    public string Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    // * property: is_input_enabled
    public bool IsInputEnabled
    {
        get => GetValue(IsInputEnabledProperty);
        set => SetValue(IsInputEnabledProperty, value);
    }

    // * attribute: text_box
    private TextBox? _textBox;

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _textBox = e.NameScope.Find<TextBox>("PART_TextBox");
        if (_textBox is not null)
            _textBox.KeyDown += OnTextBoxKeyDown;

        var sendButton = e.NameScope.Find<Button>("PART_SendButton");
        if (sendButton is not null)
            sendButton.Click += (_, _) => ExecuteSend();
    }

    // * method: on_text_box_key_down
    private void OnTextBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        if (e.KeyModifiers.HasFlag(KeyModifiers.Shift)) return;

        e.Handled = true;
        ExecuteSend();
    }

    // * method: execute_send
    private void ExecuteSend()
    {
        var text = Text?.Trim();
        if (string.IsNullOrEmpty(text)) return;
        if (SendCommand?.CanExecute(text) != true) return;

        SendCommand.Execute(text);
        Text = string.Empty;
    }
}
