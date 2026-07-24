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
/// Guards that the disposable returned by
/// <c>FrameworkElementExtensions.SubscribeToNestedElements</c> actually detaches its
/// <c>Loaded</c> handlers on dispose. Leaving them attached pins the whole subscription closure
/// (including the caller-supplied callback) to any host-owned element that outlives a previewed app
/// loaded into a collectible AssemblyLoadContext.
/// </summary>
[TestClass]
[RunsOnUIThread]
internal class FrameworkElementExtensionsLeakTests
{
	private sealed class Sentinel;

	[TestMethod]
	public async Task SubscribeToNestedElements_Dispose_DetachesLoadedHandlers()
	{
		// The root stays alive (it plays the role of the long-lived, host-owned element). After the
		// disposable is disposed, a sentinel captured only by the subscription callback must become
		// collectible — which is only possible if the Loaded handler was actually removed from the root.
		// Give the root a non-zero size: SetContentAndWait's loaded-check treats an element with
		// ActualWidth/ActualHeight == 0 as not-yet-loaded, so a bare Grid would time out.
		var root = new Grid { Width = 100, Height = 100 };
		await UnitTestUIContentHelperEx.SetContentAndWait(root);

		var sentinelRef = SubscribeThenDispose(root);

		await CollectAndWait();

		Assert.IsFalse(
			sentinelRef.IsAlive,
			"The subscription callback should be collectible after disposing the SubscribeToNestedElements handle; " +
			"if it survives, the Loaded handler was not detached and is still rooted by the (alive) root element.");

		GC.KeepAlive(root);
	}

	// Kept in a separate non-inlined frame so neither the disposable, the callback, nor the sentinel
	// stays alive via a local on the test method's stack while we assert collection.
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static WeakReference SubscribeThenDispose(Grid root)
	{
		var sentinel = new Sentinel();
		var sentinelRef = new WeakReference(sentinel);

		// The callback captures the sentinel; the whole closure is reachable through the Loaded handler
		// registered on 'root' until the returned disposable detaches it.
		var subscription = root.SubscribeToNestedElements(
			new Func<FrameworkElement, FrameworkElement?>[] { fe => fe },
			_ => GC.KeepAlive(sentinel));

		subscription.Dispose();

		return sentinelRef;
	}

	private static async Task CollectAndWait()
	{
		for (var i = 0; i < 5; i++)
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			await UnitTestUIContentHelperEx.WaitForIdle();
		}
	}
}
