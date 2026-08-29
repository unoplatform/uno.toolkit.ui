using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Windows.Foundation;
using FluentAssertions;
using Uno.UI.RuntimeTests;
using Uno.UI.Extensions;
using Uno.Toolkit.RuntimeTests.Tests;

#if IS_WINUI
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	public class ZoomContentControlSafetyTests
	{
		// Regression guard: ZoomContentControl raises RenderedContentUpdated from fire-and-forget
		// async void paths (UpdateScrollDetails, OnZoomLevelChanged) after a Task.Yield. Before the
		// fix, a throwing consumer handler resumed there was an unobserved exception and crashed
		// the process. The control must contain and log it instead.
		[TestMethod]
		public async Task When_EventHandlerThrows_ShouldNotCrash()
		{
			var content = new Border
			{
				Width = 100,
				Height = 100,
				Background = new SolidColorBrush(Colors.Blue),
			};
			var SUT = new ZoomContentControl
			{
				Width = 400,
				Height = 400,
				AutoFitToCanvas = true,
				Content = content,
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			var invoked = false;
			SUT.RenderedContentUpdated += (s, e) =>
			{
				invoked = true;
				throw new InvalidOperationException("intentional test exception");
			};

			// Act — a content resize triggers the fire-and-forget scroll-details pass
			// (and, via AutoFitToCanvas, the zoom-level-changed pass).
			content.Width = 150;
			await UnitTestUIContentHelperEx.WaitFor(() => invoked, message: "RenderedContentUpdated should have been raised");
			await UnitTestUIContentHelperEx.WaitForIdle();

			// Assert — completing without faulting the app proves the exception was contained.
			invoked.Should().BeTrue();
		}
	}

	[TestClass]
	[RunsOnUIThread]
	public class ZoomContentControlCenteringTests
	{
		// Regression guard for the "content jumps when zooming across the fit boundary" bug:
		// OnZoomLevelChanged captured VectoredScrollValue AFTER ZoomLevel had already changed,
		// so the translation direction K (whose sign flips when the scaled content outgrows the
		// viewport, or vice versa) no longer matched the zoom level the scroll value was applied
		// under. Any non-mouse-wheel zoom (slider, buttons, FitToCanvas) crossing that boundary
		// then mis-derived the anchor and the content jumped off-center.

		[TestMethod]
		public async Task When_ZoomingIn_AcrossFitBoundary_ContentStaysCentered()
		{
			var SUT = await SetupCenteredAt(initialZoom: 3.5); // scaled 350x175, fits within viewport

			SUT.ZoomLevel = 9; // scaled 900x450, overflows viewport on both axes
			await UnitTestUIContentHelperEx.WaitForIdle();

			AssertCentered(SUT);
		}

		[TestMethod]
		public async Task When_ZoomingOut_AcrossFitBoundary_ContentStaysCentered()
		{
			var SUT = await SetupCenteredAt(initialZoom: 9); // scaled 900x450, overflows viewport

			SUT.ZoomLevel = 2; // scaled 200x100, fits within viewport
			await UnitTestUIContentHelperEx.WaitForIdle();

			AssertCentered(SUT);
		}

		// Regression guard for the AdditionalMargin variant of the same jump: CalculateNewOffset
		// re-added the margin's final offset directly onto the returned scroll value, but scroll
		// values live in translation-space multiplied by K — so that was only correct while the
		// padded content overflowed the viewport (K=-1). With K=+1 (content fits), every
		// non-mouse-wheel zoom landed off-center by exactly twice the margin.
		[TestMethod]
		public async Task When_ZoomingWithAdditionalMargin_ContentStaysCentered()
		{
			var SUT = await SetupCenteredAt(initialZoom: 2, additionalMargin: 48); // padded 296x196, fits

			SUT.ZoomLevel = 3; // padded 396x246: overflows on X (K=-1), still fits on Y (K=+1)
			await UnitTestUIContentHelperEx.WaitForIdle();

			AssertCentered(SUT);
		}

		// A 400x400 control with 100x50 content: with the viewport (~400 minus scrollbars) the
		// fit boundary is crossed per-axis between the zoom levels used by the tests above.
		private static async Task<ZoomContentControl> SetupCenteredAt(double initialZoom, double additionalMargin = 0)
		{
			var content = new Border
			{
				Width = 100,
				Height = 50,
				Background = new SolidColorBrush(Colors.Blue),
			};
			var SUT = new ZoomContentControl
			{
				Width = 400,
				Height = 400,
				// auto fit/center off: nothing recenters behind the test's back,
				// so the assert observes the zoom-anchoring math alone.
				AutoFitToCanvas = false,
				AutoCenterContent = false,
				AdditionalMargin = new Thickness(additionalMargin),
				Content = content,
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			SUT.ZoomLevel = initialZoom;
			await UnitTestUIContentHelperEx.WaitForIdle();
			SUT.CenterContent();
			await UnitTestUIContentHelperEx.WaitForIdle();

			return SUT;
		}

		// Zooming anchors at the viewport center, so content centered before the zoom
		// must still be centered after: translation == (viewport - scaled content) / 2.
		private static void AssertCentered(ZoomContentControl SUT)
		{
			var translation = GetTranslation(SUT);
			var vp = SUT.ViewportSize;
			var scaled = SUT.ScaledContentSize;

			translation.X.Should().BeApproximately((vp.Width - scaled.Width) / 2, precision: 1.5);
			translation.Y.Should().BeApproximately((vp.Height - scaled.Height) / 2, precision: 1.5);
		}

		// Free panning (AllowFreePanning=true, the default) allows scroll values outside the
		// scrollbar range — mouse-wheel and pinch zooming rely on this to keep the focal point
		// stationary. The scrollbars' TwoWay Value bindings must not coerce those values back.
		[TestMethod]
		public async Task When_FreePanning_OutOfRangeScrollValue_IsHonored()
		{
			var SUT = await SetupCenteredAt(initialZoom: 2); // scaled 200x100, fits: scroll range is [0, slack]
			SUT.AllowFreePanning.Should().BeTrue();

			SUT.SetScrollValue(new Point(-50, -50), shouldClamp: false);
			await UnitTestUIContentHelperEx.WaitForIdle();

			var translation = GetTranslation(SUT);
			translation.X.Should().BeApproximately(-50, precision: 0.5);
			translation.Y.Should().BeApproximately(-50, precision: 0.5);
		}

		private static TranslateTransform GetTranslation(ZoomContentControl SUT) =>
			(SUT.GetFirstDescendant<ContentControl>(x => x.Name == "PART_InnerContentControl")?.RenderTransform as TransformGroup)?.Children[1] as TranslateTransform
				?? throw new InvalidOperationException("Failed to find PART_InnerContentControl's TranslateTransform");
	}

#if false
	[TestClass]
	[RunsOnUIThread]
	internal class ZoomContentControlTest
	{
		[TestMethod]
		public async Task When_ZoomIn_ShouldIncreaseZoomLevel()
		{
			var SUT = new ZoomContentControl()
			{
				Width = 400,
				Height = 300,
				MinZoomLevel = 1.0,
				MaxZoomLevel = 5.0,
				ZoomLevel = 1.5,
				IsZoomAllowed = true,
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			// the control will set an appropriate zoom level on load
			// based on available size & content size
			// so we need to force that to a known value here.
			SUT.ZoomLevel = 1.5;
			SUT.ZoomLevel.Should().Be(1.5);

			SUT.ZoomLevel += 0.5;
			SUT.ZoomLevel.Should().Be(2.0);

			// should be coerce back to MaxZoomLevel of 5
			SUT.ZoomLevel = 6.0;
			SUT.ZoomLevel.Should().Be(5.0);
		}

		[TestMethod]
		public async Task When_ZoomOut_ShouldDecreaseZoomLevel()
		{
			var SUT = new ZoomContentControl()
			{
				Width = 400,
				Height = 300,
				MinZoomLevel = 1.0,
				MaxZoomLevel = 5.0,
				ZoomLevel = 3.0,
				IsZoomAllowed = true,
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			// the control will set an appropriate zoom level on load
			// based on available size & content size
			// so we need to force that to a known value here.
			SUT.ZoomLevel = 3.0;
			SUT.ZoomLevel.Should().Be(3.0);

			SUT.ZoomLevel -= 0.5;
			SUT.ZoomLevel.Should().Be(2.5);

			// should be coerce back to MinZoomLevel of 1
			SUT.ZoomLevel = 0.5;
			SUT.ZoomLevel.Should().Be(1.0);
		}

		[TestMethod]
		public async Task When_Reset_ShouldResetZoomAndOffsets()
		{
			var SUT = new ZoomContentControl()
			{
				Width = 400,
				Height = 300,
				ZoomLevel = 2.0,
				HorizontalOffset = 50,
				VerticalOffset = 50,
				IsZoomAllowed = true,
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			// Perform a reset
			SUT.ResetZoom();
			SUT.ZoomLevel.Should().Be(1.0); //ZoomLevel resets to 1.0
			SUT.HorizontalOffset.Should().Be(0); //HorizontalOffset resets to 0
			SUT.VerticalOffset.Should().Be(0); //VerticalOffset resets to 0
		}

		[TestMethod]
		public async Task When_ContentBounds_ShouldHideScrollBars()
		{
			var SUT = new ZoomContentControl()
			{
				Width = 400,
				Height = 300,
				ZoomLevel = 1.0,
				IsZoomAllowed = true,
				Content = new Border
				{
					Width = 400 - 20, // the actual height/width is 12
					Height = 300 - 20,
					Background = new SolidColorBrush(Colors.Blue),
				},
			};
			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			//Expect no scrollbars when content fits within the bounds
			SUT.IsHorizontalScrollBarVisible.Should().BeFalse();
			SUT.IsVerticalScrollBarVisible.Should().BeFalse();

			//Zoom in to make content larger and display scrollbars
			SUT.ZoomLevel = 2.0;
			SUT.IsHorizontalScrollBarVisible.Should().BeTrue();
			SUT.IsVerticalScrollBarVisible.Should().BeTrue();
		}

		[TestMethod]
		public async Task When_VerticalOffset_ShouldUpdateCorrectly()
		{
			var SUT = new ZoomContentControl()
			{
				Width = 400,
				Height = 300,
				VerticalOffset = 50,
				IsPanAllowed = true,
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			// Set vertical offset
			SUT.VerticalOffset = 100;
			SUT.VerticalOffset.Should().Be(100);

			// Verify the content is scrolled correctly
			var presenter = SUT.FindFirstDescendant<ContentPresenter>("PART_Presenter");
			var translation = (presenter?.RenderTransform as TransformGroup)?.Children[1] as TranslateTransform
				?? throw new Exception("Failed to find PART_Presenter's TranslateTransform");

			translation.Y.Should().Be(100); // Verify that the content's Y translation is in sync with the vertical offset
		}

		[TestMethod]
		public async Task When_VerticalOffset_ExceedsLimits_ShouldShowScrollBars()
		{
			var SUT = new ZoomContentControl()
			{
				Width = 400,
				Height = 300,
				VerticalOffset = 0,
				IsPanAllowed = true,
				IsVerticalScrollBarVisible = true,
				Content = new Border
				{
					Width = 400 - 20, // Smaller content width
					Height = 500, // Larger content height
					Background = new SolidColorBrush(Colors.Blue),
				}
			};

			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

			// Simulate setting a large VerticalOffset
			SUT.VerticalOffset = 200;
			SUT.VerticalOffset.Should().Be(200);

			// Ensure scrollbar is shown when the content exceeds the bounds
			SUT.IsVerticalScrollBarVisible.Should().BeTrue();
		}

		[TestMethod]
		public async Task When_Pan_ShouldUpdateOffsets()
		{
			var SUT = new ZoomContentControl()
			{
				Width = 400,
				Height = 300,
				ZoomLevel = 1.0,
				IsPanAllowed = true,
				HorizontalOffset = 0,
				VerticalOffset = 0,
			};
 
			await UnitTestUIContentHelperEx.SetContentAndWait(SUT);
 
			var presenter = SUT.GetFirstDescendant<ContentPresenter>(x => x.Name == "PART_Presenter");
			var translation = (presenter?.RenderTransform as TransformGroup)?.Children[1] as TranslateTransform
				?? throw new Exception("Failed to find PART_Presenter's TranslateTransform");
 
			// Simulate panning
			SUT.HorizontalOffset = 50;
			SUT.VerticalOffset = 50;
 
			// Verify that the content's translate transform has been updated to reflect the new offsets
			translation.X.Should().Be(50);
			translation.Y.Should().Be(50);
		}
	}
#endif
}
