using System;
using Windows.UI;
using Windows.UI.ViewManagement;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;

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
		private static readonly ILogger Logger = typeof(SystemThemeHelper).Log();

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
		[Obsolete("GetApplicationTheme is obsolete. Use GetRootTheme(Window window) or GetRootTheme(FrameworkElement root) instead.")]
		public static ApplicationTheme GetApplicationTheme()
			=> GetRootTheme(GetWindowRoot()?.XamlRoot);

		/// <summary>
		/// Gets a <see cref="ApplicationTheme"/> from the provided XamlRoot.
		/// </summary>
		/// <remarks>Targets <see cref="XamlRoot.Content"/>, which is the host's root under a foreign XamlRoot (e.g. Hot Design); prefer <see cref="GetRootTheme(FrameworkElement?)"/> there.</remarks>
		public static ApplicationTheme GetRootTheme(XamlRoot? root)
			=> GetRootTheme(root?.Content as FrameworkElement);

		/// <summary>
		/// Gets a <see cref="ApplicationTheme"/> from the provided root element.
		/// </summary>
		/// <param name="root">The app's root element, typically the window's content.</param>
		public static ApplicationTheme GetRootTheme(FrameworkElement? root)
		{
			return root?.ActualTheme switch
			{
				ElementTheme.Light => ApplicationTheme.Light,
				ElementTheme.Dark => ApplicationTheme.Dark,

				_ => GetCurrentOsTheme(),
			};
		}

		/// <summary>
		/// Gets a <see cref="ApplicationTheme"/> from the content of the provided window.
		/// </summary>
		/// <param name="window">The app's window.</param>
		public static ApplicationTheme GetRootTheme(XamlWindow? window)
			=> GetRootTheme(window?.Content as FrameworkElement);

		/// <summary>
		/// Get if the application is currently in dark mode.
		/// </summary>
		[Obsolete("IsAppInDarkMode is obsolete. Use IsRootInDarkMode(Window window) or IsRootInDarkMode(FrameworkElement root) instead.")]
		public static bool IsAppInDarkMode()
			=> GetRootTheme(GetWindowRoot().XamlRoot) == ApplicationTheme.Dark;

		/// <summary>
		/// Gets whether the content of the provided XamlRoot is currently in dark mode.
		/// </summary>
		/// <remarks>Targets <see cref="XamlRoot.Content"/>, which is the host's root under a foreign XamlRoot (e.g. Hot Design); prefer <see cref="IsRootInDarkMode(FrameworkElement)"/> there.</remarks>
		public static bool IsRootInDarkMode(XamlRoot root)
			=> GetRootTheme(root) == ApplicationTheme.Dark;

		/// <summary>
		/// Gets whether the provided root element is currently in dark mode.
		/// </summary>
		/// <param name="root">The app's root element, typically the window's content.</param>
		public static bool IsRootInDarkMode(FrameworkElement root)
			=> GetRootTheme(root) == ApplicationTheme.Dark;

		/// <summary>
		/// Gets whether the content of the provided window is currently in dark mode.
		/// </summary>
		/// <param name="window">The app's window.</param>
		public static bool IsRootInDarkMode(XamlWindow window)
			=> GetRootTheme(window) == ApplicationTheme.Dark;

		[Obsolete("SetApplicationTheme(bool darkMode) is obsolete. Use SetApplicationTheme(Window? window, ElementTheme theme) or SetApplicationTheme(FrameworkElement? root, ElementTheme theme) instead.")]
		public static void SetApplicationTheme(bool darkMode)
			=> SetRootTheme(GetWindowRoot().XamlRoot, darkMode);

		/// <summary>
		/// Sets the theme for the provided XamlRoot
		/// </summary>
		/// <param name="root">The XamlRoot whose content to theme.</param>
		/// <param name="darkMode">Whether to apply the dark theme.</param>
		/// <remarks>Targets <see cref="XamlRoot.Content"/>, which is the host's root under a foreign XamlRoot (e.g. Hot Design); prefer <see cref="SetRootTheme(FrameworkElement?, bool)"/> there.</remarks>
		public static void SetRootTheme(XamlRoot? root, bool darkMode)
			=> SetRootTheme(root?.Content as FrameworkElement, darkMode);

		/// <summary>
		/// Sets the theme for the provided root element and its subtree.
		/// </summary>
		/// <param name="root">The app's root element, typically the window's content.</param>
		/// <param name="darkMode">Whether to apply the dark theme.</param>
		public static void SetRootTheme(FrameworkElement? root, bool darkMode)
			=> SetApplicationTheme(root, darkMode ? ElementTheme.Dark : ElementTheme.Light);

		/// <summary>
		/// Sets the theme for the content of the provided window.
		/// </summary>
		/// <param name="window">The app's window.</param>
		/// <param name="darkMode">Whether to apply the dark theme.</param>
		public static void SetRootTheme(XamlWindow? window, bool darkMode)
			=> SetRootTheme(window?.Content as FrameworkElement, darkMode);

		/// <summary>
		/// Sets the theme for the content of the provided XamlRoot.
		/// </summary>
		/// <remarks>Targets <see cref="XamlRoot.Content"/>, which is the host's root under a foreign XamlRoot (e.g. Hot Design); prefer <see cref="SetApplicationTheme(FrameworkElement?, ElementTheme)"/> there.</remarks>
		public static void SetApplicationTheme(XamlRoot? root, ElementTheme theme)
			=> SetApplicationTheme(root?.Content as FrameworkElement, theme);

		/// <summary>
		/// Sets the theme for the provided root element and its subtree.
		/// </summary>
		/// <param name="root">The app's root element, typically the window's content.</param>
		/// <param name="theme">The theme to apply.</param>
		public static void SetApplicationTheme(FrameworkElement? root, ElementTheme theme)
		{
			if (root is not null)
			{
				root.RequestedTheme = theme;
			}
			else
			{
				Logger.WarnIfEnabled(() => "No root element to apply the theme to (window/XamlRoot content not set, or not a FrameworkElement).");
			}
		}

		/// <summary>
		/// Sets the theme for the content of the provided window.
		/// </summary>
		/// <param name="window">The app's window.</param>
		/// <param name="theme">The theme to apply.</param>
		public static void SetApplicationTheme(XamlWindow? window, ElementTheme theme)
			=> SetApplicationTheme(window?.Content as FrameworkElement, theme);

		[Obsolete("ToggleApplicationTheme() is obsolete. Use IsRootInDarkMode + SetRootTheme with the app's Window or root element instead.")]
		public static void ToggleApplicationTheme()
			=> SetApplicationTheme(darkMode: !IsAppInDarkMode());

		private static FrameworkElement GetWindowRoot() =>
#if IS_WINUI
			throw new NotSupportedException($"This method is not supported with WinUI, use the overloads that take the app's Window or root FrameworkElement (or a XamlRoot).");
#else
			XamlWindow.Current?.Content as FrameworkElement ??
			throw new InvalidOperationException($"The current window content is not {(XamlWindow.Current?.Content == null ? "set" : "a FrameworkElement")}.");
#endif
	}
}
