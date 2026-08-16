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

namespace Uno.Toolkit.RuntimeTests.Tests.HotReload;

/// <summary>
/// Hot Reload regression tests for AutoLayout property edits and child mutations (#1574).
/// </summary>
[TestClass]
[RunsInSecondaryApp(ignoreIfNotSupported: true)]
public class AutoLayoutHrTest
{
	[TestInitialize]
	public void Setup()
	{
		HotReloadHelper.DefaultWorkspaceTimeout = TimeSpan.FromSeconds(180);
		HotReloadHelper.DefaultMetadataUpdateTimeout = TimeSpan.FromSeconds(60);
	}

	/// <summary>
	/// Verifies that changing AutoLayout.Spacing via XAML HR is applied at runtime.
	/// </summary>
	[TestMethod]
	public async Task ChangeSpacing_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new AutoLayoutPage(), ct);

		var al = UIHelper.GetChild<AutoLayout>(name: "AL");
		Assert.AreEqual(10d, al.Spacing, "Spacing should start at 10.");

		await using (await HotReloadHelper.UpdateSourceFile<AutoLayoutPage>(
			originalText: "Spacing=\"10\"",
			replacementText: "Spacing=\"20\"",
			ct))
		{
			var alDuring = UIHelper.GetChild<AutoLayout>(name: "AL");
			Assert.AreEqual(20d, alDuring.Spacing, "Spacing should be 20 after HR.");
		}

		var alAfter = UIHelper.GetChild<AutoLayout>(name: "AL");
		Assert.AreEqual(10d, alAfter.Spacing, "Spacing should be restored to 10 after dispose.");
	}

	/// <summary>
	/// Verifies that changing AutoLayout.Orientation via XAML HR is applied at runtime.
	/// </summary>
	[TestMethod]
	public async Task ChangeOrientation_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new AutoLayoutPage(), ct);

		var al = UIHelper.GetChild<AutoLayout>(name: "AL");
		Assert.AreEqual(Orientation.Vertical, al.Orientation, "Orientation should start Vertical.");

		await using (await HotReloadHelper.UpdateSourceFile<AutoLayoutPage>(
			originalText: "Orientation=\"Vertical\"",
			replacementText: "Orientation=\"Horizontal\"",
			ct))
		{
			var alDuring = UIHelper.GetChild<AutoLayout>(name: "AL");
			Assert.AreEqual(Orientation.Horizontal, alDuring.Orientation, "Orientation should be Horizontal after HR.");
		}

		var alAfter = UIHelper.GetChild<AutoLayout>(name: "AL");
		Assert.AreEqual(Orientation.Vertical, alAfter.Orientation, "Orientation should be restored to Vertical after dispose.");
	}

	/// <summary>
	/// Verifies that adding a child to AutoLayout via XAML HR is reflected at runtime.
	/// </summary>
	[TestMethod]
	public async Task AddChild_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new AutoLayoutPage(), ct);

		var al = UIHelper.GetChild<AutoLayout>(name: "AL");
		Assert.AreEqual(3, al.Children.Count, "Should start with 3 children.");

		await using (await HotReloadHelper.UpdateSourceFile<AutoLayoutPage>(
			originalText: """<TextBlock x:Name="Child3" Text="Item Three" />""",
			replacementText: """
				<TextBlock x:Name="Child3" Text="Item Three" />
				<TextBlock x:Name="Child4" Text="Item Four" />
			""",
			ct))
		{
			var alDuring = UIHelper.GetChild<AutoLayout>(name: "AL");
			Assert.AreEqual(4, alDuring.Children.Count, "Should have 4 children after HR add.");
		}

		var alAfter = UIHelper.GetChild<AutoLayout>(name: "AL");
		Assert.AreEqual(3, alAfter.Children.Count, "Should be restored to 3 children after dispose.");
	}

	/// <summary>
	/// Verifies that removing a child from AutoLayout via XAML HR is reflected at runtime.
	/// </summary>
	[TestMethod]
	public async Task RemoveChild_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new AutoLayoutPage(), ct);

		var al = UIHelper.GetChild<AutoLayout>(name: "AL");
		Assert.AreEqual(3, al.Children.Count, "Should start with 3 children.");

		var nl = Environment.NewLine;
		var originalChildren = $"<TextBlock x:Name=\"Child2\" Text=\"Item Two\" />{nl}\t\t\t<TextBlock x:Name=\"Child3\" Text=\"Item Three\" />";
		var reducedChildren = $"<TextBlock x:Name=\"Child3\" Text=\"Item Three\" />";

		await using (await HotReloadHelper.UpdateSourceFile<AutoLayoutPage>(
			originalText: originalChildren,
			replacementText: reducedChildren,
			ct))
		{
			var alDuring = UIHelper.GetChild<AutoLayout>(name: "AL");
			Assert.AreEqual(2, alDuring.Children.Count, "Should have 2 children after HR remove.");
		}

		var alAfter = UIHelper.GetChild<AutoLayout>(name: "AL");
		Assert.AreEqual(3, alAfter.Children.Count, "Should be restored to 3 children after dispose.");
	}
}
#endif
