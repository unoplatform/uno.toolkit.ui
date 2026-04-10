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
		// Allow more time for the dev-server to load the Roslyn workspace (solution can be large)
		HotReloadHelper.DefaultWorkspaceTimeout = TimeSpan.FromSeconds(180);
		// Allow more time for the first metadata update (delta compilation can be slow on CI)
		HotReloadHelper.DefaultMetadataUpdateTimeout = TimeSpan.FromSeconds(60);
	}

	[TestMethod]
	public async Task When_UpdateMethodBody_Then_MetadataUpdateReceived(CancellationToken ct)
	{
		Assert.AreEqual("original", HotReloadTarget.GetValue());

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"""return "original";""",
			"""return "updated";""",
			ct);

		Assert.AreEqual("updated", HotReloadTarget.GetValue());
	}

	[TestMethod]
	public async Task When_UpdateMethodBody_Then_RevertRestoresOriginal(CancellationToken ct)
	{
		Assert.AreEqual("original", HotReloadTarget.GetValue());

		await using (await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"""return "original";""",
			"""return "updated";""",
			ct))
		{
			Assert.AreEqual("updated", HotReloadTarget.GetValue());
		}

		// After dispose, the file is reverted and the original value is restored
		Assert.AreEqual("original", HotReloadTarget.GetValue());
	}

	[TestMethod]
	public async Task When_UpdateNumericReturn_Then_NewValueReturned(CancellationToken ct)
	{
		Assert.AreEqual(1, HotReloadTarget.GetNumber());

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"return 1;",
			"return 42;",
			ct);

		Assert.AreEqual(42, HotReloadTarget.GetNumber());
	}

	[TestMethod]
	public async Task When_UpdateBoolReturn_Then_NewValueReturned(CancellationToken ct)
	{
		Assert.AreEqual(false, HotReloadTarget.GetFlag());

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"return false;",
			"return true;",
			ct);

		Assert.AreEqual(true, HotReloadTarget.GetFlag());
	}

	[TestMethod]
	public async Task When_UpdateStringInterpolation_Then_OutputChanges(CancellationToken ct)
	{
		Assert.AreEqual("Hello, World!", HotReloadTarget.Greet("World"));

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"""return $"Hello, {name}!";""",
			"""return $"Hi, {name}!";""",
			ct);

		Assert.AreEqual("Hi, World!", HotReloadTarget.Greet("World"));
	}

	[TestMethod]
	public async Task When_UpdateConditionalLogic_Then_BehaviorChanges(CancellationToken ct)
	{
		Assert.AreEqual("condition-true", HotReloadTarget.GetConditional(true));
		Assert.AreEqual("condition-false", HotReloadTarget.GetConditional(false));

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"""condition-true";""",
			"""condition-true-updated";""",
			ct);

		Assert.AreEqual("condition-true-updated", HotReloadTarget.GetConditional(true));
		Assert.AreEqual("condition-false", HotReloadTarget.GetConditional(false));
	}

	[TestMethod]
	public async Task When_MultipleSequentialUpdates_Then_AllApplied(CancellationToken ct)
	{
		Assert.AreEqual(1, HotReloadTarget.GetNumber());

		await using (await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"return 1;",
			"return 10;",
			ct))
		{
			Assert.AreEqual(10, HotReloadTarget.GetNumber());
		}

		// After revert, apply a different update
		Assert.AreEqual(1, HotReloadTarget.GetNumber());

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"return 1;",
			"return 99;",
			ct);

		Assert.AreEqual(99, HotReloadTarget.GetNumber());
	}
}
#endif
