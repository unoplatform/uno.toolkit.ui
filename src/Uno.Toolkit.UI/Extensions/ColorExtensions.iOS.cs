#if __IOS__
using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Uno.UI.ToolkitLib.Extensions
{
	internal static class UIColorExtensions
	{
		public static UIImage ToUIImage(this UIColor color)
		{
			var rect = new CGRect(0, 0, 1, 1);

			UIGraphics.BeginImageContext(rect.Size);

			var context = UIGraphics.GetCurrentContext();
			context.SetFillColor(color.CGColor);
			context.FillRect(rect);
			var image = UIGraphics.GetImageFromCurrentImageContext();

			UIGraphics.EndImageContext();

			return image;
		}
	}
}
#endif