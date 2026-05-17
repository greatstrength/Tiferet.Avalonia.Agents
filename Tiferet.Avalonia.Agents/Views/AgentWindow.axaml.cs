using Avalonia.Controls;

namespace Tiferet.Avalonia.Agents.Views;

// *** views

// ** view: agent_window
/// <summary>
/// Top-level shell window for the agent chat interface.
/// Contains a conversation sidebar (placeholder) and the main ChatView panel.
/// </summary>
public partial class AgentWindow : Window
{
    public AgentWindow()
    {
        InitializeComponent();
    }
}
