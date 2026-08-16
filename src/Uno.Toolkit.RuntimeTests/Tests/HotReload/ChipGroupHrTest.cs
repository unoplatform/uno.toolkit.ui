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
/// Hot Reload regression tests for ChipGroup property edits (#1576).
/// </summary>
[TestClass]
[RunsInSecondaryApp(ignoreIfNotSupported: true)]
public class ChipGroupHrTest
{
	[TestInitialize]
	public void Setup()
	{
		HotReloadHelper.DefaultWorkspaceTimeout = TimeSpan.FromSeconds(180);
		HotReloadHelper.DefaultMetadataUpdateTimeout = TimeSpan.FromSeconds(60);
	}

	/// <summary>
	/// Verifies that changing ChipGroup.SelectionMode via XAML HR is applied.
	/// </summary>
	[TestMethod]
	public async Task ChangeSelectionMode_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new ChipGroupPage(), ct);

		var cg = UIHelper.GetChild<ChipGroup>(name: "CG");
		Assert.AreEqual(ChipSelectionMode.Single, cg.SelectionMode, "SelectionMode should start Single.");

		await using (await HotReloadHelper.UpdateSourceFile<ChipGroupPage>(
			originalText: "SelectionMode=\"Single\"",
			replacementText: "SelectionMode=\"Multiple\"",
			ct))
		{
			var cgDuring = UIHelper.GetChild<ChipGroup>(name: "CG");
			Assert.AreEqual(ChipSelectionMode.Multiple, cgDuring.SelectionMode, "SelectionMode should be Multiple after HR.");
		}

		var cgAfter = UIHelper.GetChild<ChipGroup>(name: "CG");
		Assert.AreEqual(ChipSelectionMode.Single, cgAfter.SelectionMode, "SelectionMode should be restored to Single after dispose.");
	}

	/// <summary>
	/// Verifies that changing ChipGroup.CanRemove via XAML HR is applied.
	/// </summary>
	[TestMethod]
	public async Task ChangeCanRemove_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new ChipGroupPage(), ct);

		var cg = UIHelper.GetChild<ChipGroup>(name: "CG");
		Assert.IsFalse(cg.CanRemove, "CanRemove should start False.");

		await using (await HotReloadHelper.UpdateSourceFile<ChipGroupPage>(
			originalText: "CanRemove=\"False\"",
			replacementText: "CanRemove=\"True\"",
			ct))
		{
			var cgDuring = UIHelper.GetChild<ChipGroup>(name: "CG");
			Assert.IsTrue(cgDuring.CanRemove, "CanRemove should be True after HR.");
		}

		var cgAfter = UIHelper.GetChild<ChipGroup>(name: "CG");
		Assert.IsFalse(cgAfter.CanRemove, "CanRemove should be restored to False after dispose.");
	}

	/// <summary>
	/// Verifies that adding a Chip to ChipGroup via XAML HR is reflected.
	/// </summary>
	[TestMethod]
	public async Task AddChip_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new ChipGroupPage(), ct);

		var cg = UIHelper.GetChild<ChipGroup>(name: "CG");
		Assert.AreEqual(3, cg.Items.Count, "Should start with 3 chips.");

		await using (await HotReloadHelper.UpdateSourceFile<ChipGroupPage>(
			originalText: """<utu:Chip Content="Gamma" />""",
			replacementText: """
				<utu:Chip Content="Gamma" />
				<utu:Chip Content="Delta" />
			""",
			ct))
		{
			var cgDuring = UIHelper.GetChild<ChipGroup>(name: "CG");
			Assert.AreEqual(4, cgDuring.Items.Count, "Should have 4 chips after HR add.");
		}

		var cgAfter = UIHelper.GetChild<ChipGroup>(name: "CG");
		Assert.AreEqual(3, cgAfter.Items.Count, "Should be restored to 3 chips after dispose.");
	}
}
#endif
