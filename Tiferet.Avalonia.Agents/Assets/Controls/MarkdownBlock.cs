using Avalonia;
using Avalonia.Controls.Primitives;

namespace Tiferet.Avalonia.Agents.Assets.Controls;

// *** controls

// ** control: markdown_block
/// <summary>
/// A templated control for rendering markdown-formatted content in chat messages.
/// For this alpha, renders plain text with wrapping. Structured so a markdown
/// parsing library (e.g., Markdig) can be integrated in a future release.
/// </summary>
public class MarkdownBlock : TemplatedControl
{
    // * attribute: text_property
    /// <summary>The markdown/text content to render.</summary>
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<MarkdownBlock, string>(nameof(Text), string.Empty);

    // * attribute: is_selectable_property
    /// <summary>Whether the rendered text is selectable by the user.</summary>
    public static readonly StyledProperty<bool> IsSelectableProperty =
        AvaloniaProperty.Register<MarkdownBlock, bool>(nameof(IsSelectable), true);

    // * property: text
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    // * property: is_selectable
    public bool IsSelectable
    {
        get => GetValue(IsSelectableProperty);
        set => SetValue(IsSelectableProperty, value);
    }
}
