using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace Tiferet.Avalonia.Agents.Assets.Controls;

// *** controls

// ** control: markdown_block
/// <summary>
/// A templated control for rendering markdown-formatted content in chat messages.
/// Parses fenced code blocks (<c>```language ... ```</c>) and renders them with
/// monospaced font, dark background, and a copy-to-clipboard button.
/// Regular text is rendered as selectable wrapped text.
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

    // * attribute: content_panel
    private StackPanel? _contentPanel;

    // * attribute: code_fence_regex
    private static readonly Regex CodeFenceRegex = new(
        @"```(\w*)[ \t]*\r?\n(.*?)```",
        RegexOptions.Singleline | RegexOptions.Compiled);

    // * init
    static MarkdownBlock()
    {
        TextProperty.Changed.AddClassHandler<MarkdownBlock>(
            (block, _) => block.RebuildContent());
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _contentPanel = e.NameScope.Find<StackPanel>("PART_ContentPanel");
        RebuildContent();
    }

    // * method: rebuild_content
    /// <summary>
    /// Parse the Text property into segments and rebuild the visual tree.
    /// </summary>
    private void RebuildContent()
    {
        if (_contentPanel is null) return;
        _contentPanel.Children.Clear();

        var text = Text ?? string.Empty;
        if (string.IsNullOrEmpty(text)) return;

        var segments = ParseSegments(text);
        foreach (var segment in segments)
        {
            if (segment.IsCode)
                _contentPanel.Children.Add(CreateCodeBlock(segment.Content, segment.Language));
            else if (!string.IsNullOrWhiteSpace(segment.Content))
                _contentPanel.Children.Add(CreateTextBlock(segment.Content.Trim()));
        }
    }

    // * method: parse_segments
    /// <summary>
    /// Split text into alternating text and code block segments.
    /// </summary>
    /// <param name="text">The raw markdown text.</param>
    /// <returns>A list of content segments.</returns>
    internal static List<ContentSegment> ParseSegments(string text)
    {
        var segments = new List<ContentSegment>();
        var lastIndex = 0;

        foreach (Match match in CodeFenceRegex.Matches(text))
        {
            // Text before this code block.
            if (match.Index > lastIndex)
            {
                segments.Add(new ContentSegment(
                    Content: text[lastIndex..match.Index],
                    Language: string.Empty,
                    IsCode: false));
            }

            // The code block itself.
            segments.Add(new ContentSegment(
                Content: match.Groups[2].Value.TrimEnd(),
                Language: match.Groups[1].Value,
                IsCode: true));

            lastIndex = match.Index + match.Length;
        }

        // Remaining text after last code block.
        if (lastIndex < text.Length)
        {
            segments.Add(new ContentSegment(
                Content: text[lastIndex..],
                Language: string.Empty,
                IsCode: false));
        }

        return segments;
    }

    // * method: create_text_block
    private static SelectableTextBlock CreateTextBlock(string content) => new()
    {
        Text = content,
        TextWrapping = TextWrapping.Wrap,
        FontSize = 14,
        Foreground = new SolidColorBrush(Color.Parse("#212121")),
    };

    // * method: create_code_block
    private Border CreateCodeBlock(string code, string language)
    {
        // Language label and copy button header.
        var header = new DockPanel
        {
            Margin = new Thickness(0, 0, 0, 4),
        };

        if (!string.IsNullOrEmpty(language))
        {
            var langLabel = new TextBlock
            {
                Text = language,
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.Parse("#9E9E9E")),
                VerticalAlignment = VerticalAlignment.Center,
            };
            DockPanel.SetDock(langLabel, Dock.Left);
            header.Children.Add(langLabel);
        }

        var copyButton = new Button
        {
            Content = "Copy",
            FontSize = 11,
            Padding = new Thickness(8, 2),
            HorizontalAlignment = HorizontalAlignment.Right,
        };
        copyButton.Click += async (_, _) =>
        {
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard is not null)
                await clipboard.SetTextAsync(code);
        };
        DockPanel.SetDock(copyButton, Dock.Right);
        header.Children.Add(copyButton);

        // Code content.
        var codeText = new SelectableTextBlock
        {
            Text = code,
            TextWrapping = TextWrapping.Wrap,
            FontFamily = new FontFamily("Cascadia Mono, Consolas, monospace"),
            FontSize = 13,
            Foreground = new SolidColorBrush(Color.Parse("#E0E0E0")),
        };

        return new Border
        {
            Background = new SolidColorBrush(Color.Parse("#1E1E1E")),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(12, 8),
            Margin = new Thickness(0, 4),
            Child = new StackPanel
            {
                Spacing = 0,
                Children = { header, codeText },
            },
        };
    }

    // * record: content_segment
    /// <summary>
    /// Represents a parsed segment of markdown content — either plain text or a code block.
    /// </summary>
    internal record ContentSegment(
        string Content,
        string Language,
        bool IsCode);
}
