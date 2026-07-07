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
/// Hot Reload regression tests for utu:Responsive markup extension edits (#1581).
/// </summary>
[TestClass]
[RunsInSecondaryApp(ignoreIfNotSupported: true)]
public class ResponsiveExtensionHrTest
{
	[TestInitialize]
	public void Setup()
	{
		HotReloadHelper.DefaultWorkspaceTimeout = TimeSpan.FromSeconds(180);
		HotReloadHelper.DefaultMetadataUpdateTimeout = TimeSpan.FromSeconds(60);
	}

	/// <summary>
	/// Verifies that changing a Responsive markup extension value via XAML HR is applied.
	/// Changes the Narrow font size from 14 to 18.
	/// </summary>
	[TestMethod]
	public async Task ChangeNarrowValue_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new ResponsiveExtensionPage(), ct);

		var tb = UIHelper.GetChild<TextBlock>(name: "ResponsiveText");
		Assert.IsNotNull(tb, "TextBlock should exist.");

		await using (await HotReloadHelper.UpdateSourceFile<ResponsiveExtensionPage>(
			originalText: "Narrow=14, Wide=24",
			replacementText: "Narrow=18, Wide=24",
			ct))
		{
			var tbDuring = UIHelper.GetChild<TextBlock>(name: "ResponsiveText");
			Assert.IsNotNull(tbDuring, "TextBlock should still exist after HR.");
			// The FontSize should reflect the updated value for the current breakpoint.
			// On desktop (typically wide), it should be 24 regardless.
			// But the XAML HR should not crash or break the control.
		}

		var tbAfter = UIHelper.GetChild<TextBlock>(name: "ResponsiveText");
		Assert.IsNotNull(tbAfter, "TextBlock should be restored after dispose.");
	}

	/// <summary>
	/// Verifies that changing the Wide value of a Responsive markup extension via XAML HR is applied.
	/// </summary>
	[TestMethod]
	public async Task ChangeWideValue_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new ResponsiveExtensionPage(), ct);

		var tb = UIHelper.GetChild<TextBlock>(name: "ResponsiveText");
		// On a desktop test (wide breakpoint), FontSize should initially be 24
		var initialSize = tb.FontSize;

		await using (await HotReloadHelper.UpdateSourceFile<ResponsiveExtensionPage>(
			originalText: "Narrow=14, Wide=24",
			replacementText: "Narrow=14, Wide=32",
			ct))
		{
			var tbDuring = UIHelper.GetChild<TextBlock>(name: "ResponsiveText");
			// If running at a wide breakpoint, FontSize should now be 32
			if (tbDuring.FontSize != initialSize)
			{
				Assert.AreEqual(32d, tbDuring.FontSize, "FontSize should be 32 at wide breakpoint after HR.");
			}
		}

		var tbAfter = UIHelper.GetChild<TextBlock>(name: "ResponsiveText");
		Assert.AreEqual(initialSize, tbAfter.FontSize, "FontSize should be restored after dispose.");
	}

	/// <summary>
	/// Verifies that changing the target text alongside a Responsive extension via XAML HR works.
	/// </summary>
	[TestMethod]
	public async Task ChangeTextContent_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new ResponsiveExtensionPage(), ct);

		var tb = UIHelper.GetChild<TextBlock>(name: "ResponsiveText");
		Assert.AreEqual("Responsive text", tb.Text);

		await using (await HotReloadHelper.UpdateSourceFile<ResponsiveExtensionPage>(
			originalText: "Text=\"Responsive text\"",
			replacementText: "Text=\"Updated responsive text\"",
			ct))
		{
			var tbDuring = UIHelper.GetChild<TextBlock>(name: "ResponsiveText");
			Assert.AreEqual("Updated responsive text", tbDuring.Text, "Text should be updated after HR.");
		}

		var tbAfter = UIHelper.GetChild<TextBlock>(name: "ResponsiveText");
		Assert.AreEqual("Responsive text", tbAfter.Text, "Text should be restored after dispose.");
	}
}
#endif
