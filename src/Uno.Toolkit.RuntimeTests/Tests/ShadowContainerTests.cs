#if IS_WINUI
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Disposables;
using Uno.Toolkit.RuntimeTests.Extensions;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.RuntimeTests.Tests.TestPages;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Windows.System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.UI;
using Windows.Graphics.Imaging;
using Windows.Storage;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;



using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI.ViewManagement;
using FluentAssertions;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using System.Drawing;
using Windows.Globalization.DateTimeFormatting;

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	internal partial class ShadowContainerTests
	{

#if !(__ANDROID__ || __IOS__)

		[TestMethod]
		[DataRow(10, 10, false, 0)]
		[DataRow(-10, -10, false, 0)]
		[DataRow(10, 10, true, 0)]
		[DataRow(-10, -10, true, 0)]
		[DataRow(10, 10, false, 100)]
		[DataRow(-10, -10, false, 100)]
		[DataRow(10, 10, true, 100)]
		[DataRow(-10, -10, true, 100)]
		public async Task ShadowsCornerRadius_Content(int offsetX, int offsetY, bool inner, double bottomRightCorner)
		{
			if (!ImageAssertHelper.IsScreenshotSupported())
			{
				Assert.Inconclusive(); // System.NotImplementedException: RenderTargetBitmap is not supported on this platform.;
			}

			var shadowContainer = new ShadowContainer
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top,
				Background = new SolidColorBrush(Colors.Green),
				Content = new Border { Height = 200, Width = 200, CornerRadius = new CornerRadius(0, 0, bottomRightCorner, 0) }
			};

			shadowContainer.Shadows.Add(new UI.Shadow
			{
				Color = Colors.Red,
				OffsetX = offsetX,
				OffsetY = offsetY,
				IsInner = inner,
				Opacity = 1,
			});

			var stackPanel = new StackPanel
			{
				Width = 220,
				Height = 220,
				Padding = new Thickness(10),
				Background = new SolidColorBrush(Colors.Yellow),
				Children =
				{
					shadowContainer,
				}
			};
			var absOffsetX = Math.Abs(offsetX);
			var absOffsetY = Math.Abs(offsetY);
			var canvasMargin = absOffsetX + absOffsetY;
			UnitTestsUIContentHelper.Content = stackPanel;

			await UnitTestsUIContentHelper.WaitForIdle();
			await UnitTestsUIContentHelper.WaitForLoaded(stackPanel);
			await Task.Yield();

			//Validate current structure.
			var grid = shadowContainer.GetChildren().First() as Grid;
			var canvas = grid?.GetChildren().First() as Canvas;
			var skXamlCanvas = canvas?.GetChildren().First() as SKXamlCanvas;
			var contentPresenter = grid?.GetChildren().Skip(1).First() as ContentPresenter;
			var border = contentPresenter?.GetChildren().First() as Border;

			//Validate element measurements
			Assert.AreEqual(grid?.ActualWidth, canvas?.ActualWidth);
			Assert.AreEqual(canvas?.ActualWidth, border?.ActualWidth);
			Assert.AreEqual(skXamlCanvas?.ActualWidth, border?.ActualWidth + canvasMargin);

			//Validate point colors
			var renderer = await stackPanel.TakeScreenshot();

			//Set 4 coners positions to be validated
			var leftX = inner ? absOffsetX + 1 : 1;
			var rightX = (int)((stackPanel?.ActualWidth ?? 0) - (inner ? absOffsetX + 1 : 1));
			var topY = inner ? absOffsetY + 1 : 1;
			var bottomY = (int)((stackPanel?.ActualHeight ?? 0) - (inner ? absOffsetX + 1 : 1));


			var topLeftColor =
				inner
					? (offsetX < 0 ? Colors.Green : Colors.Red)
					: (offsetX < 0 ? Colors.Red : Colors.Yellow);
			await renderer.AssertColorAt(topLeftColor, leftX, topY);

			//TopRight
			var topRightColor = inner ? Colors.Red : Colors.Yellow;
			await renderer.AssertColorAt(topRightColor, rightX, topY);

			//BottomRight and CornerCurve
			//Case we have RightCorner the Bottom will always be Yellow
			var bottomRightColor =
				bottomRightCorner > 50
					? Colors.Yellow
					: offsetX < 0
						? inner ? Colors.Red : Colors.Yellow
						: inner ? Colors.Green : Colors.Red;
			await renderer.AssertColorAt(bottomRightColor, rightX, bottomY);

			//BottomLeft
			await renderer.AssertColorAt(Colors.Yellow, leftX, bottomY);

		}

		[TestMethod]
		public async Task Displays_Content_With_Margin()
		{
			var shadowContainer = new ShadowContainer
			{
				Content = new Border
				{
					Margin = new Thickness(50),
					Height = 50,
					Width = 50,
					Background = new SolidColorBrush(Colors.Green)
				}
			};

			var stackPanel = new StackPanel
			{
				Background = new SolidColorBrush(Colors.Yellow),
				HorizontalAlignment = HorizontalAlignment.Center,
				Children =
				{
					new Border { Height = 150, Width = 150, Background = new SolidColorBrush(Colors.Red) },
					shadowContainer,
					new Border { Height = 150, Width = 150, Background = new SolidColorBrush(Colors.Red) },
				}
			};

			UnitTestsUIContentHelper.Content = stackPanel;

			await UnitTestsUIContentHelper.WaitForIdle();
			await UnitTestsUIContentHelper.WaitForLoaded(stackPanel);

			var renderer = await stackPanel.TakeScreenshot();
			await renderer.AssertColorAt(Colors.Green, 75, 225);
		}

		[TestMethod]
		[DataRow(10, 10, false)]
		[DataRow(10, 10, true)]
		[DataRow(-10, -10, false)]
		[DataRow(-10, -10, true)]
		[DataRow(-10, 10, true)]
		[DataRow(10, -10, true)]
		[DataRow(-10, 10, false)]
		[DataRow(10, -10, false)]
		public async Task Outer_Shadows(int offsetX, int offsetY, bool inner)
		{
			if (!ImageAssertHelper.IsScreenshotSupported())
			{
				Assert.Inconclusive(); // System.NotImplementedException: RenderTargetBitmap is not supported on this platform.;
			}

			var parentBorder = new Border {Height = 500, Width = 500, HorizontalAlignment = HorizontalAlignment.Center, Background = new SolidColorBrush(Colors.Yellow) };
			var border = new Border { HorizontalAlignment = HorizontalAlignment.Center,  Height = 200, Width = 200, Background = new SolidColorBrush(Colors.Green) };
			var shadowContainer = new ShadowContainer
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				Content = border,
			};

			shadowContainer.Shadows.Add(new UI.Shadow
			{
				Color = Colors.Red,
				OffsetX = offsetX,
				OffsetY = offsetY,
				IsInner = inner,
				Opacity = 1,
			});

			parentBorder.Child = shadowContainer;

			UnitTestsUIContentHelper.Content = parentBorder;

			await UnitTestsUIContentHelper.WaitForIdle();
			await UnitTestsUIContentHelper.WaitForLoaded(shadowContainer);

			var renderer = await parentBorder.TakeScreenshot();
			var bounds = border.TransformToVisual(shadowContainer).TransformBounds(new Windows.Foundation.Rect(0, 0, border.ActualWidth, border.ActualHeight));

			var xStart = offsetX < 0 ? (int)bounds.Left : (int)bounds.Right;
			var yStart = offsetY < 0 ? (int)bounds.Top : (int)bounds.Bottom;

			await renderer.AssertColorAt(Colors.Green, 100, 100);
			await renderer.AssertColorAt(Colors.Red, 210, 100);

			for (int x = 1; x <= Math.Abs(offsetX); x++)
			{
				x = inner ? x * -1 : x;
				await renderer.AssertColorAt(Colors.Red, xStart + x, (int)bounds.Height / 2);
			}

			for (int y = 1; y <= Math.Abs(offsetY); y++)
			{
				y = inner ? y * -1 : y;
				await renderer.AssertColorAt(Colors.Red, (int)bounds.Width / 2, yStart + y);
			}
		}
