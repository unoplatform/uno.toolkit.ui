using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endif

namespace Uno.Toolkit.UI
{
	public partial class ChipGroup : ItemsControl
	{
		private bool _isLoaded;
		private bool _isSynchronizingSelection;

		public ChipGroup()
		{
			RegisterPropertyChangedCallback(ItemsSourceProperty, (s, e) => (s as ChipGroup)?.OnItemsSourceChanged());

			this.Loaded += OnLoaded;
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			_isLoaded = true;
			SynchronizeInitialSelection();
			EnforceSelectionMode();
			ApplyIconTemplate(null, IconTemplate);
		}

		private void OnSelectionMemberPathChanged(DependencyPropertyChangedEventArgs e)
		{
			var binding = SelectionMemberPath != null
				? new Binding { Path = new PropertyPath(SelectionMemberPath), Mode = BindingMode.TwoWay }
				: null;
			foreach (var container in this.GetItemContainers<Chip>())
			{
				container.ClearValue(Chip.IsCheckedProperty);
				if (binding != null)
				{
					container.SetBinding(Chip.IsCheckedProperty, binding);
				}
			}
		}

		private void ApplyIconTemplate(DataTemplate? oldTemplate, DataTemplate? newTemplate)
		{
			if (oldTemplate == newTemplate) return;

			foreach (var container in this.GetItemContainers<Chip>())
			{
				if (newTemplate is not null)
				{
					// If there was no Icon assigned, use Content as Icon
					// Otherwise the icon presenter will not display anything without an icon
					container.Icon ??= container.Content;
					container.IconTemplate = newTemplate;
				}
				else
				{
					// clear icon if we previously used content for the icon
					if (container.Icon == container.Content)
					{
						container.Icon = null;
					}
					container.IconTemplate = null;
				}
			}
		}

		private void ApplyCanRemoveProperty()
		{
			var canRemove = CanRemove;
			foreach (var container in this.GetItemContainers<Chip>())
			{
				container.CanRemove = canRemove;
			}
		}

		private void OnSelectionModeChanged(DependencyPropertyChangedEventArgs e)
		{
			EnforceSelectionMode();
		}

		private void OnItemIsCheckedChanged(object sender, RoutedEventArgs e)
		{
			if (_isSynchronizingSelection) return;
			if (sender is Chip container)
			{
				UpdateSelection(new[] { container });
			}
		}

		protected override void OnItemsChanged(object e)
		{
			base.OnItemsChanged(e);

			SynchronizeInitialSelection();
			EnforceSelectionMode();
		}

		protected void OnItemsSourceChanged()
		{
			SynchronizeInitialSelection();
			EnforceSelectionMode();
		}

		private void OnSelectedItemChanged()
		{
			if (_isSynchronizingSelection || !IsReady) return;

			if (IsSingleSelection && GetCoercedSelection() is Chip container)
			{
				container.SetIsCheckedSilently(true);
				UpdateSelection(new[] { container });
			}
			else
			{
				UpdateSelection(null);
			}

			Chip? GetCoercedSelection() =>
				this.FindContainer<Chip>(SelectedItem) ??
				(SelectionMode is ChipSelectionMode.Single ? GetFallbackSelection() : default);
			Chip? GetFallbackSelection() =>
				this.GetItemContainers<Chip>().FirstOrDefault(x => x.IsChecked == true) ??
				this.GetItemContainers<Chip>().FirstOrDefault();
		}

		private void OnSelectedItemsChanged()
		{
			if (_isSynchronizingSelection || !IsReady) return;

			if (IsMultiSelection)
			{
				var selectedContainers = SelectedItems
					?.Cast<object>()
					.Select(x => this.FindContainer<Chip>(x))
					.OfType<Chip>() // trim null and force T from Chip? to Chip
					.ToArray();

				foreach (var container in selectedContainers ?? Enumerable.Empty<Chip>())
				{
					container.SetIsCheckedSilently(true);
				}

				UpdateSelection(selectedContainers, forceClearOthersSelection: true);
			}
			else if (SelectionMode == ChipSelectionMode.Single &&
				// setting the incorrect SelectedItem or -Items property for the current SelectionMode will trigger a full reset on Selection.
				// and, we do not preserve any valid existing selection in that case.
				this.GetItemContainers<Chip>().FirstOrDefault() is { } fallback)
			{
				fallback.SetIsCheckedSilently(true);
				UpdateSelection(new[] { fallback }, forceClearOthersSelection: true);
			}
			else
			{
				UpdateSelection(null, forceClearOthersSelection: true);
			}
		}

		protected override bool IsItemItsOwnContainerOverride(object item) => item is Chip;

		protected override DependencyObject GetContainerForItemOverride() => new Chip();

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);

