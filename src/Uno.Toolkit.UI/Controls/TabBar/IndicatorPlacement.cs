using System;
using System.Collections.Generic;
using System.Text;

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Determines the placement of the selection indicator in a <see cref="TabBar"/>
	/// </summary>
	public enum IndicatorPlacement
	{
		/// <summary>
		/// The selection indicator will be placed above the content of the <see cref="TabBar"/>
		/// </summary>
		Above,
		/// <summary>
		/// The selection indicator will be placed below, or "behind", the content of the <see cref="TabBar"/>
		/// </summary>
		Below,
	}
}
