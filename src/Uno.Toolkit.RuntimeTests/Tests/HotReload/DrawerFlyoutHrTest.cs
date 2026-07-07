#if DEBUG
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Uno.Toolkit.RuntimeTests.Tests.TestPages;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;

namespace Uno.Toolkit.RuntimeTests.Tests.HotReload;

/// <summary>
/// Hot Reload regression tests for DrawerFlyoutPresenter property edits (#1577).
/// </summary>
[TestClass]
[RunsInSecondaryApp(ignoreIfNotSupported: true)]
public class DrawerFlyoutHrTest
{
	[TestInitialize]
	public void Setup()
	{
		HotReloadHelper.DefaultWorkspaceTimeout = TimeSpan.FromSeconds(180);
		HotReloadHelper.DefaultMetadataUpdateTimeout = TimeSpan.FromSeconds(60);
	}

	/// <summary>
	/// Verifies that changing DrawerFlyoutPresenter.OpenDirection via XAML HR is applied.
	/// </summary>
	[TestMethod]
	public async Task ChangeOpenDirection_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new DrawerFlyoutPage(), ct);

		var btn = UIHelper.GetChild<Button>(name: "OpenButton");
		var flyout = (Flyout)btn.Flyout!;

		Assert.AreEqual(DrawerOpenDirection.Left, DrawerFlyoutPresenter.GetOpenDirection(flyout));

		await using (await HotReloadHelper.UpdateSourceFile<DrawerFlyoutPage>(
			originalText: "OpenDirection=\"Left\"",
			replacementText: "OpenDirection=\"Right\"",
			ct))
		{
			var btnDuring = UIHelper.GetChild<Button>(name: "OpenButton");
			var flyoutDuring = (Flyout)btnDuring.Flyout!;
			Assert.AreEqual(DrawerOpenDirection.Right, DrawerFlyoutPresenter.GetOpenDirection(flyoutDuring),
				"OpenDirection should be Right after HR.");
		}

		var btnAfter = UIHelper.GetChild<Button>(name: "OpenButton");
		var flyoutAfter = (Flyout)btnAfter.Flyout!;
		Assert.AreEqual(DrawerOpenDirection.Left, DrawerFlyoutPresenter.GetOpenDirection(flyoutAfter),
			"OpenDirection should be restored to Left after dispose.");
	}

	/// <summary>
	/// Verifies that changing DrawerFlyoutPresenter.DrawerLength via XAML HR is applied.
	/// </summary>
	[TestMethod]
	public async Task ChangeDrawerLength_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new DrawerFlyoutPage(), ct);

		var btn = UIHelper.GetChild<Button>(name: "OpenButton");
		var flyout = (Flyout)btn.Flyout!;

		var length = DrawerFlyoutPresenter.GetDrawerLength(flyout);
		Assert.AreEqual(300d, length.Value, "DrawerLength should start at 300.");

		await using (await HotReloadHelper.UpdateSourceFile<DrawerFlyoutPage>(
			originalText: "DrawerLength=\"300\"",
			replacementText: "DrawerLength=\"400\"",
			ct))
		{
			var btnDuring = UIHelper.GetChild<Button>(name: "OpenButton");
			var flyoutDuring = (Flyout)btnDuring.Flyout!;
			var lengthDuring = DrawerFlyoutPresenter.GetDrawerLength(flyoutDuring);
			Assert.AreEqual(400d, lengthDuring.Value, "DrawerLength should be 400 after HR.");
		}

		var btnAfter = UIHelper.GetChild<Button>(name: "OpenButton");
		var flyoutAfter = (Flyout)btnAfter.Flyout!;
		var lengthAfter = DrawerFlyoutPresenter.GetDrawerLength(flyoutAfter);
		Assert.AreEqual(300d, lengthAfter.Value, "DrawerLength should be restored to 300 after dispose.");
	}

	/// <summary>
	/// Verifies that changing DrawerFlyoutPresenter.IsGestureEnabled via XAML HR is applied.
	/// </summary>
	[TestMethod]
	public async Task ChangeIsGestureEnabled_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new DrawerFlyoutPage(), ct);

		var btn = UIHelper.GetChild<Button>(name: "OpenButton");
		var flyout = (Flyout)btn.Flyout!;

		Assert.IsTrue(DrawerFlyoutPresenter.GetIsGestureEnabled(flyout), "IsGestureEnabled should start True.");

		await using (await HotReloadHelper.UpdateSourceFile<DrawerFlyoutPage>(
			originalText: "IsGestureEnabled=\"True\"",
			replacementText: "IsGestureEnabled=\"False\"",
			ct))
		{
			var btnDuring = UIHelper.GetChild<Button>(name: "OpenButton");
			var flyoutDuring = (Flyout)btnDuring.Flyout!;
			Assert.IsFalse(DrawerFlyoutPresenter.GetIsGestureEnabled(flyoutDuring),
				"IsGestureEnabled should be False after HR.");
		}

		var btnAfter = UIHelper.GetChild<Button>(name: "OpenButton");
		var flyoutAfter = (Flyout)btnAfter.Flyout!;
		Assert.IsTrue(DrawerFlyoutPresenter.GetIsGestureEnabled(flyoutAfter),
			"IsGestureEnabled should be restored to True after dispose.");
	}
}
#endif
