using System;
using System.Collections.Generic;

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
	/// Exposes <see cref="ChipGroup"/> to Microsoft UI Automation.
	/// </summary>
	public partial class ChipGroupAutomationPeer : FrameworkElementAutomationPeer, ISelectionProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ChipGroupAutomationPeer"/> class.
		/// </summary>
		/// <param name="owner">The <see cref="ChipGroup"/> instance to create the peer for.</param>
		public ChipGroupAutomationPeer(ChipGroup owner) : base(owner)
		{
		}

		protected override string GetClassNameCore() => nameof(ChipGroup);

		protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.List;

		protected override object? GetPatternCore(PatternInterface patternInterface) =>
			patternInterface == PatternInterface.Selection &&
			Owner is ChipGroup { SelectionMode: not ChipSelectionMode.None }
				? this
				: base.GetPatternCore(patternInterface);

		/// <inheritdoc />
		public bool CanSelectMultiple => Owner is ChipGroup { SelectionMode: ChipSelectionMode.Multiple };

		/// <inheritdoc />
		public bool IsSelectionRequired => Owner is ChipGroup { SelectionMode: ChipSelectionMode.Single };

		/// <inheritdoc />
		public IRawElementProviderSimple[] GetSelection()
		{
			if (Owner is not ChipGroup { SelectionMode: not ChipSelectionMode.None } chipGroup)
			{
				return Array.Empty<IRawElementProviderSimple>();
			}

			var selection = new List<IRawElementProviderSimple>();
			foreach (var chip in chipGroup.GetItemContainers<Chip>())
			{
				if (chip.IsChecked == true &&
					FrameworkElementAutomationPeer.CreatePeerForElement(chip) is { } peer &&
					ProviderFromPeer(peer) is { } provider)
				{
					selection.Add(provider);
				}
			}

			return selection.ToArray();
		}
	}
}
