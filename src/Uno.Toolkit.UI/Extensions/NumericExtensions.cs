using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.UI.ToolkitLib.Extensions
{
	internal static class NumericExtensions
	{
		public static bool IsNaN(this double value)
		{
			// Get the double as an unsigned long
			NanUnion union = new NanUnion { FloatingValue = value };

			// An IEEE 754 double precision floating point number is NaN if its
			// exponent equals 2047 and it has a non-zero mantissa.
			ulong exponent = union.IntegerValue & 0xfff0000000000000L;
			if ((exponent != 0x7ff0000000000000L) && (exponent != 0xfff0000000000000L))
			{
				return false;
			}

			ulong mantissa = union.IntegerValue & 0x000fffffffffffffL;
			return mantissa != 0L;
		}

		private struct NanUnion
		{
			/// <summary>
			/// Floating point representation of the union.
			/// </summary>

			internal double FloatingValue;

			/// <summary>
			/// Integer representation of the union.
			/// </summary>
			internal ulong IntegerValue;
		}
	}
}
