using System.Reflection;

namespace Uno.Toolkit.Samples.Content
{
	public sealed partial class SettingsPage : Page
	{
		public SettingsPage()
		{
			this.InitializeComponent();
			this.Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			// Get version information
			var assembly = Assembly.GetExecutingAssembly();
			var assemblyName = assembly.GetName();
			var version = assemblyName.Version;

			// Set version text
			VersionTextBlock.Text = version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "Unknown";

			// Set framework information
			FrameworkTextBlock.Text = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;

			// Set platform information
#if __ANDROID__
			PlatformTextBlock.Text = "Android";
#elif __IOS__
			PlatformTextBlock.Text = "iOS";
#elif __WASM__
			PlatformTextBlock.Text = "WebAssembly";
#elif __MACOS__
			PlatformTextBlock.Text = "macOS";
#elif __SKIA__
			PlatformTextBlock.Text = "Skia (Desktop)";
#elif WINDOWS_UWP
			PlatformTextBlock.Text = "UWP";
#elif WINDOWS
			PlatformTextBlock.Text = "Windows (WinUI)";
#else
			PlatformTextBlock.Text = "Unknown";
#endif
		}
	}
}
