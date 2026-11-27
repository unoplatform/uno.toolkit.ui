using System;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Exposes <see cref="Chip"/> to Microsoft UI Automation.
	/// </summary>
	public partial class ChipAutomationPeer : ButtonBaseAutomationPeer, IToggleProvider, ISelectionItemProvider, IInvokeProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ChipAutomationPeer"/> class.
		/// </summary>
		/// <param name="owner">The <see cref="Chip"/> instance to create the peer for.</param>
		public ChipAutomationPeer(Chip owner) : base(owner)
		{
		}

		protected override string GetClassNameCore() => nameof(Chip);

		protected override AutomationControlType GetAutomationControlTypeCore() =>
			GetSelectionGroup() is null ? AutomationControlType.Button : AutomationControlType.ListItem;

		protected override string GetNameCore()
		{
			var name = base.GetNameCore();
			if (Owner is not Chip chip ||
				!string.IsNullOrEmpty(AutomationProperties.GetName(chip)) ||
				AutomationProperties.GetLabeledBy(chip) is not null ||
				chip.GetContentTemplateRoot() is not { } contentTemplateRoot)
			{
				return name;
			}

			var contentName = GetContentName(contentTemplateRoot);
			return string.IsNullOrEmpty(contentName) ? name : contentName;
		}

		protected override object? GetPatternCore(PatternInterface patternInterface)
		{
			if (Owner is not Chip chip)
			{
				return base.GetPatternCore(patternInterface);
			}

			if (chip.OwningChipGroup is { } chipGroup)
			{
				if (patternInterface == PatternInterface.SelectionItem &&
					chipGroup.SelectionMode != ChipSelectionMode.None)
				{
					return this;
				}

				if (patternInterface == PatternInterface.Invoke &&
					chipGroup.SelectionMode == ChipSelectionMode.None)
				{
					return this;
				}

				return base.GetPatternCore(patternInterface);
			}

			return patternInterface == PatternInterface.Toggle
				? this
				: base.GetPatternCore(patternInterface);
		}

		/// <inheritdoc />
		public ToggleState ToggleState => (Owner as Chip)?.IsChecked switch
		{
			true => ToggleState.On,
			false => ToggleState.Off,
			_ => ToggleState.Indeterminate,
		};

		/// <inheritdoc />
		public void Toggle()
		{
			if (Owner is Chip { IsEnabled: true, OwningChipGroup: null } chip)
			{
				new ToggleButtonAutomationPeer(chip).Toggle();
			}
		}

		/// <inheritdoc />
		public void Invoke()
		{
			if (Owner is Chip
				{
					IsEnabled: true,
					OwningChipGroup: { SelectionMode: ChipSelectionMode.None },
				} chip)
			{
				new ToggleButtonAutomationPeer(chip).Toggle();
			}
		}

		/// <inheritdoc />
		public bool IsSelected => GetSelectionGroup() is not null && (Owner as Chip)?.IsChecked == true;

		/// <inheritdoc />
		public IRawElementProviderSimple? SelectionContainer
		{
			get
			{
				if (GetSelectionGroup() is not { } chipGroup ||
					FrameworkElementAutomationPeer.CreatePeerForElement(chipGroup) is not { } peer)
				{
					return null;
				}

				return ProviderFromPeer(peer);
			}
		}

		/// <inheritdoc />
		public void Select()
		{
			var (chip, chipGroup) = GetSelectionContext();
			if (!chip.IsEnabled)
			{
				return;
			}

			if (chipGroup.SelectionMode == ChipSelectionMode.Multiple)
			{
				foreach (var container in chipGroup.GetItemContainers<Chip>())
				{
					if (!ReferenceEquals(container, chip) && container.IsChecked == true)
					{
						container.IsChecked = false;
					}
				}
			}

			chip.IsChecked = true;
		}

		/// <inheritdoc />
		public void AddToSelection()
		{
			var (chip, chipGroup) = GetSelectionContext();
			if (!chip.IsEnabled || chip.IsChecked == true)
			{
				return;
			}

			if (chipGroup.SelectionMode != ChipSelectionMode.Multiple)
			{
				foreach (var container in chipGroup.GetItemContainers<Chip>())
				{
					if (container.IsChecked == true)
					{
						throw new InvalidOperationException("ChipGroup does not support multiple selection.");
					}
				}
			}

			chip.IsChecked = true;
		}

		/// <inheritdoc />
		public void RemoveFromSelection()
		{
			var (chip, chipGroup) = GetSelectionContext();
			if (!chip.IsEnabled || chip.IsChecked != true)
			{
				return;
			}

			if (chipGroup.SelectionMode == ChipSelectionMode.Single)
			{
				throw new InvalidOperationException("ChipGroup requires a selection.");
			}

			chip.IsChecked = false;
		}

		internal void RaiseSelectionChanged(bool oldValue, bool newValue, ChipSelectionMode selectionMode)
		{
			if (oldValue == newValue)
			{
				return;
			}

			if (AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged))
			{
				RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, oldValue, newValue);
			}

			var selectionEvent = newValue
				? selectionMode == ChipSelectionMode.Multiple
					? AutomationEvents.SelectionItemPatternOnElementAddedToSelection
					: AutomationEvents.SelectionItemPatternOnElementSelected
				: AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection;

			if (AutomationPeer.ListenerExists(selectionEvent))
			{
				RaiseAutomationEvent(selectionEvent);
			}
		}

		internal void RaiseToggleStateChanged(bool? oldValue, bool? newValue)
		{
			var oldState = ConvertToToggleState(oldValue);
			var newState = ConvertToToggleState(newValue);
			if (oldState != newState)
			{
				RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, oldState, newState);
			}
		}

		private ChipGroup? GetSelectionGroup() =>
			Owner is Chip { OwningChipGroup: { SelectionMode: not ChipSelectionMode.None } chipGroup }
				? chipGroup
				: null;

		private (Chip Chip, ChipGroup ChipGroup) GetSelectionContext()
		{
			if (Owner is Chip { OwningChipGroup: { SelectionMode: not ChipSelectionMode.None } chipGroup } chip)
			{
				return (chip, chipGroup);
			}

			throw new InvalidOperationException("The Chip is not in a selectable ChipGroup.");
		}

		private static ToggleState ConvertToToggleState(bool? value) => value switch
		{
			true => ToggleState.On,
			false => ToggleState.Off,
			_ => ToggleState.Indeterminate,
		};

		private static string GetContentName(UIElement element)
		{
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
