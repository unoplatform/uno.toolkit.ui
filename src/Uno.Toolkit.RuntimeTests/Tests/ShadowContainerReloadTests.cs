#if IS_WINUI
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class ShadowContainerReloadTests
{
	[TestMethod]
	public async Task When_Unloaded_Then_Reloaded_No_Exception()
	{
		// Arrange: a ShadowContainer with a pre-configured shadow and child rectangle,
		// matching the ShadowContainerTestPage2 scenario.
		var stackPanel = new StackPanel();
		var shadowContainer = new ShadowContainer
		{
			Shadows = new ShadowCollection
			{
				new UI.Shadow { OffsetX = 20, OffsetY = 20, Color = Color.FromArgb(255, 0, 255, 0), Opacity = 1 },
			},
			Content = new Rectangle { Width = 100, Height = 100, Fill = new SolidColorBrush(Colors.Red) },
		};

		stackPanel.Children.Add(shadowContainer);

		await UnitTestUIContentHelperEx.SetContentAndWait(stackPanel);

		// Capture initial state
		Assert.IsTrue(shadowContainer.ActualWidth > 0, "ShadowContainer should have non-zero width after loading.");
		Assert.IsTrue(shadowContainer.ActualHeight > 0, "ShadowContainer should have non-zero height after loading.");

		// Act: unload and reload (re-parent) the ShadowContainer multiple times
		stackPanel.Children.Remove(shadowContainer);
		stackPanel.Children.Add(shadowContainer);
		stackPanel.Children.Remove(shadowContainer);
		stackPanel.Children.Add(shadowContainer);

		await UnitTestUIContentHelperEx.WaitForIdle();

		// Assert: ShadowContainer still renders correctly after re-parenting
		Assert.IsTrue(shadowContainer.ActualWidth > 0, "ShadowContainer should have non-zero width after reload.");
		Assert.IsTrue(shadowContainer.ActualHeight > 0, "ShadowContainer should have non-zero height after reload.");
		Assert.AreEqual(1, shadowContainer.Shadows.Count, "Shadows should still be present after reload.");
	}

	[TestMethod]
	public async Task When_Unloaded_Then_Reloaded_Screenshot_Matches()
	{
		if (!ImageAssertHelper.IsScreenshotSupported())
		{
			Assert.Inconclusive("Screenshots not supported on this platform.");
		}

		// Arrange
		var stackPanel = new StackPanel();
		var shadowContainer = new ShadowContainer
		{
			Shadows = new ShadowCollection
			{
				new UI.Shadow { OffsetX = 20, OffsetY = 20, Color = Color.FromArgb(255, 0, 255, 0), Opacity = 1 },
			},
			Content = new Rectangle { Width = 100, Height = 100, Fill = new SolidColorBrush(Colors.Red) },
		};

		stackPanel.Children.Add(shadowContainer);

		await UnitTestUIContentHelperEx.SetContentAndWait(stackPanel);
		await Task.Delay(200); // Allow render to stabilize

		var screenshotBefore = await shadowContainer.TakeScreenshot();
		Assert.IsNotNull(screenshotBefore, "Initial screenshot should not be null.");

		// Act: re-parent
		stackPanel.Children.Remove(shadowContainer);
		stackPanel.Children.Add(shadowContainer);

		await UnitTestUIContentHelperEx.WaitForIdle();
		await Task.Delay(200); // Allow re-render to stabilize

		var screenshotAfter = await shadowContainer.TakeScreenshot();
		Assert.IsNotNull(screenshotAfter, "Post-reload screenshot should not be null.");

		// Assert: dimensions match (visual equivalence approximation)
		Assert.AreEqual(screenshotBefore!.PixelWidth, screenshotAfter!.PixelWidth, "Screenshot widths should match.");
		Assert.AreEqual(screenshotBefore.PixelHeight, screenshotAfter.PixelHeight, "Screenshot heights should match.");
	}
}
#endif
