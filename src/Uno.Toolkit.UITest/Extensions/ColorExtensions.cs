using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Toolkit.UITest.Extensions
{
	public static  class ColorExtensions
	{
		public static System.Drawing.Color ToColor(this SkiaSharp.SKColor color)
			=> System.Drawing.Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
	}
}
