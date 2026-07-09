#if DEBUG && (HAS_UNO || !IS_UWP)
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Foundation;
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
/// Guards that <see cref="ResponsiveExtension"/> does not accumulate dead instances in its
/// process-lifetime statics. In a fixed-size preview canvas the window never resizes, so the
/// <c>WindowSizeChanged</c>-driven cleanup never runs; dead extensions would otherwise pile up forever,
/// each pinning its target/host graph (and, across a collectible AssemblyLoadContext boundary, the
/// previewed app's ALC).
/// </summary>
[TestClass]
[RunsOnUIThread]
internal class ResponsiveExtensionsLeakTests
{
	private readonly static ResponsiveLayout DefaultLayout = ResponsiveLayout.Create(150, 300, 600, 800, 1080);

	[TestMethod]
	public async Task SweepDeadInstances_Prunes_CollectedTrackedInstances()
	{
		var before = ResponsiveExtension.TestHook_TrackedInstanceCount;

		// Add a tracked entry whose extension is collectible and then goes away (modelling an extension
		// abandoned without a graceful Uninstall/Unloaded — e.g. an abrupt ALC teardown). The dead
		// weak-reference tuple must not linger in the process-lifetime list.
		AddCollectibleTrackedEntry();

		Assert.AreEqual(
			before + 1,
			ResponsiveExtension.TestHook_TrackedInstanceCount,
			"A tracked entry should have been added.");

		await CollectAndWait();
		ResponsiveExtension.TestHook_SweepDeadInstances();

		Assert.AreEqual(
			before,
			ResponsiveExtension.TestHook_TrackedInstanceCount,
			"After the extension is collected, SweepDeadInstances should prune the dead tracked entry " +
			"back to the pre-install count.");
	}

	// Mirrors the shape of ResponsiveExtension.Initialize()'s TrackedInstances.Add call, but with
	// throwaway collectible targets so the entry becomes dead once this frame returns. (A real
	// ResponsiveExtension is also rooted by HardSelfReferences until swept, so a plain object is used
	// here to isolate the TrackedInstances-pruning behaviour.)
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void AddCollectibleTrackedEntry()
	{
		var owner = new object();
		var extension = new object();
		ResponsiveExtension.TrackedInstances.Add((new WeakReference(owner), "Text", new WeakReference(extension, trackResurrection: true)));
	}

	[TestMethod]
	public async Task Connected_Extension_IsCollectible_WithoutResizeOrExplicitUninstall()
	{
		// Neither a resize (fixed preview canvas) nor an explicit Uninstall happens here. The extension
		// must still be collectible once its host is gone, because the static WindowSizeChanged event only
		// holds a weak reference to it.
		var extensionRef = await InstallAndAbandon();

		await CollectAndWait();

		Assert.IsFalse(
			extensionRef.IsAlive,
			"A connected ResponsiveExtension should be collectible after its host is gone even without a " +
			"resize or Uninstall; if it survives, the static WindowSizeChanged event is strongly rooting it.");
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static async Task<WeakReference> InstallAndAbandon()
	{
		var container = new ContentControl();
		var host = new TextBlock { Text = "Uninitialized" };
		container.Content = host;
		await UnitTestUIContentHelperEx.SetContentAndWait(container);

		var markup = new ResponsiveExtension { Layout = DefaultLayout, Narrow = "Narrow", Wide = "Wide" };
		var extensionRef = new WeakReference(markup);

		// Connect via a loaded host, then unload it — but intentionally never call Uninstall and never resize.
		ResponsiveExtension.Install(host, null, nameof(host.Text), markup);
		Assert.IsTrue(markup.IsConnected, "Extension should be connected once installed on a loaded host.");

		container.Content = null;
		await UnitTestUIContentHelperEx.WaitForIdle();

		return extensionRef;
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
