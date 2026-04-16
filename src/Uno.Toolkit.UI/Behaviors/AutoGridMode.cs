namespace Uno.Toolkit.UI;

public enum AutoGridMode
{
	/// <summary>Auto-placement is disabled. Grid.Row and Grid.Column are not modified.</summary>
	None = 0,

	/// <summary>Children are placed top-to-bottom, wrapping to the next column. Row count is determined by <c>Grid.RowDefinitions</c>.</summary>
	Row = 1,

	/// <summary>Children are placed left-to-right, wrapping to the next row. Column count is determined by <c>Grid.ColumnDefinitions</c>.</summary>
	Column = 2,
}
