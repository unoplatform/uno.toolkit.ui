using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Toolkit.UI.Controls
{
	/// <summary>
	/// Defines constants that specify the direction in which the DrawerControl drawer opens.
	/// </summary>
	public enum DrawerOpenDirection
	{
		/// <summary>The drawer opens to the right, and is shown on the left corner when opened.</summary>
		Right = 0,
		/// <summary>The drawer opens to the left, and is shown on the right corner when opened.</summary>
		Left,
		/// <summary>The drawer opens to the bottom, and is shown on the top corner when opened.</summary>
		Down,
		/// <summary>The drawer opens to the top, and is shown on the bottom corner when opened.</summary>
		Up,
	}
}
