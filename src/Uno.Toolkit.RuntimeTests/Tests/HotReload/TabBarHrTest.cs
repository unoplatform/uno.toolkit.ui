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
			// UpdateSourceFile completes once the UI is updated
			var tbDuring = UIHelper.GetChild<TabBar>(name: "TB");
			Assert.AreEqual("Updated marker", UIHelper.GetChild<TextBlock>(name: "Marker").Text, "Marker should be updated after HR.");
			Assert.AreEqual(1, tbDuring.SelectedIndex, "SelectedIndex should be preserved after HR.");
			Assert.AreEqual(3, tbDuring.Items.Count, "Items count should remain 3.");
		}

		// After dispose, UI is restored to original state
		var tbAfter = UIHelper.GetChild<TabBar>(name: "TB");
		Assert.AreEqual("Original marker", UIHelper.GetChild<TextBlock>(name: "Marker").Text, "Marker should be restored after dispose.");
		Assert.AreEqual(1, tbAfter.SelectedIndex, "SelectedIndex should be preserved after restoration.");
		Assert.AreEqual(3, tbAfter.Items.Count, "Items count should remain 3.");
	}

	/// <summary>
	/// Verifies that adding a new TabBarItem via XAML Hot Reload is reflected at runtime
	/// and the previously selected index is preserved.
	/// Uses TabBarPage4 for test isolation.
	/// </summary>
	[TestMethod]
	public async Task AddingTabBarItem_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new TabBarPage4(), ct);

		var tb = UIHelper.GetChild<TabBar>(name: "TB");

		Assert.AreEqual(3, tb.Items.Count, "Should start with 3 tabs.");

		// Select the second tab before HR
		tb.SelectedIndex = 1;

		// Add a fourth TabBarItem via XAML HR
		await using (await HotReloadHelper.UpdateSourceFile<TabBarPage4>(
			originalText: """"<utu:TabBarItem Content="Tab Three" />"""",
			replacementText:
				""""
				<utu:TabBarItem Content="Tab Three" />
				<utu:TabBarItem Content="Tab Four" />
				"""",
			ct))
		{
			var tbDuring = UIHelper.GetChild<TabBar>(name: "TB");
			Assert.AreEqual(4, tbDuring.Items.Count, "Should now have 4 tabs after HR.");
			Assert.AreEqual(1, tbDuring.SelectedIndex, "SelectedIndex should be preserved after adding item.");
		}

		// After dispose, UI is restored to original 3 tabs
		var tbAfter = UIHelper.GetChild<TabBar>(name: "TB");
		Assert.AreEqual(3, tbAfter.Items.Count, "Should be restored to 3 tabs after dispose.");
	}

	/// <summary>
	/// Verifies that modifying a TabBarItem's Content text via XAML Hot Reload is reflected at runtime.
	/// </summary>
	[TestMethod]
	public async Task ModifyTabBarItemContent_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new TabBarPage(), ct);

		var tb = UIHelper.GetChild<TabBar>(name: "TB");
		var firstItem = (TabBarItem)tb.Items[0];

		Assert.AreEqual("Tab One", firstItem.Content);

		// Change the first tab's label via XAML HR
		await using (await HotReloadHelper.UpdateSourceFile<TabBarPage>(
			originalText: "Content=\"Tab One\"",
			replacementText: "Content=\"Tab First\"",
			ct))
		{
			var tbDuring = UIHelper.GetChild<TabBar>(name: "TB");
			var firstItemDuring = (TabBarItem)tbDuring.Items[0];
			Assert.AreEqual("Tab First", firstItemDuring.Content as string, "Tab content should be updated after HR.");
			Assert.AreEqual(3, tbDuring.Items.Count, "Items count should remain 3.");
		}

		// After dispose, UI is restored to original content
		var tbAfter = UIHelper.GetChild<TabBar>(name: "TB");
		var firstItemAfter = (TabBarItem)tbAfter.Items[0];
		Assert.AreEqual("Tab One", firstItemAfter.Content as string, "Tab content should be restored after dispose.");
	}

	/// <summary>
	/// Verifies that changing TabBar Orientation via XAML Hot Reload is applied at runtime.
	/// </summary>
	[TestMethod]
	public async Task ChangeOrientation_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new TabBarPage(), ct);

		var tb = UIHelper.GetChild<TabBar>(name: "TB");

		// Default is Horizontal (not set in XAML, so it uses the DP default)
		Assert.AreEqual(Orientation.Horizontal, tb.Orientation);

		tb.SelectedIndex = 1;

		// Add Orientation="Vertical" to the TabBar via XAML HR
		await using (await HotReloadHelper.UpdateSourceFile<TabBarPage>(
			originalText: "<utu:TabBar x:Name=\"TB\" Grid.Row=\"1\" SelectedIndex=\"0\">",
			replacementText: "<utu:TabBar x:Name=\"TB\" Grid.Row=\"1\" SelectedIndex=\"0\" Orientation=\"Vertical\">",
			ct))
		{
			var tbDuring = UIHelper.GetChild<TabBar>(name: "TB");
			Assert.AreEqual(Orientation.Vertical, tbDuring.Orientation, "Orientation should be Vertical after HR.");
			Assert.AreEqual(1, tbDuring.SelectedIndex, "SelectedIndex should be preserved after orientation change.");
		}

		// After dispose, UI is restored to original Horizontal orientation
		var tbAfter = UIHelper.GetChild<TabBar>(name: "TB");
		Assert.AreEqual(Orientation.Horizontal, tbAfter.Orientation, "Orientation should be restored to Horizontal after dispose.");
		Assert.AreEqual(1, tbAfter.SelectedIndex, "SelectedIndex should be preserved after restoration.");
	}

	/// <summary>
	/// Verifies that when items are removed via XAML HR causing SelectedIndex to be out of range,
	/// the TabBar handles it gracefully (clamps or resets).
	/// Uses TabBarPage2 for test isolation.
	/// </summary>
	[TestMethod]
	public async Task SelectedIndex_OutOfRange_After_Remove_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new TabBarPage2(), ct);

		var tb = UIHelper.GetChild<TabBar>(name: "TB");

		Assert.AreEqual(3, tb.Items.Count, "Should start with 3 tabs.");

		// Select the last tab (index 2)
		tb.SelectedIndex = 2;
		Assert.AreEqual(2, tb.SelectedIndex);

		// Remove the last two TabBarItems, making index 2 out of range
		await using (await HotReloadHelper.UpdateSourceFile<TabBarPage2>(
			originalText: "<utu:TabBarItem Content=\"Tab Two\" />\n\t\t\t\t<utu:TabBarItem Content=\"Tab Three\" />",
			replacementText: "",
			ct))
		{
			var tbDuring = UIHelper.GetChild<TabBar>(name: "TB");
			Assert.AreEqual(1, tbDuring.Items.Count, "Should now have 1 tab after HR.");
			// SelectedIndex should be clamped to 0 since index 2 is no longer valid
			Assert.IsTrue(tbDuring.SelectedIndex >= 0 && tbDuring.SelectedIndex < tbDuring.Items.Count,
				$"SelectedIndex ({tbDuring.SelectedIndex}) should be within valid range [0, {tbDuring.Items.Count - 1}].");
		}

		// After dispose, UI is restored to original 3 tabs
		var tbAfter = UIHelper.GetChild<TabBar>(name: "TB");
		Assert.AreEqual(3, tbAfter.Items.Count, "Should be restored to 3 tabs after dispose.");
	}

	/// <summary>
	/// Verifies that reordering TabBarItems via XAML Hot Reload is reflected at runtime.
	/// Uses TabBarPage3 for test isolation.
	/// </summary>
	[TestMethod]
	public async Task ReorderTabBarItems_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new TabBarPage3(), ct);

		var tb = UIHelper.GetChild<TabBar>(name: "TB");

		Assert.AreEqual(3, tb.Items.Count, "Should start with 3 tabs.");
		Assert.AreEqual("Tab One", ((TabBarItem)tb.Items[0]).Content);
		Assert.AreEqual("Tab Two", ((TabBarItem)tb.Items[1]).Content);
		Assert.AreEqual("Tab Three", ((TabBarItem)tb.Items[2]).Content);

		// Reorder: move Tab Three to be first
		await using (await HotReloadHelper.UpdateSourceFile<TabBarPage3>(
			originalText: "<utu:TabBarItem Content=\"Tab One\" />\n\t\t\t\t<utu:TabBarItem Content=\"Tab Two\" />\n\t\t\t\t<utu:TabBarItem Content=\"Tab Three\" />",
			replacementText: "<utu:TabBarItem Content=\"Tab Three\" />\n\t\t\t\t<utu:TabBarItem Content=\"Tab One\" />\n\t\t\t\t<utu:TabBarItem Content=\"Tab Two\" />",
			ct))
		{
			var tbDuring = UIHelper.GetChild<TabBar>(name: "TB");
			Assert.AreEqual(3, tbDuring.Items.Count, "Should still have 3 tabs after reorder.");
			Assert.AreEqual("Tab Three", ((TabBarItem)tbDuring.Items[0]).Content as string, "First tab should now be Tab Three.");
			Assert.AreEqual("Tab One", ((TabBarItem)tbDuring.Items[1]).Content as string, "Second tab should now be Tab One.");
			Assert.AreEqual("Tab Two", ((TabBarItem)tbDuring.Items[2]).Content as string, "Third tab should now be Tab Two.");
		}

		// After dispose, UI is restored to original order
		var tbAfter = UIHelper.GetChild<TabBar>(name: "TB");
		Assert.AreEqual("Tab One", ((TabBarItem)tbAfter.Items[0]).Content as string, "First tab should be restored to Tab One.");
		Assert.AreEqual("Tab Two", ((TabBarItem)tbAfter.Items[1]).Content as string, "Second tab should be restored to Tab Two.");
		Assert.AreEqual("Tab Three", ((TabBarItem)tbAfter.Items[2]).Content as string, "Third tab should be restored to Tab Three.");
	}
}
#endif
