using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Toolkit.UI;

public enum DockDirection
{
	None,
	Left, Top, Right, Bottom,
	//LeftMost, TopMost, RightMost, BottomMost,  // maybe use the pane argument to specify "-most" or not
}
