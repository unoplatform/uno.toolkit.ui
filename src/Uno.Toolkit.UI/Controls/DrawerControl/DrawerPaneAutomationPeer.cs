using System;
using System.Collections.Generic;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Exposes the pane of a <see cref="DrawerControl"/> to Microsoft UI Automation.
	/// </summary>
	internal partial class DrawerPaneAutomationPeer : FrameworkElementAutomationPeer, IWindowProvider
	{
		private readonly DrawerControl? _drawerOwner;

		public DrawerPaneAutomationPeer(FrameworkElement owner, DrawerControl? drawerOwner = null) : base(owner)
		{
			_drawerOwner = drawerOwner;
		}

		protected override string GetClassNameCore() => "DrawerPane";

		protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Window;

		protected override string GetNameCore()
		{
			var paneName = AutomationProperties.GetName(Owner);
			if (!string.IsNullOrEmpty(paneName))
			{
				return paneName;
			}

			if (GetDrawer() is { } drawer)
			{
				var drawerName = AutomationProperties.GetName(drawer);
				if (!string.IsNullOrEmpty(drawerName))
				{
					return drawerName;
				}
			}

			return base.GetNameCore();
		}

		protected override object? GetPatternCore(PatternInterface patternInterface) =>
			patternInterface == PatternInterface.Window && IsOpen()
				? this
				: base.GetPatternCore(patternInterface);

		protected override IList<AutomationPeer> GetChildrenCore() =>
			IsOpen() ? base.GetChildrenCore() : Array.Empty<AutomationPeer>();

		public bool IsModal => true;

		public bool IsTopmost => true;

		public bool Maximizable => false;

		public bool Minimizable => false;

		public WindowInteractionState InteractionState => WindowInteractionState.Running;

		public WindowVisualState VisualState => WindowVisualState.Normal;

		public void Close()
		{
			if (GetDrawer() is { } drawer)
			{
				drawer.Dismiss();
			}
		}

		public void SetVisualState(WindowVisualState state)
		{
		}

		public bool WaitForInputIdle(int milliseconds) => false;

		private DrawerControl? GetDrawer() =>
			_drawerOwner ??
			(Owner as DrawerPane)?.DrawerOwner ??
			DrawerControl.FindOwner(Owner);

		private bool IsOpen() => GetDrawer()?.IsOpen == true;
	}
}
