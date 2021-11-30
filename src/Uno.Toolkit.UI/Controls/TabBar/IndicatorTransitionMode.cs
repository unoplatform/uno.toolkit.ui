using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Determines the method for which the selection indicator will move to the selected tab
	/// </summary>
	public enum IndicatorTransitionMode
	{
		/// <summary>
		/// Upon <see cref="TabBar.SelectionChanged", the selection indicator will immediately appear at the selected <see cref="TabBarItem"/>/>
		/// </summary>
		Snap,
		/// <summary>
		/// For use with the <see cref="TabBarSelectorBehavior"/>. The selection indicator will slide along the <see cref="TabBar"/> as a function the current scrolled offset from the attached <see cref="TabBarSelectorBehavior.SelectorProperty"/>
		/// </summary>
		Slide,
	}
}
