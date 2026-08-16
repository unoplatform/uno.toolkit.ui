using System;

#if IS_WINUI
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
#else
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Exposes <see cref="TabBar"/> to Microsoft UI Automation.
	/// </summary>
	/// <remarks>
	/// <see cref="FrameworkElementAutomationPeer"/> is intentional: a <see cref="TabBarItem"/>
	/// can be nested inside a generated <c>ContentPresenter</c>,
	/// and the dedicated item peer must remain the element exposed in the automation tree.
	/// </remarks>
	public partial class TabBarAutomationPeer : FrameworkElementAutomationPeer, ISelectionProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TabBarAutomationPeer"/> class.
		/// </summary>
		/// <param name="owner">The <see cref="TabBar"/> instance to create the peer for.</param>
		public TabBarAutomationPeer(TabBar owner) : base(owner)
		{
		}

		protected override string GetClassNameCore() => nameof(TabBar);

		protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Tab;

		protected override object? GetPatternCore(PatternInterface patternInterface) =>
			patternInterface == PatternInterface.Selection ? this : base.GetPatternCore(patternInterface);

		/// <inheritdoc />
		public bool CanSelectMultiple => false;

		/// <inheritdoc />
		public bool IsSelectionRequired => false;

		/// <inheritdoc />
		public IRawElementProviderSimple[] GetSelection()
		{
			if (Owner is not TabBar tabBar ||
				tabBar.GetSelectedTabBarItem() is not { } selected)
			{
				return Array.Empty<IRawElementProviderSimple>();
			}

			if (FrameworkElementAutomationPeer.CreatePeerForElement(selected) is not { } peer)
			{
				return Array.Empty<IRawElementProviderSimple>();
			}

			var provider = ProviderFromPeer(peer);
			return provider is null
				? Array.Empty<IRawElementProviderSimple>()
				: new[] { provider };
		}
	}
}
