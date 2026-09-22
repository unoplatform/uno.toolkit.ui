#if HAS_UNO || !IS_UWP
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
/// Guards that <see cref="ResponsiveExtension"/> keeps working across an unload/reload of its host. A host
/// that leaves the tree is not necessarily a dead one: a designer or preview surface re-parents live content,
/// a <c>ContentControl</c> swaps its content back, a <c>Frame</c> re-shows a cached page. Such a host loads
/// again and its responsive values must follow the size in effect then, not the one resolved at first load.
/// </summary>
[TestClass]
[RunsOnUIThread]
internal class ResponsiveExtensionsReconnectTests
{
	private static readonly ResponsiveLayout DefaultLayout = ResponsiveLayout.Create(150, 300, 600, 800, 1080);
	private static readonly Size NarrowSize = new(300, 400);
	private static readonly Size WideSize = new(900, 400);

	[TestMethod]
	public async Task Reparented_Host_ResolvesForTheSizeInEffectOnReload()
	{
		var provider = new ResponsiveSizeProvider { Size = WideSize };
		ResponsiveHelper.SetOverrideSizeProvider(provider);
		try
		{
			var container = new ContentControl();
			var host = new TextBlock { Text = "Uninitialized" };
			container.Content = host;
			await UnitTestUIContentHelperEx.SetContentAndWait(container);

			var markup = new ResponsiveExtension { Layout = DefaultLayout, Narrow = "Narrow", Wide = "Wide" };
			Assert.IsTrue(
				ResponsiveExtension.Install(host, null, nameof(host.Text), markup),
				"The extension should install on the TextBlock's Text property.");
			Assert.AreEqual("Wide", host.Text, "Precondition: the host resolves against the wide size it loaded at.");

			// Re-parent: out of the tree, size changes while it is detached, back into the tree. The size
			// change is deliberately made while detached — that is what a live host would miss, and what the
			// reload has to make up for.
			container.Content = null;
			await UnitTestUIContentHelperEx.WaitForIdle();

			provider.Size = NarrowSize;
			await UnitTestUIContentHelperEx.WaitForIdle();

			container.Content = host;
			await UnitTestUIContentHelperEx.WaitForIdle();

			Assert.AreEqual(
				"Narrow",
				host.Text,
				"A re-parented host must reconnect on its next Loaded and re-resolve against the current size; " +
				"keeping the value from before the unload leaves it frozen for the rest of its life.");
			Assert.IsTrue(markup.IsConnected, "The reconnected extension should be subscribed to size changes again.");

			// And it follows size changes from then on, like a freshly loaded host does.
			provider.Size = WideSize;
			await UnitTestUIContentHelperEx.WaitForIdle();

			Assert.AreEqual("Wide", host.Text, "After reconnecting, the extension should follow later size changes.");
		}
		finally
		{
			ResponsiveHelper.SetOverrideSizeProvider(null);
		}
	}
}
#endif
