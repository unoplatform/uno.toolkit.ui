using System.Reflection;
using System.Runtime.InteropServices;
using Uno.Extensions;
using Uno.UI.Extensions;
using Windows.ApplicationModel.DataTransfer;

namespace Uno.Toolkit.Samples.Content
{
	public sealed partial class SettingsPage : Page
	{
		public SettingsPage()
		{
			this.InitializeComponent();

			UpdateAppInfoSafe();
		}

		private void UpdateAppInfoSafe()
		{
			// this is called in ctor, dont fail creating the page and the subsequent navigation if anything went wrong
			try
			{
				UpdateAppInfo();
			}
			catch (Exception) { }
		}
		private void UpdateAppInfo()
		{
			// Set version text
			VersionTextBlock.Text = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";

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

			// Set packages information
			var assemblyMarkerTypes = new[]
			{
				typeof(Microsoft.UI.Xaml.Application),
				typeof(Uno.Material.MaterialTheme),
				// typeof(Uno.Cupertino.CupertinoResources),
				typeof(Uno.Toolkit.UI.ToolkitResources),
			};
			PackageVersionsTextBlock.Text = string.Join("\n", assemblyMarkerTypes
				.Select(x =>
				{
					var name = x.Assembly.GetName();
					var version =
						(x.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion?.Trim() is { Length: > 0 } aiv ? aiv : null) ??
						name.Version?.ToString() ??
						"Unknown";

					return $"{name.Name}: {version}";
				})
			);
		}

		private void EnableDebugPanel(object sender, RoutedEventArgs e)
		{
			if (this.FindFirstAncestor<Shell>() is { } shell)
			{
				shell.EnableDebugPanel();
			}
		}

		private void CopyInfo(object sender, RoutedEventArgs e)
		{
			var text = string.Join('\n', AppInfoPanel.Children.OfType<StackPanel>()
				.Select(x =>
				{
					var label = (x.Children.ElementAtOrDefault(0) as TextBlock)?.Text ?? "???";
					var value = string.Join('\n', x.Children.Skip(1)
						.Select(x => x switch
						{
							TextBlock tblock => tblock.Text,
							TextBox tbox => tbox.Text,
							_ => null,
						})
						.Where(x => !string.IsNullOrEmpty(x))
					);

					return value.Split(['\n', '\r']) is { Length: > 1 } multilines
						? string.Join('\n', [$"{label}:", .. multilines.Select(x => "   " + x)])
						: $"{label}: {value}";
				})
			);

			Clipboard.SetContent(new DataPackage().Apply(x => x.SetText(text)));
			Console.WriteLine(text);
		}
	}
}
