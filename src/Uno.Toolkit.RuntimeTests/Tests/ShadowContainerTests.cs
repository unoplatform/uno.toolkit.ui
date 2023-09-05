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

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	#if HAS_UNO_WINUI && !(NET6_0_OR_GREATER || NETSTANDARD2_0)
	[Ignore("Disabled because Skia.Sharp doesn't support Xamarin+WinUI.")]
	#endif
	internal partial class ShadowContainerTests
	{
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

#if !(__ANDROID__ || __IOS__)
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
