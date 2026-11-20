#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI
{
	internal static class CardAutomationPeerHelper
	{
		public static string GetContentName(UIElement? element)
		{
			if (element is null)
			{
				return string.Empty;
			}

			if (FrameworkElementAutomationPeer.CreatePeerForElement(element) is { } peer)
			{
				var name = peer.GetName();
				if (!string.IsNullOrEmpty(name))
				{
					return name;
				}
			}

			for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
			{
				if (VisualTreeHelper.GetChild(element, i) is UIElement child)
				{
					var name = GetContentName(child);
					if (!string.IsNullOrEmpty(name))
					{
						return name;
					}
				}
			}

			return string.Empty;
		}
	}
}
