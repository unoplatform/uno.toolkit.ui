using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Windows.Foundation;
using FluentAssertions;
using Uno.UI.RuntimeTests;

#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	public class ZoomContentControlAdditionalMarginTests
	{
		// Repro + regression guard: a change to AdditionalMargin must re-run the auto fit/center pass,
		// consistent with a viewport-size change. Before the fix, OnAdditionalMarginChanged only refreshed
		// the scrollbars, so the content stayed fitted against the previous margin (zoom unchanged).
		[TestMethod]
		public async Task When_AdditionalMarginIncreases_AndAutoFit_ThenRefitsToSmallerZoom()
		{
			var SUT = new ZoomContentControl
			{
				Width = 400,
				Height = 400,
				MinZoomLevel = 0.1,
				MaxZoomLevel = 10,
				AutoFitToCanvas = true,
				Content = new Border
				{
					Width = 100,
					Height = 100,
					Background = new SolidColorBrush(Colors.Blue),
				},
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			// The content (smaller than the viewport) is fitted on load.
			var fittedZoom = SUT.ZoomLevel;
			fittedZoom.Should().BeGreaterThan(0, "the content should be fitted into the viewport on load");

			// Act — reserve a large inset on every side, shrinking the usable viewport.
			SUT.AdditionalMargin = new Thickness(100);

			// Assert — the content is re-fitted into the reduced usable viewport, so the fitted zoom
			// decreases. Before the fix the margin change did not re-fit and the zoom stayed at fittedZoom.
			SUT.ZoomLevel.Should().BeLessThan(
				fittedZoom,
				"a larger AdditionalMargin reduces the usable viewport, so AutoFitToCanvas must re-fit to a smaller zoom");
		}

		// Guard: the re-fit is opt-in. With AutoFitToCanvas disabled, a margin change must not force a
		// re-fit (same contract as a viewport-size change).
		[TestMethod]
		public async Task When_AdditionalMarginChanges_AndAutoFitDisabled_ThenZoomUnchanged()
		{
			var SUT = new ZoomContentControl
			{
				Width = 400,
				Height = 400,
				MinZoomLevel = 0.1,
				MaxZoomLevel = 10,
				AutoFitToCanvas = false,
				Content = new Border
				{
					Width = 100,
					Height = 100,
					Background = new SolidColorBrush(Colors.Blue),
				},
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			var zoomBefore = SUT.ZoomLevel;

			// Act
			SUT.AdditionalMargin = new Thickness(100);

			// Assert — no forced re-fit when AutoFitToCanvas is off.
			SUT.ZoomLevel.Should().Be(
				zoomBefore,
				"with AutoFitToCanvas disabled, a margin change must not change the zoom");
		}
	}
}
