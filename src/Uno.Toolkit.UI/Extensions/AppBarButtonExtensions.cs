#if HAS_UNO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Automation.Peers;

#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

namespace Uno.Toolkit.UI
{
	internal static class AppBarButtonExtensions
	{
		public static void RaiseClick(this AppBarButton? button)
		{
			var peer = button?.GetAutomationPeer() as ButtonAutomationPeer;
			peer?.Invoke();
		}

		public static bool TryGetIconColor(this AppBarButton appBarButton, out Color iconColor)
		{
			iconColor = default;

			if (appBarButton.Icon?.ReadLocalValue(IconElement.ForegroundProperty) != DependencyProperty.UnsetValue &&
				ColorHelper.TryGetColorWithOpacity(appBarButton.Icon?.Foreground, out var iconForeground))
			{
				iconColor = iconForeground;
				return true;
			}

			return false;
		}
	}
}
#endif
