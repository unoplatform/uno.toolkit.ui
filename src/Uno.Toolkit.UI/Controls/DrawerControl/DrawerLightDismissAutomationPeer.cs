#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Exposes the light-dismiss surface of a <see cref="DrawerControl"/> to Microsoft UI Automation.
	/// </summary>
	internal partial class DrawerLightDismissAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
	{
		private readonly DrawerControl? _drawerOwner;

		public DrawerLightDismissAutomationPeer(FrameworkElement owner, DrawerControl? drawerOwner = null) : base(owner)
		{
			_drawerOwner = drawerOwner;
		}

		protected override string GetClassNameCore() => "DrawerLightDismiss";

		protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Button;

		protected override string GetNameCore()
		{
			var name = base.GetNameCore();
			return string.IsNullOrEmpty(name) ? "Close" : name;
		}

		protected override string GetAutomationIdCore() => "LightDismiss";

		protected override object? GetPatternCore(PatternInterface patternInterface) =>
			patternInterface == PatternInterface.Invoke && IsLightDismissEnabled()
				? this
				: base.GetPatternCore(patternInterface);

		public void Invoke()
		{
			if (GetDrawer() is { } drawer)
			{
				drawer.Dismiss();
			}
		}

		private bool IsLightDismissEnabled() =>
			GetDrawer() is
			{
				IsOpen: true,
				IsLightDismissEnabled: true,
			};

		private DrawerControl? GetDrawer() =>
			_drawerOwner ??
			DrawerControl.FindOwner(Owner);
	}
}
