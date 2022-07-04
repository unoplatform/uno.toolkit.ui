#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Text;

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
			var activity = Uno.UI.ContextHelper.Current as Android.App.Activity;

			activity?.Window?.SetStatusBarColor(value);
		}
	}
}
#endif
