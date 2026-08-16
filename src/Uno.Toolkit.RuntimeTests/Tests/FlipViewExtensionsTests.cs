using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Microsoft.UI.Xaml.Controls;

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class FlipViewExtensionsTests
{
	[TestMethod]
	public async Task When_Next_Button_Tapped_Advances_SelectedIndex()
	{
		// Arrange: FlipView with 3 items and Next buttons wired via FlipViewExtensions
		var flipView = new FlipView { Width = 300, Height = 200 };
		var btn1Next = new Button { Content = "Next" };
		var btn2Next = new Button { Content = "Next" };

		var grid1 = new Grid();
		grid1.Children.Add(btn1Next);
		var grid2 = new Grid();
		grid2.Children.Add(btn2Next);
		var grid3 = new Grid();

		FlipViewExtensions.SetNext(btn1Next, flipView);
		FlipViewExtensions.SetNext(btn2Next, flipView);

		flipView.Items.Add(grid1);
		flipView.Items.Add(grid2);
		flipView.Items.Add(grid3);

		await UnitTestUIContentHelperEx.SetContentAndWait(flipView);

		// Act & Assert: start at 0, advance to 2
		Assert.AreEqual(0, flipView.SelectedIndex);

		btn1Next.RaiseClick();
		await UnitTestUIContentHelperEx.WaitForIdle();
		Assert.AreEqual(1, flipView.SelectedIndex);

		btn2Next.RaiseClick();
		await UnitTestUIContentHelperEx.WaitForIdle();
		Assert.AreEqual(2, flipView.SelectedIndex);
	}

	[TestMethod]
	public async Task When_Previous_Button_Tapped_Returns_To_Start()
	{
		// Arrange: FlipView with 2 items, Next and Previous buttons
		var flipView = new FlipView { Width = 300, Height = 200 };
		var btnNext = new Button { Content = "Next" };
		var btnPrev = new Button { Content = "Previous" };

		var grid1 = new Grid();
		grid1.Children.Add(btnNext);
		var grid2 = new Grid();
		grid2.Children.Add(btnPrev);

		FlipViewExtensions.SetNext(btnNext, flipView);
		FlipViewExtensions.SetPrevious(btnPrev, flipView);

		flipView.Items.Add(grid1);
		flipView.Items.Add(grid2);

		await UnitTestUIContentHelperEx.SetContentAndWait(flipView);

		Assert.AreEqual(0, flipView.SelectedIndex);

		// Act: go forward then backward
		btnNext.RaiseClick();
		await UnitTestUIContentHelperEx.WaitForIdle();
		Assert.AreEqual(1, flipView.SelectedIndex);

		btnPrev.RaiseClick();
		await UnitTestUIContentHelperEx.WaitForIdle();
		Assert.AreEqual(0, flipView.SelectedIndex);
	}

	[TestMethod]
	public async Task When_PipsPager_Bound_NumberOfPages_Matches_Items()
	{
		// Arrange
		var grid = new Grid();
		var flipView = new FlipView { Width = 300, Height = 200 };
		var pipsPager = new Microsoft.UI.Xaml.Controls.PipsPager();

		grid.Children.Add(flipView);
		grid.Children.Add(pipsPager);

		SelectorExtensions.SetPipsPager(flipView, pipsPager);

		flipView.Items.Add(new Grid());
		flipView.Items.Add(new Grid());
		flipView.Items.Add(new Grid());

		await UnitTestUIContentHelperEx.SetContentAndWait(grid);

		// Assert: PipsPager reflects item count
		Assert.AreEqual(3, pipsPager.NumberOfPages);
	}

	[TestMethod]
	public async Task When_Item_Added_PipsPager_Updates()
	{
		// Arrange
		var grid = new Grid();
		var flipView = new FlipView { Width = 300, Height = 200 };
		var pipsPager = new Microsoft.UI.Xaml.Controls.PipsPager();

		grid.Children.Add(flipView);
		grid.Children.Add(pipsPager);

		SelectorExtensions.SetPipsPager(flipView, pipsPager);

		flipView.Items.Add(new Grid());
		flipView.Items.Add(new Grid());

		await UnitTestUIContentHelperEx.SetContentAndWait(grid);

		var initialCount = pipsPager.NumberOfPages;
		Assert.AreEqual(2, initialCount);

		// Act: add an item
		flipView.Items.Add(new Grid());
		await UnitTestUIContentHelperEx.WaitForIdle();

		// Assert: count updated
		Assert.AreEqual(3, pipsPager.NumberOfPages);
		Assert.AreNotEqual(initialCount, pipsPager.NumberOfPages);
	}

	[TestMethod]
	public async Task When_FlipView_Navigated_PipsPager_SelectedIndex_Matches()
	{
		// Arrange
		var grid = new Grid();
		var flipView = new FlipView { Width = 300, Height = 200 };
		var pipsPager = new Microsoft.UI.Xaml.Controls.PipsPager();
		var btnNext1 = new Button { Content = "Next" };
		var btnNext2 = new Button { Content = "Next" };

		FlipViewExtensions.SetNext(btnNext1, flipView);
		FlipViewExtensions.SetNext(btnNext2, flipView);

		var g1 = new Grid();
		g1.Children.Add(btnNext1);
		var g2 = new Grid();
		g2.Children.Add(btnNext2);
		var g3 = new Grid();

		flipView.Items.Add(g1);
		flipView.Items.Add(g2);
		flipView.Items.Add(g3);

		grid.Children.Add(flipView);
		grid.Children.Add(pipsPager);

		SelectorExtensions.SetPipsPager(flipView, pipsPager);

		await UnitTestUIContentHelperEx.SetContentAndWait(grid);

		// Assert initial sync
		Assert.AreEqual(flipView.SelectedIndex, pipsPager.SelectedPageIndex);

		// Act: navigate
		btnNext1.RaiseClick();
		await UnitTestUIContentHelperEx.WaitForIdle();
		btnNext2.RaiseClick();
		await UnitTestUIContentHelperEx.WaitForIdle();

		// Assert: SelectedPageIndex kept in sync
		Assert.AreEqual(2, flipView.SelectedIndex);
		Assert.AreEqual(flipView.SelectedIndex, pipsPager.SelectedPageIndex);
	}
}

internal static class ButtonTestExtensions
{
	/// <summary>
	/// Programmatically raises the Click event on a <see cref="Button"/>.
	/// </summary>
	public static void RaiseClick(this Button button)
	{
		// The Uno/WinUI ButtonAutomationPeer supports Invoke
		var peer = Microsoft.UI.Xaml.Automation.Peers.FrameworkElementAutomationPeer
			.CreatePeerForElement(button) as Microsoft.UI.Xaml.Automation.Peers.ButtonAutomationPeer;
		var invokeProvider = peer as Microsoft.UI.Xaml.Automation.Provider.IInvokeProvider;
		invokeProvider?.Invoke();
	}
}
