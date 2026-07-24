#if DEBUG
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

/// <summary>
/// Guards that <see cref="ExtendedSplashScreen"/> releases its process-lifetime static <c>Instance</c>
/// (and its splash content) when it unloads. Otherwise the last-created splash screen — and, when it
/// belongs to a previewed app loaded into a collectible AssemblyLoadContext, that app's whole ALC —
/// stays pinned for the process lifetime.
/// </summary>
[TestClass]
[RunsOnUIThread]
public class ExtendedSplashScreenLeakTests
{
	[TestMethod]
#if __ANDROID__ || __IOS__
	[Ignore("Splash screen control hosting differs on native platforms.")]
#endif
	public async Task Instance_IsReleased_OnUnloaded()
	{
		var container = new ContentControl();

		var splash = new ExtendedSplashScreen { Platforms = SplashScreenPlatform.None };
		container.Content = splash;
		await UnitTestUIContentHelperEx.SetContentAndWait(container);

		Assert.IsTrue(
			ExtendedSplashScreen.TestHook_HasInstance,
			"The static Instance should reference the loaded splash screen.");

		container.Content = null;
		await UnitTestUIContentHelperEx.WaitFor(() => !ExtendedSplashScreen.TestHook_HasInstance);

		Assert.IsFalse(
			ExtendedSplashScreen.TestHook_HasInstance,
			"The static Instance should be released once the splash screen unloads.");
	}

	[TestMethod]
#if __ANDROID__ || __IOS__
	[Ignore("Splash screen control hosting differs on native platforms.")]
#endif
	public async Task SplashScreen_IsCollectible_AfterUnloaded()
	{
		var splashRef = await LoadAndUnloadSplash();

		await CollectAndWait();

		Assert.IsFalse(
			splashRef.IsAlive,
			"An ExtendedSplashScreen should be collectible after unloading; if it survives, the static " +
			"Instance (or retained splash content) is still rooting it.");
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static async Task<WeakReference> LoadAndUnloadSplash()
	{
		var container = new ContentControl();
		var splash = new ExtendedSplashScreen { Platforms = SplashScreenPlatform.None };
		var splashRef = new WeakReference(splash);

		container.Content = splash;
		await UnitTestUIContentHelperEx.SetContentAndWait(container);

		container.Content = null;
		await UnitTestUIContentHelperEx.WaitFor(() => !ExtendedSplashScreen.TestHook_HasInstance);

		return splashRef;
	}

	private static async Task CollectAndWait()
	{
		for (var i = 0; i < 8; i++)
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			await UnitTestUIContentHelperEx.WaitForIdle();
		}
	}
}
#endif
