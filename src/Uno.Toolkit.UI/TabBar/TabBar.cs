using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Uno.Disposables;
using Uno.Extensions.Specialized;
using Windows.Foundation;

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

namespace Uno.UI.ToolkitLib
{
	/// <summary>
	/// A control to display a set of <see cref="TabBarItem"/>s horizontally with the ability to display a custom view to denote selected state
	/// </summary>
	[TemplatePart(Name = TabBarGridName, Type = typeof(Grid))]
	public partial class TabBar : ItemsControl
	{
		private const string TabBarGridName = "TabBarGrid";

		private Grid? _tabBarGrid;
		private bool _isSynchronizingSelection = false;
		private object? _previouslySelectedItem = null;

		public TabBarTemplateSettings TemplateSettings { get; }

		public TabBar()
		{
			DefaultStyleKey = typeof(TabBar);

			RegisterPropertyChangedCallback(ItemsSourceProperty, (s, e) => (s as TabBar)?.OnItemsSourceChanged());
			Loaded += OnLoaded;
			TemplateSettings = new TabBarTemplateSettings();
			SizeChanged += OnSizeChanged;
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.PreviousSize.Width != e.NewSize.Width)
			{
				TemplateSettings.SelectionIndicatorWidth = e.NewSize.Width / Items.Count;
			}
		}

		protected override void OnApplyTemplate()
		{
			_tabBarGrid = GetTemplateChild(TabBarGridName) as Grid;

			base.OnApplyTemplate();
		}

		protected override bool IsItemItsOwnContainerOverride(object item) => item is TabBarItem;

		protected override DependencyObject GetContainerForItemOverride() => new TabBarItem();

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);

			if (element is TabBarItem container)
			{
				container.Click += OnTabBarItemClick;
				container.IsSelectedChanged += OnTabBarIsSelectedChanged;
				container.Style = ItemContainerStyle ?? ItemContainerStyleSelector?.SelectStyle(item, container);
			}
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
			base.OnItemsChanged(e);

			var selectedContainer = GetItemContainers().FirstOrDefault(x => x.IsSelected);
			if (selectedContainer != null)
			{
				TemplateSettings.SelectionIndicatorWidth = selectedContainer.Width;
			}

			UpdateSelection(selectedContainer);
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			var selectedContainer = GetItemContainers().FirstOrDefault(x => x.IsSelected);
			UpdateSelection(selectedContainer);
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
			if (_isSynchronizingSelection)
			{
				return;
			}

			if (sender is TabBarItem container)
			{
				_previouslySelectedItem = SelectedItem;
				UpdateSelection(container);
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
		}

		private void OnItemsSourceChanged()
		{
			var selectedContainer = GetItemContainers().FirstOrDefault(x => x.IsSelected);
			UpdateSelection(selectedContainer);
		}

		private void OnSelectedItemChanged(DependencyPropertyChangedEventArgs args)
		{
			if (_isSynchronizingSelection)
			{
				return;
			}

			ChangeSelection(args.OldValue, args.NewValue);
		}

		private void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs args)
		{
			if (_isSynchronizingSelection)
			{
				return;
			}

			var oldItem = FindContainerByIndex((int)args.OldValue);
			var newItem = FindContainerByIndex((int)args.NewValue);

			ChangeSelection(oldItem, newItem);
		}

		private void ChangeSelection(object? oldItem, object? newItem)
		{
			_previouslySelectedItem = oldItem;

			var container = FindContainer(newItem);
			if (container?.IsSelectable ?? false)
			{
				container.IsSelected = true;
			}
		}


		private void UpdateSelection(TabBarItem item)
		{
			if (ItemsPanelRoot == null || _isSynchronizingSelection)
			{
				return;
			}

			try
			{
				_isSynchronizingSelection = true;

				foreach (var container in GetItemContainers())
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

		private TabBarItem? FindContainer(object? item)
		{
			if (item == null)
		{
				return null;
			}

			return item as TabBarItem ??
				ContainerFromItem(item) as TabBarItem;
		}

		/// <summary>
		/// Get the item containers.
		/// </summary>
		/// <remarks>An empty enumerable will returned if the <see cref="ItemsControl.ItemsPanelRoot"/> and the containers have not been materialized.</remarks>
		private IEnumerable<TabBarItem> GetItemContainers() =>
			ItemsPanelRoot?.Children.OfType<TabBarItem>() ??
			Enumerable.Empty<TabBarItem>();

		private TabBarItem? FindContainerByIndex(int index)
		{
			return GetItemContainers().ElementAtOrDefault(index);
		}
	}
}
