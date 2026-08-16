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
/// Hot Reload regression tests for SafeArea property edits (#1582).
/// </summary>
[TestClass]
[RunsInSecondaryApp(ignoreIfNotSupported: true)]
public class SafeAreaHrTest
{
	[TestInitialize]
	public void Setup()
	{
		HotReloadHelper.DefaultWorkspaceTimeout = TimeSpan.FromSeconds(180);
		HotReloadHelper.DefaultMetadataUpdateTimeout = TimeSpan.FromSeconds(60);
	}

	/// <summary>
	/// Verifies that changing SafeArea.Insets via XAML HR is applied.
	/// </summary>
	[TestMethod]
	public async Task ChangeInsets_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new SafeAreaPage(), ct);

		var border = UIHelper.GetChild<Border>(name: "SafeBorder");
		var insets = SafeArea.GetInsets(border);
		Assert.AreEqual(SafeArea.InsetMask.Top | SafeArea.InsetMask.Bottom, insets, "Insets should start as Top,Bottom.");

		await using (await HotReloadHelper.UpdateSourceFile<SafeAreaPage>(
			originalText: "utu:SafeArea.Insets=\"Top,Bottom\"",
			replacementText: "utu:SafeArea.Insets=\"Left,Right\"",
			ct))
		{
			var borderDuring = UIHelper.GetChild<Border>(name: "SafeBorder");
			var insetsDuring = SafeArea.GetInsets(borderDuring);
			Assert.AreEqual(SafeArea.InsetMask.Left | SafeArea.InsetMask.Right, insetsDuring, "Insets should be Left,Right after HR.");
		}

		var borderAfter = UIHelper.GetChild<Border>(name: "SafeBorder");
		var insetsAfter = SafeArea.GetInsets(borderAfter);
		Assert.AreEqual(SafeArea.InsetMask.Top | SafeArea.InsetMask.Bottom, insetsAfter, "Insets should be restored to Top,Bottom after dispose.");
	}

	/// <summary>
	/// Verifies that changing SafeArea.Mode via XAML HR is applied.
	/// </summary>
	[TestMethod]
	public async Task ChangeMode_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new SafeAreaPage(), ct);

		var border = UIHelper.GetChild<Border>(name: "SafeBorder");
		var mode = SafeArea.GetMode(border);
		Assert.AreEqual(SafeArea.InsetMode.Padding, mode, "Mode should start as Padding.");

		await using (await HotReloadHelper.UpdateSourceFile<SafeAreaPage>(
			originalText: "utu:SafeArea.Mode=\"Padding\"",
			replacementText: "utu:SafeArea.Mode=\"Margin\"",
			ct))
		{
			var borderDuring = UIHelper.GetChild<Border>(name: "SafeBorder");
			var modeDuring = SafeArea.GetMode(borderDuring);
			Assert.AreEqual(SafeArea.InsetMode.Margin, modeDuring, "Mode should be Margin after HR.");
		}

		var borderAfter = UIHelper.GetChild<Border>(name: "SafeBorder");
		var modeAfter = SafeArea.GetMode(borderAfter);
		Assert.AreEqual(SafeArea.InsetMode.Padding, modeAfter, "Mode should be restored to Padding after dispose.");
	}

	/// <summary>
	/// Verifies that changing content inside SafeArea via XAML HR works.
	/// </summary>
	[TestMethod]
	public async Task ChangeContent_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new SafeAreaPage(), ct);

		var content = UIHelper.GetChild<TextBlock>(name: "SafeContent");
		Assert.AreEqual("Safe content", content.Text);

		await using (await HotReloadHelper.UpdateSourceFile<SafeAreaPage>(
			originalText: "Text=\"Safe content\"",
			replacementText: "Text=\"Updated safe content\"",
			ct))
		{
			var contentDuring = UIHelper.GetChild<TextBlock>(name: "SafeContent");
			Assert.AreEqual("Updated safe content", contentDuring.Text, "Content should be updated after HR.");
		}

		var contentAfter = UIHelper.GetChild<TextBlock>(name: "SafeContent");
		Assert.AreEqual("Safe content", contentAfter.Text, "Content should be restored after dispose.");
	}
}
#endif
