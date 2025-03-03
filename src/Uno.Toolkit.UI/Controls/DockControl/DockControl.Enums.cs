namespace Uno.Toolkit.UI;

public enum DockDirection
{
	None,
	Left, Top, Right, Bottom,
	//LeftMost, TopMost, RightMost, BottomMost,  // maybe use the pane argument to specify "-most" or not
}

public enum DockPaneClosingBehavior
{
	CloseItem,
	ClosePane,
}
