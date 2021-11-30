using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;

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
		public static ApplicationTheme GetApplicationTheme()
		{
			return GetWindowRoot().ActualTheme switch
			{
				ElementTheme.Light => ApplicationTheme.Light,
				ElementTheme.Dark => ApplicationTheme.Dark,

				_ => GetCurrentOsTheme(),
			};
		}
		

		/// <summary>
		/// Get if the application is currently in dark mode.
		/// </summary>
		public static bool IsAppInDarkMode() => GetApplicationTheme() == ApplicationTheme.Dark;
		
		public static void SetApplicationTheme(bool darkMode) => SetApplicationTheme(darkMode ? ElementTheme.Dark : ElementTheme.Light);

		public static void SetApplicationTheme(ElementTheme theme)
		{
			GetWindowRoot().RequestedTheme = theme;
		}

		public static void ToggleApplicationTheme() => SetApplicationTheme(darkMode: !IsAppInDarkMode());

		private static FrameworkElement GetWindowRoot() =>
			XamlWindow.Current.Content as FrameworkElement ??
			throw new InvalidOperationException($"The current window content is not {(XamlWindow.Current.Content == null ? "set" : "a FrameworkElement")}.");
	}
}
