#if __IOS__
using System;
using System.Collections.Generic;
using System.Text;
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

#pragma warning disable CA1416 // This call site is reachable on: 'iOS' 15.4 and later, 'maccatalyst' 15.4 and later. 'UIApplication.Windows.get' is unsupported on: 'ios' 15.0.0 and later, 'maccatalyst' 15.0.0 and later.
			foreach (var window in UIApplication.SharedApplication.Windows)
#pragma warning restore CA1416
			{
#pragma warning disable CA1416 // This call site is reachable on: 'iOS' 15.4 and later, 'maccatalyst' 15.4 and later. 'UIApplication.StatusBarFrame.get' is unsupported on: 'ios' 13.0.0 and later, 'maccatalyst' 13.0.0 and later.
				var sbar = window.ViewWithTag(StatusBarViewTag) ?? new UIView(UIApplication.SharedApplication.StatusBarFrame) { Tag = StatusBarViewTag };
#pragma warning restore CA1416
				sbar.BackgroundColor = value;
				sbar.TintColor = value;

				window.AddSubview(sbar);
			}
		}
	}
}
#endif
