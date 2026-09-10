using System.Diagnostics.CodeAnalysis;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI;

partial class FlexPanel
{
	// -- Direction DependencyProperty --
	public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
		nameof(Direction),
		typeof(FlexDirection),
		typeof(FlexPanel),
		// The default is deliberately Row, which is NOT the enum's zero value: FlexEnums.cs keeps
		// Yoga's numbering, where Column = 0. Row is the CSS `flex-direction` initial value and the
		// one a XAML author expects from a bare <FlexPanel>. Do not "simplify" this to default(T).
		new PropertyMetadata(FlexDirection.Row, propertyChangedCallback: OnContainerPropertyChanged));

	/// <summary>
	/// Gets or sets the direction of the main axis along which children are laid out.
	/// CSS equivalent: <c>flex-direction</c>.
	/// </summary>
	/// <remarks>
	/// Defaults to <see cref="FlexDirection.Row"/>. Note this differs from the zero value of
	/// <see cref="FlexDirection"/>, which is <see cref="FlexDirection.Column"/>.
	/// </remarks>
	public FlexDirection Direction
	{
		get => (FlexDirection)GetValue(DirectionProperty);
		set => SetValue(DirectionProperty, value);
	}

	// -- Wrap DependencyProperty --
	public static readonly DependencyProperty WrapProperty = DependencyProperty.Register(
		nameof(Wrap),
		typeof(FlexWrap),
		typeof(FlexPanel),
		new PropertyMetadata(FlexWrap.NoWrap, propertyChangedCallback: OnContainerPropertyChanged));

	/// <summary>
	/// Gets or sets whether children are forced onto a single line, or may wrap onto multiple lines.
	/// CSS equivalent: <c>flex-wrap</c>.
	/// </summary>
	public FlexWrap Wrap
	{
		get => (FlexWrap)GetValue(WrapProperty);
		set => SetValue(WrapProperty, value);
	}

	// -- JustifyContent DependencyProperty --
	public static readonly DependencyProperty JustifyContentProperty = DependencyProperty.Register(
		nameof(JustifyContent),
		typeof(FlexJustify),
		typeof(FlexPanel),
		new PropertyMetadata(FlexJustify.FlexStart, propertyChangedCallback: OnContainerPropertyChanged));

	/// <summary>
	/// Gets or sets how children are distributed along the main axis.
	/// CSS equivalent: <c>justify-content</c>.
	/// </summary>
	/// <remarks>
	/// Meaningful values are <see cref="FlexJustify.FlexStart"/>, <see cref="FlexJustify.Center"/>,
	/// <see cref="FlexJustify.FlexEnd"/>, <see cref="FlexJustify.SpaceBetween"/>,
	/// <see cref="FlexJustify.SpaceAround"/> and <see cref="FlexJustify.SpaceEvenly"/>. The remaining
	/// members of <see cref="FlexJustify"/> exist for engine parity and behave as
	/// <see cref="FlexJustify.FlexStart"/> or <see cref="FlexJustify.FlexEnd"/>.
	/// </remarks>
	public FlexJustify JustifyContent
	{
		get => (FlexJustify)GetValue(JustifyContentProperty);
		set => SetValue(JustifyContentProperty, value);
	}

	// -- AlignItems DependencyProperty --
	public static readonly DependencyProperty AlignItemsProperty = DependencyProperty.Register(
		nameof(AlignItems),
		typeof(FlexAlign),
		typeof(FlexPanel),
		new PropertyMetadata(FlexAlign.Stretch, propertyChangedCallback: OnContainerPropertyChanged));

	/// <summary>
	/// Gets or sets the default cross-axis alignment applied to every child.
	/// CSS equivalent: <c>align-items</c>.
	/// </summary>
	/// <remarks>
	/// Meaningful values are <see cref="FlexAlign.Stretch"/>, <see cref="FlexAlign.FlexStart"/>,
	/// <see cref="FlexAlign.Center"/>, <see cref="FlexAlign.FlexEnd"/> and
	/// <see cref="FlexAlign.Baseline"/>. The space-distribution members apply to
	/// <see cref="AlignContent"/>, not to this property.
	/// </remarks>
	public FlexAlign AlignItems
	{
		get => (FlexAlign)GetValue(AlignItemsProperty);
		set => SetValue(AlignItemsProperty, value);
	}

	// -- AlignContent DependencyProperty --
	public static readonly DependencyProperty AlignContentProperty = DependencyProperty.Register(
		nameof(AlignContent),
		typeof(FlexAlign),
		typeof(FlexPanel),
		new PropertyMetadata(FlexAlign.FlexStart, propertyChangedCallback: OnContainerPropertyChanged));

	/// <summary>
	/// Gets or sets how wrapped lines are distributed along the cross axis.
	/// CSS equivalent: <c>align-content</c>.
	/// </summary>
	/// <remarks>
	/// Only has an effect when <see cref="Wrap"/> allows wrapping and the content occupies more than
	/// one line. <see cref="FlexAlign.Baseline"/> is not meaningful here.
	/// </remarks>
	public FlexAlign AlignContent
	{
		get => (FlexAlign)GetValue(AlignContentProperty);
		set => SetValue(AlignContentProperty, value);
	}

	// -- ColumnGap DependencyProperty --
	public static readonly DependencyProperty ColumnGapProperty = DependencyProperty.Register(
		nameof(ColumnGap),
		typeof(double),
		typeof(FlexPanel),
		new PropertyMetadata(0d, propertyChangedCallback: OnContainerPropertyChanged));

	/// <summary>
	/// Gets or sets the gutter between columns. CSS equivalent: <c>column-gap</c>.
	/// </summary>
	/// <remarks>Negative values are clamped to <c>0</c> by the layout engine.</remarks>
	public double ColumnGap
	{
		get => (double)GetValue(ColumnGapProperty);
		set => SetValue(ColumnGapProperty, value);
	}

	// -- RowGap DependencyProperty --
	public static readonly DependencyProperty RowGapProperty = DependencyProperty.Register(
		nameof(RowGap),
		typeof(double),
		typeof(FlexPanel),
		new PropertyMetadata(0d, propertyChangedCallback: OnContainerPropertyChanged));

	/// <summary>
	/// Gets or sets the gutter between rows. CSS equivalent: <c>row-gap</c>.
	/// </summary>
	/// <remarks>Negative values are clamped to <c>0</c> by the layout engine.</remarks>
	public double RowGap
	{
		get => (double)GetValue(RowGapProperty);
		set => SetValue(RowGapProperty, value);
	}

	// -- Padding DependencyProperty --
	public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
		nameof(Padding),
		typeof(Thickness),
		typeof(FlexPanel),
		new PropertyMetadata(default(Thickness), propertyChangedCallback: OnContainerPropertyChanged));

	/// <summary>
	/// Gets or sets the padding between the panel's edges and its content area.
	/// CSS equivalent: <c>padding</c>.
	/// </summary>
	public Thickness Padding
	{
		get => (Thickness)GetValue(PaddingProperty);
		set => SetValue(PaddingProperty, value);
	}

	// -- LayoutDirection DependencyProperty --
	public static readonly DependencyProperty LayoutDirectionProperty = DependencyProperty.Register(
		nameof(LayoutDirection),
		typeof(FlexLayoutDirection),
		typeof(FlexPanel),
		new PropertyMetadata(FlexLayoutDirection.LeftToRight, propertyChangedCallback: OnContainerPropertyChanged));

	/// <summary>
	/// Gets or sets the writing direction used to resolve the main axis.
	/// CSS equivalent: <c>direction</c>.
	/// </summary>
	/// <remarks>
	/// This is the only right-to-left input <see cref="FlexPanel"/> reads;
	/// <see cref="FrameworkElement.FlowDirection"/> is ignored. Setting both this property and
	/// <c>FlowDirection</c> to a right-to-left value mirrors the layout twice.
	/// </remarks>
	public FlexLayoutDirection LayoutDirection
	{
		get => (FlexLayoutDirection)GetValue(LayoutDirectionProperty);
		set => SetValue(LayoutDirectionProperty, value);
	}

	// -- Grow Attached Property --
	[DynamicDependency(nameof(GetGrow))]
	public static readonly DependencyProperty GrowProperty = DependencyProperty.RegisterAttached(
		"Grow",
		typeof(double),
		typeof(FlexPanel),
		new PropertyMetadata(0d, propertyChangedCallback: OnChildPropertyChanged));

	/// <summary>
	/// Sets the share of leftover main-axis space the element receives.
	/// CSS equivalent: <c>flex-grow</c>.
	/// </summary>
	/// <remarks>
	/// For equal-sized columns, pair this with a <c>Basis</c> of <c>0</c> — the CSS
	/// <c>flex: 1 1 0</c> shorthand. Grow alone distributes only the leftover space, so children
	/// with different content sizes stay different sizes.
	/// </remarks>
	[DynamicDependency(nameof(GetGrow))]
	public static void SetGrow(DependencyObject element, double value)
	{
		element.SetValue(GrowProperty, value);
	}

	/// <summary>
	/// Gets the share of leftover main-axis space the element receives.
	/// CSS equivalent: <c>flex-grow</c>.
	/// </summary>
	[DynamicDependency(nameof(SetGrow))]
	public static double GetGrow(DependencyObject element)
	{
		return (double)element.GetValue(GrowProperty);
	}

	// -- Shrink Attached Property --
	[DynamicDependency(nameof(GetShrink))]
	public static readonly DependencyProperty ShrinkProperty = DependencyProperty.RegisterAttached(
		"Shrink",
		typeof(double),
		typeof(FlexPanel),
		new PropertyMetadata(1d, propertyChangedCallback: OnChildPropertyChanged));

	/// <summary>
	/// Sets the share of an overflow deficit the element absorbs.
	/// CSS equivalent: <c>flex-shrink</c>.
	/// </summary>
	/// <remarks>Defaults to <c>1</c>. Set to <c>0</c> to keep an element at its natural size.</remarks>
	[DynamicDependency(nameof(GetShrink))]
	public static void SetShrink(DependencyObject element, double value)
	{
		element.SetValue(ShrinkProperty, value);
	}

	/// <summary>
	/// Gets the share of an overflow deficit the element absorbs.
	/// CSS equivalent: <c>flex-shrink</c>.
	/// </summary>
	[DynamicDependency(nameof(SetShrink))]
	public static double GetShrink(DependencyObject element)
	{
		return (double)element.GetValue(ShrinkProperty);
	}

	// -- Basis Attached Property --
	[DynamicDependency(nameof(GetBasis))]
	public static readonly DependencyProperty BasisProperty = DependencyProperty.RegisterAttached(
		"Basis",
		typeof(double),
		typeof(FlexPanel),
		new PropertyMetadata(double.NaN, propertyChangedCallback: OnChildPropertyChanged));

	/// <summary>
	/// Sets the initial main-axis size of the element before growing or shrinking.
	/// CSS equivalent: <c>flex-basis</c>, in points.
	/// </summary>
	/// <remarks>
	/// <see cref="double.NaN"/> (the default) means <c>auto</c> — the element's own size is used.
	/// When set, this takes precedence over <see cref="FrameworkElement.Width"/> /
	/// <see cref="FrameworkElement.Height"/> on the main axis. Percentage values are not supported.
	/// </remarks>
	[DynamicDependency(nameof(GetBasis))]
	public static void SetBasis(DependencyObject element, double value)
	{
		element.SetValue(BasisProperty, value);
	}

	/// <summary>
	/// Gets the initial main-axis size of the element before growing or shrinking.
	/// CSS equivalent: <c>flex-basis</c>.
	/// </summary>
	[DynamicDependency(nameof(SetBasis))]
	public static double GetBasis(DependencyObject element)
	{
		return (double)element.GetValue(BasisProperty);
	}

	// -- FlexMinWidth Attached Property --
	// Named FlexMinWidth rather than MinWidth because it would otherwise sit next to
	// FrameworkElement.MinWidth on the same element while meaning something different:
	// FrameworkElement.MinWidth forces Measure to return at least X, whereas this clamps the
	// flex-resolved slot to at least X. The stutter is cheaper than that trap.
	[DynamicDependency(nameof(GetFlexMinWidth))]
	public static readonly DependencyProperty FlexMinWidthProperty = DependencyProperty.RegisterAttached(
		"FlexMinWidth",
		typeof(double),
		typeof(FlexPanel),
		new PropertyMetadata(double.NaN, propertyChangedCallback: OnChildPropertyChanged));

	/// <summary>
	/// Sets the minimum size of the element on the main axis. CSS equivalent: <c>min-width</c>.
	/// </summary>
	/// <remarks>
	/// <see cref="double.NaN"/> (the default) means <c>auto</c>: the element is floored at its
	/// min-content size, per CSS Flexbox section 4.5. Any explicit value — including <c>0</c> — opts
	/// out of that automatic floor.
	/// </remarks>
	[DynamicDependency(nameof(GetFlexMinWidth))]
	public static void SetFlexMinWidth(DependencyObject element, double value)
	{
		element.SetValue(FlexMinWidthProperty, value);
	}

	/// <summary>
	/// Gets the minimum size of the element on the main axis. CSS equivalent: <c>min-width</c>.
	/// </summary>
	[DynamicDependency(nameof(SetFlexMinWidth))]
	public static double GetFlexMinWidth(DependencyObject element)
	{
		return (double)element.GetValue(FlexMinWidthProperty);
	}

	// -- FlexMinHeight Attached Property --
	[DynamicDependency(nameof(GetFlexMinHeight))]
	public static readonly DependencyProperty FlexMinHeightProperty = DependencyProperty.RegisterAttached(
		"FlexMinHeight",
		typeof(double),
		typeof(FlexPanel),
		new PropertyMetadata(double.NaN, propertyChangedCallback: OnChildPropertyChanged));

	/// <summary>
	/// Sets the minimum size of the element on the cross axis. CSS equivalent: <c>min-height</c>.
	/// </summary>
	/// <remarks>
	/// <see cref="double.NaN"/> (the default) means <c>auto</c>. See <see cref="SetFlexMinWidth"/>
	/// for the automatic-minimum rules; they apply to whichever axis is the main axis.
	/// </remarks>
	[DynamicDependency(nameof(GetFlexMinHeight))]
	public static void SetFlexMinHeight(DependencyObject element, double value)
	{
		element.SetValue(FlexMinHeightProperty, value);
	}

	/// <summary>
	/// Gets the minimum size of the element on the cross axis. CSS equivalent: <c>min-height</c>.
	/// </summary>
	[DynamicDependency(nameof(SetFlexMinHeight))]
	public static double GetFlexMinHeight(DependencyObject element)
	{
		return (double)element.GetValue(FlexMinHeightProperty);
	}

	// -- AlignSelf Attached Property --
	[DynamicDependency(nameof(GetAlignSelf))]
	public static readonly DependencyProperty AlignSelfProperty = DependencyProperty.RegisterAttached(
		"AlignSelf",
		typeof(FlexAlign),
		typeof(FlexPanel),
		new PropertyMetadata(FlexAlign.Auto, propertyChangedCallback: OnChildPropertyChanged));

	/// <summary>
	/// Sets the cross-axis alignment for a single element, overriding the panel's
	/// <see cref="AlignItems"/>. CSS equivalent: <c>align-self</c>.
	/// </summary>
	/// <remarks><see cref="FlexAlign.Auto"/> (the default) defers to <see cref="AlignItems"/>.</remarks>
	[DynamicDependency(nameof(GetAlignSelf))]
	public static void SetAlignSelf(DependencyObject element, FlexAlign value)
	{
		element.SetValue(AlignSelfProperty, value);
	}

	/// <summary>
	/// Gets the cross-axis alignment for a single element. CSS equivalent: <c>align-self</c>.
	/// </summary>
	[DynamicDependency(nameof(SetAlignSelf))]
	public static FlexAlign GetAlignSelf(DependencyObject element)
	{
		return (FlexAlign)element.GetValue(AlignSelfProperty);
	}

	// -- Position Attached Property --
	[DynamicDependency(nameof(GetPosition))]
	public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached(
		"Position",
		typeof(FlexPositionType),
		typeof(FlexPanel),
		new PropertyMetadata(FlexPositionType.Relative, propertyChangedCallback: OnChildPropertyChanged));

	/// <summary>
	/// Sets how the element participates in flex layout. CSS equivalent: <c>position</c>.
	/// </summary>
	/// <remarks>
	/// <see cref="FlexPositionType.Absolute"/> removes the element from the flow: it no longer
	/// contributes to the panel's size or to sibling positions, and is placed using the
	/// <c>Left</c> / <c>Top</c> / <c>Right</c> / <c>Bottom</c> inset properties.
	/// </remarks>
	[DynamicDependency(nameof(GetPosition))]
	public static void SetPosition(DependencyObject element, FlexPositionType value)
	{
		element.SetValue(PositionProperty, value);
	}

	/// <summary>
	/// Gets how the element participates in flex layout. CSS equivalent: <c>position</c>.
	/// </summary>
	[DynamicDependency(nameof(SetPosition))]
	public static FlexPositionType GetPosition(DependencyObject element)
	{
		return (FlexPositionType)element.GetValue(PositionProperty);
	}

	// -- Left Attached Property --
	[DynamicDependency(nameof(GetLeft))]
	public static readonly DependencyProperty LeftProperty = DependencyProperty.RegisterAttached(
		"Left",
		typeof(double),
		typeof(FlexPanel),
		new PropertyMetadata(double.NaN, propertyChangedCallback: OnChildPropertyChanged));

	/// <summary>
	/// Sets the offset from the container's left content edge. CSS equivalent: <c>left</c>.
	/// </summary>
	[DynamicDependency(nameof(GetLeft))]
	public static void SetLeft(DependencyObject element, double value)
	{
		element.SetValue(LeftProperty, value);
	}

	/// <summary>
	/// Gets the offset from the container's left content edge. CSS equivalent: <c>left</c>.
	/// </summary>
	[DynamicDependency(nameof(SetLeft))]
	public static double GetLeft(DependencyObject element)
	{
		return (double)element.GetValue(LeftProperty);
	}

	// -- Top Attached Property --
	[DynamicDependency(nameof(GetTop))]
	public static readonly DependencyProperty TopProperty = DependencyProperty.RegisterAttached(
		"Top",
		typeof(double),
		typeof(FlexPanel),
		new PropertyMetadata(double.NaN, propertyChangedCallback: OnChildPropertyChanged));

	/// <summary>
	/// Sets the offset from the container's top content edge. CSS equivalent: <c>top</c>.
	/// </summary>
	[DynamicDependency(nameof(GetTop))]
	public static void SetTop(DependencyObject element, double value)
	{
		element.SetValue(TopProperty, value);
	}

	/// <summary>
	/// Gets the offset from the container's top content edge. CSS equivalent: <c>top</c>.
	/// </summary>
	[DynamicDependency(nameof(SetTop))]
	public static double GetTop(DependencyObject element)
	{
		return (double)element.GetValue(TopProperty);
	}

	// -- Right Attached Property --
	[DynamicDependency(nameof(GetRight))]
	public static readonly DependencyProperty RightProperty = DependencyProperty.RegisterAttached(
		"Right",
		typeof(double),
		typeof(FlexPanel),
		new PropertyMetadata(double.NaN, propertyChangedCallback: OnChildPropertyChanged));

	/// <summary>
	/// Sets the offset from the container's right content edge. CSS equivalent: <c>right</c>.
	/// </summary>
	[DynamicDependency(nameof(GetRight))]
	public static void SetRight(DependencyObject element, double value)
	{
		element.SetValue(RightProperty, value);
	}

	/// <summary>
	/// Gets the offset from the container's right content edge. CSS equivalent: <c>right</c>.
	/// </summary>
	[DynamicDependency(nameof(SetRight))]
	public static double GetRight(DependencyObject element)
	{
		return (double)element.GetValue(RightProperty);
	}

	// -- Bottom Attached Property --
	[DynamicDependency(nameof(GetBottom))]
	public static readonly DependencyProperty BottomProperty = DependencyProperty.RegisterAttached(
		"Bottom",
		typeof(double),
		typeof(FlexPanel),
		new PropertyMetadata(double.NaN, propertyChangedCallback: OnChildPropertyChanged));

	/// <summary>
	/// Sets the offset from the container's bottom content edge. CSS equivalent: <c>bottom</c>.
	/// </summary>
	[DynamicDependency(nameof(GetBottom))]
	public static void SetBottom(DependencyObject element, double value)
	{
		element.SetValue(BottomProperty, value);
	}

	/// <summary>
	/// Gets the offset from the container's bottom content edge. CSS equivalent: <c>bottom</c>.
	/// </summary>
	[DynamicDependency(nameof(SetBottom))]
	public static double GetBottom(DependencyObject element)
	{
		return (double)element.GetValue(BottomProperty);
	}
}
