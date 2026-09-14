using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

/// <summary>
/// Guards that <see cref="FlexPanel"/> releases children it no longer owns.
/// </summary>
/// <remarks>
/// The panel keeps a per-child <c>YogaNode</c> cache and an attached-property snapshot cache, both
/// keyed by the child element. <c>SyncYogaTree</c> prunes them against the live <c>Children</c> on
/// every layout pass, so a removed child is released on the next pass. These tests pin that: a
/// regression would turn every panel into a slow leak of detached visual trees, which is exactly the
/// failure mode that is invisible until an app runs for a while.
/// </remarks>
[TestClass]
[RunsOnUIThread]
internal class FlexPanelLeakTests
{
	[TestMethod]
	public async Task When_ChildRemovedAndRelaidOut_ThenChildIsCollectable()
	{
		var SUT = new FlexPanel { Width = 200, Height = 200 };
		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var childRef = AddChildAndDropReference(SUT);

		// The child participated in a layout pass, so it is in both caches at this point.
		await UnitTestsUIContentHelper.WaitForIdle();

		SUT.Children.Clear();

		// The pruning happens during the next layout pass, not at removal time.
		SUT.InvalidateMeasure();
		await UnitTestsUIContentHelper.WaitForIdle();

		await CollectAndWait();

		Assert.IsFalse(
			childRef.IsAlive,
			"A child removed from a FlexPanel must be collectable once the panel has laid out again; " +
			"if it survives, the node cache or the attached-property cache is still holding it.");

		GC.KeepAlive(SUT);
	}

	[TestMethod]
	public async Task When_PanelUnloaded_ThenChildrenAreCollectable()
	{
		// The Unloaded handler clears both caches. Without it, a panel that leaves the tree and is
		// never laid out again would pin its whole child set.
		var host = new Grid { Width = 200, Height = 200 };
		await UnitTestUIContentHelperEx.SetContentAndWait(host);

		var childRef = AddPanelWithChildAndDropReference(host);
		await UnitTestsUIContentHelper.WaitForIdle();

		host.Children.Clear();
		await UnitTestsUIContentHelper.WaitForIdle();

		await CollectAndWait();

		Assert.IsFalse(
			childRef.IsAlive,
			"Unloading a FlexPanel must release its cached child nodes.");

		GC.KeepAlive(host);
	}

	[TestMethod]
	public async Task When_ChildrenRecycledRepeatedly_ThenNoAccumulation()
	{
		// Approximates an ItemsRepeater recycle loop: the same panel churns through many children.
		// Only the final set may still be reachable.
		var SUT = new FlexPanel { Width = 200, Height = 200 };
		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var refs = await RecycleChildren(SUT, generations: 5);

		SUT.Children.Clear();
		SUT.InvalidateMeasure();
		await UnitTestsUIContentHelper.WaitForIdle();

		await CollectAndWait();

		for (var i = 0; i < refs.Length; i++)
		{
			Assert.IsFalse(
				refs[i].IsAlive,
				$"Recycled child from generation {i} should not survive; the panel's caches are accumulating.");
		}

		GC.KeepAlive(SUT);
	}

	// Kept in separate non-inlined frames so the child never stays alive through a caller local.

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static WeakReference AddChildAndDropReference(FlexPanel panel)
	{
		var child = new Border { Width = 50, Height = 50 };
		FlexPanel.SetGrow(child, 1);
		panel.Children.Add(child);

		return new WeakReference(child);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static WeakReference AddPanelWithChildAndDropReference(Grid host)
	{
		var panel = new FlexPanel { Width = 200, Height = 200 };
		var child = new Border { Width = 50, Height = 50 };
		FlexPanel.SetGrow(child, 1);
		panel.Children.Add(child);
		host.Children.Add(panel);

		return new WeakReference(child);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static async Task<WeakReference[]> RecycleChildren(FlexPanel panel, int generations)
	{
		var refs = new WeakReference[generations];

		for (var i = 0; i < generations; i++)
		{
			panel.Children.Clear();
			refs[i] = AddChildAndDropReference(panel);
			panel.InvalidateMeasure();
			await UnitTestsUIContentHelper.WaitForIdle();
		}

		return refs;
	}

	private static async Task CollectAndWait()
	{
		for (var i = 0; i < 3; i++)
		{
			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
			GC.WaitForPendingFinalizers();
			await UnitTestsUIContentHelper.WaitForIdle();
		}
	}
}
