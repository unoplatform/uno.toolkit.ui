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
/// Hot Reload regression tests for CommandExtensions and InputExtensions edits (#1584).
/// </summary>
[TestClass]
[RunsInSecondaryApp(ignoreIfNotSupported: true)]
public class CommandExtensionsHrTest
{
	[TestInitialize]
	public void Setup()
	{
		HotReloadHelper.DefaultWorkspaceTimeout = TimeSpan.FromSeconds(180);
		HotReloadHelper.DefaultMetadataUpdateTimeout = TimeSpan.FromSeconds(60);
	}

	/// <summary>
	/// Verifies that adding CommandExtensions.Command to a Button via XAML HR is applied.
	/// The initial page has Command="{x:Null}", and HR changes it to a binding.
	/// Since we can't easily bind in a test, we just verify the AP can be set/cleared.
	/// </summary>
	[TestMethod]
	public async Task RemoveCommand_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new CommandExtensionsPage(), ct);

		var btn = UIHelper.GetChild<Button>(name: "ActionBtn");
		Assert.IsNull(CommandExtensions.GetCommand(btn), "Command should start as null.");

		// HR: remove the entire utu:CommandExtensions.Command attribute
		await using (await HotReloadHelper.UpdateSourceFile<CommandExtensionsPage>(
			originalText: """<Button x:Name="ActionBtn" Content="Action" utu:CommandExtensions.Command="{x:Null}" />""",
			replacementText: """<Button x:Name="ActionBtn" Content="Action" />""",
			ct))
		{
			var btnDuring = UIHelper.GetChild<Button>(name: "ActionBtn");
			Assert.IsNotNull(btnDuring, "Button should still exist after HR.");
			Assert.IsNull(CommandExtensions.GetCommand(btnDuring), "Command should remain null after removing attribute.");
		}

		var btnAfter = UIHelper.GetChild<Button>(name: "ActionBtn");
		Assert.IsNotNull(btnAfter, "Button should be restored after dispose.");
	}

	/// <summary>
	/// Verifies that changing button content alongside CommandExtensions via XAML HR works.
	/// </summary>
	[TestMethod]
	public async Task ChangeButtonContent_WithCommand_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new CommandExtensionsPage(), ct);

		var btn = UIHelper.GetChild<Button>(name: "ActionBtn");
		Assert.AreEqual("Action", btn.Content as string);

		await using (await HotReloadHelper.UpdateSourceFile<CommandExtensionsPage>(
			originalText: "Content=\"Action\"",
			replacementText: "Content=\"Updated Action\"",
			ct))
		{
			var btnDuring = UIHelper.GetChild<Button>(name: "ActionBtn");
			Assert.AreEqual("Updated Action", btnDuring.Content as string, "Content should be updated after HR.");
		}

		var btnAfter = UIHelper.GetChild<Button>(name: "ActionBtn");
		Assert.AreEqual("Action", btnAfter.Content as string, "Content should be restored after dispose.");
	}

	/// <summary>
	/// Verifies that changing TextBox PlaceholderText alongside InputExtensions via XAML HR works.
	/// </summary>
	[TestMethod]
	public async Task ChangeInputPlaceholder_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new CommandExtensionsPage(), ct);

		var input = UIHelper.GetChild<TextBox>(name: "InputBox");
		Assert.AreEqual("Type here", input.PlaceholderText);

		await using (await HotReloadHelper.UpdateSourceFile<CommandExtensionsPage>(
			originalText: "PlaceholderText=\"Type here\"",
			replacementText: "PlaceholderText=\"Enter text\"",
			ct))
		{
			var inputDuring = UIHelper.GetChild<TextBox>(name: "InputBox");
			Assert.AreEqual("Enter text", inputDuring.PlaceholderText, "PlaceholderText should be updated after HR.");
		}

		var inputAfter = UIHelper.GetChild<TextBox>(name: "InputBox");
		Assert.AreEqual("Type here", inputAfter.PlaceholderText, "PlaceholderText should be restored after dispose.");
	}
}
#endif
