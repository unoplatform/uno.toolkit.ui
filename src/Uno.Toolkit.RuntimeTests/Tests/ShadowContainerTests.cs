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
using Uno.WinUI.Graphics2DSK;
using System.Drawing;
using Windows.Globalization.DateTimeFormatting;
using Windows.Devices.Haptics;

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	internal partial class ShadowContainerTests
	{

#if !(__ANDROID__ || __IOS__)
		[TestMethod]
		public async Task When_Element_Is_Resized()
		{
			var sp = new StackPanel()
			{
				Children =
				{
					new Border() { Width = 50, Height = 50 },
				}
			};

			var shadowContainer = new ShadowContainer
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top,
				Background = new SolidColorBrush(Colors.Green),
				Content = sp,
				Shadows =
				{
					new UI.Shadow()
					{
						BlurRadius = 20,
						OffsetX = 10,
						OffsetY = 10,
						Opacity = 0.5,
						Spread = 0,
						Color = Colors.Red,
					},
				}
			};

			var grid = new Grid()
			{
				RowDefinitions =
				{
					new RowDefinition()
					{
						Height = GridLength.Auto,
					},
					new RowDefinition()
					{
						Height = new GridLength(1, GridUnitType.Star),
					},
				},
				Children =
				{
					shadowContainer,
				},
			};

			UnitTestsUIContentHelper.Content = grid;
			await UnitTestsUIContentHelper.WaitForIdle();
			await UnitTestsUIContentHelper.WaitForLoaded(grid);

			var shadowContainerChildGrid = (Grid)shadowContainer.GetChildren().First();
			var canvas = (Canvas)shadowContainerChildGrid.GetChildren().First();
			var skiaCanvasElement = (FrameworkElement)canvas.GetChildren().First();
			Assert.IsTrue(
				SKCanvasElement.IsSupportedOnCurrentPlatform()
					? skiaCanvasElement is SKCanvasElement
					: skiaCanvasElement is SKXamlCanvas
			);

			var lastActualHeight = double.NaN;
			var paintTcs = new TaskCompletionSource<double>();
			shadowContainer.SurfacePaintCompleted += (a, b) =>
			{
				if (double.IsNaN(lastActualHeight) || skiaCanvasElement.ActualHeight > lastActualHeight)
				{
					lastActualHeight = skiaCanvasElement.ActualHeight;
					paintTcs.TrySetResult(lastActualHeight);
				}
			};

			shadowContainer.Shadows.Add(new UI.Shadow()
			{
				BlurRadius = 20,
				OffsetX = -10,
				OffsetY = -10,
				Opacity = 0.5,
				Spread = 0,
				Color = Colors.Blue,
			});

			// Adding 50x50 border to stack panel so that the stack panel itself is 50x100
			sp.Children.Add(new Border() { Width = 50, Height = 50 });

			await UnitTestsUIContentHelper.WaitForIdle();

			// Wait for the paint cycle to complete after the layout change
			var completedTask = await Task.WhenAny(paintTcs.Task, Task.Delay(5000));
			Assert.AreEqual(paintTcs.Task, completedTask, "SurfacePaintCompleted was not raised after resize");

			// The expected height is 160
			// 100 is the stack panel height, and (10 offset + 20 blur) (top and bottom)
			Assert.AreEqual(160d, lastActualHeight);
		}


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
			var skiaCanvasElement = canvas?.GetChildren().First() as FrameworkElement;
			Assert.IsTrue(
				SKCanvasElement.IsSupportedOnCurrentPlatform()
					? skiaCanvasElement is SKCanvasElement
					: skiaCanvasElement is SKXamlCanvas
			);
			var contentPresenter = grid?.GetChildren().Skip(1).First() as ContentPresenter;
			var border = contentPresenter?.GetChildren().First() as Border;

			//Validate element measurements
			Assert.AreEqual(grid?.ActualWidth, canvas?.ActualWidth * 2);
			Assert.AreEqual(canvas?.ActualWidth, border?.ActualWidth / 2);
			Assert.AreEqual(skiaCanvasElement?.ActualWidth, border?.ActualWidth + canvasMargin);

			//Validate point colors
			var renderer = await stackPanel.TakeScreenshot();

			//Set 4 corners positions to be validated (use 2px inset to avoid edge anti-aliasing/bleed)
			var leftX = inner ? absOffsetX + 2 : 2;
			var rightX = (int)((stackPanel?.ActualWidth ?? 0) - (inner ? absOffsetX + 2 : 2));
			var topY = inner ? absOffsetY + 2 : 2;
			var bottomY = (int)((stackPanel?.ActualHeight ?? 0) - (inner ? absOffsetY + 2 : 2));


			// For inner shadows, the shadow band appears on the edge the offset pulls away from:
			//   offsetX > 0 → left band,  offsetX < 0 → right band
			//   offsetY > 0 → top band,   offsetY < 0 → bottom band
			// A corner shows shadow (Red) if either adjacent edge has a shadow band.
			// For outer shadows, the shadow rect is offset from content:
			//   A corner shows shadow (Red) only if the offset moves the shadow toward that corner.

			//TopLeft
			var topLeftColor = inner
				? (offsetX > 0 || offsetY > 0 ? Colors.Red : Colors.Green)
				: (offsetX < 0 && offsetY < 0 ? Colors.Red : Colors.Yellow);
			await renderer.AssertColorAt(topLeftColor, leftX, topY);

			//TopRight
			var topRightColor = inner
				? (offsetX < 0 || offsetY > 0 ? Colors.Red : Colors.Green)
				: (offsetX > 0 && offsetY < 0 ? Colors.Red : Colors.Yellow);
			await renderer.AssertColorAt(topRightColor, rightX, topY);

			//BottomRight
			var bottomRightColor = bottomRightCorner > 50
				? Colors.Yellow
				: inner
					? (offsetX < 0 || offsetY < 0 ? Colors.Red : Colors.Green)
					: (offsetX > 0 && offsetY > 0 ? Colors.Red : Colors.Yellow);
			await renderer.AssertColorAt(bottomRightColor, rightX, bottomY);

			//BottomLeft
			var bottomLeftColor = inner
				? (offsetX > 0 || offsetY < 0 ? Colors.Red : Colors.Green)
				: (offsetX < 0 && offsetY > 0 ? Colors.Red : Colors.Yellow);
			await renderer.AssertColorAt(bottomLeftColor, leftX, bottomY);

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

			var parentBorder = new Border { Height = 500, Width = 500, HorizontalAlignment = HorizontalAlignment.Center, Background = new SolidColorBrush(Colors.Yellow) };
			var border = new Border { HorizontalAlignment = HorizontalAlignment.Center, Height = 200, Width = 200 };
			var shadowContainer = new ShadowContainer
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				Content = border,
			};

			// Inner shadows are painted behind the content on a Skia canvas.
			// If the content has an opaque background it hides the inner shadow.
			// Place the Green background on ShadowContainer for inner, on Border for outer.
			if (inner)
			{
				shadowContainer.Background = new SolidColorBrush(Colors.Green);
			}
			else
			{
				border.Background = new SolidColorBrush(Colors.Green);
			}

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
			// Transform bounds relative to parentBorder (screenshot root) so pixel coordinates are correct
			var bounds = border.TransformToVisual(parentBorder).TransformBounds(new Windows.Foundation.Rect(0, 0, border.ActualWidth, border.ActualHeight));

			var centerX = (int)(bounds.X + bounds.Width / 2);
			var centerY = (int)(bounds.Y + bounds.Height / 2);

			// Center of content should always be green
			await renderer.AssertColorAt(Colors.Green, centerX, centerY);

			// Check shadow pixels along the offset direction (use < to avoid the exact shadow boundary pixel)
			for (int i = 1; i < Math.Abs(offsetX); i++)
			{
				// For outer: shadow is outside the content on the offset side
				// For inner: shadow is inside the content on the opposite side of the offset
				var sampleX = inner
					? (offsetX > 0 ? (int)bounds.Left + i : (int)bounds.Right - i)
					: (offsetX > 0 ? (int)bounds.Right + i : (int)bounds.Left - i);
				await renderer.AssertColorAt(Colors.Red, sampleX, centerY);
			}

			for (int i = 1; i < Math.Abs(offsetY); i++)
			{
				var sampleY = inner
					? (offsetY > 0 ? (int)bounds.Top + i : (int)bounds.Bottom - i)
					: (offsetY > 0 ? (int)bounds.Bottom + i : (int)bounds.Top - i);
				await renderer.AssertColorAt(Colors.Red, centerX, sampleY);
			}
		}
