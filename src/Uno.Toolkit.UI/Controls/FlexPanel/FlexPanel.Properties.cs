// Adapted from microsoft/microsoft-ui-reactor's src/Reactor/Yoga/FlexPanel.cs
// @ v0.1.0-preview.13 (c9191b97c40a2e4d6bcbc72df7714184862b4d36). Unlike the engine under
// Layout/Yoga/, this file is ours to maintain and has diverged - edit it freely.
// SPDX-License-Identifier: MIT -- (c) Microsoft Corporation (the original adapter);
// (c) Facebook, Inc. and its affiliates (Yoga). Full license text: THIRD-PARTY-NOTICES.md

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
	/// <summary>Identifies the <see cref="Direction"/> dependency property.</summary>
	public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
		nameof(Direction),
		typeof(FlexDirection),
		typeof(FlexPanel),
		new PropertyMetadata(FlexDirection.Row, propertyChangedCallback: OnContainerPropertyChanged));

	/// <summary>
	/// Gets or sets the direction of the main axis along which children are laid out.
	/// CSS equivalent: <c>flex-direction</c>.
	/// </summary>
	/// <remarks>Defaults to <see cref="FlexDirection.Row"/>, the CSS initial value.</remarks>
	public FlexDirection Direction
	{
		get => (FlexDirection)GetValue(DirectionProperty);
		set => SetValue(DirectionProperty, value);
	}

	// -- Wrap DependencyProperty --
	/// <summary>Identifies the <see cref="Wrap"/> dependency property.</summary>
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
	/// <summary>Identifies the <see cref="JustifyContent"/> dependency property.</summary>
	public static readonly DependencyProperty JustifyContentProperty = DependencyProperty.Register(
		nameof(JustifyContent),
		typeof(FlexJustify),
		typeof(FlexPanel),
		new PropertyMetadata(FlexJustify.FlexStart, propertyChangedCallback: OnContainerPropertyChanged));

	/// <summary>
	/// Gets or sets how children are distributed along the main axis.
	/// CSS equivalent: <c>justify-content</c>.
	/// </summary>
	public FlexJustify JustifyContent
	{
		get => (FlexJustify)GetValue(JustifyContentProperty);
		set => SetValue(JustifyContentProperty, value);
	}

	// -- AlignItems DependencyProperty --
	/// <summary>Identifies the <see cref="AlignItems"/> dependency property.</summary>
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
	/// <summary>Identifies the <see cref="AlignContent"/> dependency property.</summary>
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
	/// <summary>Identifies the <see cref="ColumnGap"/> dependency property.</summary>
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
	/// <summary>Identifies the <see cref="RowGap"/> dependency property.</summary>
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
	/// <summary>Identifies the <see cref="Padding"/> dependency property.</summary>
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

	// -- Grow Attached Property --
	/// <summary>Identifies the <c>FlexPanel.Grow</c> attached property.</summary>
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
	/// <summary>Identifies the <c>FlexPanel.Shrink</c> attached property.</summary>
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
	/// <summary>Identifies the <c>FlexPanel.Basis</c> attached property.</summary>
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
	/// <summary>Identifies the <c>FlexPanel.FlexMinWidth</c> attached property.</summary>
	[DynamicDependency(nameof(GetFlexMinWidth))]
	public static readonly DependencyProperty FlexMinWidthProperty = DependencyProperty.RegisterAttached(
		"FlexMinWidth",
		typeof(double),
		typeof(FlexPanel),
		new PropertyMetadata(double.NaN, propertyChangedCallback: OnChildPropertyChanged));

	/// <summary>
	/// Sets the minimum width of the element's flex slot, whatever the <see cref="Direction"/>.
	/// CSS equivalent: <c>min-width</c>.
	/// </summary>
	/// <remarks>
	/// <see cref="double.NaN"/> (the default) means no floor, so the element can shrink to <c>0</c>.
	/// Unlike CSS, there is no automatic min-content floor: WinUI offers no way to ask an element for
	/// its min-content size. Set <c>Shrink</c> to <c>0</c> to keep an element at its natural size.
	/// </remarks>
	[DynamicDependency(nameof(GetFlexMinWidth))]
	public static void SetFlexMinWidth(DependencyObject element, double value)
	{
		element.SetValue(FlexMinWidthProperty, value);
	}

	/// <summary>
	/// Gets the minimum width of the element's flex slot. CSS equivalent: <c>min-width</c>.
	/// </summary>
	[DynamicDependency(nameof(SetFlexMinWidth))]
	public static double GetFlexMinWidth(DependencyObject element)
	{
		return (double)element.GetValue(FlexMinWidthProperty);
	}

	// -- FlexMinHeight Attached Property --
	/// <summary>Identifies the <c>FlexPanel.FlexMinHeight</c> attached property.</summary>
	[DynamicDependency(nameof(GetFlexMinHeight))]
	public static readonly DependencyProperty FlexMinHeightProperty = DependencyProperty.RegisterAttached(
		"FlexMinHeight",
		typeof(double),
		typeof(FlexPanel),
		new PropertyMetadata(double.NaN, propertyChangedCallback: OnChildPropertyChanged));

	/// <summary>
	/// Sets the minimum height of the element's flex slot, whatever the <see cref="Direction"/>.
	/// CSS equivalent: <c>min-height</c>.
	/// </summary>
	/// <remarks>
	/// <see cref="double.NaN"/> (the default) means no floor. See <see cref="SetFlexMinWidth"/>.
	/// </remarks>
	[DynamicDependency(nameof(GetFlexMinHeight))]
	public static void SetFlexMinHeight(DependencyObject element, double value)
	{
		element.SetValue(FlexMinHeightProperty, value);
	}

	/// <summary>
	/// Gets the minimum height of the element's flex slot. CSS equivalent: <c>min-height</c>.
	/// </summary>
	[DynamicDependency(nameof(SetFlexMinHeight))]
	public static double GetFlexMinHeight(DependencyObject element)
	{
		return (double)element.GetValue(FlexMinHeightProperty);
	}

	// -- AlignSelf Attached Property --
	/// <summary>Identifies the <c>FlexPanel.AlignSelf</c> attached property.</summary>
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
	/// <summary>Identifies the <c>FlexPanel.Position</c> attached property.</summary>
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
	/// <summary>Identifies the <c>FlexPanel.Left</c> attached property.</summary>
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
	/// <summary>Identifies the <c>FlexPanel.Top</c> attached property.</summary>
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
	/// <summary>Identifies the <c>FlexPanel.Right</c> attached property.</summary>
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
	/// <summary>Identifies the <c>FlexPanel.Bottom</c> attached property.</summary>
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
