#if __IOS__
using System;
using System.Linq;
using System.Collections.Generic;
using CoreGraphics;
using UIKit;

using XamlColor = Windows.UI.Color;
using XamlStatusBar = Windows.UI.ViewManagement.StatusBar;

namespace Uno.Toolkit.UI
{
	public static partial class StatusBar
	{
		static partial void SetForegroundCore(XamlColor value)
		{
			XamlStatusBar.GetForCurrentView().ForegroundColor = value;
		}

		static partial void SetBackgroundCore(XamlColor value)
		{
			// random unique tag to avoid recreating the view
			const int StatusBarViewTag = 38482;

			var (windows, statusBarFrame) = GetWindowsAndStatusBarFrame();
			foreach (var window in windows)
			{
				var sbar = window.ViewWithTag(StatusBarViewTag) ?? new UIView(statusBarFrame) { Tag = StatusBarViewTag };
				sbar.BackgroundColor = value;
				sbar.TintColor = value;

				window.AddSubview(sbar);
			}
		}

#pragma warning disable CA1416 // see https://github.com/xamarin/xamarin-macios/issues/16250
		private static (UIWindow[] Windows, CGRect StatusBarFrame) GetWindowsAndStatusBarFrame()
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
			{
				IEnumerable<UIScene> scenes = UIApplication.SharedApplication.ConnectedScenes;
				var currentScene = scenes.FirstOrDefault(n => n.ActivationState == UISceneActivationState.ForegroundActive);

				if (currentScene is not UIWindowScene uiWindowScene)
					throw new InvalidOperationException("Unable to find current window scene.");

				if (uiWindowScene.StatusBarManager is not { } statusBarManager)
					throw new InvalidOperationException("Unable to find a status bar manager.");

				return (uiWindowScene.Windows, statusBarManager.StatusBarFrame);
			}
			else
			{
				return (UIApplication.SharedApplication.Windows, UIApplication.SharedApplication.StatusBarFrame);
			}
		}
#pragma warning restore CA1416
	}
}
#endif
