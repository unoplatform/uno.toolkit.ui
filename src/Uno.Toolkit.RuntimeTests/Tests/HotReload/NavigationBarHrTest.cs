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
/// Hot Reload regression tests for NavigationBar property edits (#1578).
/// </summary>
[TestClass]
[RunsInSecondaryApp(ignoreIfNotSupported: true)]
public class NavigationBarHrTest
{
	[TestInitialize]
	public void Setup()
	{
		HotReloadHelper.DefaultWorkspaceTimeout = TimeSpan.FromSeconds(180);
		HotReloadHelper.DefaultMetadataUpdateTimeout = TimeSpan.FromSeconds(60);
	}

	/// <summary>
	/// Verifies that changing NavigationBar.Content via XAML HR is applied.
	/// </summary>
	[TestMethod]
	public async Task ChangeContent_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new NavigationBarPage(), ct);

		var navBar = UIHelper.GetChild<NavigationBar>(name: "NavBar");
		Assert.AreEqual("Page Title", navBar.Content as string, "Content should start as 'Page Title'.");

		await using (await HotReloadHelper.UpdateSourceFile<NavigationBarPage>(
			originalText: "Content=\"Page Title\"",
			replacementText: "Content=\"Updated Title\"",
			ct))
		{
			var navBarDuring = UIHelper.GetChild<NavigationBar>(name: "NavBar");
			Assert.AreEqual("Updated Title", navBarDuring.Content as string, "Content should be 'Updated Title' after HR.");
		}

		var navBarAfter = UIHelper.GetChild<NavigationBar>(name: "NavBar");
		Assert.AreEqual("Page Title", navBarAfter.Content as string, "Content should be restored to 'Page Title' after dispose.");
	}

	/// <summary>
	/// Verifies that changing an AppBarButton.Label in PrimaryCommands via XAML HR is applied.
	/// </summary>
	[TestMethod]
	public async Task ChangePrimaryCommandLabel_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new NavigationBarPage(), ct);

		var navBar = UIHelper.GetChild<NavigationBar>(name: "NavBar");
		var searchBtn = navBar.PrimaryCommands.OfType<AppBarButton>().First();
		Assert.AreEqual("Search", searchBtn.Label, "AppBarButton label should start as 'Search'.");

		await using (await HotReloadHelper.UpdateSourceFile<NavigationBarPage>(
			originalText: "Label=\"Search\"",
			replacementText: "Label=\"Find\"",
			ct))
		{
			var navBarDuring = UIHelper.GetChild<NavigationBar>(name: "NavBar");
			var searchBtnDuring = navBarDuring.PrimaryCommands.OfType<AppBarButton>().First();
			Assert.AreEqual("Find", searchBtnDuring.Label, "AppBarButton label should be 'Find' after HR.");
		}

		var navBarAfter = UIHelper.GetChild<NavigationBar>(name: "NavBar");
		var searchBtnAfter = navBarAfter.PrimaryCommands.OfType<AppBarButton>().First();
		Assert.AreEqual("Search", searchBtnAfter.Label, "AppBarButton label should be restored to 'Search' after dispose.");
	}

	/// <summary>
	/// Verifies that adding a PrimaryCommand to NavigationBar via XAML HR is reflected.
	/// </summary>
	[TestMethod]
	public async Task AddPrimaryCommand_Via_Xaml_HotReload(CancellationToken ct)
	{
		await UIHelper.Load(new NavigationBarPage(), ct);

		var navBar = UIHelper.GetChild<NavigationBar>(name: "NavBar");
		Assert.AreEqual(1, navBar.PrimaryCommands.Count, "Should start with 1 primary command.");

		await using (await HotReloadHelper.UpdateSourceFile<NavigationBarPage>(
			originalText: """<AppBarButton x:Name="SearchBtn" Label="Search" />""",
			replacementText: """
				<AppBarButton x:Name="SearchBtn" Label="Search" />
				<AppBarButton x:Name="SettingsBtn" Label="Settings" />
			""",
			ct))
		{
			var navBarDuring = UIHelper.GetChild<NavigationBar>(name: "NavBar");
			Assert.AreEqual(2, navBarDuring.PrimaryCommands.Count, "Should have 2 primary commands after HR add.");
		}

		var navBarAfter = UIHelper.GetChild<NavigationBar>(name: "NavBar");
		Assert.AreEqual(1, navBarAfter.PrimaryCommands.Count, "Should be restored to 1 primary command after dispose.");
	}
}
#endif
