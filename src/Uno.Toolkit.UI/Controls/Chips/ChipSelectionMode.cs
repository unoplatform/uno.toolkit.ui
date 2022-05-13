using System;
using System.Collections.Generic;
using System.Text;

namespace Uno.Toolkit.UI
{
	public enum ChipSelectionMode
	{
		/// <summary>
		/// Selection is disabled.
		/// </summary>
		None,

		/// <summary>
		/// Only one item can be selected at a time.
		/// </summary>
		Single,

		/// <summary>
		/// Multiple items can be selected.
		/// </summary>
		Multiple,
	}
}
