#if DEBUG
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
/// Guards that <see cref="ResponsiveHelper"/> does not permanently root its process-lifetime
/// default size provider (and, through it, the host <see cref="XamlRoot"/>) via a strong
/// <see cref="XamlRoot.Changed"/> subscription. This matters for a downstream host that loads
/// previewed apps into their own collectible AssemblyLoadContexts, where such a pin would keep
/// the Toolkit ALC alive for the whole process lifetime.
/// </summary>
[TestClass]
[RunsOnUIThread]
internal class ResponsiveHelperLeakTests
{
	[TestMethod]
	public async Task Reset_TearsDown_DefaultProvider()
	{
		try
		{
			var host = new Border();
			await UnitTestUIContentHelperEx.SetContentAndWait(host);

			var xamlRoot = host.XamlRoot ?? throw new InvalidOperationException("XamlRoot was not available on the loaded host.");

			ResponsiveHelper.InitializeIfNeeded(xamlRoot);
			Assert.IsTrue(ResponsiveHelper.TestHook_IsDefaultProviderInitialized, "Default provider should be initialized after InitializeIfNeeded.");

			ResponsiveHelper.Reset();
			Assert.IsFalse(ResponsiveHelper.TestHook_IsDefaultProviderInitialized, "Default provider should be torn down after Reset.");

			// A subsequent initialize must re-create a fresh provider (i.e. Reset did not leave the
			// singleton in a half-initialized state).
			ResponsiveHelper.InitializeIfNeeded(xamlRoot);
			Assert.IsTrue(ResponsiveHelper.TestHook_IsDefaultProviderInitialized, "Default provider should be re-initialized after a Reset + InitializeIfNeeded cycle.");
		}
		finally
		{
			ResponsiveHelper.Reset();
		}
	}

	[TestMethod]
	public async Task DefaultProvider_IsCollectible_WhileXamlRootAlive()
	{
		// The host XamlRoot is process/window-owned and outlives previewed apps. Even while it is kept
		// alive and subscribed via XamlRoot.Changed, the default size provider must remain collectible
		// once Reset() drops the static reference — proving the subscription is weak/self-detaching and
		// not a strong closure pin.
		try
		{
			var host = new Border();
			await UnitTestUIContentHelperEx.SetContentAndWait(host);

			// Keep the XamlRoot strongly alive for the duration of the test.
			var xamlRoot = host.XamlRoot ?? throw new InvalidOperationException("XamlRoot was not available on the loaded host.");

			var providerRef = InitializeAndTrack(xamlRoot);

			// Drop the static reference to the default provider (mirrors a host tearing down between app loads).
			ResponsiveHelper.Reset();

			await CollectAndWait();

			Assert.IsFalse(
				providerRef.IsAlive,
				"The default ResponsiveSizeProvider should be collectible after Reset while the XamlRoot is still alive; " +
				"if it survives, the XamlRoot.Changed subscription is strongly rooting it.");
		}
		finally
		{
			ResponsiveHelper.Reset();
		}
	}

	// Kept in a separate non-inlined frame so the just-created provider is not kept alive by a local
	// on the test method's stack while we assert collection.
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static WeakReference InitializeAndTrack(XamlRoot xamlRoot)
	{
		ResponsiveHelper.InitializeIfNeeded(xamlRoot);
		var provider = ResponsiveHelper.TestHook_GetDefaultProvider();
		Assert.IsNotNull(provider, "Default provider should be available immediately after InitializeIfNeeded.");
		return new WeakReference(provider);
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
