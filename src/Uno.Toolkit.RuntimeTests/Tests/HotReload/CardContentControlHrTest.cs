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
/// Hot Reload regression tests for CardContentControl property edits (#1575).
/// </summary>
[TestClass]
[RunsInSecondaryApp(ignoreIfNotSupported: true)]
public class CardContentControlHrTest
{
	[TestInitialize]
	public void Setup()
	{
		HotReloadHelper.DefaultWorkspaceTimeout = TimeSpan.FromSeconds(180);
		HotReloadHelper.DefaultMetadataUpdateTimeout = TimeSpan.FromSeconds(60);
	}

	/// <summary>
	/// Verifies that changing CardContentControl.Elevation via XAML HR is applied.
	/// </summary>
	[TestMethod]
	public async Task ChangeElevation_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new CardContentControlPage(), ct);

		var card = UIHelper.GetChild<CardContentControl>(name: "Card");
		Assert.AreEqual(2, card.Elevation, "Elevation should start at 2.");

		await using (await HotReloadHelper.UpdateSourceFile<CardContentControlPage>(
			originalText: "Elevation=\"2\"",
			replacementText: "Elevation=\"8\"",
			ct))
		{
			var cardDuring = UIHelper.GetChild<CardContentControl>(name: "Card");
			Assert.AreEqual(8, cardDuring.Elevation, "Elevation should be 8 after HR.");
		}

		var cardAfter = UIHelper.GetChild<CardContentControl>(name: "Card");
		Assert.AreEqual(2, cardAfter.Elevation, "Elevation should be restored to 2 after dispose.");
	}

	/// <summary>
	/// Verifies that changing CardContentControl.IsClickable via XAML HR is applied.
	/// </summary>
	[TestMethod]
	public async Task ChangeIsClickable_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new CardContentControlPage(), ct);

		var card = UIHelper.GetChild<CardContentControl>(name: "Card");
		Assert.IsTrue(card.IsClickable, "IsClickable should start True.");

		await using (await HotReloadHelper.UpdateSourceFile<CardContentControlPage>(
			originalText: "IsClickable=\"True\"",
			replacementText: "IsClickable=\"False\"",
			ct))
		{
			var cardDuring = UIHelper.GetChild<CardContentControl>(name: "Card");
			Assert.IsFalse(cardDuring.IsClickable, "IsClickable should be False after HR.");
		}

		var cardAfter = UIHelper.GetChild<CardContentControl>(name: "Card");
		Assert.IsTrue(cardAfter.IsClickable, "IsClickable should be restored to True after dispose.");
	}

	/// <summary>
	/// Verifies that changing CardContentControl content via XAML HR is reflected.
	/// </summary>
	[TestMethod]
	public async Task ChangeContent_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new CardContentControlPage(), ct);

		var content = UIHelper.GetChild<TextBlock>(name: "CardContent");
		Assert.AreEqual("Card content", content.Text);

		await using (await HotReloadHelper.UpdateSourceFile<CardContentControlPage>(
			originalText: "Text=\"Card content\"",
			replacementText: "Text=\"Updated card content\"",
			ct))
		{
			var contentDuring = UIHelper.GetChild<TextBlock>(name: "CardContent");
			Assert.AreEqual("Updated card content", contentDuring.Text, "Content text should be updated after HR.");
		}

		var contentAfter = UIHelper.GetChild<TextBlock>(name: "CardContent");
		Assert.AreEqual("Card content", contentAfter.Text, "Content text should be restored after dispose.");
	}
}
#endif
