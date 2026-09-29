using YogaAlign = Uno.Toolkit.UI.Yoga.FlexAlign;
using YogaDirection = Uno.Toolkit.UI.Yoga.FlexDirection;
using YogaJustify = Uno.Toolkit.UI.Yoga.FlexJustify;
using YogaPositionType = Uno.Toolkit.UI.Yoga.FlexPositionType;
using YogaWrap = Uno.Toolkit.UI.Yoga.FlexWrap;

namespace Uno.Toolkit.UI;

/// <summary>
/// Maps <see cref="FlexPanel"/>'s public enums onto the vendored engine's internal ones.
/// </summary>
/// <remarks>
/// The two sets differ in numbering (Yoga puts <c>Column</c> first) and in membership (the engine
/// carries members FlexPanel does not expose), so the mapping is by name, never by cast. A value
/// outside the declared members — only reachable through an explicit integer cast — falls back to
/// the property's default rather than throwing from a layout pass.
/// </remarks>
internal static class FlexEnumMapping
{
	public static YogaDirection ToYoga(this FlexDirection value) => value switch
	{
		FlexDirection.RowReverse => YogaDirection.RowReverse,
		FlexDirection.Column => YogaDirection.Column,
		FlexDirection.ColumnReverse => YogaDirection.ColumnReverse,
		_ => YogaDirection.Row,
	};

	public static YogaWrap ToYoga(this FlexWrap value) => value switch
	{
		FlexWrap.Wrap => YogaWrap.Wrap,
		FlexWrap.WrapReverse => YogaWrap.WrapReverse,
		_ => YogaWrap.NoWrap,
	};

	public static YogaJustify ToYoga(this FlexJustify value) => value switch
	{
		FlexJustify.Center => YogaJustify.Center,
		FlexJustify.FlexEnd => YogaJustify.FlexEnd,
		FlexJustify.SpaceBetween => YogaJustify.SpaceBetween,
		FlexJustify.SpaceAround => YogaJustify.SpaceAround,
		FlexJustify.SpaceEvenly => YogaJustify.SpaceEvenly,
		_ => YogaJustify.FlexStart,
	};

	/// <param name="value">The public value.</param>
	/// <param name="fallback">The engine value for an undeclared input; each property has its own default.</param>
	public static YogaAlign ToYoga(this FlexAlign value, YogaAlign fallback) => value switch
	{
		FlexAlign.Auto => YogaAlign.Auto,
		FlexAlign.FlexStart => YogaAlign.FlexStart,
		FlexAlign.Center => YogaAlign.Center,
		FlexAlign.FlexEnd => YogaAlign.FlexEnd,
		FlexAlign.Stretch => YogaAlign.Stretch,
		FlexAlign.Baseline => YogaAlign.Baseline,
		FlexAlign.SpaceBetween => YogaAlign.SpaceBetween,
		FlexAlign.SpaceAround => YogaAlign.SpaceAround,
		FlexAlign.SpaceEvenly => YogaAlign.SpaceEvenly,
		_ => fallback,
	};

	public static YogaPositionType ToYoga(this FlexPositionType value) => value switch
	{
		FlexPositionType.Absolute => YogaPositionType.Absolute,
		_ => YogaPositionType.Relative,
	};
}
