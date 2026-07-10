using System;
using System.Runtime.CompilerServices;
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

		// Maps a XamlRoot to the app's own root element, for apps hosted under a XamlRoot they
		// don't own. Held weakly on both sides so an unloaded app is not kept alive.
		private static readonly ConditionalWeakTable<XamlRoot, WeakReference<FrameworkElement>> _appRootOverrides = new();

		/// <summary>
		/// Registers the element to treat as the app's root for the given XamlRoot, in place of
		/// <see cref="XamlRoot.Content"/>. Pass null to clear the override.
		/// </summary>
		internal static void SetAppRootOverride(XamlRoot root, FrameworkElement? appRoot)
		{
			if (appRoot is null)
			{
				_appRootOverrides.Remove(root);
			}
			else
			{
				_appRootOverrides.AddOrUpdate(root, new WeakReference<FrameworkElement>(appRoot));
			}
		}

		internal static FrameworkElement? ResolveThemeTarget(XamlRoot? root)
		{
			if (root is null)
			{
				return null;
			}

			if (_appRootOverrides.TryGetValue(root, out var weakAppRoot) &&
				weakAppRoot.TryGetTarget(out var appRoot) &&
				appRoot.XamlRoot == root)
			{
				return appRoot;
			}

			return root.Content as FrameworkElement;
		}

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
		[Obsolete("GetApplicationTheme is obsolete. Use GetRootTheme(XamlRoot root) instead.")]
		public static ApplicationTheme GetApplicationTheme()
			=> GetRootTheme(GetWindowRoot()?.XamlRoot);

		/// <summary>
		/// Gets a <see cref="ApplicationTheme"/> from the provided XamlRoot.
		/// </summary>
		public static ApplicationTheme GetRootTheme(XamlRoot? root)
		{
			return ResolveThemeTarget(root)?.ActualTheme switch
			{
				ElementTheme.Light => ApplicationTheme.Light,
				ElementTheme.Dark => ApplicationTheme.Dark,

				_ => GetCurrentOsTheme(),
			};
		}

		/// <summary>
		/// Get if the application is currently in dark mode.
		/// </summary>
		[Obsolete("IsAppInDarkMode is obsolete. Use IsRootInDarkMode(XamlRoot root) instead.")]
		public static bool IsAppInDarkMode()
			=> GetRootTheme(GetWindowRoot().XamlRoot) == ApplicationTheme.Dark;

		public static bool IsRootInDarkMode(XamlRoot root)
			=> GetRootTheme(root) == ApplicationTheme.Dark;

		[Obsolete("SetApplicationTheme(bool darkMode) is obsolete. Use SetApplicationTheme(XamlRoot? root, ElementTheme theme) instead.")]
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
			if (ResolveThemeTarget(root) is { } target)
			{
				target.RequestedTheme = theme;
			}
			else
			{
				Logger.WarnIfEnabled(() => "No root element to apply the theme to (XamlRoot is null, or its content is not a FrameworkElement).");
			}
		}

		[Obsolete("ToggleApplicationTheme() is obsolete. Use SetApplicationTheme(XamlRoot? root, ElementTheme theme) instead.")]
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
