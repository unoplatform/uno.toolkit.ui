using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.UI.RuntimeTests;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.Extensions;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
public class DrawerTests
{
	[TestMethod]
#if __IOS__
	[Ignore("Opacity is not supposed to be modified from non ui-thread")]
#endif
	public async Task IsOpen_FromNonUIThread()
	{
		var drawer = new DrawerControl
		{
			Content = new Grid()
		};

		// don't wait for loaded, start the task immediately
		UIHelper.Content = drawer;
		await Task.Run(async () =>
		{
			drawer.IsOpen = true;

			await UIHelper.WaitForLoaded(drawer);
			await UIHelper.WaitForIdle();
			await UnitTestUIContentHelperEx.WaitFor(() => drawer.AnimationStoryboard?.GetCurrentState() == ClockState.Stopped);

			drawer.IsOpen = false;
		});

		// leave time for IsOpen=false (animation or not) to finish (if it doesn't throw)
		await UIHelper.WaitForIdle();
		await UnitTestUIContentHelperEx.WaitFor(() => drawer.AnimationStoryboard?.GetCurrentState() == ClockState.Stopped);

		var lightDismissOverlay = drawer.GetFirstDescendant<Border>(x => x.Name == DrawerControl.TemplateParts.LightDismissOverlayName) ??
			throw new Exception($"Failed to find {DrawerControl.TemplateParts.LightDismissOverlayName}");

		await UnitTestUIContentHelperEx.WaitFor(
			() => lightDismissOverlay.Opacity == 0,
			message: $"Expected lightDismissOverlay.Opacity to be 0, got {lightDismissOverlay.Opacity}");
	}
}
