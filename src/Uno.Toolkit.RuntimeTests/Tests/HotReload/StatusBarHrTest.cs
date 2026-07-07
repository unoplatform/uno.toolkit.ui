#if DEBUG
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Uno.Toolkit.RuntimeTests.Tests.TestPages;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Windows.UI;

namespace Uno.Toolkit.RuntimeTests.Tests.HotReload;

/// <summary>
/// Hot Reload regression tests for StatusBar Background/Foreground edits (#1585).
/// On platforms without a native status bar, this validates that the attached
/// property values propagate correctly after XAML HR.
/// </summary>
[TestClass]
[RunsInSecondaryApp(ignoreIfNotSupported: true)]
public class StatusBarHrTest
{
	[TestInitialize]
	public void Setup()
	{
		HotReloadHelper.DefaultWorkspaceTimeout = TimeSpan.FromSeconds(180);
		HotReloadHelper.DefaultMetadataUpdateTimeout = TimeSpan.FromSeconds(60);
	}

	/// <summary>
	/// Verifies that changing StatusBar.Foreground via XAML HR is applied.
	/// </summary>
	[TestMethod]
	public async Task ChangeForeground_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new StatusBarPage(), ct);

		// StatusBar.Foreground is set on the Page itself
		var page = UIHelper.GetChild<Page>();
		Assert.IsNotNull(page);
		var foreground = StatusBar.GetForeground(page);
		Assert.AreEqual(StatusBarForegroundTheme.Dark, foreground, "Foreground should start as Dark.");

		await using (await HotReloadHelper.UpdateSourceFile<StatusBarPage>(
			originalText: "utu:StatusBar.Foreground=\"Dark\"",
			replacementText: "utu:StatusBar.Foreground=\"Light\"",
			ct))
		{
			var pageDuring = UIHelper.GetChild<Page>();
			var foregroundDuring = StatusBar.GetForeground(pageDuring!);
			Assert.AreEqual(StatusBarForegroundTheme.Light, foregroundDuring, "Foreground should be Light after HR.");
		}

		var pageAfter = UIHelper.GetChild<Page>();
		var foregroundAfter = StatusBar.GetForeground(pageAfter!);
		Assert.AreEqual(StatusBarForegroundTheme.Dark, foregroundAfter, "Foreground should be restored to Dark after dispose.");
	}

	/// <summary>
	/// Verifies that changing StatusBar.Background via XAML HR is applied.
	/// </summary>
	[TestMethod]
	public async Task ChangeBackground_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new StatusBarPage(), ct);

		var page = UIHelper.GetChild<Page>();
		Assert.IsNotNull(page);
		var bg = StatusBar.GetBackground(page) as SolidColorBrush;
		Assert.IsNotNull(bg, "Background should be set initially.");
		Assert.AreEqual(Colors.Blue, bg!.Color, "Background should start as Blue.");

		await using (await HotReloadHelper.UpdateSourceFile<StatusBarPage>(
			originalText: "utu:StatusBar.Background=\"Blue\"",
			replacementText: "utu:StatusBar.Background=\"Red\"",
			ct))
		{
			var pageDuring = UIHelper.GetChild<Page>();
			var bgDuring = StatusBar.GetBackground(pageDuring!) as SolidColorBrush;
			Assert.IsNotNull(bgDuring, "Background should be set after HR.");
			Assert.AreEqual(Colors.Red, bgDuring!.Color, "Background should be Red after HR.");
		}

		var pageAfter = UIHelper.GetChild<Page>();
		var bgAfter = StatusBar.GetBackground(pageAfter!) as SolidColorBrush;
		Assert.IsNotNull(bgAfter, "Background should be set after restore.");
		Assert.AreEqual(Colors.Blue, bgAfter!.Color, "Background should be restored to Blue after dispose.");
	}

	/// <summary>
	/// Verifies that page content alongside StatusBar properties can be changed via XAML HR.
	/// </summary>
	[TestMethod]
	public async Task ChangeContent_WithStatusBar_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new StatusBarPage(), ct);

		var content = UIHelper.GetChild<TextBlock>(name: "StatusContent");
		Assert.AreEqual("StatusBar test page", content.Text);

		await using (await HotReloadHelper.UpdateSourceFile<StatusBarPage>(
			originalText: "Text=\"StatusBar test page\"",
			replacementText: "Text=\"Updated page\"",
			ct))
		{
			var contentDuring = UIHelper.GetChild<TextBlock>(name: "StatusContent");
			Assert.AreEqual("Updated page", contentDuring.Text, "Content should be updated after HR.");
		}

		var contentAfter = UIHelper.GetChild<TextBlock>(name: "StatusContent");
		Assert.AreEqual("StatusBar test page", contentAfter.Text, "Content should be restored after dispose.");
	}
}
#endif
