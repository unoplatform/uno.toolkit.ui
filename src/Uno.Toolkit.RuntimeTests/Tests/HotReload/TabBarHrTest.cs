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

[TestClass]
[RunsInSecondaryApp(ignoreIfNotSupported: true)]
public class TabBarHrTest
{
	[TestInitialize]
	public void Setup()
	{
		HotReloadHelper.DefaultWorkspaceTimeout = TimeSpan.FromSeconds(180);
		HotReloadHelper.DefaultMetadataUpdateTimeout = TimeSpan.FromSeconds(60);
	}

	/// <summary>
	/// Verifies that the selected tab index is preserved after a Hot Reload update.
	/// This exercises the TabBarElementMetadataUpdateHandler.CaptureState/RestoreState flow.
	/// </summary>
	[TestMethod]
	public async Task SelectedIndex_Preserved_After_XamlChange_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new TabBarPage(), ct);

		var tb = UIHelper.GetChild<TabBar>(name: "TB");
		var marker = UIHelper.GetChild<TextBlock>(name: "Marker");

		// Preconditions
		Assert.AreEqual(0, tb.SelectedIndex, "SelectedIndex should start at 0.");
		Assert.AreEqual(3, tb.Items.Count, "TabBar should have 3 items initially.");
		Assert.AreEqual("Original marker", marker.Text);

		// Move selection to the second tab
		tb.SelectedIndex = 1;
		Assert.AreEqual(1, tb.SelectedIndex);

		// Trigger HR by changing the marker text in XAML
		await using (await HotReloadHelper.UpdateSourceFile<TabBarPage>(
			originalText: "Original marker",
			replacementText: "Updated marker",
			ct))
		{
			// Wait for the XAML change to propagate
			await TestHelper.WaitFor(
				() => UIHelper.GetChild<TextBlock>(name: "Marker").Text == "Updated marker", ct);
		}

		var tbAfter = UIHelper.GetChild<TabBar>(name: "TB");

		// The HR handler should have restored SelectedIndex
		Assert.AreEqual(1, tbAfter.SelectedIndex, "SelectedIndex should be preserved after HR.");
		Assert.AreEqual(3, tbAfter.Items.Count, "Items count should remain 3.");
	}

	/// <summary>
	/// Verifies that adding a new TabBarItem via XAML Hot Reload is reflected at runtime
	/// and the previously selected index is preserved.
	/// </summary>
	[TestMethod]
	public async Task AddingTabBarItem_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new TabBarPage(), ct);

		var tb = UIHelper.GetChild<TabBar>(name: "TB");

		Assert.AreEqual(3, tb.Items.Count, "Should start with 3 tabs.");

		// Select the second tab before HR
		tb.SelectedIndex = 1;

		// Add a fourth TabBarItem via XAML HR
		await using (await HotReloadHelper.UpdateSourceFile<TabBarPage>(
			originalText: """"<utu:TabBarItem Content="Tab Three" />"""",
			replacementText:
				""""
				<utu:TabBarItem Content="Tab Three" />
				<utu:TabBarItem Content="Tab Four" />
				"""",
			ct))
		{
			await TestHelper.WaitFor(
				() => UIHelper.GetChild<TabBar>(name: "TB").Items.Count == 4, ct);
		}

		var tbAfter = UIHelper.GetChild<TabBar>(name: "TB");

		Assert.AreEqual(4, tbAfter.Items.Count, "Should now have 4 tabs after HR.");
		Assert.AreEqual(1, tbAfter.SelectedIndex, "SelectedIndex should be preserved after adding item.");
	}

	/// <summary>
	/// Verifies that removing a TabBarItem via XAML Hot Reload is reflected at runtime
	/// and selection adjusts appropriately.
	/// </summary>
	[TestMethod]
	public async Task RemovingTabBarItem_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new TabBarPage(), ct);

		var tb = UIHelper.GetChild<TabBar>(name: "TB");

		Assert.AreEqual(3, tb.Items.Count, "Should start with 3 tabs.");

		// Select the first tab
		tb.SelectedIndex = 0;

		// Remove the third TabBarItem via XAML HR
		await using (await HotReloadHelper.UpdateSourceFile<TabBarPage>(
			originalText: "<utu:TabBarItem Content=\"Tab Three\" />",
			replacementText: "",
			ct))
		{
			await TestHelper.WaitFor(
				() => UIHelper.GetChild<TabBar>(name: "TB").Items.Count == 2, ct);
		}

		var tbAfter = UIHelper.GetChild<TabBar>(name: "TB");

		Assert.AreEqual(2, tbAfter.Items.Count, "Should now have 2 tabs after HR.");
		Assert.AreEqual(0, tbAfter.SelectedIndex, "SelectedIndex 0 should still be valid.");
	}
}
#endif