			if (element is Chip container)
			{
				if (SelectionMemberPath != null)
				{
					container.SetBinding(Chip.IsCheckedProperty, new Binding { Path = new PropertyPath(SelectionMemberPath), Mode = BindingMode.TwoWay });
				}
				else
				{
					container.IsChecked = IsItemSelected(item);
				}

				if (IconTemplate != null)
				{
					container.Icon = container.Content;
					container.IconTemplate = IconTemplate;
				}

				container.CanRemove = CanRemove;

				container.IsCheckedChanged += OnItemIsCheckedChanged;
				container.Click += OnItemClick;
				container.Checked += OnItemChecked;
				container.Unchecked += OnItemUnchecked;
				container.Removing += OnItemRemoving;
				container.Removed += OnItemRemoved;
			}
		}

		private bool IsItemSelected(object item)
		{
			// It's important to use Equals and not == because of boxing.
			return Equals(item, SelectedItem) || (SelectedItems?.Contains(item) ?? false);
		}

		protected override void ClearContainerForItemOverride(DependencyObject element, object item)
		{
			base.ClearContainerForItemOverride(element, item);

			if (element is Chip container)
			{
				container.ClearValue(Chip.IsCheckedProperty);
				container.IsChecked = false;

				container.Icon = null;
				container.IconTemplate = null;
				container.CanRemove = false;

				container.IsCheckedChanged -= OnItemIsCheckedChanged;
				container.Click -= OnItemClick;
				container.Checked -= OnItemChecked;
				container.Unchecked -= OnItemUnchecked;
				container.Removing -= OnItemRemoving;
				container.Removed -= OnItemRemoved;
			}
		}

		private void OnItemClick(object sender, RoutedEventArgs e) => RaiseItemEvent(ItemClick, sender);

		private void OnItemChecked(object sender, RoutedEventArgs e) => RaiseItemEvent(ItemChecked, sender);

		private void OnItemUnchecked(object sender, RoutedEventArgs e) => RaiseItemEvent(ItemUnchecked, sender);

		private void OnItemRemoving(object sender, ChipRemovingEventArgs e)
		{
			if (sender is Chip container)
			{
				var args = new ChipItemRemovingEventArgs(ItemFromContainer(container));
				args.Cancel = e.Cancel;

				ItemRemoving?.Invoke(this, new ChipItemRemovingEventArgs(ItemFromContainer(container)));
				e.Cancel = args.Cancel;
			}
		}

		private void OnItemRemoved(object sender, RoutedEventArgs e)
		{
			if (sender is Chip container)
			{
				// there isn't much that can be done here if the item is generated from an ItemsSource
				// in such case, the removal should be handled from the source provider (view-model or code-behind)

				// remove the item only if it was added via xaml or .Add(item)
				if (ItemsSource == null &&
					Items?.IndexOf(container) is int index && index != -1)
				{
					Items.RemoveAt(index);
				}

				RaiseItemEvent(ItemRemoved, sender);
			}
		}

		private void RaiseItemEvent(ChipItemEventHandler? handler, object? originalSender)
		{
			if (originalSender is Chip container)
			{
				handler?.Invoke(this, new ChipItemEventArgs(ItemFromContainer(container)));
			}
		}

		private bool IsSingleSelection => SelectionMode is ChipSelectionMode.SingleOrNone or ChipSelectionMode.Single;

		private bool IsMultiSelection => SelectionMode is ChipSelectionMode.Multiple;

		private void SynchronizeInitialSelection()
		{
			if (!IsReady) return;

			if (SelectedItem != null)
			{
				OnSelectedItemChanged();
			}

			if (SelectedItems != null)
			{
				OnSelectedItemsChanged();
			}
		}

		private void EnforceSelectionMode()
		{
			if (!IsReady) return;

			var selected = default(Chip);
			foreach (var container in this.GetItemContainers<Chip>())
			{
				if (IsSingleSelection && container.IsChecked == true)
				{
					// preserve first existing selection and clear the rest
					if (selected is { })
					{
						container.SetIsCheckedSilently(false);
					}
					else
					{
						selected = container;
					}
				}
			}

			// try enforce Single if nothing is selected
			if (SelectionMode is ChipSelectionMode.Single && selected is null)
			{
				selected = this.GetItemContainers<Chip>().FirstOrDefault();
				selected?.SetIsCheckedSilently(true);
			}

			UpdateSelection(selected is { } ? new[] { selected } : default);
		}

		/// <summary>
		/// Update selection with coercion for SelectionMode.
		/// </summary>
		/// <param name="newlySelectedContainers">New selection which should be spared from coercion.</param>
		/// <param name="forceClearOthersSelection">
		/// Indicates if other selection should be cleared. This is implied for single selection.
		/// In multi-select mode, this is used to differentiate between appending and resetting the selection.
		/// </param>
		/// <remarks>
		/// This method does not set the IsChecked for each of newlySelectedContainers; it expects them to be already set.
		/// However, it will uncheck other container when forceClearOthersSelection is set or when IsSingleSelect.
		/// </remarks>
		private void UpdateSelection(Chip[]? newlySelectedContainers, bool forceClearOthersSelection = false)
		{
			if (!IsReady) return;
			if (_isSynchronizingSelection) return;

			try
			{
				_isSynchronizingSelection = true;

				var selectedItems = new List<object>();
				foreach (var container in this.GetItemContainers<Chip>())
				{
					if (!container.IsChecked ?? false)
					{
						continue;
					}

					if (ShouldClearSelection(container))
					{
						container.SetIsCheckedSilently(false);
					}
					else
					{
						selectedItems.Add(ItemFromContainer(container));
					}
				}

				// update selection properties
				SelectedItem = IsSingleSelection && selectedItems?.Count == 1
					? selectedItems[0]
					: null;
				SelectedItems = IsMultiSelection && selectedItems?.Count >= 1
					? selectedItems
					: null;
			}
			finally
			{
				_isSynchronizingSelection = false;
			}

			bool ShouldClearSelection(Chip container)
			{
				switch (SelectionMode)
				{
					case ChipSelectionMode.None:
						// uncheck all
						return true;
					case ChipSelectionMode.SingleOrNone:
					case ChipSelectionMode.Single:
						// uncheck every other items
						return newlySelectedContainers?.Contains(container) != true;
					case ChipSelectionMode.Multiple:
						// uncheck other items if SelectedItem or SelectedItems got updated
						return forceClearOthersSelection && newlySelectedContainers?.Contains(container) != true;

					default: throw new ArgumentOutOfRangeException(nameof(SelectionMode));
				}
			}
		}

		private bool IsReady => _isLoaded && HasItems && HasContainers;

		private bool HasItems => this.GetItems().OfType<object>().Any();

		private bool HasContainers => this.GetItemContainers<Chip>().Any();
	}
}
