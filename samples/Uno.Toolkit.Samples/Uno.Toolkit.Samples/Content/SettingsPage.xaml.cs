using System.Reflection;
using System.Runtime.InteropServices;

namespace Uno.Toolkit.Samples.Content
{
	public sealed partial class SettingsPage : Page
	{
		public SettingsPage()
		{
			this.InitializeComponent();
			
			// Initialize version information once since the page is cached
			var assembly = Assembly.GetExecutingAssembly();
			var assemblyName = assembly.GetName();
			var version = assemblyName.Version;

			// Set version text
			VersionTextBlock.Text = version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "Unknown";

			// Set framework information
			FrameworkTextBlock.Text = RuntimeInformation.FrameworkDescription;

			// Set platform information
#if __ANDROID__
			PlatformTextBlock.Text = "Android";
#elif __IOS__
			PlatformTextBlock.Text = "iOS";
#elif __WASM__
			PlatformTextBlock.Text = "WebAssembly";
#elif __MACOS__
			PlatformTextBlock.Text = "macOS";
#elif WINDOWS_UWP
			PlatformTextBlock.Text = "UWP";
#elif WINDOWS || WINDOWS_WINUI
			PlatformTextBlock.Text = "Windows (WinUI)";
#else
			// For Skia desktop builds, detect at runtime
			// Check if we're running on a desktop platform (Windows, Linux, macOS) but not in browser
			var isDesktop = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || 
			                RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || 
			                RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
			
			if (isDesktop)
			{
				PlatformTextBlock.Text = "Skia (Desktop)";
			}
			else
			{
				PlatformTextBlock.Text = "Unknown";
			}
#endif
		}
	}
}
