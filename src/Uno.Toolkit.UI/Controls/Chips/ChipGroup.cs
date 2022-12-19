using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uno.Extensions.Specialized;

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
			ApplyIconTemplate();
		}

		private void OnSelectionMemberPathChanged(DependencyPropertyChangedEventArgs e)
		{
			var binding = SelectionMemberPath != null
				? new Binding { Path = new PropertyPath(SelectionMemberPath), Mode = BindingMode.TwoWay }
				: null;
			foreach (var container in GetItemContainers())
			{
				container.ClearValue(Chip.IsCheckedProperty);
				if (binding != null)
				{
					container.SetBinding(Chip.IsCheckedProperty, binding);
				}
			}
		}

		private void ApplyIconTemplate()
		{
			var itemTemplate = IconTemplate;
			foreach (var container in GetItemContainers())
			{
				container.Icon = itemTemplate != null ? container.Content : null;
				container.IconTemplate = itemTemplate;
			}
		}

		private void ApplyCanRemoveProperty()
		{
			var canRemove = CanRemove;
			foreach (var container in GetItemContainers())
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
				FindContainer(SelectedItem) ??
				(SelectionMode is ChipSelectionMode.Single ? GetFallbackSelection() : default);
			Chip? GetFallbackSelection () =>
				GetItemContainers().FirstOrDefault(x => x.IsChecked == true) ??
				GetItemContainers().FirstOrDefault();
		}

		private void OnSelectedItemsChanged()
		{
			if (_isSynchronizingSelection || !IsReady) return;

			if (IsMultiSelection)
			{
				var selectedContainers = SelectedItems
					?.Cast<object>()
					.Select(x => FindContainer(x))
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
				GetItemContainers().FirstOrDefault() is { } fallback)
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
			foreach (var container in GetItemContainers())
			{
				container.IsCheckable = SelectionMode != ChipSelectionMode.None;

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
				selected = GetItemContainers().FirstOrDefault();
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
				foreach (var container in GetItemContainers())
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

		private bool HasItems => GetItems().Any();

		private bool HasContainers => GetItemContainers().Any();

		/// <summary>
		/// Get the items.
		/// </summary>
		/// <remarks>The item itself maybe its own container, as in the case of <see cref="Chip"/> added as child to <see cref="ItemsControl.Items"/>.</remarks>
		private IEnumerable GetItems() =>
			ItemsSource as IEnumerable ??
			(ItemsSource as CollectionViewSource)?.View ??
			Items ??
			Enumerable.Empty<object>();

		private Chip? FindContainer(object? item)
		{
			if (item == null) return null;

			// For some obscure reason, ContainerFromItem returns null when item is an enum.
			// Note however that it works fine for other value types such as int.
			// Because of this, we retrieve the container using the index instead.
			if (item is Enum)
			{
				var index = GetItems().IndexOf(item);
				if (index != -1)
				{
					return ContainerFromIndex(index) as Chip;
				}
			}

			return
				item as Chip ??
				ContainerFromItem(item) as Chip;
		}

		/// <summary>
		/// Get the item containers.
		/// </summary>
		/// <remarks>An empty enumerable will returned if the <see cref="ItemsControl.ItemsPanelRoot"/> and the containers have not been materialized.</remarks>
		private IEnumerable<Chip> GetItemContainers() =>
			ItemsPanelRoot?.Children.OfType<Chip>() ??
			Enumerable.Empty<Chip>();

	}
}
