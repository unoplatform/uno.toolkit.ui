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
}