#endif

		[TestMethod]
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

			var paintTcs = new TaskCompletionSource<bool>();
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

			// Wait for the initial paint cycle to complete
			var completed = await Task.WhenAny(paintTcs.Task, Task.Delay(5000));
			Assert.AreEqual(paintTcs.Task, completed, "Initial SurfacePaintCompleted was not raised");

			// On SKXamlCanvas, CreatedNewCanvas is true; on SKCanvasElement it's always false.
			if (!SKCanvasElement.IsSupportedOnCurrentPlatform())
			{
				createdNewCanvas.Should().BeTrue();
			}
			createdNewCanvas = false;

			// Changing the height of the children content of the shadow. 
			paintTcs = new TaskCompletionSource<bool>();
			internalBorder.Height = 400;

			await UnitTestsUIContentHelper.WaitForIdle();

			// Wait for the resize paint cycle to complete
			completed = await Task.WhenAny(paintTcs.Task, Task.Delay(5000));
			Assert.AreEqual(paintTcs.Task, completed, "Resize SurfacePaintCompleted was not raised");

			// On SKXamlCanvas, CreatedNewCanvas is true; on SKCanvasElement it's always false.
			if (!SKCanvasElement.IsSupportedOnCurrentPlatform())
			{
				createdNewCanvas.Should().BeTrue();
			}

			shadowContainer.SurfacePaintCompleted -= SurfacePaintCompleted;

			void SurfacePaintCompleted(object? sender, ShadowContainer.SurfacePaintCompletedEventArgs args)
			{
				// OnSurfacePainted can be called several times, we must make sure that the canvas is regenerated at least once.
				if (createdNewCanvas == false)
				{
					createdNewCanvas = args.CreatedNewCanvas;
				}
				paintTcs.TrySetResult(true);
			}
		}
	}
}
#endif
