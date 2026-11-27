using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Uno.Toolkit.RuntimeTests.Helpers;

internal static class InkHelper
{
	// A pixel counts as ink when it is at least this far from white, summed over the three channels.
	private const int InkThreshold = 120;

	/// <summary>
	/// Renders <paramref name="element"/> over a white background and counts the visible pixels in its left and right halves.
	/// Layout coordinates do not tell a mirrored layout from a normal one, but the rendered pixels do.
	/// </summary>
	public static async Task<(int Left, int Right)> CountInk(FrameworkElement element)
	{
		var bitmap = await element.TakeScreenshot();
		Assert.IsNotNull(bitmap, "A screenshot is needed to tell a mirrored layout from a normal one.");

		var pixels = (await bitmap.GetPixelsAsync()).ToArray();
		int left = 0, right = 0;
		for (var y = 0; y < bitmap.PixelHeight; y++)
		{
			for (var x = 0; x < bitmap.PixelWidth; x++)
			{
				var offset = (y * bitmap.PixelWidth + x) * 4;
				var distance = (255 - pixels[offset]) + (255 - pixels[offset + 1]) + (255 - pixels[offset + 2]);
				if (pixels[offset + 3] > 0 && distance > InkThreshold)
				{
					if (x < bitmap.PixelWidth / 2)
					{
						left++;
					}
					else
					{
						right++;
					}
				}
			}
		}

		return (left, right);
	}

	/// <summary>The share of the ink that is in the right half: below 0.5 for content that sits at the left.</summary>
	public static async Task<double> RightShare(FrameworkElement element)
	{
		var (left, right) = await CountInk(element);
		Assert.IsTrue(left + right > 0, "Nothing was rendered.");
		return (double)right / (left + right);
	}
}
