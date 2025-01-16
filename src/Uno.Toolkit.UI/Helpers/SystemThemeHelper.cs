using System;
using Windows.UI;
using Windows.UI.ViewManagement;
using System.ComponentModel;

#if IS_WINUI
using Microsoft.UI.Xaml;
using XamlWindow = Microsoft.UI.Xaml.Window;
#else
using Windows.UI.Xaml;
using XamlWindow = Windows.UI.Xaml.Window;
#endif


namespace Uno.Toolkit.UI
{
	public static class SystemThemeHelper
	{
		/// <summary>
		/// Get the current theme of the operating system.
		/// </summary>
		public static ApplicationTheme GetCurrentOsTheme()
		{
			var settings = new UISettings();
			var systemBackground = settings.GetColorValue(UIColorType.Background);
			var black = Color.FromArgb(255, 0, 0, 0);

			return systemBackground == black ? ApplicationTheme.Dark : ApplicationTheme.Light;
		}

		/// <summary>
		/// Get the current theme of the application.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ApplicationTheme GetApplicationTheme()
			=> GetRootTheme(GetWindowRoot()?.XamlRoot);

		/// <summary>
		/// Gets a <see cref="ApplicationTheme"/> from the provided XamlRoot.
		/// </summary>
		public static ApplicationTheme GetRootTheme(XamlRoot? root)
		{
			return (root?.Content as FrameworkElement)?.ActualTheme switch
			{
				ElementTheme.Light => ApplicationTheme.Light,
				ElementTheme.Dark => ApplicationTheme.Dark,

				_ => GetCurrentOsTheme(),
			};
		}

		/// <summary>
		/// Get if the application is currently in dark mode.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsAppInDarkMode()
			=> GetRootTheme(GetWindowRoot().XamlRoot) == ApplicationTheme.Dark;

		public static bool IsRootInDarkMode(XamlRoot root)
			=> GetRootTheme(root) == ApplicationTheme.Dark;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetApplicationTheme(bool darkMode)
			=> SetRootTheme(GetWindowRoot().XamlRoot, darkMode);

		/// <summary>
		/// Sets the theme for the provided XamlRoot
		/// </summary>
		/// <param name="root"></param>
		/// <param name="darkMode"></param>
		public static void SetRootTheme(XamlRoot? root, bool darkMode)
			=> SetApplicationTheme(root, darkMode ? ElementTheme.Dark : ElementTheme.Light);

		public static void SetApplicationTheme(XamlRoot? root, ElementTheme theme)
		{
			if (root?.Content is FrameworkElement fe)
			{
				fe.RequestedTheme = theme;
			}
		}

		public static void ToggleApplicationTheme()
			=> SetApplicationTheme(darkMode: !IsAppInDarkMode());

		private static FrameworkElement GetWindowRoot() =>
#if IS_WINUI
			throw new NotSupportedException($"This method is not supported with WinUI, use methods that take a XamlRoot as a parameter");
#else
			XamlWindow.Current?.Content as FrameworkElement ??
			throw new InvalidOperationException($"The current window content is not {(XamlWindow.Current?.Content == null ? "set" : "a FrameworkElement")}.");
#endif
	}
}
