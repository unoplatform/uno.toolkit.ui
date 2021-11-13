using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Uno.Toolkit.UI.Extensions
{
	internal static class LayoutExtensions
	{
		internal static Size FiniteOrDefault(this Size value, Size defaultValue)
		{
			return new Size(
				value.Width.FiniteOrDefault(defaultValue.Width),
				value.Height.FiniteOrDefault(defaultValue.Height)
			);
		}

		internal static double FiniteOrDefault(this double value, double defaultValue)
		{
			return double.IsInfinity(value) || double.IsNaN(value)
				? defaultValue
				: value;
		}
	}
}
