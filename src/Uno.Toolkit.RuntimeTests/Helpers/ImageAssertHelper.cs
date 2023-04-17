using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions.Execution;
using System.Threading.Tasks;
using Windows.UI;
using Windows.Foundation.Metadata;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.AppService;
using FluentAssertions;

#if IS_WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace Uno.Toolkit.RuntimeTests.Helpers
{
	internal static class ImageAssertHelper
	{
		/// <summary>
		/// Asserts that the given <paramref name="bitmap"/> contains the given <paramref name="expected"/> color at the given <paramref name="x"/> and <paramref name="y"/> coordinates.
		/// </summary>
		/// <param name="bitmap">The bitmap to check.</param>
		/// <param name="expected">The expected color.</param>
		/// <param name="x">The x-coordinate of the pixel to check.</param>
		/// <param name="y">The y-coordinate of the pixel to check.</param>
		public static async Task AssertColorAt(this RenderTargetBitmap? bitmap, Color expected, int x, int y)
		{
			if (bitmap is null)
			{
				return;
			}

			using var assertionScope = new AssertionScope();
			assertionScope.AddReportable("Expected Color", expected.ToString());
			assertionScope.AddReportable("Pixel Location", $"({x},{y})");

			var pixelBuffer = await bitmap.GetPixelsAsync();
			var pixels = pixelBuffer.ToArray();

			var offset = (y * bitmap.PixelWidth + x) * 4;
			var a = pixels[offset + 3];
			var r = pixels[offset + 2];
			var g = pixels[offset + 1];
			var b = pixels[offset + 0];

			var color = Color.FromArgb(a, r, g, b);

			AssertExpectedColor(expected, color);
		}

		/// <summary>
		/// Returns a screenshot of the given <paramref name="element"/> as a <see cref="RenderTargetBitmap"/>.
		/// </summary>
		/// <param name="element">The element to take a screenshot of.</param>
		public static async Task<RenderTargetBitmap?> TakeScreenshot(this UIElement? element)
		{
			if (IsScreenshotSupported())
			{
				var renderer = new RenderTargetBitmap();
				await renderer.RenderAsync(element);
				return renderer;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Returns whether screenshots are supported on the current platform.
		/// </summary>
		public static bool IsScreenshotSupported()
		{
			return ApiInformation.IsTypePresent(
#if IS_WINUI
				"Microsoft.UI.Xaml.Media.Imaging.RenderTargetBitmap"
#else
				"Windows.UI.Xaml.Media.Imaging.RenderTargetBitmap"
#endif
			);
		}

		private static void AssertExpectedColor(Color expected, Color? actual)
		{
			expected.Should().BeEquivalentTo(actual, config: d => d.ComparingByValue<Color?>());
		}
	}
}
