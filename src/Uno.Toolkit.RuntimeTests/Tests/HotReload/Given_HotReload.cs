using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.UI.RuntimeTests;

namespace Uno.Toolkit.RuntimeTests.Tests.HotReload;

[TestClass]
[RunsInSecondaryApp(ignoreIfNotSupported: true)]
public class Given_HotReload
{
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
