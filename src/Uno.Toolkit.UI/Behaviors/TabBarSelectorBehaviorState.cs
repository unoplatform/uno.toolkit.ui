using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Disposables;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI
{
	internal partial class TabBarSelectorBehaviorState
	{
		public Selector Selector { get; }
		public TabBar TabBar { get; }

		private bool _isSynchronizing;
		private double _lastOffsetX;

		public TabBarSelectorBehaviorState(Selector selector, TabBar tabBar)
		{
			Selector = selector;
			TabBar = tabBar;
		}

		public void Connect()
		{
			if (Selector == null || TabBar == null)
			{
				return;
			}

			Selector.SelectionChanged += OnSelectorSelectionChanged;
			TabBar.SelectionChanged += OnTabBarSelectionChanged;


			OnSelectorSelectionChanged(null, null);

			ConnectPartial();
		}

		partial void ConnectPartial();

		public void Disconnect()
		{ 
			if (Selector != null)
			{
				Selector.SelectionChanged -= OnSelectorSelectionChanged;
			}

			if (TabBar != null)
			{
				TabBar.SelectionChanged -= OnTabBarSelectionChanged;
			}

			DisconnectPartial();
		}

		partial void DisconnectPartial();

		public int? MapIndexToSelector(int index)
		{
			int unselectableCount = 0;
			for (int i = 0; i < index; i++)
			{
				if (TabBar.ContainerFromIndex(i) is TabBarItem { IsSelectable: false })
				{
					unselectableCount++;
				}
			}

			return index - unselectableCount;
		}

		public int? MapIndexToTabBar(int index)
		{
			int skipped = 0;
			for (int i = 0; i < TabBar.Items.Count; i++)
			{
				if (TabBar.ContainerFromIndex(i) is TabBarItem { IsSelectable: true })
				{
					if (skipped == index)
					{
						return i;
					}

					skipped++;
				}

			}

			return null;
		}

		public double GetRelativeX(UIElement? element)
		{
			if (element == null)
			{
				return 0d;
			}

			var point = new Point(0, 0);

			var nextPosPoint = element.TransformToVisual(TabBar).TransformPoint(point);

			return nextPosPoint.X;
		}

		// TO REVIEW: #719
		// This no longer seems to serve any purpose, selector offset seems to render correctly without it.
		// Having this actually causes buggy behavior
		// ******************************************************************************************************
		public void UpdateOffset(int position, double progress, double totalOffset)
		{
			UIElement? selectionIndicator;

			if (totalOffset == _lastOffsetX
				|| (selectionIndicator = TabBar.FindChild<TabBarSelectionIndicatorPresenter>()?.GetSelectionIndicator()) is null)
			{
				return;
			}

			if (progress == 0)
			{
				UpdateOffsetToPosition(position, selectionIndicator);
				_lastOffsetX = totalOffset;

				return;
			}

			var toRight = totalOffset > _lastOffsetX && progress > 0;
			_lastOffsetX = totalOffset;

			var previousIndex = toRight ? MapIndexToTabBar(position) : MapIndexToTabBar(position + 1);
			if (previousIndex == null || previousIndex < 0 || previousIndex >= TabBar.Items.Count)
			{
				return;
			}

			var nextTabIndex = toRight ? MapIndexToTabBar(position + 1) : MapIndexToTabBar(position);
			if (nextTabIndex == null || nextTabIndex < 0 || nextTabIndex >= TabBar.Items.Count)
			{
				return;
			}

			var selectedTabBarItem = TabBar.ContainerFromIndex(previousIndex.Value) as TabBarItem;
			var nextTabBarItem = TabBar.ContainerFromIndex(nextTabIndex.Value) as TabBarItem;

			var currentX = GetRelativeX(selectedTabBarItem);
			var nextX = GetRelativeX(nextTabBarItem);

			var distance = Math.Abs(nextX - currentX);

			var indicatorPosition = currentX + ((selectedTabBarItem?.ActualWidth ?? 0) / 2) - (selectionIndicator.ActualSize.X / 2);

			SelectorExtensions.SetSelectionOffset(
				TabBar, 
				toRight
				? indicatorPosition + (progress * distance)
				: indicatorPosition - ((1 - progress) * distance)
			);
		}
		// ******************************************************************************************************

		private void UpdateOffsetToPosition(int position, UIElement selectionIndicator)
		{
			if (MapIndexToTabBar(position) is { } index && index >= 0 && index < TabBar.Items.Count)
			{
				var tabItem = TabBar.ContainerFromIndex(index) as TabBarItem;
				SelectorExtensions.SetSelectionOffset(TabBar, GetRelativeX(tabItem) + ((tabItem?.ActualWidth ?? 0) / 2) - (selectionIndicator.ActualSize.X / 2));
			}
		}

		private void OnTabBarSelectionChanged(TabBar sender, TabBarSelectionChangedEventArgs args)
		{
			if (_isSynchronizing)
			{
				return;
			}

			try
			{
				if (Selector == null)
				{
					return;
				}

				var tabBarIndex = TabBar.SelectedIndex;
				var mappedIndex = MapIndexToSelector(tabBarIndex);

				if (mappedIndex is { } index && index >= 0 && index < Selector.Items.Count)
				{
					_isSynchronizing = true;
					Selector.SelectedIndex = index;
				}
			}
			finally
			{
				_isSynchronizing = false;
			}
		}

		private void OnSelectorSelectionChanged(object? sender, SelectionChangedEventArgs? e)
		{
			if (_isSynchronizing)
			{
				return;
			}

			try
			{
				if (TabBar == null)
				{
					return;
				}

				var selectorIndex = Selector.SelectedIndex;
				var mappedIndex = MapIndexToTabBar(selectorIndex);

				if (mappedIndex is { } index && index >= 0 && index < TabBar.Items.Count)
				{
					_isSynchronizing = true;
					TabBar.SelectedIndex = index;
				}
			}
			finally
			{
				_isSynchronizing = false;
			}
		}

	}
}
