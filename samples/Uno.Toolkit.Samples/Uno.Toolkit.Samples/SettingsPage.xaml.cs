using System.Reflection;

namespace Uno.Toolkit.Samples;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Get the version from the assembly
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        
        // Try to get the informational version which includes git version info
        var infoVersionAttr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        var displayVersion = infoVersionAttr?.InformationalVersion ?? version?.ToString() ?? "Unknown";
        
        VersionText.Text = displayVersion;
    }
}
