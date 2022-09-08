using System;
using System.Drawing;
using System.Linq;

namespace Uno.Toolkit.UITest.Framework
{
	internal static class ColorCodeParser
	{
		/// <summary>
		/// <para>Parses various color code strings to a <see cref="Color"/></para>
		/// <para>Supports codes in the form of: #RGB, #ARGB, #RRGGBB, or #AARRGGBB</para>
		/// </summary>
		/// <param name="colorCode"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="FormatException"></exception>
		public static Color Parse(string colorCode)
		{
			if (colorCode == null)
			{
				throw new ArgumentNullException(nameof(colorCode));
			}

			if (!colorCode.StartsWith("#", StringComparison.OrdinalIgnoreCase))
			{
				throw new FormatException("Color code must start with #");
			}

			byte a = 0x00;
			byte r = 0x00;
			byte g = 0x00;
			byte b = 0x00;

			colorCode = colorCode.Substring(1);

			// RGB
			if (colorCode.Length == 3)
			{
				a = 0xFF;
				r = Convert.ToByte(new String(colorCode[0], 2), 16);
				g = Convert.ToByte(new String(colorCode[1], 2), 16);
				b = Convert.ToByte(new String(colorCode[2], 2), 16);
			}
			// ARGB
			else if (colorCode.Length == 4)
			{
				a = Convert.ToByte(new String(colorCode[0], 2), 16);
				r = Convert.ToByte(new String(colorCode[1], 2), 16);
				g = Convert.ToByte(new String(colorCode[2], 2), 16);
				b = Convert.ToByte(new String(colorCode[3], 2), 16);
			}
			// RRGGBB
			else if (colorCode.Length == 6)
			{
				a = 0xFF;
				r = Convert.ToByte(colorCode.Substring(0, 2), 16);
				g = Convert.ToByte(colorCode.Substring(2, 2), 16);
				b = Convert.ToByte(colorCode.Substring(4, 2), 16);
			}
			// AARRGGBB
			else if (colorCode.Length == 8)
			{
				a = Convert.ToByte(colorCode.Substring(0, 2), 16);
				r = Convert.ToByte(colorCode.Substring(2, 2), 16);
				g = Convert.ToByte(colorCode.Substring(4, 2), 16);
				b = Convert.ToByte(colorCode.Substring(6, 2), 16);
			}
			else
			{
				throw new FormatException($"Failed to parse color code: #{colorCode}");
			}

			return System.Drawing.Color.FromArgb(a, r, g, b);
		}
	}
}
