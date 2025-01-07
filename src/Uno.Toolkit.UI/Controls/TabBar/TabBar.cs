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
			if (IsUsingOwnContainerAsTemplateRoot && element is ContentPresenter cp)
			{
				// ItemsControl::PrepareContainerForItemOverride will apply the ItemContainerStyle to the element which is not something we want here,
				// since it can throw: The DP [WrongDP] is owned by [Control] and cannot be used on [ContentPresenter].
				// While this doesnt break the control or the visual, it can cause a scaling performance degradation.

				cp.ContentTemplate = ItemTemplate;
				cp.ContentTemplateSelector = ItemTemplateSelector;

				cp.DataContext = item;
				SetContent(cp, item);

#if !HAS_UNO
				// force template materialization
				cp.Measure(Size.Empty);
#endif

				if (cp.GetFirstChild() is TabBarItem tbi)
				{
					ApplyContainerStyle(tbi);
					SetupTabBarItem(tbi);
				}
			}
			else
			{
				base.PrepareContainerForItemOverride(element, item);
				if (element is TabBarItem tbi)
				{
					SetupTabBarItem(tbi);
				}
			}

			void SetContent(ContentPresenter cp, object item)
			{
				if (string.IsNullOrEmpty(DisplayMemberPath))
				{
					cp.Content = item;
				}
				else
				{
					cp.SetBinding(ContentPresenter.ContentProperty, new Binding
					{
						Source = item,
						Path = new(DisplayMemberPath),
					});
				}
			}
			void ApplyContainerStyle(TabBarItem tbi)
			{
				var localStyleValue = tbi.ReadLocalValue(FrameworkElement.StyleProperty);
				var isStyleSetFromTabBar = tbi.IsStyleSetFromTabBar;

				if (localStyleValue == DependencyProperty.UnsetValue || isStyleSetFromTabBar)
				{
					var style = ItemContainerStyle ?? ItemContainerStyleSelector?.SelectStyle(item, tbi);
					if (style is { })
					{
						tbi.Style = style;
						tbi.IsStyleSetFromTabBar = true;
					}
					else
					{
						tbi.ClearValue(FrameworkElement.StyleProperty);
						tbi.IsStyleSetFromTabBar = false;
					}
				}
			}
			void SetupTabBarItem(TabBarItem tbi)
			{
				tbi.IsSelected = IsSelected(IndexFromContainer(element));
				tbi.Click += OnTabBarItemClick;
				tbi.IsSelectedChanged += OnTabBarIsSelectedChanged;
			}
		}

		internal virtual bool IsSelected(int index)
		{
			return SelectedIndex == index;
		}

		protected override void ClearContainerForItemOverride(DependencyObject element, object item)
		{
			if (IsUsingOwnContainerAsTemplateRoot && element is ContentPresenter cp)
			{
				if (cp.GetFirstChild() is TabBarItem tbi)
				{
					TearDownTabBarItem(tbi);
				}
			}
			else
			{
				base.ClearContainerForItemOverride(element, item);
				if (element is TabBarItem tbi)
				{
					TearDownTabBarItem(tbi);
				}
			}

			void TearDownTabBarItem(TabBarItem item)
			{
				item.Click -= OnTabBarItemClick;
				item.IsSelectedChanged -= OnTabBarIsSelectedChanged;
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

					if (GetInnerContainer(item as DependencyObject) is { IsSelected: true } selected) // see comment on GetInnerContainer
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

		internal void OnItemsPanelConnected(TabBarListPanel panel)
		{
			System.Diagnostics.Debug.Assert(ItemsPanelRoot != null, "ItemsPanelRoot is expected to be already set in here.");

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
			IsUsingOwnContainerAsTemplateRoot = IsItemItsOwnContainerOverride(newItemTemplate?.LoadContent());
			base.OnItemTemplateChanged(oldItemTemplate, newItemTemplate);
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
					var tbi = GetInnerContainer(container); // see comment on GetInnerContainer
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

		// When using an `ItemTemplate` with a `TabBarItem`, the container will be a `ContentPresenter` that wraps the `TabBarItem`.
		// In that case, to access the `ContentPresenter` from a `TabBarItem`, you must first call `ContainerFromItem`.
		// Afterward, pass the resulting `ContentPresenter` as a parameter to this method.
		internal TabBarItem? GetInnerContainer(DependencyObject? container)
		{
			if (IsUsingOwnContainerAsTemplateRoot)
			{
				return (container as ContentPresenter)?.GetFirstChild() as TabBarItem;
			}

			return container as TabBarItem;
		}

		internal DependencyObject? InnerContainerFromIndex(int index)
		{
			var container = ContainerFromIndex(index);
			var inner = GetInnerContainer(container);

			return inner;
		}

		private TabBarItem? InnerContainerFromIndexSafe(int index)
		{
			if (index >= 0 && index < Items.Count)
			{
				return InnerContainerFromIndex(index) as TabBarItem;
			}

			return null;
		}

		private bool IsReady => _isLoaded && HasItems && ItemsPanelRoot is { };

		private bool HasItems => this.GetItems().Any();
	}
}
