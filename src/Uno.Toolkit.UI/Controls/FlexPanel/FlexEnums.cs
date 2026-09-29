namespace Uno.Toolkit.UI;

// These are FlexPanel's public enums. The engine under Layout/Yoga/ has its own internal
// equivalents with Yoga's numbering and extra engine-only members; FlexEnumMapping.cs converts
// between the two. Each enum lists the member that is the property's default first, so that
// default(T) is also the CSS initial value.

/// <summary>
/// The direction of a <see cref="FlexPanel"/>'s main axis. CSS equivalent: <c>flex-direction</c>.
/// </summary>
public enum FlexDirection
{
	/// <summary>Children are laid out left to right.</summary>
	Row,

	/// <summary>Children are laid out right to left, starting from the end edge.</summary>
	RowReverse,

	/// <summary>Children are laid out top to bottom.</summary>
	Column,

	/// <summary>Children are laid out bottom to top, starting from the end edge.</summary>
	ColumnReverse,
}

/// <summary>
/// Whether a <see cref="FlexPanel"/>'s children may wrap onto several lines. CSS equivalent:
/// <c>flex-wrap</c>.
/// </summary>
public enum FlexWrap
{
	/// <summary>All children stay on a single line and may overflow it.</summary>
	NoWrap,

	/// <summary>Children wrap onto additional lines along the cross axis.</summary>
	Wrap,

	/// <summary>Children wrap onto additional lines, stacked in reverse cross-axis order.</summary>
	WrapReverse,
}

/// <summary>
/// How a <see cref="FlexPanel"/> distributes its children along the main axis. CSS equivalent:
/// <c>justify-content</c>.
/// </summary>
public enum FlexJustify
{
	/// <summary>Children are packed against the start of the main axis.</summary>
	FlexStart,

	/// <summary>Children are packed around the center of the main axis.</summary>
	Center,

	/// <summary>Children are packed against the end of the main axis.</summary>
	FlexEnd,

	/// <summary>Leftover space goes between children; the first and last touch the edges.</summary>
	SpaceBetween,

	/// <summary>Each child gets an equal share of leftover space on both of its sides.</summary>
	SpaceAround,

	/// <summary>Leftover space is split evenly between children and the edges.</summary>
	SpaceEvenly,
}

/// <summary>
/// Cross-axis alignment, shared by <see cref="FlexPanel.AlignItems"/>,
/// <see cref="FlexPanel.AlignContent"/> and the <c>AlignSelf</c> attached property. CSS
/// equivalent: <c>align-items</c> / <c>align-content</c> / <c>align-self</c>.
/// </summary>
/// <remarks>
/// As in CSS, the three properties share one keyword set, so not every member applies to every
/// property: <see cref="Auto"/> only means something on <c>AlignSelf</c>, the space-distribution
/// members only on <see cref="FlexPanel.AlignContent"/>, and <see cref="Baseline"/> not on
/// <see cref="FlexPanel.AlignContent"/>.
/// </remarks>
public enum FlexAlign
{
	/// <summary>Defers to the panel's <see cref="FlexPanel.AlignItems"/>. Meaningful on <c>AlignSelf</c> only.</summary>
	Auto,

	/// <summary>Aligned against the start of the cross axis.</summary>
	FlexStart,

	/// <summary>Centered on the cross axis.</summary>
	Center,

	/// <summary>Aligned against the end of the cross axis.</summary>
	FlexEnd,

	/// <summary>Stretched to fill the cross axis, unless the child has an explicit cross size.</summary>
	Stretch,

	/// <summary>Aligned so that the children's text baselines coincide.</summary>
	Baseline,

	/// <summary>Lines are spread out, the first and last touching the edges. <c>AlignContent</c> only.</summary>
	SpaceBetween,

	/// <summary>Each line gets an equal share of leftover space on both of its sides. <c>AlignContent</c> only.</summary>
	SpaceAround,

	/// <summary>Leftover space is split evenly between lines and the edges. <c>AlignContent</c> only.</summary>
	SpaceEvenly,
}

/// <summary>
/// How a child participates in <see cref="FlexPanel"/> layout. CSS equivalent: <c>position</c>.
/// </summary>
public enum FlexPositionType
{
	/// <summary>
	/// The child takes part in the flex flow; the <c>Left</c> / <c>Top</c> / <c>Right</c> /
	/// <c>Bottom</c> insets offset it from its flowed position.
	/// </summary>
	Relative,

	/// <summary>
	/// The child is taken out of the flow and placed against the panel's content edges using the
	/// <c>Left</c> / <c>Top</c> / <c>Right</c> / <c>Bottom</c> insets.
	/// </summary>
	Absolute,
}
