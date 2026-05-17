using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Tiferet.Avalonia.Agents.Assets.Views;

namespace Tiferet.Avalonia.Agents.Examples.Chat;

// *** views

// ** view: app
/// <summary>
/// Application entry point for the example chat application.
/// Creates an <see cref="AgentWindow"/> with mock services for demonstration.
/// </summary>
public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new AgentWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
