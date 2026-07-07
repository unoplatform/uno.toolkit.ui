#if DEBUG
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uno.Toolkit.RuntimeTests.Tests.TestPages;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using static Uno.Toolkit.UI.TabBarItemExtensions;

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

			// Validate items are actually laid out vertically (stacked on Y axis)
			var item0 = (TabBarItem)tbDuring.Items[0];
			var item1 = (TabBarItem)tbDuring.Items[1];
			var pos0 = item0.TransformToVisual(tbDuring).TransformPoint(new Windows.Foundation.Point(0, 0));
			var pos1 = item1.TransformToVisual(tbDuring).TransformPoint(new Windows.Foundation.Point(0, 0));
			Assert.AreEqual(pos0.X, pos1.X, 1, "Vertical items should have the same X position.");
			Assert.IsTrue(pos1.Y > pos0.Y, "Second item should be below first when Vertical.");

			// Validate ActualSize reflects vertical layout (height > single item height)
			Assert.IsTrue(tbDuring.ActualHeight > item0.ActualHeight, "TabBar ActualHeight should exceed a single item's height when Vertical.");
			Assert.IsTrue(item0.ActualWidth > 0, "Item should have non-zero width when rendered vertically.");
		}

		// After dispose, UI is restored to original Horizontal orientation
		var tbAfter = UIHelper.GetChild<TabBar>(name: "TB");
		Assert.AreEqual(Orientation.Horizontal, tbAfter.Orientation, "Orientation should be restored to Horizontal after dispose.");
		Assert.AreEqual(1, tbAfter.SelectedIndex, "SelectedIndex should be preserved after restoration.");

		// Validate items are laid out horizontally again
		var itemA = (TabBarItem)tbAfter.Items[0];
		var itemB = (TabBarItem)tbAfter.Items[1];
		var posA = itemA.TransformToVisual(tbAfter).TransformPoint(new Windows.Foundation.Point(0, 0));
		var posB = itemB.TransformToVisual(tbAfter).TransformPoint(new Windows.Foundation.Point(0, 0));
		Assert.AreEqual(posA.Y, posB.Y, 1, "Horizontal items should have the same Y position.");
		Assert.IsTrue(posB.X > posA.X, "Second item should be to the right of first when Horizontal.");
	}

	/// <summary>
	/// Verifies that removing the entire TabBar via XAML Hot Reload is reflected at runtime,
	/// and that the TabBar is restored after dispose.
	/// Uses TabBarPage2 for test isolation.
	/// </summary>
	[TestMethod]
	public async Task RemoveTabBar_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new TabBarPage2(), ct);

		var tb = UIHelper.GetChild<TabBar>(name: "TB");

		Assert.AreEqual(3, tb.Items.Count, "Should start with 3 tabs.");

		// Select the last tab (index 2)
		tb.SelectedIndex = 2;
		Assert.AreEqual(2, tb.SelectedIndex);

		// Build the expected text with platform-correct line endings (CRLF on Windows, LF on Linux)
		var nl = Environment.NewLine;
		var tabBarXaml = $"<utu:TabBar x:Name=\"TB\" Grid.Row=\"1\" SelectedIndex=\"0\">{nl}\t\t\t<utu:TabBar.Items>{nl}\t\t\t\t<utu:TabBarItem Content=\"Tab One\" />{nl}\t\t\t\t<utu:TabBarItem Content=\"Tab Two\" />{nl}\t\t\t\t<utu:TabBarItem Content=\"Tab Three\" />{nl}\t\t\t</utu:TabBar.Items>{nl}\t\t</utu:TabBar>";

		// Remove the entire TabBar element, replacing it with a comment placeholder
		await using (await HotReloadHelper.UpdateSourceFile<TabBarPage2>(
			originalText: tabBarXaml,
			replacementText: "<!-- Removed_TabBar -->",
			ct))
		{
			var tbDuring = UIHelper.GetChildren<TabBar>().FirstOrDefault(t => t.Name == "TB");
			Assert.IsNull(tbDuring, "TabBar should be removed after HR.");
		}

		// After dispose, UI is restored — TabBar should be back
		var tbAfter = UIHelper.GetChild<TabBar>(name: "TB");
		Assert.IsNotNull(tbAfter, "TabBar should be restored after dispose.");
		Assert.AreEqual(3, tbAfter.Items.Count, "Should be restored to 3 tabs after dispose.");
	}

	/// <summary>
	/// Baseline test: verifies that removing and restoring a plain Button via HR works.
	/// If this passes but RemoveTabBar fails, the issue is TabBar-specific.
	/// Uses ButtonTestPage for test isolation.
	/// </summary>
	[TestMethod]
	public async Task RemoveButton_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new ButtonTestPage(), ct);

		var btn = UIHelper.GetChild<Button>(name: "TestButton");
		Assert.AreEqual("Click Me", btn.Content as string, "Button should start with expected content.");

		// Remove the Button, replacing it with a comment placeholder
		await using (await HotReloadHelper.UpdateSourceFile<ButtonTestPage>(
			originalText: "<Button x:Name=\"TestButton\" Grid.Row=\"1\" Content=\"Click Me\" />",
			replacementText: "<!-- Removed_Button -->",
			ct))
		{
			var btnDuring = UIHelper.GetChildren<Button>().FirstOrDefault(b => b.Name == "TestButton");
			Assert.IsNull(btnDuring, "Button should be removed after HR.");
		}

		// After dispose, UI is restored — Button should be back
		var btnAfter = UIHelper.GetChild<Button>(name: "TestButton");
		Assert.IsNotNull(btnAfter, "Button should be restored after dispose.");
		Assert.AreEqual("Click Me", btnAfter.Content as string, "Button content should be restored.");
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

		// Build the expected text with platform-correct line endings (CRLF on Windows, LF on Linux)
		var nl = Environment.NewLine;
		var originalItems = $"<utu:TabBarItem Content=\"Tab One\" />{nl}\t\t\t\t<utu:TabBarItem Content=\"Tab Two\" />{nl}\t\t\t\t<utu:TabBarItem Content=\"Tab Three\" />";
		var reorderedItems = $"<utu:TabBarItem Content=\"Tab Three\" />{nl}\t\t\t\t<utu:TabBarItem Content=\"Tab One\" />{nl}\t\t\t\t<utu:TabBarItem Content=\"Tab Two\" />";

		// Reorder: move Tab Three to be first
		await using (await HotReloadHelper.UpdateSourceFile<TabBarPage3>(
			originalText: originalItems,
			replacementText: reorderedItems,
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

	/// <summary>
	/// Verifies that adding OnClickBehaviors to a TabBarItem via XAML Hot Reload
	/// causes the attached property to be set correctly.
	/// Uses TabBarPage5 — Tab2 starts without OnClickBehaviors.
	/// </summary>
	[TestMethod]
	public async Task AddOnClickBehaviors_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new TabBarPage5(), ct);

		var tb = UIHelper.GetChild<TabBar>(name: "TB");
		var tab2 = (TabBarItem)tb.Items[1];
		Assert.AreEqual(TBIOnClickBehaviors.None, GetOnClickBehaviors(tab2), "Tab2 should start without OnClickBehaviors.");

		// Add OnClickBehaviors="ScrollToTop" to Tab2 via HR
		await using (await HotReloadHelper.UpdateSourceFile<TabBarPage5>(
			originalText: """<utu:TabBarItem Content="Tab Two" />""",
			replacementText: """<utu:TabBarItem Content="Tab Two" utu:TabBarItemExtensions.OnClickBehaviors="ScrollToTop" />""",
			ct))
		{
			var tbDuring = UIHelper.GetChild<TabBar>(name: "TB");
			var tab2During = (TabBarItem)tbDuring.Items[1];
			Assert.AreEqual(TBIOnClickBehaviors.ScrollToTop, GetOnClickBehaviors(tab2During), "Tab2 should have ScrollToTop after HR add.");
		}

		// After dispose, Tab2 should be restored without OnClickBehaviors
		var tbAfter2 = UIHelper.GetChild<TabBar>(name: "TB");
		var tab2After = (TabBarItem)tbAfter2.Items[1];
		Assert.AreEqual(TBIOnClickBehaviors.None, GetOnClickBehaviors(tab2After), "Tab2 should be restored without OnClickBehaviors.");
	}

	/// <summary>
	/// Verifies that removing OnClickBehaviors from a TabBarItem via XAML Hot Reload
	/// causes the attached property to revert to its default value (None).
	/// Uses TabBarPage5 — Tab1 starts with OnClickBehaviors="ScrollToTop".
	/// </summary>
	[TestMethod]
	public async Task RemoveOnClickBehaviors_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new TabBarPage5(), ct);

		var tb = UIHelper.GetChild<TabBar>(name: "TB");
		var tab1 = (TabBarItem)tb.Items[0];
		Assert.AreEqual(TBIOnClickBehaviors.ScrollToTop, GetOnClickBehaviors(tab1), "Tab1 should start with ScrollToTop.");

		// Remove OnClickBehaviors from Tab1 via HR
		await using (await HotReloadHelper.UpdateSourceFile<TabBarPage5>(
			originalText: """<utu:TabBarItem Content="Tab One" utu:TabBarItemExtensions.OnClickBehaviors="ScrollToTop" />""",
			replacementText: """<utu:TabBarItem Content="Tab One" />""",
			ct))
		{
			var tbDuring = UIHelper.GetChild<TabBar>(name: "TB");
			var tab1During = (TabBarItem)tbDuring.Items[0];
			Assert.AreEqual(TBIOnClickBehaviors.None, GetOnClickBehaviors(tab1During), "Tab1 should have None after HR remove.");
		}

		// After dispose, Tab1 should be restored with ScrollToTop
		var tbAfter1 = UIHelper.GetChild<TabBar>(name: "TB");
		var tab1After = (TabBarItem)tbAfter1.Items[0];
		Assert.AreEqual(TBIOnClickBehaviors.ScrollToTop, GetOnClickBehaviors(tab1After), "Tab1 should be restored with ScrollToTop.");
	}

	/// <summary>
	/// Verifies that editing OnClickBehaviors on a TabBarItem via XAML Hot Reload
	/// changes the attached property value correctly.
	/// Uses TabBarPage5 — Tab1 starts with OnClickBehaviors="ScrollToTop".
	/// </summary>
	[TestMethod]
	public async Task EditOnClickBehaviors_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new TabBarPage5(), ct);

		var tb = UIHelper.GetChild<TabBar>(name: "TB");
		var tab1 = (TabBarItem)tb.Items[0];
		Assert.AreEqual(TBIOnClickBehaviors.ScrollToTop, GetOnClickBehaviors(tab1), "Tab1 should start with ScrollToTop.");

		// Change ScrollToTop -> BackNavigation via HR
		await using (await HotReloadHelper.UpdateSourceFile<TabBarPage5>(
			originalText: "utu:TabBarItemExtensions.OnClickBehaviors=\"ScrollToTop\"",
			replacementText: "utu:TabBarItemExtensions.OnClickBehaviors=\"BackNavigation\"",
			ct))
		{
			var tbDuring = UIHelper.GetChild<TabBar>(name: "TB");
			var tab1During = (TabBarItem)tbDuring.Items[0];
			Assert.AreEqual(TBIOnClickBehaviors.BackNavigation, GetOnClickBehaviors(tab1During), "Tab1 should have BackNavigation after HR edit.");
		}

		// After dispose, Tab1 should be restored with ScrollToTop
		var tbAfter1 = UIHelper.GetChild<TabBar>(name: "TB");
		var tab1After = (TabBarItem)tbAfter1.Items[0];
		Assert.AreEqual(TBIOnClickBehaviors.ScrollToTop, GetOnClickBehaviors(tab1After), "Tab1 should be restored with ScrollToTop.");
	}

	/// <summary>
	/// Verifies that removing a specific TabBarItem via XAML Hot Reload is reflected at runtime
	/// and selection state is adjusted. Simulates responsive hiding of a tab.
	/// Uses TabBarPage4 (3 tabs).
	/// </summary>
	[TestMethod]
	public async Task RemoveSpecificTabBarItem_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new TabBarPage4(), ct);

		var tb = UIHelper.GetChild<TabBar>(name: "TB");
		Assert.AreEqual(3, tb.Items.Count, "Should start with 3 tabs.");

		// Select the second tab
		tb.SelectedIndex = 1;

		// Remove the middle tab (Tab Two) via HR — simulates responsive hide
		var nl = Environment.NewLine;
		var originalItems = $"<utu:TabBarItem Content=\"Tab One\" />{nl}\t\t\t\t<utu:TabBarItem Content=\"Tab Two\" />{nl}\t\t\t\t<utu:TabBarItem Content=\"Tab Three\" />";
		var reducedItems = $"<utu:TabBarItem Content=\"Tab One\" />{nl}\t\t\t\t<utu:TabBarItem Content=\"Tab Three\" />";

		await using (await HotReloadHelper.UpdateSourceFile<TabBarPage4>(
			originalText: originalItems,
			replacementText: reducedItems,
			ct))
		{
			var tbDuring = UIHelper.GetChild<TabBar>(name: "TB");
			Assert.AreEqual(2, tbDuring.Items.Count, "Should have 2 tabs after removing Tab Two.");
			Assert.AreEqual("Tab One", ((TabBarItem)tbDuring.Items[0]).Content as string);
			Assert.AreEqual("Tab Three", ((TabBarItem)tbDuring.Items[1]).Content as string);
		}

		// After dispose, UI is restored to 3 tabs
		var tbAfter = UIHelper.GetChild<TabBar>(name: "TB");
		Assert.AreEqual(3, tbAfter.Items.Count, "Should be restored to 3 tabs.");
		Assert.AreEqual("Tab Two", ((TabBarItem)tbAfter.Items[1]).Content as string);
	}

	/// <summary>
	/// Verifies that adding multiple TabBarItems at once via XAML Hot Reload works.
	/// Simulates a responsive breakpoint adding extra navigation tabs.
	/// Uses TabBarPage4 (3 tabs).
	/// </summary>
	[TestMethod]
	public async Task AddMultipleTabBarItems_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new TabBarPage4(), ct);

		var tb = UIHelper.GetChild<TabBar>(name: "TB");
		Assert.AreEqual(3, tb.Items.Count, "Should start with 3 tabs.");

		// Add two tabs at once via HR — simulates responsive showing of extra tabs
		await using (await HotReloadHelper.UpdateSourceFile<TabBarPage4>(
			originalText: """"<utu:TabBarItem Content="Tab Three" />"""",
			replacementText:
				""""
				<utu:TabBarItem Content="Tab Three" />
				<utu:TabBarItem Content="Tab Four" />
				<utu:TabBarItem Content="Tab Five" />
				"""",
			ct))
		{
			var tbDuring = UIHelper.GetChild<TabBar>(name: "TB");
			Assert.AreEqual(5, tbDuring.Items.Count, "Should have 5 tabs after adding two.");
			Assert.AreEqual("Tab Four", ((TabBarItem)tbDuring.Items[3]).Content as string);
			Assert.AreEqual("Tab Five", ((TabBarItem)tbDuring.Items[4]).Content as string);
		}

		// After dispose, UI is restored to 3 tabs
		var tbAfter = UIHelper.GetChild<TabBar>(name: "TB");
		Assert.AreEqual(3, tbAfter.Items.Count, "Should be restored to 3 tabs.");
	}

	/// <summary>
	/// Verifies that adding an entire TabBar to a page that starts without one via XAML HR works.
	/// Simulates responsive late-add of a TabBar at a narrow breakpoint.
	/// Uses TabBarPage6 (starts with no TabBar).
	/// </summary>
	[TestMethod]
	public async Task LateAddTabBar_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new TabBarPage6(), ct);

		// Verify no TabBar initially
		var tbBefore = UIHelper.GetChildren<TabBar>().FirstOrDefault();
		Assert.IsNull(tbBefore, "Should start with no TabBar.");

		var placeholder = UIHelper.GetChild<TextBlock>(name: "Placeholder");
		Assert.AreEqual("No TabBar yet", placeholder.Text);

		// Add a complete TabBar via HR — simulates responsive late-add
		await using (await HotReloadHelper.UpdateSourceFile<TabBarPage6>(
			originalText: """<TextBlock x:Name="Placeholder" Text="No TabBar yet" />""",
			replacementText: """
				<Grid.RowDefinitions>
					<RowDefinition Height="Auto" />
					<RowDefinition Height="Auto" />
				</Grid.RowDefinitions>
				<TextBlock x:Name="Placeholder" Text="TabBar added" />
				<utu:TabBar x:Name="TB" Grid.Row="1" SelectedIndex="0">
					<utu:TabBar.Items>
						<utu:TabBarItem Content="Home" />
						<utu:TabBarItem Content="Settings" />
					</utu:TabBar.Items>
				</utu:TabBar>
			""",
			ct))
		{
			var tbDuring = UIHelper.GetChild<TabBar>(name: "TB");
			Assert.IsNotNull(tbDuring, "TabBar should exist after HR late-add.");
			Assert.AreEqual(2, tbDuring.Items.Count, "Should have 2 tabs.");
			Assert.AreEqual("Home", ((TabBarItem)tbDuring.Items[0]).Content as string);
			Assert.AreEqual("Settings", ((TabBarItem)tbDuring.Items[1]).Content as string);

			var placeholderDuring = UIHelper.GetChild<TextBlock>(name: "Placeholder");
			Assert.AreEqual("TabBar added", placeholderDuring.Text);
		}

		// After dispose, UI is restored — no TabBar, placeholder text reset
		var tbAfterRestore = UIHelper.GetChildren<TabBar>().FirstOrDefault();
		Assert.IsNull(tbAfterRestore, "TabBar should be removed after dispose.");
		var placeholderAfter = UIHelper.GetChild<TextBlock>(name: "Placeholder");
		Assert.AreEqual("No TabBar yet", placeholderAfter.Text);
	}
}
#endif
