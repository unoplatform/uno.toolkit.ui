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
/// Hot Reload regression tests for FlipView property edits and item additions (#1583).
/// </summary>
[TestClass]
[RunsInSecondaryApp(ignoreIfNotSupported: true)]
public class FlipViewHrTest
{
	[TestInitialize]
	public void Setup()
	{
		HotReloadHelper.DefaultWorkspaceTimeout = TimeSpan.FromSeconds(180);
		HotReloadHelper.DefaultMetadataUpdateTimeout = TimeSpan.FromSeconds(60);
	}

	/// <summary>
	/// Verifies that adding an item to FlipView via XAML HR is reflected at runtime.
	/// </summary>
	[TestMethod]
	public async Task AddFlipViewItem_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new FlipViewPage(), ct);

		var fv = UIHelper.GetChild<FlipView>(name: "FV");
		Assert.AreEqual(2, fv.Items.Count, "Should start with 2 slides.");

		await using (await HotReloadHelper.UpdateSourceFile<FlipViewPage>(
			originalText: """<TextBlock Text="Slide Two" />""",
			replacementText: """
				<TextBlock Text="Slide Two" />
				<TextBlock Text="Slide Three" />
			""",
			ct))
		{
			var fvDuring = UIHelper.GetChild<FlipView>(name: "FV");
			Assert.AreEqual(3, fvDuring.Items.Count, "Should have 3 slides after HR add.");
		}

		var fvAfter = UIHelper.GetChild<FlipView>(name: "FV");
		Assert.AreEqual(2, fvAfter.Items.Count, "Should be restored to 2 slides after dispose.");
	}

	/// <summary>
	/// Verifies that changing FlipView Height via XAML HR is applied.
	/// </summary>
	[TestMethod]
	public async Task ChangeFlipViewHeight_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new FlipViewPage(), ct);

		var fv = UIHelper.GetChild<FlipView>(name: "FV");
		Assert.AreEqual(200d, fv.Height, "Height should start at 200.");

		await using (await HotReloadHelper.UpdateSourceFile<FlipViewPage>(
			originalText: "Height=\"200\"",
			replacementText: "Height=\"300\"",
			ct))
		{
			var fvDuring = UIHelper.GetChild<FlipView>(name: "FV");
			Assert.AreEqual(300d, fvDuring.Height, "Height should be 300 after HR.");
		}

		var fvAfter = UIHelper.GetChild<FlipView>(name: "FV");
		Assert.AreEqual(200d, fvAfter.Height, "Height should be restored to 200 after dispose.");
	}

	/// <summary>
	/// Verifies that changing FlipViewExtensions.Next button content via XAML HR works.
	/// </summary>
	[TestMethod]
	public async Task ChangeNextButtonContent_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new FlipViewPage(), ct);

		var nextBtn = UIHelper.GetChild<Button>(name: "NextBtn");
		Assert.AreEqual("Next", nextBtn.Content as string);

		await using (await HotReloadHelper.UpdateSourceFile<FlipViewPage>(
			originalText: """<Button x:Name="NextBtn" Content="Next" """,
			replacementText: """<Button x:Name="NextBtn" Content="Forward" """,
			ct))
		{
			var nextBtnDuring = UIHelper.GetChild<Button>(name: "NextBtn");
			Assert.AreEqual("Forward", nextBtnDuring.Content as string, "Button content should be 'Forward' after HR.");
		}

		var nextBtnAfter = UIHelper.GetChild<Button>(name: "NextBtn");
		Assert.AreEqual("Next", nextBtnAfter.Content as string, "Button content should be restored to 'Next' after dispose.");
	}
}
#endif
