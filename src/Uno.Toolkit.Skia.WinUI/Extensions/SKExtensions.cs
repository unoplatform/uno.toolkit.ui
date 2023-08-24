using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using Windows.UI;

namespace Uno.Toolkit.UI;

internal static class SKExtensions
{
	public static SKRect Scale(this SKRect x, float scale) => new(
		x.Left * scale,
		x.Top * scale,
		x.Right * scale,
		x.Bottom * scale
	);

	public static SKColor ToSkiaColor(this Color x) => new(x.R, x.G, x.B, x.A);
}
