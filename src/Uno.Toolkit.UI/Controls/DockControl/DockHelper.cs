using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace Uno.Toolkit.UI;

internal static class DockHelper
{
	public static Orientation? ToOrientation(this DockDirection direction)
	{
		return direction switch
		{
			DockDirection.Left or DockDirection.Right => Orientation.Horizontal,
			DockDirection.Top or DockDirection.Bottom => Orientation.Vertical,

			_ => null,
		};
	}

	public static Orientation Opposite(this Orientation orientation) => orientation switch
	{
		Orientation.Horizontal => Orientation.Vertical,
		Orientation.Vertical => Orientation.Horizontal,

		_ => throw new ArgumentOutOfRangeException(orientation.ToString()),
	};
}
