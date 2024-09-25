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

			SUT.ResetZoom();
			SUT.ZoomLevel.Should().Be(1.0);
			SUT.HorizontalOffset.Should().Be(0);
			SUT.VerticalOffset.Should().Be(0);
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

			SUT.IsHorizontalScrollBarVisible.Should().BeFalse();
			SUT.IsVerticalScrollBarVisible.Should().BeFalse();

			SUT.ZoomLevel = 2.0;
			SUT.IsHorizontalScrollBarVisible.Should().BeTrue();
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

			var presenter = SUT.FindFirstDescendant<ContentPresenter>("PART_Presenter");
			var translation = (presenter?.RenderTransform as TransformGroup)?.Children[1] as TranslateTransform
				?? throw new Exception("Failed to find PART_Presenter's TranslateTransform");

			SUT.HorizontalOffset = 50;
			SUT.VerticalOffset = 50;

			translation.X.Should().Be(50);
			translation.Y.Should().Be(50);
		}
	}
}
