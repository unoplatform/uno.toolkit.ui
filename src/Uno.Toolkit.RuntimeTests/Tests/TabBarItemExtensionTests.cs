using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Uno.Extensions;
using Uno.Toolkit.RuntimeTests.Extensions;

using Uno.Toolkit.RuntimeTests.Tests.TestPages;

using static Uno.Toolkit.UI.TabBarItemExtensions;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal partial class TabBarItemExtensionTests
{
	private const int _defaultDelayValue = 500;
	[TestMethod]
	public async Task When_TabBarItem_NavigatesBackToRoot()
	{
		var root = CreateGrid();

		var frame = new Frame();

		var tabBarItem = CreateTabBarItem(TBIOnClickBehaviors.BackNavigationToRoot);

		TabBarItem[] source = [tabBarItem];

		var tabBar = CreateTabBar(source);

		Grid.SetRow(frame, 0);
		Grid.SetRow(tabBar, 1);

		root.Children.Add(frame);
		root.Children.Add(tabBar);

		await UnitTestUIContentHelperEx.SetContentAndWait(root);

		NavigateToLastPage(frame);

		// Ensure it navigated to the last page
		Assert.AreEqual(typeof(NavBarSimplePage), frame.Content.GetType());

		tabBarItem.ExecuteTap();

		Assert.AreEqual(typeof(NavBarFirstPage), frame.Content.GetType());
	}

	[TestMethod]
	public async Task When_TabBarItem_NavigatesBack()
	{
		var root = CreateGrid();

		var frame = new Frame();

		var tabBarItem = CreateTabBarItem(TBIOnClickBehaviors.BackNavigation);

		TabBarItem[] source = [tabBarItem];

		var tabBar = CreateTabBar(source);

		Grid.SetRow(frame, 0);
		Grid.SetRow(tabBar, 1);

		root.Children.Add(frame);
		root.Children.Add(tabBar);

		await UnitTestUIContentHelperEx.SetContentAndWait(root);

		NavigateToLastPage(frame);

		// Ensure it navigated to the last page
		Assert.AreEqual(typeof(NavBarSimplePage), frame.Content.GetType());

		tabBarItem.ExecuteTap();

		Assert.AreEqual(typeof(NavBarSecondPage), frame.Content.GetType());
	}

	[TestMethod]
	public async Task When_TabBarItem_ScrollToTop_ScrollViewer()
	{
		var root = CreateGrid();

		var scrollViewer = new ScrollViewer();

		// Create some content for the ScrollViewer (StackPanel with buttons)
		var stackPanel = new StackPanel();
		for (int i = 0; i < 50; i++)
		{
			stackPanel.Children.Add(new Button { Content = $"Button {i + 1}" });
		}

		scrollViewer.Content = stackPanel;

		var tabBarItem = CreateTabBarItem(TBIOnClickBehaviors.ScrollToTop);

		TabBarItem[] source = [tabBarItem];

		var tabBar = CreateTabBar(source);

		Grid.SetRow(scrollViewer, 0);
		Grid.SetRow(tabBar, 1);
		root.Children.Add(scrollViewer);
		root.Children.Add(tabBar);

		await UnitTestUIContentHelperEx.SetContentAndWait(root);

		scrollViewer.ChangeView(0, 100, null, false);
		await Task.Delay(_defaultDelayValue);
		// Ensure scrolled down
		Assert.AreEqual(100, scrollViewer.VerticalOffset);

		tabBarItem.ExecuteTap();
		await Task.Delay(_defaultDelayValue);
		Assert.AreEqual(0, scrollViewer.VerticalOffset);
	}

#if __IOS__ || __ANDROID__ || WINDOWS_WINUI
	[TestMethod]
	public async Task When_TabBarItem_ScrollToTop_ListView()
	{
		var root = CreateGrid();

		// Create a ListView with some items
		var listView = new ListView();
		listView.ItemsSource = Enumerable.Range(1, 50).Select(i => $"Item {i}");

		// Optional: Add some styling to make the list more scrollable
		listView.Height = 300;  // Set a fixed height for scrolling

		var tabBarItem = CreateTabBarItem(TBIOnClickBehaviors.ScrollToTop);

		TabBarItem[] source = [tabBarItem];

		var tabBar = CreateTabBar(source);

		Grid.SetRow(listView, 0);
		Grid.SetRow(tabBar, 1);
		root.Children.Add(listView);
		root.Children.Add(tabBar);

		await UnitTestUIContentHelperEx.SetContentAndWait(root);

		var item50 = listView.Items[49];
		listView.ScrollIntoView(item50);

		await Task.Delay(_defaultDelayValue);

		// Ensure the item is visible
		var isItemVisible = IsItemVisible(listView, item50);
		Assert.IsTrue(isItemVisible);

		tabBarItem.ExecuteTap();

		await Task.Delay(_defaultDelayValue);

		isItemVisible = IsItemVisible(listView, item50);

		Assert.IsFalse(isItemVisible);
	}
#endif

	#region Helper methods
	private static bool IsItemVisible(ListView list, object item)
	{
		var listViewItem = (ListViewItem)list.ContainerFromItem(item);
		return listViewItem != null;
	}

	private Grid CreateGrid()
	{
		var root = new Grid();
		root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
		root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		return root;
	}

	private static TabBarItem CreateTabBarItem(TBIOnClickBehaviors behavior)
	{
		var tabBarItem = new TabBarItem()
		{
			Content = "Tab 1"
		};

		SetOnClickBehaviors(tabBarItem, behavior);

		return tabBarItem;
	}

	private static TabBar CreateTabBar(TabBarItem[] source)
		=> new()
			{
				ItemsSource = source,
				SelectedIndex = 0
			};

	private static void NavigateToLastPage(Frame frame)
	{
		frame.Navigate(typeof(NavBarFirstPage));
		frame.Navigate(typeof(NavBarSecondPage));
		frame.Navigate(typeof(NavBarSimplePage));
	}
	#endregion
}
