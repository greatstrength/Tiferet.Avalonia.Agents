using Avalonia.Controls;
using Avalonia.Interactivity;
using Tiferet.Avalonia.Agents.Contexts;

namespace Tiferet.Avalonia.Agents.Assets.Views;

// *** views

// ** view: chat_view
/// <summary>
/// Code-behind for the ChatView user control.
/// Wires export buttons to clipboard copy via ChatViewModel.
/// </summary>
public partial class ChatView : UserControl
{
    public ChatView()
    {
        InitializeComponent();

        // Wire export button click handlers.
        var copyMd = this.FindControl<Button>("CopyMarkdownButton");
        var copyJson = this.FindControl<Button>("CopyJsonButton");

        if (copyMd is not null)
            copyMd.Click += OnCopyMarkdownClick;
        if (copyJson is not null)
            copyJson.Click += OnCopyJsonClick;
    }

    // * method: on_copy_markdown_click
    /// <summary>
    /// Copy the conversation as Markdown to the clipboard.
    /// </summary>
    private async void OnCopyMarkdownClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not ChatViewModel vm) return;
        var markdown = vm.GenerateMarkdownExport();
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is not null)
            await clipboard.SetTextAsync(markdown);
    }

    // * method: on_copy_json_click
    /// <summary>
    /// Copy the conversation as JSON to the clipboard.
    /// </summary>
    private async void OnCopyJsonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not ChatViewModel vm) return;
        var json = vm.GenerateJsonExport();
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is not null)
            await clipboard.SetTextAsync(json);
    }
}
