#if DEBUG // Hot-reload tests are only relevant in debug configuration
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.UI.RuntimeTests;

namespace Uno.Toolkit.RuntimeTests.Tests.HotReload;

[TestClass]
[RunsInSecondaryApp(ignoreIfNotSupported: true)]
public class Given_HotReload
{
	[TestInitialize]
	public void Setup()
	{
		// Allow more time for the dev-server to load the Roslyn workspace
		HotReloadHelper.DefaultWorkspaceTimeout = TimeSpan.FromSeconds(120);
	}

	[TestMethod]
	public async Task When_UpdateMethodBody_Then_MetadataUpdateReceived(CancellationToken ct)
	{
		Assert.AreEqual("original", HotReloadTarget.GetValue());

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"""GetValue() => "original";""",
			"""GetValue() => "updated";""",
			ct);

		Assert.AreEqual("updated", HotReloadTarget.GetValue());
	}
}
#endif
