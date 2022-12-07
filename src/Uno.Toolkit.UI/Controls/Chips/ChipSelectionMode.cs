using System;
using System.Collections.Generic;
using System.Text;

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Defines constants that specify the selection behavior for a ChipGroup.
	/// </summary>
	/// <remarks>
	/// Different numbers of selected items are guaranteed:
	/// None=0, SingleOrNone=0 or 1, Single=1, Multiple=0 or many.
	/// </remarks>
	public enum ChipSelectionMode
	{
		/// <summary>
		/// Selection is disabled.
		/// </summary>
		None,

		/// <summary>
		/// Up to one item can be selected at a time.
		/// </summary>
		/// <remarks>The current item can be deselected.</remarks>
		SingleOrNone,

		/// <summary>
		/// One item is selected at any time.
		/// </summary>
		/// <remarks>The current item cannot be deselected.</remarks>
		Single,

		/// <summary>
		/// Multiple items can be selected.
		/// </summary>
		Multiple,
	}
}
