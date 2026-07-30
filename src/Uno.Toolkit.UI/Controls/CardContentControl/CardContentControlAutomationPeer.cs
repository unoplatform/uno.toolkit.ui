#if IS_WINUI
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
#else
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Exposes <see cref="CardContentControl"/> to Microsoft UI Automation.
	/// </summary>
	public partial class CardContentControlAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CardContentControlAutomationPeer"/> class.
		/// </summary>
		/// <param name="owner">The <see cref="CardContentControl"/> instance to create the peer for.</param>
		public CardContentControlAutomationPeer(CardContentControl owner) : base(owner)
		{
		}

		protected override string GetClassNameCore() => nameof(CardContentControl);

		protected override AutomationControlType GetAutomationControlTypeCore() =>
			Owner is CardContentControl { IsClickable: true }
				? AutomationControlType.Button
				: AutomationControlType.Group;

		protected override string GetNameCore()
		{
			var name = base.GetNameCore();
			if (Owner is not CardContentControl { IsClickable: true } card ||
				!string.IsNullOrEmpty(AutomationProperties.GetName(card)) ||
				AutomationProperties.GetLabeledBy(card) is not null)
			{
				return name;
			}

			var contentName = CardAutomationPeerHelper.GetContentName(card.GetContentTemplateRoot());
			return string.IsNullOrEmpty(contentName) ? name : contentName;
		}

		protected override object? GetPatternCore(PatternInterface patternInterface) =>
			patternInterface == PatternInterface.Invoke && Owner is CardContentControl { IsClickable: true }
				? this
				: base.GetPatternCore(patternInterface);

		/// <inheritdoc />
		public void Invoke()
		{
			if (Owner is CardContentControl { IsClickable: true, IsEnabled: true } card)
			{
				card.InvokeClick();
			}
		}
	}
}
