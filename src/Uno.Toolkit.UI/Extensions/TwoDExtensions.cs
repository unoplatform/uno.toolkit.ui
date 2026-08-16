#if !HAS_UNO
#define MISSING_POINT_ARITHMETICS
#endif

using System;
using System.Numerics;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
	public static Point MultiplyBy(this Point x, Point scale) => new Point(x.X * scale.X, x.Y * scale.Y);
	public static Point MultiplyBy(this Point x, double scaleX, double scaleY) => new Point(x.X * scaleX, x.Y * scaleY);
	public static Point DivideBy(this Point x, double scale) => new Point(x.X / scale, x.Y / scale);

	public static Point CopySign(this Point magnitude, Point sign) => new Point(Math.CopySign(magnitude.X, sign.X), Math.CopySign(magnitude.Y, sign.Y));
	public static Point CopySign(this Point magnitude, Size sign) => new Point(Math.CopySign(magnitude.X, sign.Width), Math.CopySign(magnitude.Y, sign.Height));

#if !NETCOREAPP
	private static class Math
	{
		public static double CopySign(double a, double b)
		{
			// note: Math.Sign doesn't distinguish between 0 and -0
			var direction = b != 0
				? System.Math.Sign(b) > 0 ? 1 : -1
				: BitConverter.DoubleToInt64Bits(b) == 0L ? 1 : -1;
			var magnitude = System.Math.Abs(a);
		
			return direction * magnitude; 
		}
	}
#endif
}

partial class TwoDExtensions // Size arithmetics
{
	public static Size ToSize(this Point x) => new Size(x.X, x.Y);
	public static Size ToSize(this Vector2 x) => new Size(x.X, x.Y);

	public static Size Add(this Size x, Size y) => new Size(x.Width + y.Width, x.Height + y.Height);
	public static Size Add(this Size x, Thickness value) => new Size(x.Width + value.Left + value.Right, x.Height + value.Top + value.Bottom);
	public static Size Subtract(this Size x, Size y) => new Size(x.Width - y.Width, x.Height - y.Height);
	public static Size Subtract(this Size x, Thickness value) => new Size(x.Width - value.Left - value.Right, x.Height - value.Top - value.Bottom);

	public static Size MultiplyBy(this Size x, double scale) => new Size(x.Width * scale, x.Height * scale);
	public static Size MultiplyBy(this Size x, double scaleX, double scaleY) => new Size(x.Width * scaleX, x.Width * scaleY);
	public static Size DivideBy(this Size x, double scale) => new Size(x.Width / scale, x.Height / scale);
}

partial class TwoDExtensions // Thickness arithmetics
{
	public static Thickness Add(this Thickness x, Thickness y) => new Thickness(x.Left + y.Left, x.Top + y.Top, x.Right + y.Right, x.Bottom + y.Bottom);
	public static Thickness Subtract(this Thickness x, Thickness y) => new Thickness(x.Left - y.Left, x.Top - y.Top, x.Right - y.Right, x.Bottom - y.Bottom);

	public static double GetComponents(this Thickness x, Orientation orientation) => orientation == Orientation.Horizontal ? x.Left + x.Right : x.Top + x.Bottom;
}
