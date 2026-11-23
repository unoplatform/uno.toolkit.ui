using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using static Uno.Toolkit.UI.TabBarItemExtensions;

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
public class TabBarItemExtensionsTests
{
	// Smooth scrolling settles on a sub-pixel offset, so "at the top" needs a tolerance.
	private const double TopOffsetTolerance = 0.5;

	[TestMethod]
	[DataRow(typeof(ListView))]
	[DataRow(typeof(ScrollViewer))]
	public async Task When_Selected_Item_Clicked_ScrollToTop_Resets_ContentHost(Type contentHostType)
	{
		var contentHost = CreateScrollableContentHost(contentHostType);
		var selectedItem = CreateSelectedTabBarItem(TBIOnClickBehaviors.ScrollToTop, out var tabBar);

		// The content host is resolved from the TabBar's parent.
		var root = new StackPanel();
		root.Children.Add(contentHost);
		root.Children.Add(tabBar);

		await UnitTestUIContentHelperEx.SetContentAndWait(root);

		var scrollViewer = contentHost as ScrollViewer
			?? contentHost.GetFirstDescendant<ScrollViewer>()
			?? throw new AssertFailedException($"Failed to find the ScrollViewer of {contentHostType.Name}");

		await ScrollDown(scrollViewer);
		Assert.IsTrue(selectedItem.IsSelected);

		selectedItem.ExecuteTap();

		await UnitTestUIContentHelperEx.WaitFor(() => scrollViewer.VerticalOffset <= TopOffsetTolerance, timeoutMS: 3000, message: "The content host was not scrolled back to the top");
	}

	[TestMethod]
	[DataRow(TBIOnClickBehaviors.ScrollToTop, true)]
	[DataRow(TBIOnClickBehaviors.ScrollToTop, false)]
	[DataRow(TBIOnClickBehaviors.BackNavigation, true)]
	[DataRow(TBIOnClickBehaviors.BackNavigation, false)]
	[DataRow(TBIOnClickBehaviors.Auto, true)]
	[DataRow(TBIOnClickBehaviors.Auto, false)]
	public async Task When_Selected_Item_Clicked_Only_Requested_Behavior_Applies(TBIOnClickBehaviors behaviors, bool frameFirst)
	{
		// Both a Frame with a back stack and a scrolled ScrollViewer are candidates;
		// the order decides which one a depth-first search meets first.
		var frame = CreateFrameWithBackStack();
		var scrollViewer = (ScrollViewer)CreateScrollableContentHost(typeof(ScrollViewer));
		var selectedItem = CreateSelectedTabBarItem(behaviors, out var tabBar);

		var root = new StackPanel();
		root.Children.Add(frameFirst ? frame : scrollViewer);
		root.Children.Add(frameFirst ? scrollViewer : frame);
		root.Children.Add(tabBar);

		await UnitTestUIContentHelperEx.SetContentAndWait(root);
		await ScrollDown(scrollViewer);
		var initialDepth = frame.BackStackDepth;
		var scrolledOffset = scrollViewer.VerticalOffset;

		selectedItem.ExecuteTap();

		// Auto tries back navigation first, then falls back to scrolling.
		if (behaviors.HasFlag(TBIOnClickBehaviors.BackNavigation))
		{
			await UnitTestUIContentHelperEx.WaitFor(() => frame.BackStackDepth == initialDepth - 1, message: "The frame did not navigate back");
			await UnitTestsUIContentHelper.WaitForIdle();
			Assert.AreEqual(scrolledOffset, scrollViewer.VerticalOffset, "The ScrollViewer should not have been scrolled");
		}
		else
		{
			await UnitTestUIContentHelperEx.WaitFor(() => scrollViewer.VerticalOffset <= TopOffsetTolerance, timeoutMS: 3000, message: "The ScrollViewer was not scrolled back to the top");
			Assert.AreEqual(initialDepth, frame.BackStackDepth, "The frame should not have navigated back");
		}
	}

	[TestMethod]
	public async Task When_Auto_And_No_Back_Stack_Scrolls_To_Top()
	{
		var frame = CreateFrameWithBackStack();
		var scrollViewer = (ScrollViewer)CreateScrollableContentHost(typeof(ScrollViewer));
		var selectedItem = CreateSelectedTabBarItem(TBIOnClickBehaviors.Auto, out var tabBar);

		var root = new StackPanel();
		root.Children.Add(frame);
		root.Children.Add(scrollViewer);
		root.Children.Add(tabBar);

		await UnitTestUIContentHelperEx.SetContentAndWait(root);
		await ScrollDown(scrollViewer);

		// First click empties the back stack, second click has nothing to go back to.
		selectedItem.ExecuteTap();
		await UnitTestUIContentHelperEx.WaitFor(() => !frame.CanGoBack, message: "The frame did not navigate back");

		selectedItem.ExecuteTap();
		await UnitTestUIContentHelperEx.WaitFor(() => scrollViewer.VerticalOffset <= TopOffsetTolerance, timeoutMS: 3000, message: "The ScrollViewer was not scrolled back to the top");
	}

	private static TabBarItem CreateSelectedTabBarItem(TBIOnClickBehaviors behaviors, out TabBar tabBar)
	{
		var selectedItem = new TabBarItem { Content = "Tab 1" };
		SetOnClickBehaviors(selectedItem, behaviors);

		tabBar = new TabBar();
		tabBar.Items.Add(selectedItem);
		tabBar.Items.Add(new TabBarItem { Content = "Tab 2" });
		tabBar.SelectedIndex = 0;

		return selectedItem;
	}

	private static Frame CreateFrameWithBackStack()
	{
		var frame = new Frame { Height = 100 };
		frame.Navigate(typeof(Page));
		frame.Navigate(typeof(Page));

		return frame;
	}

	private static async Task ScrollDown(ScrollViewer scrollViewer)
	{
		scrollViewer.ChangeView(null, 500, null, disableAnimation: true);
		await UnitTestUIContentHelperEx.WaitFor(() => scrollViewer.VerticalOffset > 0, message: "The ScrollViewer did not scroll down");
	}

	private static FrameworkElement CreateScrollableContentHost(Type contentHostType)
	{
		if (contentHostType == typeof(ListView))
		{
			return new ListView
			{
				Height = 200,
				ItemsSource = Enumerable.Range(0, 100).ToArray(),
				ItemTemplate = XamlHelper.LoadXaml<DataTemplate>("""
					<DataTemplate>
						<TextBlock Height="40" Text="{Binding}" />
					</DataTemplate>
				"""),
			};
		}

		if (contentHostType == typeof(ScrollViewer))
		{
			return new ScrollViewer
			{
				Height = 200,
				Content = new Border { Height = 4000 },
			};
		}

		throw new ArgumentOutOfRangeException(nameof(contentHostType));
	}
}
