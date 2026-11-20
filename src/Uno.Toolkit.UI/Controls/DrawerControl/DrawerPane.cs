#if IS_WINUI
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI
{
	internal partial class DrawerPane : ContentControl
	{
		internal DrawerControl? DrawerOwner { get; set; }

		protected override AutomationPeer OnCreateAutomationPeer() => new DrawerPaneAutomationPeer(this, DrawerOwner);
	}
}
