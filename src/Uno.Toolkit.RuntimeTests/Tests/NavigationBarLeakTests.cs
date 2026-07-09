#if DEBUG && (!IS_WINUI || HAS_UNO)
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Windows.UI.Core;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

/// <summary>
/// Guards that <see cref="NavigationBar"/>'s subscription to the process-global
/// <see cref="SystemNavigationManager.BackRequested"/> is weak. The instance is normally detached on
/// <c>Unloaded</c>, but <c>Unloaded</c> does not fire when the owner's AssemblyLoadContext is torn down
/// abruptly (a downstream host that loads previewed apps into their own collectible ALCs). Even in that
/// case the global singleton must not strongly root the NavigationBar.
/// </summary>
[TestClass]
[RunsOnUIThread]
internal class NavigationBarLeakTests
{
	[TestMethod]
	public async Task BackRequested_Subscription_IsWeak_WhenUnloadedNeverFires()
	{
		// Subscribe the weak BackRequested handler exactly as OnLoaded does, but without recording the
		// teardown disposable — modelling an abrupt ALC teardown. The NavigationBar must still be
		// collectible while the process-global SystemNavigationManager stays alive and subscribed.
		var navRef = SubscribeWeakThenAbandon();

		await CollectAndWait();

		Assert.IsFalse(
			navRef.IsAlive,
			"NavigationBar should be collectible even when Unloaded never fired, because the global " +
			"SystemNavigationManager.BackRequested subscription is weak. If it survives, the global " +
			"singleton is strongly rooting it.");

		// Keep the global singleton referenced through the assertion so it cannot be argued away.
		GC.KeepAlive(SystemNavigationManager.GetForCurrentView());
	}

	[TestMethod]
#if __ANDROID__ || __IOS__
	[Ignore("Native NavigationBar hosting differs on mobile platforms.")]
#endif
	public async Task NavigationBar_IsCollectible_AfterRemovedFromTree()
	{
		// Real-path regression guard: drives the production OnLoaded subscription (not the DEBUG hook), then
		// removes the bar from the tree. It must be collectible afterwards. This turns red if the whole
		// BackRequested subscription/teardown wiring regresses (e.g. a strong sub with no working teardown).
		var navRef = await LoadThenRemoveNavigationBar();

		await CollectAndWait();

		Assert.IsFalse(
			navRef.IsAlive,
			"A NavigationBar should be collectible after being removed from the visual tree; if it survives, " +
			"the BackRequested subscription is not being torn down.");
	}

	// Kept in a separate non-inlined frame so the NavigationBar is not kept alive by a local on the test
	// method's stack while we assert collection.
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static WeakReference SubscribeWeakThenAbandon()
	{
		var nav = new NavigationBar();
		nav.TestHook_SubscribeWeakBackRequestedWithoutTeardown();
		return new WeakReference(nav);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static async Task<WeakReference> LoadThenRemoveNavigationBar()
	{
		var container = new ContentControl();
		var nav = new NavigationBar { Content = "Title" };
		var navRef = new WeakReference(nav);

		container.Content = nav;
		await UnitTestUIContentHelperEx.SetContentAndWait(container);

		container.Content = null;
		await UnitTestUIContentHelperEx.WaitForIdle();

		return navRef;
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
#endif
