#if !HAS_UNO
#define MISSING_POINT_ARITHMETICS
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI;

internal static partial class TwoDExtensions;

partial class TwoDExtensions // Point arithmetics
{
	public static Point ToPoint(this Size x) => new Point(x.Width, x.Height);
	public static Point ToPoint(this Vector2 x) => new Point(x.X, x.Y);

#if MISSING_POINT_ARITHMETICS
	public static Point Add(this Point x, Point y) => new Point(x.X + y.X, x.Y + y.Y);
	public static Point Subtract(this Point x, Point y) => new Point(x.X - y.X, x.Y - y.Y);
#else
	public static Point Add(this Point x, Point y) => x + y;
	public static Point Subtract(this Point x, Point y) => x - y;
#endif
	public static Point MultiplyBy(this Point x, double scale) => new Point(x.X * scale, x.Y * scale);
	public static Point MultiplyBy(this Point x, double scaleX, double scaleY) => new Point(x.X * scaleX, x.Y * scaleY);
	public static Point DivideBy(this Point x, double scale) => new Point(x.X / scale, x.Y / scale);
}

partial class TwoDExtensions // Size arithmetics
{
	public static Size ToSize(this Point x) => new Size(x.X, x.Y);
	public static Size ToSize(this Vector2 x) => new Size(x.X, x.Y);

	public static Size MultiplyBy(this Size x, double scale) => new Size(x.Width * scale, x.Height * scale);
	public static Size MultiplyBy(this Size x, double scaleX, double scaleY) => new Size(x.Width * scaleX, x.Width * scaleY);
	public static Size DivideBy(this Size x, double scale) => new Size(x.Width / scale, x.Height / scale);
}

partial class TwoDExtensions // Thickness arithmetics
{
	public static Thickness Add(this Thickness x, Thickness y) => new Thickness(x.Left + y.Left, x.Top + y.Top, x.Right + y.Right, x.Bottom + y.Bottom);
	public static Thickness Subtract(this Thickness x, Thickness y) => new Thickness(x.Left - y.Left, x.Top - y.Top, x.Right - y.Right, x.Bottom - y.Bottom);
}