#endif

		[TestMethod]
		[Ignore("https://github.com/unoplatform/uno.toolkit.ui/issues/887")]
		public async Task Displays_Content()
		{
			if (!ImageAssertHelper.IsScreenshotSupported())
			{
				Assert.Inconclusive(); // System.NotImplementedException: RenderTargetBitmap is not supported on this platform.;
			}

			var greenBorder = new ShadowContainer
			{
				Content = new Border { Height = 200, Width = 200, Background = new SolidColorBrush(Colors.Green) }
			};

			var shadowContainer = new ShadowContainer
			{
				Content = greenBorder
			};

			var stackPanel = new StackPanel
			{
				Background = new SolidColorBrush(Colors.Yellow),
				HorizontalAlignment = HorizontalAlignment.Center,
				Children =
				{
					new Border { Height = 200, Width = 200, Background = new SolidColorBrush(Colors.Red) },
					shadowContainer,
					new Border { Height = 200, Width = 200, Background = new SolidColorBrush(Colors.Red) },
				}
			};

			UnitTestsUIContentHelper.Content = stackPanel;

			await UnitTestsUIContentHelper.WaitForIdle();
			await UnitTestsUIContentHelper.WaitForLoaded(stackPanel);

			var renderer = await stackPanel.TakeScreenshot();
			await renderer.AssertColorAt(Colors.Green, 100, 300);
		}

		[TestMethod]
		public async Task ShadowContainer_ReLayoutsAfterChangeInSize()
		{
			ShadowContainer.ClearCache();

			var internalBorder = new Border { Height = 200, Width = 200, Background = new SolidColorBrush(Colors.Green) };
			bool createdNewCanvas = false;

			var shadowContainer = new ShadowContainer
			{
				Content = internalBorder,
				Shadows = new ShadowCollection
				{
					new UI.Shadow
					{
						Color = Colors.Blue,
						OffsetX = 10,
						OffsetY = 10,
						IsInner = false,
						Opacity = 1,

					}
				}
			};

			UnitTestsUIContentHelper.Content = shadowContainer;

			shadowContainer.SurfacePaintCompleted += SurfacePaintCompleted;

			shadowContainer.Shadows.Add(new UI.Shadow
			{
				Color = Colors.Red,
				OffsetX = 10,
				OffsetY = 10,
				IsInner = false,
				Opacity = 1,
			});

			await UnitTestsUIContentHelper.WaitForIdle();
			await UnitTestsUIContentHelper.WaitForLoaded(shadowContainer);
			createdNewCanvas.Should().BeTrue();
			createdNewCanvas = false;

			// Changing the height of the children content of the shadow. 
			internalBorder.Height = 400;

			await UnitTestsUIContentHelper.WaitForIdle();

			createdNewCanvas.Should().BeTrue();

			shadowContainer.SurfacePaintCompleted -= SurfacePaintCompleted; ;

			void SurfacePaintCompleted(object? sender, ShadowContainer.SurfacePaintCompletedEventArgs args)
			{
				// OnSurfacePainted can be called several times, we must make sure that the canvas is regenerated at least once.
				if (createdNewCanvas == false)
				{
					createdNewCanvas = args.CreatedNewCanvas;
				}
			}
		}
	}
}
#endif
