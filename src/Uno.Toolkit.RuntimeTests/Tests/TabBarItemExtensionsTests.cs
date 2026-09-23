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
internal class TabBarItemExtensionsTests
{
	// Smooth scrolling settles on a sub-pixel offset, so "at the top" needs a tolerance.
	private const double TopOffsetTolerance = 0.5;

	[TestMethod]
	[DataRow(typeof(ListView))]
	[DataRow(typeof(ScrollViewer))]
	public async Task When_Selected_Item_Clicked_ScrollToTop_Resets_ContentHost(Type contentHostType)
	{
		var contentHost = CreateScrollableContentHost(contentHostType);
		var selectedItem = new TabBarItem { Content = "Tab 1" };
		SetOnClickBehaviors(selectedItem, TBIOnClickBehaviors.ScrollToTop);

		var tabBar = new TabBar();
		tabBar.Items.Add(selectedItem);
		tabBar.Items.Add(new TabBarItem { Content = "Tab 2" });
		tabBar.SelectedIndex = 0;

		// The content host is resolved from the TabBar's parent.
		var root = new StackPanel();
		root.Children.Add(contentHost);
		root.Children.Add(tabBar);

		await UnitTestUIContentHelperEx.SetContentAndWait(root);

		var scrollViewer = contentHost as ScrollViewer
			?? contentHost.GetFirstDescendant<ScrollViewer>()
			?? throw new AssertFailedException($"Failed to find the ScrollViewer of {contentHostType.Name}");

		scrollViewer.ChangeView(null, 500, null, disableAnimation: true);
		await UnitTestUIContentHelperEx.WaitFor(() => scrollViewer.VerticalOffset > 0, message: "The content host did not scroll down");
		Assert.IsTrue(selectedItem.IsSelected);

		selectedItem.ExecuteTap();

		await UnitTestUIContentHelperEx.WaitFor(() => scrollViewer.VerticalOffset <= TopOffsetTolerance, timeoutMS: 3000, message: "The content host was not scrolled back to the top");
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
