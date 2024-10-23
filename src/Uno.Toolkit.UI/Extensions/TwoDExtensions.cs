using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.WebUI;

namespace Uno.Toolkit.UI;

internal static partial class TwoDExtensions // Point Mathematics
{
	public static Point ToPoint(this Size x) => new Point(x.Width, x.Height);
	public static Point ToPoint(this Vector2 x) => new Point(x.X, x.Y);

	public static Point MultiplyBy(this Point x, double scale) => new Point(x.X * scale, x.Y * scale);
	public static Point MultiplyBy(this Point x, double scaleX, double scaleY) => new Point(x.X * scaleX, x.Y * scaleY);
	public static Point DivideBy(this Point x, double scale) => new Point(x.X / scale, x.Y / scale);
}

internal static partial class TwoDExtensions // Size Mathematics
{
	public static Size ToSize(this Point x) => new Size(x.X, x.Y);
	public static Size ToSize(this Vector2 x) => new Size(x.X, x.Y);

	public static Size MultiplyBy(this Size x, double scale) => new Size(x.Width * scale, x.Height * scale);
	public static Size DivideBy(this Size x, double scale) => new Size(x.Width / scale, x.Height / scale);
}
