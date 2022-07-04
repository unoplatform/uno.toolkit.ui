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

			foreach (var window in UIApplication.SharedApplication.Windows)
			{
				var sbar = window.ViewWithTag(StatusBarViewTag) ?? new UIView(UIApplication.SharedApplication.StatusBarFrame) { Tag = StatusBarViewTag };
				sbar.BackgroundColor = value;
				sbar.TintColor = value;

				window.AddSubview(sbar);
			}
		}
	}
}
#endif
