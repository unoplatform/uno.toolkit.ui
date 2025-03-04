namespace Uno.Toolkit.UI;

public enum DockDirection
{
	None,
	Left, Top, Right, Bottom, // to split relative to the target pane
	OuterLeft, OuterTop, OuterRight, OuterBottom, // to split relative to a Editor/DocumentPane, as a ToolPane instead of a split DocumentPane
	EdgeLeft, EdgeTop, EdgeRight, EdgeBottom,
}

public enum DockPaneClosingBehavior
{
	CloseItem,
	ClosePane,
}
