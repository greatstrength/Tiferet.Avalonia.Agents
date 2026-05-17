using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Tiferet.Avalonia.Agents.Assets.Controls;

// *** controls

// ** control: streaming_text_block
/// <summary>
/// A control that renders text incrementally as tokens are appended.
/// Used inside AI chat bubbles during streaming to display tokens
/// as they arrive from the LLM.
/// </summary>
public class StreamingTextBlock : TemplatedControl
{
    // * attribute: text_property
    /// <summary>The accumulated text content.</summary>
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<StreamingTextBlock, string>(nameof(Text), string.Empty);

    // * attribute: is_streaming_property
    /// <summary>Whether the control is currently receiving tokens.</summary>
    public static readonly StyledProperty<bool> IsStreamingProperty =
        AvaloniaProperty.Register<StreamingTextBlock, bool>(nameof(IsStreaming), false);

    // * property: text
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    // * property: is_streaming
    public bool IsStreaming
    {
        get => GetValue(IsStreamingProperty);
        set => SetValue(IsStreamingProperty, value);
    }

    // * attribute: inner_text_block
    private TextBlock? _innerTextBlock;

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _innerTextBlock = e.NameScope.Find<TextBlock>("PART_TextBlock");
    }

    // * method: append_text
    /// <summary>
    /// Append a token chunk to the displayed text.
    /// Must be called on the UI thread.
    /// </summary>
    /// <param name="token">The text chunk to append.</param>
    public void AppendText(string token)
    {
        Text += token;
    }

    // * method: clear
    /// <summary>
    /// Clear all accumulated text.
    /// </summary>
    public void Clear()
    {
        Text = string.Empty;
    }
}
