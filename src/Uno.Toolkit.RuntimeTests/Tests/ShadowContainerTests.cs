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

namespace Uno.Toolkit.RuntimeTests.Tests
{
	[TestClass]
	[RunsOnUIThread]
	#if HAS_UNO_WINUI && !NET6_0_OR_GREATER
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

		[TestMethod]
		[DataRow(10, 10, false)]
		//[DataRow(10, 10, true)]
		//[DataRow(-10, -10, false)]
		//[DataRow(-10, -10, true)]
		//[DataRow(-10, 10, true)]
		//[DataRow(10, -10, true)]
		//[DataRow(-10, 10, false)]
		//[DataRow(10, -10, false)]
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


			var pixels = await renderer!.GetPixelsAsync();
			var dir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

			var c = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "image.png");
			using (var fileStream = File.Create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "image.png")).AsRandomAccessStream())
			{
				var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);

				encoder.SetPixelData(
					BitmapPixelFormat.Bgra8,
					BitmapAlphaMode.Ignore,
					(uint)renderer.PixelWidth,
					(uint)renderer.PixelHeight,
					96, 96,
					pixels.ToArray()
				);

				await encoder.FlushAsync();
			}

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
	}
}
#endif
