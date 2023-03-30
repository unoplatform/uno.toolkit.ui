using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Uno.Disposables;
using Uno.Extensions.Specialized;
using Windows.Foundation;
using Windows.Foundation.Collections;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// A control to display a set of <see cref="TabBarItem"/>s horizontally with the ability to display a custom view to denote selected state
	/// </summary>
	[TemplatePart(Name = TabBarGridName, Type = typeof(Grid))]
	public partial class TabBar : ItemsControl
	{
		private const string TabBarGridName = "TabBarGrid";

		private bool _isSynchronizingSelection;
		private object? _previouslySelectedItem;
		private bool _isLoaded;
		private bool _isUpdatingSelectedItem;

		public TabBar()
		{
			DefaultStyleKey = typeof(TabBar);
			RegisterPropertyChangedCallback(ItemsSourceProperty, (s, e) => (s as TabBar)?.OnItemsSourceChanged());
			Loaded += OnLoaded;
			TemplateSettings = new TabBarTemplateSettings();
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			UpdateOrientation();
		}

		protected override bool IsItemItsOwnContainerOverride(object item) => item is TabBarItem;

		protected override DependencyObject GetContainerForItemOverride() => new TabBarItem();

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);

			if (element is TabBarItem container)
			{	
				container.IsSelected = IsSelected(IndexFromContainer(element));
				container.Click += OnTabBarItemClick;
				container.IsSelectedChanged += OnTabBarIsSelectedChanged;
			}
		}

		internal virtual bool IsSelected(int index)
		{
			return SelectedIndex == index;
		}

		protected override void ClearContainerForItemOverride(DependencyObject element, object item)
		{
			base.ClearContainerForItemOverride(element, item);

			if (element is TabBarItem container)
			{
				container.Click -= OnTabBarItemClick;
				container.IsSelectedChanged -= OnTabBarIsSelectedChanged;
				container.Style = null;
			}
		}

		protected override void OnItemsChanged(object e)
		{
			if (
				// When ItemsSource is set, we get collection changes from it directly (and it's not possible to directly modify Items)
				ItemsSource == null &&
				e is IVectorChangedEventArgs iVCE
			)
			{
				if (iVCE.CollectionChange == CollectionChange.ItemChanged
					|| (iVCE.CollectionChange == CollectionChange.ItemInserted && iVCE.Index < Items.Count))
				{
					var item = Items[(int)iVCE.Index];

					if (item is TabBarItem tabBarItem && tabBarItem.IsSelected)
					{
						SelectedItem = tabBarItem;
					}
				}
				else if (iVCE.CollectionChange == CollectionChange.ItemRemoved)
				{
					// If the removed item is the currently selected one, Set SelectedIndex to -1
					if ((int)iVCE.Index == SelectedIndex)
					{
						SelectedIndex = -1;
					}
					// But if it's before the currently selected one, decrement SelectedIndex
					else if ((int)iVCE.Index < SelectedIndex)
					{
						SelectedIndex--;
					}
				}
			}

			SynchronizeInitialSelection();
		}

		private void SynchronizeInitialSelection()
		{
			if (!IsReady)
			{
				return;
			}

			if (SelectedItem != null)
			{
				OnSelectedItemChanged(null);
			} 
			else if (SelectedIndex >= 0)
			{
				OnSelectedIndexChanged(null);
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			_isLoaded = true;
			SynchronizeInitialSelection();
			UpdateOrientation();
		}

		private void OnTabBarItemClick(object sender, RoutedEventArgs e)
		{
			if (_isSynchronizingSelection)
			{
				return;
			}

			if (sender is TabBarItem container && container.IsSelectable)
			{
				container.IsSelected = true;
			}
		}

		private void OnTabBarIsSelectedChanged(object sender, RoutedEventArgs e)
		{
			if (_isSynchronizingSelection || _isUpdatingSelectedItem)
			{
				return;
			}

			if (sender is TabBarItem container)
			{
				_previouslySelectedItem = SelectedItem;
				SynchronizeSelection(container);
			}
		}

		private void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			DependencyProperty property = args.Property;

			if (property == SelectedItemProperty)
			{
				OnSelectedItemChanged(args);
			}
			else if (property == SelectedIndexProperty)
			{
				OnSelectedIndexChanged(args);
			}
			else if (property == OrientationProperty)
			{
				UpdateOrientation();
			}
		}

		private void UpdateOrientation()
		{
			var orientation = Orientation;
			if (ItemsPanelRoot is TabBarListPanel panel)
			{
				panel.Orientation = orientation;
			}

			VisualStateManager.GoToState(this, orientation == Orientation.Horizontal ? "Horizontal" : "Vertical", useTransitions: false);
		}

		private void OnItemsSourceChanged()
		{
			SynchronizeInitialSelection();
		}

		private void OnSelectedItemChanged(DependencyPropertyChangedEventArgs? args)
		{
			if (_isSynchronizingSelection)
			{
				return;
			}

			var newlySelectedItem = SelectedItem as TabBarItem;
			if (!IsReady && newlySelectedItem != null)
			{
				return;
			}

			if (TryUpdateTabBarItemSelectedState(args?.OldValue, newlySelectedItem))
			{
				SynchronizeSelection(newlySelectedItem);
			}
		}

		private void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs? args)
		{
			if (_isSynchronizingSelection)
			{
				return;
			}

			TabBarItem? oldItem = null;
			if (args?.OldValue is int oldIndex)
			{
				oldItem = this.ContainerFromIndexSafe<TabBarItem>(oldIndex);
			}

			var newItem = this.ContainerFromIndexSafe<TabBarItem>(SelectedIndex);

			if (TryUpdateTabBarItemSelectedState(oldItem, newItem))
			{
				SynchronizeSelection(newItem);
			}
		}

		//Update the IsSelected state for the TabBarItem
		private bool TryUpdateTabBarItemSelectedState(object? oldItem, object? newItem)
		{
			try
			{
				_isUpdatingSelectedItem = true;
				_previouslySelectedItem = oldItem;

				var container = this.FindContainer<TabBarItem>(newItem);
				if (container?.IsSelectable ?? false)
				{
					container.IsSelected = true;
					return true;
				}
				return false;
			}
			finally
			{
				_isUpdatingSelectedItem = false;
			}
		}

		//Sync the SelectedIndex and the SelectedItem properties for TabBar as well as the IsSelected states of the TabBarItems
		private void SynchronizeSelection(TabBarItem? item)
		{
			if (!IsReady || _isSynchronizingSelection)
			{
				return;
			}

			try
			{
				_isSynchronizingSelection = true;

				foreach (var container in this.GetItemContainers<TabBarItem>())
				{
					if (!container.IsSelected)
					{
						continue;
					}

					if (container != item)
					{
						container.IsSelected = false;
					}
					else
					{
						SelectedItem = ItemFromContainer(container);
						SelectedIndex = IndexFromContainer(container);
						RaiseSelectionChangedEvent(_previouslySelectedItem, item);
					}
				}
			}
			finally
			{
				_isSynchronizingSelection = false;
			}
		}

		private void RaiseSelectionChangedEvent(object? prevItem, object? nextItem)
		{
			var eventArgs = new TabBarSelectionChangedEventArgs
			{
				OldItem = prevItem,
				NewItem = nextItem
			};

			SelectionChanged?.Invoke(this, eventArgs);
		}

		private bool IsReady => _isLoaded && HasItems;

		private bool HasItems => this.GetItems().Any();
	}
}
