using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Uno.Toolkit.UI
{
	internal static class RectExtensions
	{
		public static bool IsEmptyOrZero(this Rect rect) => rect is { Width: 0, Height: 0 };
	}
}
