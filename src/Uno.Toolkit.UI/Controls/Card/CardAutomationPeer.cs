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
	/// Exposes <see cref="Card"/> to Microsoft UI Automation.
	/// </summary>
	public partial class CardAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CardAutomationPeer"/> class.
		/// </summary>
		/// <param name="owner">The <see cref="Card"/> instance to create the peer for.</param>
		public CardAutomationPeer(Card owner) : base(owner)
		{
		}

		protected override string GetClassNameCore() => nameof(Card);

		protected override AutomationControlType GetAutomationControlTypeCore() =>
			Owner is Card { IsClickable: true }
				? AutomationControlType.Button
				: AutomationControlType.Group;

		protected override string GetNameCore()
		{
			var name = base.GetNameCore();
			if (Owner is not Card { IsClickable: true } card ||
				!string.IsNullOrEmpty(AutomationProperties.GetName(card)) ||
				AutomationProperties.GetLabeledBy(card) is not null)
			{
				return name;
			}

			var contentName = CardAutomationPeerHelper.GetContentName(card.GetHeaderContentPresenter());
			if (string.IsNullOrEmpty(contentName))
			{
				contentName = CardAutomationPeerHelper.GetContentName(card.GetSubHeaderContentPresenter());
			}

			if (string.IsNullOrEmpty(contentName))
			{
				contentName = CardAutomationPeerHelper.GetContentName(card.GetSupportingContentPresenter());
			}

			if (string.IsNullOrEmpty(contentName))
			{
				contentName = CardAutomationPeerHelper.GetContentName(card.GetTemplateVisualRoot());
			}

			return string.IsNullOrEmpty(contentName) ? name : contentName;
		}

		protected override object? GetPatternCore(PatternInterface patternInterface) =>
			patternInterface == PatternInterface.Invoke && Owner is Card { IsClickable: true }
				? this
				: base.GetPatternCore(patternInterface);

		/// <inheritdoc />
		public void Invoke()
		{
			if (Owner is Card { IsClickable: true, IsEnabled: true } card)
			{
				card.InvokeClick();
			}
		}
	}
}
