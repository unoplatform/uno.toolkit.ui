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

		internal bool IsUsingOwnContainerAsTemplateRoot { get; private set; }

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
			UpdateIndicatorPlacement();
		}

		protected override bool IsItemItsOwnContainerOverride(object? item) => item is TabBarItem;

		protected override DependencyObject GetContainerForItemOverride()
		{
			if (IsUsingOwnContainerAsTemplateRoot)
			{
				return new ContentPresenter();
			}

			return new TabBarItem();
		}

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);

			void SetupTabBarItem(TabBarItem item)
			{
				item.IsSelected = IsSelected(IndexFromContainer(element));
				item.Click += OnTabBarItemClick;
				item.IsSelectedChanged += OnTabBarIsSelectedChanged;
			}

			if (element is TabBarItem container)
			{
				SetupTabBarItem(container);
			}
			else if (IsUsingOwnContainerAsTemplateRoot &&
				element is ContentPresenter outerContainer)
			{
				var templateRoot = outerContainer.ContentTemplate.LoadContent();
				if (templateRoot is TabBarItem tabBarItem)
				{
					outerContainer.ContentTemplate = null;

					SetupTabBarItem(tabBarItem);

					tabBarItem.DataContext = item;

					outerContainer.Content = tabBarItem;
				}
			}
		}

		internal virtual bool IsSelected(int index)
		{
			return SelectedIndex == index;
		}

		protected override void ClearContainerForItemOverride(DependencyObject element, object item)
		{
			base.ClearContainerForItemOverride(element, item);

			void TearDownTabBarItem(TabBarItem item)
			{
				item.Click -= OnTabBarItemClick;
				item.IsSelectedChanged -= OnTabBarIsSelectedChanged;
				if (!IsUsingOwnContainerAsTemplateRoot)
				{
					item.Style = null;
				}
			}
			if (element is TabBarItem container)
			{
				TearDownTabBarItem(container);
			}
			else if (IsUsingOwnContainerAsTemplateRoot &&
				element is ContentPresenter outerContainer)
			{
				if (outerContainer.Content is TabBarItem innerContainer)
				{
					TearDownTabBarItem(innerContainer);
					innerContainer.DataContext = null;
				}
				outerContainer.Content = null;
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

					if (GetInnerContainer(item as DependencyObject) is { IsSelected: true } selected)
					{
						SelectedItem = selected;
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
			else if (property == SelectionIndicatorPlacementProperty)
			{
				UpdateIndicatorPlacement();
			}
		}

		private void UpdateIndicatorPlacement()
		{
			var state = SelectionIndicatorPlacement switch
			{
				IndicatorPlacement.Above => "Above",
				IndicatorPlacement.Below => "Below",
				_ => throw new ArgumentOutOfRangeException(nameof(SelectionIndicatorPlacement))
			};

			VisualStateManager.GoToState(this, state, useTransitions: false);
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

		protected override void OnItemTemplateChanged(DataTemplate oldItemTemplate, DataTemplate newItemTemplate)
		{
			base.OnItemTemplateChanged(oldItemTemplate, newItemTemplate);

			IsUsingOwnContainerAsTemplateRoot = IsItemItsOwnContainerOverride(newItemTemplate?.LoadContent());
		}

		private void OnSelectedItemChanged(DependencyPropertyChangedEventArgs? args)
		{
			if (_isSynchronizingSelection)
			{
				return;
			}

			var newlySelectedItem = this.FindContainer<TabBarItem>(SelectedItem);
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
			if (args?.OldValue is int oldIndex && oldIndex != -1)
			{
				oldItem = this.InnerContainerFromIndexSafe(oldIndex);
			}

			var newItem = this.InnerContainerFromIndexSafe(SelectedIndex);

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

				var containers = this.GetItemContainers<UIElement>();
				foreach (var container in containers)
				{
					var tbi = GetInnerContainer(container);
					if (tbi is not { }) continue;

					if (!tbi.IsSelected)
					{
						continue;
					}

					if (tbi != item)
					{
						tbi.IsSelected = false;
					}
					else
					{
						var oldSelectedItem = SelectedItem;
						var newSelectedItem = ItemFromContainer(container);
						if (oldSelectedItem != newSelectedItem)
						{
							SelectedItem = newSelectedItem;
						}

						var oldSelectedIndex = SelectedIndex;
						var newSelectedIndex = IndexFromContainer(container);
						if (oldSelectedIndex != newSelectedIndex)
						{
							SelectedIndex = newSelectedIndex;
						}

						if (!object.ReferenceEquals(_previouslySelectedItem, item))
						{
							RaiseSelectionChangedEvent(_previouslySelectedItem, item);
						}
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

		internal TabBarItem? GetInnerContainer(DependencyObject? container)
		{
			if (IsUsingOwnContainerAsTemplateRoot)
			{
				return (container as ContentPresenter)?.Content as TabBarItem;
			}

			return container as TabBarItem;
		}

		internal DependencyObject? InnerContainerFromIndex(int index)
		{
			var container = ContainerFromIndex(index);
			if (IsUsingOwnContainerAsTemplateRoot)
			{
				container = (container as ContentPresenter)?.Content as DependencyObject;
			}

			return container;
		}

		private TabBarItem? InnerContainerFromIndexSafe(int index)
		{
			if (index >= 0 && index < Items.Count)
			{
				return InnerContainerFromIndex(index) as TabBarItem;
			}

			return null;
		}

		private bool IsReady => _isLoaded && HasItems;

		private bool HasItems => this.GetItems().Any();
	}
}
