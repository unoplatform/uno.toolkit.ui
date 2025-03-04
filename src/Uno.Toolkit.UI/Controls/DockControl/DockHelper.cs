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
			DockDirection.None => null,

			DockDirection.Left => Orientation.Horizontal,
			DockDirection.Top => Orientation.Vertical,
			DockDirection.Right => Orientation.Horizontal,
			DockDirection.Bottom => Orientation.Vertical,

			DockDirection.OuterLeft => Orientation.Horizontal,
			DockDirection.OuterTop => Orientation.Vertical,
			DockDirection.OuterRight => Orientation.Horizontal,
			DockDirection.OuterBottom => Orientation.Vertical,

			DockDirection.EdgeLeft => Orientation.Horizontal,
			DockDirection.EdgeTop => Orientation.Vertical,
			DockDirection.EdgeRight => Orientation.Horizontal,
			DockDirection.EdgeBottom => Orientation.Vertical,

			_ => throw new ArgumentNullException(direction.ToString()),
		};
	}
	public static (bool IsOriented, bool IsInner, bool IsOuter, bool IsEdge) Explode(this DockDirection direction)
	{
		return direction switch
		{
			DockDirection.None => (false, true, false, false),

			DockDirection.Left => (true, true, false, false),
			DockDirection.Top => (true, true, false, false),
			DockDirection.Right => (true, true, false, false),
			DockDirection.Bottom => (true, true, false, false),

			DockDirection.OuterLeft => (true, false, true, false),
			DockDirection.OuterTop => (true, false, true, false),
			DockDirection.OuterRight => (true, false, true, false),
			DockDirection.OuterBottom => (true, false, true, false),

			DockDirection.EdgeLeft => (true, false, false, true),
			DockDirection.EdgeTop => (true, false, false, true),
			DockDirection.EdgeRight => (true, false, false, true),
			DockDirection.EdgeBottom => (true, false, false, true),

			_ => throw new ArgumentNullException(direction.ToString()),
		};
	}

	// here leading/trailing refer to the relative insert position
	public static bool IsLeading(this DockDirection direction)
	{
		return direction switch
		{
			DockDirection.None => false,

			DockDirection.Left => true,
			DockDirection.Top => true,
			DockDirection.Right => false,
			DockDirection.Bottom => false,

			DockDirection.OuterLeft => true,
			DockDirection.OuterTop => true,
			DockDirection.OuterRight => false,
			DockDirection.OuterBottom => false,

			DockDirection.EdgeLeft => true,
			DockDirection.EdgeTop => true,
			DockDirection.EdgeRight => false,
			DockDirection.EdgeBottom => false,

			_ => throw new ArgumentNullException(direction.ToString()),
		};
	}

	public static Orientation ToOpposite(this Orientation orientation) => orientation switch
	{
		Orientation.Horizontal => Orientation.Vertical,
		Orientation.Vertical => Orientation.Horizontal,

		_ => throw new ArgumentOutOfRangeException(orientation.ToString()),
	};
}
