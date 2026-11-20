using System.Collections.Generic;

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
	/// <summary>
	/// Exposes <see cref="DrawerControl"/> to Microsoft UI Automation.
	/// </summary>
	public partial class DrawerControlAutomationPeer : FrameworkElementAutomationPeer
	{
		private FrameworkElement? _panePeerOwner;
		private DrawerPaneAutomationPeer? _panePeer;
		private FrameworkElement? _lightDismissPeerOwner;
		private DrawerLightDismissAutomationPeer? _lightDismissPeer;

		/// <summary>
		/// Initializes a new instance of the <see cref="DrawerControlAutomationPeer"/> class.
		/// </summary>
		/// <param name="owner">The <see cref="DrawerControl"/> instance to create the peer for.</param>
		public DrawerControlAutomationPeer(DrawerControl owner) : base(owner)
		{
		}

		protected override string GetClassNameCore() => nameof(DrawerControl);

		protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Group;

		protected override IList<AutomationPeer> GetChildrenCore()
		{
			var children = new List<AutomationPeer>();
			if (Owner is not DrawerControl drawer)
			{
				return children;
			}

			var pane = drawer.DrawerContentPart;
			var lightDismiss = drawer.LightDismissPart;

			if (base.GetChildrenCore() is { } baseChildren)
			{
				foreach (var child in baseChildren)
				{
					if (child is FrameworkElementAutomationPeer frameworkElementPeer &&
						(IsPartOrDescendant(frameworkElementPeer.Owner, pane) ||
							IsPartOrDescendant(frameworkElementPeer.Owner, lightDismiss)))
					{
						continue;
					}

					children.Add(child);
				}
			}

			if (drawer.IsOpen && pane is not null)
			{
				children.Add(GetPanePeer(pane, drawer));
			}

			if (drawer.IsOpen && drawer.IsLightDismissEnabled && lightDismiss is not null)
			{
				children.Add(GetLightDismissPeer(lightDismiss, drawer));
			}

			return children;
		}

		private AutomationPeer GetPanePeer(FrameworkElement pane, DrawerControl drawer)
		{
			if (FrameworkElementAutomationPeer.CreatePeerForElement(pane) is DrawerPaneAutomationPeer peer)
			{
				return peer;
			}

			if (!ReferenceEquals(_panePeerOwner, pane) || _panePeer is null)
			{
				_panePeerOwner = pane;
				_panePeer = new DrawerPaneAutomationPeer(pane, drawer);
			}

			return _panePeer;
		}

		private AutomationPeer GetLightDismissPeer(FrameworkElement lightDismiss, DrawerControl drawer)
		{
			// Border is sealed on WinUI, so expose its peer as a logical child of the drawer.
			if (!ReferenceEquals(_lightDismissPeerOwner, lightDismiss) || _lightDismissPeer is null)
			{
				_lightDismissPeerOwner = lightDismiss;
				_lightDismissPeer = new DrawerLightDismissAutomationPeer(lightDismiss, drawer);
			}

			return _lightDismissPeer;
		}

		private static bool IsPartOrDescendant(DependencyObject element, DependencyObject? part)
		{
			if (part is null)
			{
				return false;
			}

			for (var current = element; current is not null; current = VisualTreeHelper.GetParent(current))
			{
				if (ReferenceEquals(current, part))
				{
					return true;
				}
			}

			return false;
		}
	}
}
