using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using ItemsRepeater = Microsoft.UI.Xaml.Controls.ItemsRepeater;
#endif

namespace Uno.Toolkit.UI
{
	[TemplatePart(Name = RemoveButtonName, Type = typeof(Button))]
	public partial class Chip : ToggleButton
	{
		private const string ContentPresenterName = "ContentPresenter";
		private const string RemoveButtonName = "PART_RemoveButton";
		private const string RemoveButtonAutomationName = "Remove";

		private bool? _automationIsChecked;
		private Button? _removeButton;
		private bool _ownsRemoveButtonAutomationName;
		private bool _shouldRaiseIsCheckedChanged;

		internal ChipGroup? OwningChipGroup => ItemsControl.ItemsControlFromItemContainer(this) as ChipGroup;

		public Chip()
		{
			_automationIsChecked = IsChecked;
			Checked += RaiseIsCheckedChanged;
			Unchecked += RaiseIsCheckedChanged;
			Loaded += OnLoaded;

			RegisterPropertyChangedCallback(IsCheckedProperty, OnIsCheckedPropertyChanged);
			RegisterPropertyChangedCallback(ContentProperty, OnAutomationNameSourceChanged);
			RegisterPropertyChangedCallback(ContentTemplateProperty, OnAutomationNameSourceChanged);
			RegisterPropertyChangedCallback(ContentTemplateSelectorProperty, OnAutomationNameSourceChanged);
			RegisterPropertyChangedCallback(AutomationProperties.NameProperty, OnAutomationNameSourceChanged);
			RegisterPropertyChangedCallback(AutomationProperties.LabeledByProperty, OnAutomationNameSourceChanged);
		}

		/// <inheritdoc />
		protected override AutomationPeer OnCreateAutomationPeer() => new ChipAutomationPeer(this);

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (_removeButton is not null)
			{
				_removeButton.Click -= RaiseRemoveButtonClicked;
				_removeButton.Loaded -= OnRemoveButtonLoaded;
			}

			_removeButton = null;
			_ownsRemoveButtonAutomationName = false;

			if (GetTemplateChild(RemoveButtonName) is Button removeButton)
			{
				_removeButton = removeButton;
				_ownsRemoveButtonAutomationName = string.IsNullOrEmpty(AutomationProperties.GetName(removeButton));
				removeButton.Click += RaiseRemoveButtonClicked;
				removeButton.Loaded += OnRemoveButtonLoaded;
				UpdateRemoveButtonAutomationName();
			}
		}

		internal UIElement? GetContentTemplateRoot()
		{
			if (ContentTemplateRoot as UIElement is { } root)
			{
				return root;
			}

			if (GetTemplateChild(ContentPresenterName) is ContentPresenter presenter &&
				VisualTreeHelper.GetChildrenCount(presenter) > 0)
			{
				return VisualTreeHelper.GetChild(presenter, 0) as UIElement;
			}

			return null;
		}

		internal void OnChipSelectionModeChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue is ChipSelectionMode.None)
			{
				IsChecked = false;
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs e) => UpdateRemoveButtonAutomationName();

		private void OnRemoveButtonLoaded(object sender, RoutedEventArgs e) => UpdateRemoveButtonAutomationName();

		private void OnAutomationNameSourceChanged(DependencyObject sender, DependencyProperty property) =>
			UpdateRemoveButtonAutomationName();

		private void UpdateRemoveButtonAutomationName()
		{
			if (!_ownsRemoveButtonAutomationName || _removeButton is null)
			{
				return;
			}

			var chipName = FrameworkElementAutomationPeer.CreatePeerForElement(this)?.GetName();
			AutomationProperties.SetName(
				_removeButton,
				string.IsNullOrWhiteSpace(chipName)
					? RemoveButtonAutomationName
					: $"{RemoveButtonAutomationName} {chipName}");
		}

		private void OnIsCheckedPropertyChanged(DependencyObject sender, DependencyProperty property)
		{
			var oldValue = _automationIsChecked;
			var newValue = IsChecked;
			_automationIsChecked = newValue;

			if (oldValue == newValue)
			{
				return;
			}

			if (OwningChipGroup is { SelectionMode: not ChipSelectionMode.None } chipGroup)
			{
				var oldSelection = oldValue == true;
				var newSelection = newValue == true;
				if (oldSelection == newSelection)
				{
					return;
				}

				var selectionEvent = newSelection
					? chipGroup.SelectionMode == ChipSelectionMode.Multiple
						? AutomationEvents.SelectionItemPatternOnElementAddedToSelection
						: AutomationEvents.SelectionItemPatternOnElementSelected
					: AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection;

				if ((AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged) ||
					AutomationPeer.ListenerExists(selectionEvent)) &&
					FrameworkElementAutomationPeer.CreatePeerForElement(this) is ChipAutomationPeer peer)
				{
					peer.RaiseSelectionChanged(oldSelection, newSelection, chipGroup.SelectionMode);
				}
			}
			else if (OwningChipGroup is null &&
				AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged) &&
				FrameworkElementAutomationPeer.CreatePeerForElement(this) is ChipAutomationPeer peer)
			{
				peer.RaiseToggleStateChanged(oldValue, newValue);
			}
		}

		private void RaiseIsCheckedChanged(object sender, RoutedEventArgs e)
		{
			if (!_shouldRaiseIsCheckedChanged)
			{
				IsCheckedChanged?.Invoke(sender, e);
			}
		}

		private void RaiseRemoveButtonClicked(object sender, RoutedEventArgs e)
		{
			// note: sender is the RemoveButton, do not pass it as the event sender
			// as ChipGroup expect the sender to be an instance of Chip

			if (CanRemove)
			{
				var removingArgs = new ChipRemovingEventArgs();
				Removing?.Invoke(this, removingArgs);

				if (!removingArgs.Cancel)
				{
					Removed?.Invoke(this, e);

					var param = RemovedCommandParameter;
					if (RemovedCommand is ICommand command && command.CanExecute(param))
					{
						command.Execute(param);
					}
				}
			}
		}

		internal void SetIsCheckedSilently(bool? value)
		{
			try
			{
				_shouldRaiseIsCheckedChanged = true;
				IsChecked = value;
			}
			finally
			{
				_shouldRaiseIsCheckedChanged = false;
			}
		}

		protected override void OnToggle()
		{
			var mode = OwningChipGroup?.SelectionMode;
			if (mode is ChipSelectionMode.None)
			{
				SetIsCheckedSilently(false);
				return;
			}
			if (mode is ChipSelectionMode.Single && IsChecked == true)
			{
				return;
			}

			base.OnToggle();
		}
	}
}
