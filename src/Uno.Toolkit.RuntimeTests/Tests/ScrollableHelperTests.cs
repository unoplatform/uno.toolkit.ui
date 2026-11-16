using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
public class ScrollableHelperTests
{
	// Smooth scrolling settles on a sub-pixel offset, so "at the top" needs a tolerance.
	private const double TopOffsetTolerance = 0.5;

	[TestMethod]
	public async Task When_SmoothScrollTop_ListView_Scrolls_To_Start()
	{
		var (listView, scrollViewer) = await SetupListView();

		scrollViewer.ChangeView(null, 500, null, disableAnimation: true);
		await UnitTestUIContentHelperEx.WaitFor(() => scrollViewer.VerticalOffset > 0, message: "The ListView did not scroll down");

		ScrollableHelper.SmoothScrollTop(listView);

		await UnitTestUIContentHelperEx.WaitFor(() => scrollViewer.VerticalOffset <= TopOffsetTolerance, timeoutMS: 3000, message: "The ListView was not scrolled back to the top");
	}

	[TestMethod]
	public async Task When_SmoothScrollBottom_ListView_Scrolls_To_End()
	{
		var (listView, scrollViewer) = await SetupListView();
		Assert.AreEqual(0, scrollViewer.VerticalOffset);

		ScrollableHelper.SmoothScrollBottom(listView);

		await UnitTestUIContentHelperEx.WaitFor(
			() => scrollViewer.VerticalOffset > 0 && scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight - 1,
			timeoutMS: 3000,
			message: "The ListView was not scrolled to the bottom");
	}

	private static async Task<(ListView ListView, ScrollViewer ScrollViewer)> SetupListView()
	{
		var listView = new ListView
		{
			Height = 200,
			ItemsSource = Enumerable.Range(0, 100).ToArray(),
			ItemTemplate = XamlHelper.LoadXaml<DataTemplate>("""
				<DataTemplate>
					<TextBlock Height="40" Text="{Binding}" />
				</DataTemplate>
			"""),
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(listView);

		var scrollViewer = listView.GetFirstDescendant<ScrollViewer>()
			?? throw new AssertFailedException("Failed to find the ScrollViewer of the ListView");

		return (listView, scrollViewer);
	}
}
