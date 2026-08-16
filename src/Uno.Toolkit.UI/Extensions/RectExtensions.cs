using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI
{
	internal static class RectExtensions
	{
		public static bool IsEmptyOrZero(this Rect rect) => rect is { Width: 0, Height: 0 };

		public static Rect Multiply(this Rect x, double value) => new Rect(x.X * value, x.Y * value, x.Width * value, x.Height * value);

		public static Rect Inflate(this Rect x, Thickness value) => new Rect(
			x.X - value.Left,
			x.Y - value.Top,
			x.Width + (value.Left + value.Right),
			x.Height + (value.Top + value.Bottom)
		);
	}
}
