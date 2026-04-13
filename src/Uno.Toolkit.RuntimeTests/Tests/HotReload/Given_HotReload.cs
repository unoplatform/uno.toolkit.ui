#if DEBUG // Hot-reload tests are only relevant in debug configuration
using System;
using System.Linq;
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

	// --- Complex type returns ---

	[TestMethod]
	public async Task When_UpdateEnumReturn_Then_NewValue(CancellationToken ct)
	{
		Assert.AreEqual(HotReloadTarget.Status.Active, HotReloadTarget.GetStatus());

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"return Status.Active;",
			"return Status.Pending;",
			ct);

		Assert.AreEqual(HotReloadTarget.Status.Pending, HotReloadTarget.GetStatus());
	}

	[TestMethod]
	public async Task When_UpdateCollectionReturn_Then_NewList(CancellationToken ct)
	{
		var items = HotReloadTarget.GetItems();
		Assert.AreEqual(3, items.Count);
		Assert.AreEqual("A", items[0]);

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"""{ "A", "B", "C" }""",
			"""{ "X", "Y" }""",
			ct);

		var updated = HotReloadTarget.GetItems();
		Assert.AreEqual(2, updated.Count);
		Assert.AreEqual("X", updated[0]);
		Assert.AreEqual("Y", updated[1]);
	}

	[TestMethod]
	public async Task When_UpdateTupleReturn_Then_NewTuple(CancellationToken ct)
	{
		Assert.AreEqual(("Alice", 30), HotReloadTarget.GetTuple());

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"""("Alice", 30)""",
			"""("Bob", 25)""",
			ct);

		Assert.AreEqual(("Bob", 25), HotReloadTarget.GetTuple());
	}

	[TestMethod]
	public async Task When_UpdateDictionaryReturn_Then_NewDict(CancellationToken ct)
	{
		var dict = HotReloadTarget.GetLookup();
		Assert.AreEqual(1, dict["x"]);

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"""["x"] = 1""",
			"""["y"] = 99""",
			ct);

		var updated = HotReloadTarget.GetLookup();
		Assert.AreEqual(99, updated["y"]);
	}

	// --- Control flow ---

	[TestMethod]
	public async Task When_UpdateSwitchBranch_Then_BranchChanges(CancellationToken ct)
	{
		Assert.AreEqual("one", HotReloadTarget.GetSwitch(1));
		Assert.AreEqual("two", HotReloadTarget.GetSwitch(2));
		Assert.AreEqual("other", HotReloadTarget.GetSwitch(99));

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"""1 => "one",""",
			"""1 => "uno",""",
			ct);

		Assert.AreEqual("uno", HotReloadTarget.GetSwitch(1));
		Assert.AreEqual("two", HotReloadTarget.GetSwitch(2));
		Assert.AreEqual("other", HotReloadTarget.GetSwitch(99));
	}

	[TestMethod]
	public async Task When_UpdateLoopBody_Then_OutputChanges(CancellationToken ct)
	{
		Assert.AreEqual("aaa", HotReloadTarget.GetRepeated(3));

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"""result += "a";""",
			"""result += "b";""",
			ct);

		Assert.AreEqual("bbb", HotReloadTarget.GetRepeated(3));
	}

	[TestMethod]
	public async Task When_UpdateCatchBlock_Then_NewHandler(CancellationToken ct)
	{
		Assert.AreEqual("caught-original", HotReloadTarget.GetSafe(true));
		Assert.AreEqual("no-throw", HotReloadTarget.GetSafe(false));

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"""return "caught-original";""",
			"""return "caught-updated";""",
			ct);

		Assert.AreEqual("caught-updated", HotReloadTarget.GetSafe(true));
		Assert.AreEqual("no-throw", HotReloadTarget.GetSafe(false));
	}

	[TestMethod]
	public async Task When_UpdateLinqLambda_Then_FilterChanges(CancellationToken ct)
	{
		var filtered = HotReloadTarget.GetFiltered();
		Assert.AreEqual(3, filtered.Count); // 3, 4, 5

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"x => x > 2",
			"x => x > 0",
			ct);

		var updated = HotReloadTarget.GetFiltered();
		Assert.AreEqual(5, updated.Count); // 1, 2, 3, 4, 5
	}

	// --- State & instance ---

	[TestMethod]
	public async Task When_UpdateStaticFieldMutation_Then_NewIncrement(CancellationToken ct)
	{
		HotReloadTarget.ResetCounter();

		Assert.AreEqual(1, HotReloadTarget.GetCounter()); // 0 + 1 = 1

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"_counter += 1;",
			"_counter += 10;",
			ct);

		// Static field _counter retains value 1 from previous call, new increment is 10
		Assert.AreEqual(11, HotReloadTarget.GetCounter()); // 1 + 10 = 11
	}

	[TestMethod]
	public async Task When_UpdateNestedClass_Then_NewValue(CancellationToken ct)
	{
		Assert.AreEqual("inner-original", HotReloadTarget.GetNested());

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			""""inner-original"""",
			""""inner-updated"""",
			ct);

		Assert.AreEqual("inner-updated", HotReloadTarget.GetNested());
	}

	// --- Edge cases ---

	[TestMethod]
	public async Task When_RapidRevertCycle_Then_AllStatesCorrect(CancellationToken ct)
	{
		Assert.AreEqual("original", HotReloadTarget.GetValue());

		// First update
		await using (await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"""return "original";""",
			"""return "first";""",
			ct))
		{
			Assert.AreEqual("first", HotReloadTarget.GetValue());
		}

		// After revert
		Assert.AreEqual("original", HotReloadTarget.GetValue());

		// Second update to a different value
		await using (await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"""return "original";""",
			"""return "second";""",
			ct))
		{
			Assert.AreEqual("second", HotReloadTarget.GetValue());
		}

		// After second revert
		Assert.AreEqual("original", HotReloadTarget.GetValue());
	}

	[TestMethod]
	public async Task When_UpdateMultiLineString_Then_SpecificLineChanges(CancellationToken ct)
	{
		var result = HotReloadTarget.GetMultiLine();
		Assert.IsTrue(result.Contains("line-two-original"));

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"\"line-two-original\\n\" +",
			"\"line-two-updated\\n\" +",
			ct);

		var updated = HotReloadTarget.GetMultiLine();
		Assert.IsTrue(updated.Contains("line-two-updated"));
		Assert.IsFalse(updated.Contains("line-two-original"));
		Assert.IsTrue(updated.Contains("line-one"));
		Assert.IsTrue(updated.Contains("line-three"));
	}

	[TestMethod]
	public async Task When_UpdateEmptyToNonEmpty_Then_NewValue(CancellationToken ct)
	{
		Assert.AreEqual("", HotReloadTarget.GetOptional());

		await using var _ = await HotReloadHelper.UpdateSourceFile(
			"../../src/Uno.Toolkit.RuntimeTests/Tests/HotReload/HotReloadTarget.cs",
			"""return "";""",
			"""return "filled";""",
			ct);

		Assert.AreEqual("filled", HotReloadTarget.GetOptional());
	}
}
#endif
