using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;

namespace Uno.Toolkit.RuntimeTests.Tests;

/// <summary>
/// Guards that the <see cref="NavigationBar"/> properties mirroring <see cref="CommandBar"/> ones reach the
/// inner <see cref="CommandBar"/>. <c>ClosedDisplayMode</c>, <c>DefaultLabelPosition</c> and
/// <c>OverflowButtonVisibility</c> used to be dropped, so the overflow button stayed visible even though
/// every NavigationBar style collapses it.
/// </summary>
public partial class NavigationBarTests
{
	[TestMethod]
	public async Task CommandBar_Gets_Default_Style_OverflowButtonVisibility()
	{
		var navigationBar = new NavigationBar { Content = "Title" };

		await UnitTestUIContentHelperEx.SetContentAndWait(navigationBar);

		var commandBar = GetInnerCommandBar(navigationBar);
		Assert.AreEqual(NavigationBarOverflowButtonVisibility.Collapsed, navigationBar.OverflowButtonVisibility);
		Assert.AreEqual(CommandBarOverflowButtonVisibility.Collapsed, commandBar.OverflowButtonVisibility);
	}

	[TestMethod]
	public async Task CommandBar_Properties_Forwarded_When_Set_Before_Load()
	{
		var navigationBar = new NavigationBar
		{
			Content = "Title",
			ClosedDisplayMode = NavigationBarClosedDisplayMode.Minimal,
			DefaultLabelPosition = NavigationBarDefaultLabelPosition.Right,
			OverflowButtonVisibility = NavigationBarOverflowButtonVisibility.Visible,
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(navigationBar);

		var commandBar = GetInnerCommandBar(navigationBar);
		Assert.AreEqual(AppBarClosedDisplayMode.Minimal, commandBar.ClosedDisplayMode);
		Assert.AreEqual(CommandBarDefaultLabelPosition.Right, commandBar.DefaultLabelPosition);
		Assert.AreEqual(CommandBarOverflowButtonVisibility.Visible, commandBar.OverflowButtonVisibility);
	}

	[TestMethod]
	public async Task CommandBar_Properties_Forwarded_When_Changed_After_Load()
	{
		var navigationBar = new NavigationBar { Content = "Title" };

		await UnitTestUIContentHelperEx.SetContentAndWait(navigationBar);
		var commandBar = GetInnerCommandBar(navigationBar);

		navigationBar.ClosedDisplayMode = NavigationBarClosedDisplayMode.Hidden;
		navigationBar.DefaultLabelPosition = NavigationBarDefaultLabelPosition.Collapsed;
		navigationBar.OverflowButtonVisibility = NavigationBarOverflowButtonVisibility.Auto;

		Assert.AreEqual(AppBarClosedDisplayMode.Hidden, commandBar.ClosedDisplayMode);
		Assert.AreEqual(CommandBarDefaultLabelPosition.Collapsed, commandBar.DefaultLabelPosition);
		Assert.AreEqual(CommandBarOverflowButtonVisibility.Auto, commandBar.OverflowButtonVisibility);
	}

	private static CommandBar GetInnerCommandBar(NavigationBar navigationBar)
		=> navigationBar.GetFirstDescendant<CommandBar>("XamlNavigationBarCommandBar")
			?? throw new AssertFailedException("The NavigationBar template has no inner CommandBar");
}
