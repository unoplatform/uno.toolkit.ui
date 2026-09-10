// Adapted from microsoft/microsoft-ui-reactor's src/Reactor/Yoga/FlexPanel.cs
// @ v0.1.0-preview.13 (c9191b97c40a2e4d6bcbc72df7714184862b4d36). Unlike the engine under
// Layout/Yoga/, this file is ours to maintain and has diverged - edit it freely.
// SPDX-License-Identifier: MIT -- (c) Microsoft Corporation (the original adapter);
// (c) Facebook, Inc. and its affiliates (Yoga). Full license text: THIRD-PARTY-NOTICES.md

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Uno.Toolkit.UI.Yoga;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

using Windows.Foundation;

namespace Uno.Toolkit.UI;

/// <summary>
/// A <see cref="Panel"/> that arranges its children using CSS Flexbox semantics, backed by the Yoga
/// layout engine.
/// </summary>
/// <remarks>
/// <para>
/// Container behavior is configured with <see cref="Direction"/>, <see cref="Wrap"/>,
/// <see cref="JustifyContent"/>, <see cref="AlignItems"/>, <see cref="AlignContent"/>,
/// <see cref="ColumnGap"/>, <see cref="RowGap"/>, <see cref="Padding"/> and
/// <see cref="LayoutDirection"/>. Per-child behavior uses the attached properties <c>Grow</c>,
/// <c>Shrink</c>, <c>Basis</c>, <c>FlexMinWidth</c>, <c>FlexMinHeight</c>, <c>AlignSelf</c>,
/// <c>Position</c> and the <c>Left</c> / <c>Top</c> / <c>Right</c> / <c>Bottom</c> insets.
/// </para>
/// <para>
/// A child's <see cref="FrameworkElement.Margin"/>, <see cref="FrameworkElement.Width"/>,
/// <see cref="FrameworkElement.Height"/> and <see cref="UIElement.Visibility"/> participate in
/// layout without any attached property.
/// </para>
/// <para>
/// Being a <see cref="Panel"/>, <c>FlexPanel</c> cannot draw a border or corner radius — wrap it in
/// a <see cref="Border"/> for that. CSS <c>order</c> is not supported, because the underlying engine
/// does not implement it.
/// </para>
/// </remarks>
public partial class FlexPanel : Panel
{
	// Per-instance config so PointScaleFactor can track the live XamlRoot.RasterizationScale.
	private readonly YogaConfig _yogaConfig = new();
	private readonly YogaNode _rootNode;

	// NFR-2: every collection below is an instance field reused across layout passes. Nothing here
	// may be reallocated per pass, and MeasureOverride / ArrangeOverride must stay LINQ-free.
	private readonly Dictionary<UIElement, YogaNode> _nodeCache = new();
	private readonly HashSet<UIElement> _syncCurrentChildren = new();
	private readonly List<UIElement> _syncToRemove = new();
	private readonly List<ChildLayout> _cachedChildLayouts = new();

	// Per-child snapshot of the last-read attached-property values. Reading an attached DP boxes the
	// double/enum, so an uncached read costs ~11 boxed allocations per child per pass. Attached DPs
	// can only change through the property system, which raises OnChildPropertyChanged and drops the
	// entry. Width/Height/Margin/Visibility are deliberately NOT cached: they change without any
	// callback of ours, so they are re-read every pass.
	private readonly Dictionary<UIElement, AttachedProps> _attachedCache = new();

	// An attached DP can change while a child is detached from its panel (removed, re-styled, then
	// re-added before the next measure). OnChildPropertyChanged cannot reach the owning panel in that
	// window, so the child is flagged here instead and re-read on the next sync. Weak-keyed, so it
	// holds nothing alive.
	private static readonly ConditionalWeakTable<UIElement, object> s_detachedAttachedDirty = new();
	private static readonly object s_detachedDirtyMarker = new();

	// Children already measured through Yoga's MeasureFunction this pass. Such a child must not be
	// measured a second time at Yoga's resolved (rounded) size: a finite re-measure makes the child
	// re-run its own measure logic and DesiredSize can drift sub-pixel, which shows up as a 1px
	// wobble during resize.
	private readonly HashSet<UIElement> _measuredThisPass = new();

	private Size _cachedDesiredSize;
	private bool _arranging;

	// Yoga's height-axis MeasureMode for the current MeasureFunction call, threaded down to a nested
	// FlexPanel. It lets the inner panel distinguish two different meanings of "AtMost": the ordinary
	// WinUI contract (Stretch means fill up to this) from an outer FlexPanel measuring content for
	// basis resolution (a soft cap, not a fill target). Treating the latter as fill would make the
	// inner panel report the cap as its DesiredSize and defeat the outer's grow distribution.
	// null = not nested under a FlexPanel measurement, so the WinUI contract applies.
	[ThreadStatic]
	private static YogaMeasureMode? _outerYogaHeightMode;

	private struct ChildLayout
	{
		public float X, Y, Width, Height;
	}

	private struct AttachedProps
	{
		public double Grow, Shrink, Basis, MinWidth, MinHeight, Left, Top, Right, Bottom;
		public FlexAlign AlignSelf;
		public FlexPositionType Position;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="FlexPanel"/> class.
	/// </summary>
	public FlexPanel()
	{
		_rootNode = new YogaNode(_yogaConfig);
		Unloaded += OnUnloaded;
	}

	private static void OnContainerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is FlexPanel panel)
		{
			panel.InvalidateMeasure();
		}
	}

	private static void OnChildPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not UIElement element)
		{
			return;
		}

		if (VisualTreeHelper.GetParent(element) is FlexPanel panel)
		{
			// Drop the cached snapshot so the next sync re-reads the changed value.
			panel._attachedCache.Remove(element);
			panel.InvalidateMeasure();
		}
		else
		{
			// Detached, or not yet parented: we cannot reach the owning panel's cache, so flag the
			// element. The next ApplyAttachedProperties re-reads it rather than trusting a stale
			// snapshot.
			s_detachedAttachedDirty.AddOrUpdate(element, s_detachedDirtyMarker);
		}
	}

	private void OnUnloaded(object sender, RoutedEventArgs e)
	{
		// Release the node graph when leaving the visual tree so removed children are collectable.
		foreach (var node in _nodeCache.Values)
		{
			_rootNode.RemoveChild(node);
		}

		_nodeCache.Clear();
		_attachedCache.Clear();

		// The scratch collections are normally emptied at the end of each pass. Clearing them here
		// too makes the release unconditional rather than dependent on the last pass having
		// completed normally -- a panel unloaded mid-pass would otherwise keep whatever they held.
		_measuredThisPass.Clear();
		_syncToRemove.Clear();
		_syncCurrentChildren.Clear();
		_cachedChildLayouts.Clear();
	}

	/// <summary>
	/// Reads the current rasterization scale and updates the engine's pixel-grid rounding to match.
	/// </summary>
	/// <remarks>
	/// Called from <see cref="MeasureOverride"/> rather than by subscribing to
	/// <c>XamlRoot.Changed</c>: that subscription would pin every panel through XamlRoot's multicast
	/// delegate, which leaks in virtualized lists where the recycle path does not reliably raise
	/// <c>Unloaded</c>.
	/// <para>
	/// When <see cref="UIElement.UseLayoutRounding"/> is <c>false</c>, the scale is set to <c>0</c>,
	/// which the engine treats as "do not round" — leaving the platform's own rounding as the only
	/// one applied.
	/// </para>
	/// </remarks>
	private void SyncPointScaleLazy()
	{
		var scale = UseLayoutRounding
			? (float)(XamlRoot?.RasterizationScale ?? 1.0)
			: 0f;

		if (UseLayoutRounding && scale <= 0)
		{
			return;
		}

		if (Math.Abs(_yogaConfig.PointScaleFactor - scale) < 0.0001f)
		{
			return;
		}

		_yogaConfig.PointScaleFactor = scale;
		_rootNode.MarkDirtyAndPropagate();
	}

	/// <inheritdoc />
	protected override Size MeasureOverride(Size availableSize)
	{
		SyncPointScaleLazy();

		// The whole pass is wrapped so the scratch set is released even if a child's Measure throws.
		// Clearing it only on the way out of a *successful* pass would leave a failed pass holding
		// its children until the panel happens to measure again -- the same "released only on the
		// next pass" shape as the leak the tier-3 tests caught.
		try
		{
			return MeasureCore(availableSize);
		}
		finally
		{
			_measuredThisPass.Clear();
		}
	}

	private Size MeasureCore(Size availableSize)
	{
		SyncYogaTree();
		SetRootConstraints();

		var hasDefiniteWidth = !double.IsInfinity(availableSize.Width);
		var hasDefiniteHeight = !double.IsInfinity(availableSize.Height);

		// Inline axis. A flex container is a block-level box: its width resolves against the
		// containing block before flex layout runs, so `width: auto` fills the parent. Measuring
		// children under a definite cross-axis constraint is also what lets text wrap in one pass.
		// HorizontalAlignment != Stretch is the opt-in `width: fit-content` escape hatch.
		var fillInlineAxis = HorizontalAlignment == HorizontalAlignment.Stretch;

		float rootWidth;
		if (fillInlineAxis && hasDefiniteWidth)
		{
			rootWidth = (float)availableSize.Width;
			_rootNode.MaxWidth = YogaValue.Undefined;
		}
		else
		{
			rootWidth = float.NaN;
			_rootNode.MaxWidth = hasDefiniteWidth
				? YogaValue.Point((float)availableSize.Width)
				: YogaValue.Undefined;
		}

		// Block axis, in priority order:
		//  1. Explicit Height(N) - a definite container size, so the engine can distribute it.
		//  2. VerticalAlignment.Stretch against a definite offer - "fill my parent's slot", the
		//     symmetric counterpart to the inline axis. This is what makes the canonical
		//     header(auto) / body(grow) / footer(auto) pattern fill a viewport without a hardcoded
		//     height. Suppressed when an outer FlexPanel is measuring us for content rather than
		//     fill, which _outerYogaHeightMode reports.
		//  3. Otherwise `height: auto` - content-sized, and content overflows a smaller parent.
		var hasExplicitHeight = !double.IsNaN(Height);
		var outerFlexWantsContent =
			_outerYogaHeightMode == YogaMeasureMode.Undefined ||
			_outerYogaHeightMode == YogaMeasureMode.AtMost;
		var fillBlockAxis = !hasExplicitHeight
			&& !outerFlexWantsContent
			&& VerticalAlignment == VerticalAlignment.Stretch
			&& hasDefiniteHeight;

		_rootNode.MaxHeight = YogaValue.Undefined;

		var rootHeight = hasExplicitHeight
			? (float)Height
			: fillBlockAxis ? (float)availableSize.Height
			: float.NaN;

		_rootNode.CalculateLayout(rootWidth, rootHeight, LayoutDirection);

		// When the panel has a definite own height, the box resolves to that size and never more.
		var hasDefiniteOwnHeight = hasExplicitHeight || fillBlockAxis;
		var reportedHeight = hasDefiniteOwnHeight
			? Math.Min(_rootNode.LayoutHeight, (float)availableSize.Height)
			: _rootNode.LayoutHeight;

		_cachedDesiredSize = new Size(_rootNode.LayoutWidth, reportedHeight);

		// Cache child rects for arrange, and satisfy the WinUI contract that every child is measured
		// during MeasureOverride.
		_cachedChildLayouts.Clear();
		for (var i = 0; i < Children.Count; i++)
		{
			var child = Children[i];
			if (_nodeCache.TryGetValue(child, out var childNode))
			{
				var layout = new ChildLayout
				{
					X = childNode.LayoutX,
					Y = childNode.LayoutY,
					Width = childNode.LayoutWidth,
					Height = childNode.LayoutHeight,
				};
				_cachedChildLayouts.Add(layout);

				// Only measure children Yoga skipped (fixed-dimension children, where it uses the
				// explicit value and bypasses MeasureFunction). Re-measuring one Yoga already
				// measured is the resize-wobble bug.
				if (!_measuredThisPass.Contains(child))
				{
					var margin = child is FrameworkElement childElement ? childElement.Margin : default;
					child.Measure(new Size(
						layout.Width + margin.Left + margin.Right,
						layout.Height + margin.Top + margin.Bottom));
				}
			}
			else
			{
				_cachedChildLayouts.Add(default);
			}
		}

		return _cachedDesiredSize;
	}

	/// <inheritdoc />
	protected override Size ArrangeOverride(Size finalSize)
	{
		// Reuse the measured positions when the final size matches, so Yoga is not re-run here.
		// Re-running it would call child.Measure() through MeasureFunction, and measuring during
		// arrange can raise a layout-cycle exception.
		var sizeChanged =
			Math.Abs(finalSize.Width - _cachedDesiredSize.Width) > 0.5 ||
			Math.Abs(finalSize.Height - _cachedDesiredSize.Height) > 0.5;

		if (sizeChanged)
		{
			_arranging = true;
			try
			{
				_rootNode.MaxWidth = YogaValue.Undefined;
				_rootNode.MaxHeight = YogaValue.Undefined;

				// Same block-axis policy as measure. This runs in arrange, so DesiredSize is
				// unaffected, and _arranging makes MeasureFunction serve cached child sizes rather
				// than re-measuring.
				var hasExplicitHeight = !double.IsNaN(Height);
				var fillBlockAxisAtArrange = !hasExplicitHeight
					&& VerticalAlignment == VerticalAlignment.Stretch
					&& !double.IsInfinity(finalSize.Height);
				var arrangeHeight = hasExplicitHeight || fillBlockAxisAtArrange
					? (float)finalSize.Height
					: float.NaN;

				_rootNode.CalculateLayout((float)finalSize.Width, arrangeHeight, LayoutDirection);

				_cachedChildLayouts.Clear();
				for (var i = 0; i < Children.Count; i++)
				{
					var child = Children[i];
					if (_nodeCache.TryGetValue(child, out var childNode))
					{
						_cachedChildLayouts.Add(new ChildLayout
						{
							X = childNode.LayoutX,
							Y = childNode.LayoutY,
							Width = childNode.LayoutWidth,
							Height = childNode.LayoutHeight,
						});
					}
					else
					{
						_cachedChildLayouts.Add(default);
					}
				}
			}
			finally
			{
				_arranging = false;
			}
		}

		for (var i = 0; i < Children.Count && i < _cachedChildLayouts.Count; i++)
		{
			var layout = _cachedChildLayouts[i];
			var child = Children[i];

			// Yoga positions and sizes the content area; WinUI's Arrange subtracts the child's
			// margin from the rect it is given, so expand by the margin to avoid double-counting.
			var margin = child is FrameworkElement childElement ? childElement.Margin : default;
			child.Arrange(new Rect(
				layout.X - margin.Left,
				layout.Y - margin.Top,
				layout.Width + margin.Left + margin.Right,
				layout.Height + margin.Top + margin.Bottom));
		}

		return finalSize;
	}

	private void SetRootConstraints()
	{
		_rootNode.FlexDirection = Direction;
		_rootNode.JustifyContent = JustifyContent;
		_rootNode.AlignItems = AlignItems;
		_rootNode.AlignContent = AlignContent;
		_rootNode.FlexWrap = Wrap;
		_rootNode.SetGap(YogaGutter.Column, (float)ColumnGap);
		_rootNode.SetGap(YogaGutter.Row, (float)RowGap);

		var padding = Padding;
		_rootNode.SetPadding(YogaEdge.Left, YogaValue.Point((float)padding.Left));
		_rootNode.SetPadding(YogaEdge.Top, YogaValue.Point((float)padding.Top));
		_rootNode.SetPadding(YogaEdge.Right, YogaValue.Point((float)padding.Right));
		_rootNode.SetPadding(YogaEdge.Bottom, YogaValue.Point((float)padding.Bottom));
	}

	private void SyncYogaTree()
	{
		try
		{
			SyncYogaTreeCore();
		}
		finally
		{
			// Release the scratch references unconditionally. These are instance fields reused
			// across passes (NFR-2), so anything left in them stays reachable until the next
			// layout -- and for _syncToRemove that means the very children just evicted from the
			// caches would be pinned by the panel that no longer has them. Clearing allocates
			// nothing.
			_syncToRemove.Clear();
			_syncCurrentChildren.Clear();
		}
	}

	private void SyncYogaTreeCore()
	{
		// Drop nodes for children that are gone. This is also what keeps removed children
		// collectable, so it must run on every pass.
		_syncCurrentChildren.Clear();
		foreach (UIElement child in Children)
		{
			_syncCurrentChildren.Add(child);
		}

		_syncToRemove.Clear();
		foreach (var pair in _nodeCache)
		{
			if (!_syncCurrentChildren.Contains(pair.Key))
			{
				_syncToRemove.Add(pair.Key);
			}
		}

		for (var i = 0; i < _syncToRemove.Count; i++)
		{
			var element = _syncToRemove[i];
			if (_nodeCache.TryGetValue(element, out var node))
			{
				_rootNode.RemoveChild(node);
			}

			_nodeCache.Remove(element);
			_attachedCache.Remove(element);
		}

		for (var i = 0; i < Children.Count; i++)
		{
			var child = Children[i];
			if (!_nodeCache.TryGetValue(child, out var childNode))
			{
				childNode = new YogaNode(_yogaConfig);
				_nodeCache[child] = childNode;
				childNode.MeasureFunction = CreateMeasureFunction(child);
			}

			ApplyAttachedProperties(child, childNode);

			// Visibility="Collapsed" is the XAML equivalent of `display: none`: the child
			// contributes no size and no gap slot. StackPanel behaves the same way.
			childNode.Display = child.Visibility == Visibility.Collapsed
				? YogaDisplay.None
				: YogaDisplay.Flex;

			if (i < _rootNode.ChildCount)
			{
				if (_rootNode.GetChild(i) != childNode)
				{
					_rootNode.RemoveChild(childNode);
					_rootNode.InsertChild(childNode, i);
				}
			}
			else if (childNode.Owner != _rootNode)
			{
				_rootNode.InsertChild(childNode, i);
			}
		}

		while (_rootNode.ChildCount > Children.Count)
		{
			_rootNode.RemoveChild(_rootNode.ChildCount - 1);
		}
	}

	/// <summary>
	/// Builds the bridge that lets the layout engine measure a child through WinUI.
	/// </summary>
	/// <remarks>
	/// The engine's constraints describe the content area and exclude margin, while WinUI subtracts
	/// margin during Measure — so margin is added on the way in and subtracted on the way out.
	/// </remarks>
	private YogaMeasureFunc CreateMeasureFunction(UIElement child)
	{
		return (node, width, widthMode, height, heightMode) =>
		{
			var margin = child is FrameworkElement childElement ? childElement.Margin : default;
			var marginH = margin.Left + margin.Right;
			var marginV = margin.Top + margin.Bottom;

			if (_arranging)
			{
				// Measuring during arrange can raise a layout-cycle exception, so serve the cached
				// size. Still clamp to an Exactly slot, so a stale oversized DesiredSize from an
				// earlier pass cannot re-expand the box.
				var cachedWidth = (float)(child.DesiredSize.Width - marginH);
				var cachedHeight = (float)(child.DesiredSize.Height - marginV);

				if (widthMode == YogaMeasureMode.Exactly)
				{
					cachedWidth = (float)width;
				}

				if (heightMode == YogaMeasureMode.Exactly)
				{
					cachedHeight = (float)height;
				}

				return new YogaSize(Math.Max(0, cachedWidth), Math.Max(0, cachedHeight));
			}

			// Undefined means "report your content size", so the constraint is infinite. AtMost and
			// Exactly pass a real cap, which TextBlock wrapping and Image stretch sizing depend on.
			// The "is this AtMost a fill target?" question is answered by publishing heightMode for
			// a nested FlexPanel to read, not by altering the constraint.
			var constraintWidth = widthMode == YogaMeasureMode.Undefined
				? double.PositiveInfinity
				: width + marginH;
			var constraintHeight = heightMode == YogaMeasureMode.Undefined
				? double.PositiveInfinity
				: height + marginV;

			var previousMode = _outerYogaHeightMode;
			_outerYogaHeightMode = heightMode;
			try
			{
				child.Measure(new Size(constraintWidth, constraintHeight));
			}
			finally
			{
				_outerYogaHeightMode = previousMode;
			}

			_measuredThisPass.Add(child);

			// In Exactly mode the engine is the authority on the slot size. A child may report a
			// larger DesiredSize if it ignores its constraint; per CSS the layout box is still the
			// slot, and the content simply overflows visually. Without the clamp, that child would
			// widen the panel and defeat grow distribution.
			var measuredWidth = (float)(child.DesiredSize.Width - marginH);
			var measuredHeight = (float)(child.DesiredSize.Height - marginV);

			if (widthMode == YogaMeasureMode.Exactly)
			{
				measuredWidth = (float)width;
			}

			if (heightMode == YogaMeasureMode.Exactly)
			{
				measuredHeight = (float)height;
			}

			return new YogaSize(Math.Max(0, measuredWidth), Math.Max(0, measuredHeight));
		};
	}

	private void ApplyAttachedProperties(UIElement element, YogaNode node)
	{
		var hit = _attachedCache.TryGetValue(element, out var attached);
		if (hit && s_detachedAttachedDirty.TryGetValue(element, out _))
		{
			// The snapshot was invalidated while the child was detached from a panel.
			hit = false;
		}

		if (!hit)
		{
			s_detachedAttachedDirty.Remove(element);
			attached = new AttachedProps
			{
				Grow = GetGrow(element),
				Shrink = GetShrink(element),
				Basis = GetBasis(element),
				AlignSelf = GetAlignSelf(element),
				Position = GetPosition(element),
				MinWidth = GetFlexMinWidth(element),
				MinHeight = GetFlexMinHeight(element),
				Left = GetLeft(element),
				Top = GetTop(element),
				Right = GetRight(element),
				Bottom = GetBottom(element),
			};
			_attachedCache[element] = attached;
		}

		node.Style.FlexGrow = (float)attached.Grow;
		node.Style.FlexShrink = (float)attached.Shrink;
		node.Style.FlexBasis = double.IsNaN(attached.Basis)
			? YogaValue.Auto
			: YogaValue.Point((float)attached.Basis);
		node.Style.AlignSelf = attached.AlignSelf;
		node.Style.PositionType = attached.Position;

		var mainAxisIsRow = FlexDirectionHelper.IsRow(Direction);
		node.MinWidth = ResolveMinDimension(
			element,
			axisIsMain: mainAxisIsRow,
			explicitMin: attached.MinWidth,
			basis: attached.Basis,
			isWidth: true);
		node.MinHeight = ResolveMinDimension(
			element,
			axisIsMain: !mainAxisIsRow,
			explicitMin: attached.MinHeight,
			basis: attached.Basis,
			isWidth: false);

		node.Style.Position[(int)YogaEdge.Left] = ToInset(attached.Left);
		node.Style.Position[(int)YogaEdge.Top] = ToInset(attached.Top);
		node.Style.Position[(int)YogaEdge.Right] = ToInset(attached.Right);
		node.Style.Position[(int)YogaEdge.Bottom] = ToInset(attached.Bottom);

		// FR-4: Width / Height / Margin participate with no attached property. These are re-read
		// every pass because they change without raising OnChildPropertyChanged.
		if (element is FrameworkElement frameworkElement)
		{
			node.Width = double.IsNaN(frameworkElement.Width)
				? YogaValue.Auto
				: YogaValue.Point((float)frameworkElement.Width);
			node.Height = double.IsNaN(frameworkElement.Height)
				? YogaValue.Auto
				: YogaValue.Point((float)frameworkElement.Height);

			var margin = frameworkElement.Margin;
			if (margin.Left != 0 || margin.Top != 0 || margin.Right != 0 || margin.Bottom != 0)
			{
				node.SetMargin(YogaEdge.Left, YogaValue.Point((float)margin.Left));
				node.SetMargin(YogaEdge.Top, YogaValue.Point((float)margin.Top));
				node.SetMargin(YogaEdge.Right, YogaValue.Point((float)margin.Right));
				node.SetMargin(YogaEdge.Bottom, YogaValue.Point((float)margin.Bottom));
			}
			else
			{
				node.SetMargin(YogaEdge.Left, YogaValue.Undefined);
				node.SetMargin(YogaEdge.Top, YogaValue.Undefined);
				node.SetMargin(YogaEdge.Right, YogaValue.Undefined);
				node.SetMargin(YogaEdge.Bottom, YogaValue.Undefined);
			}
		}

		static YogaValue ToInset(double value)
			=> double.IsNaN(value) ? YogaValue.Undefined : YogaValue.Point((float)value);
	}

	/// <summary>
	/// Resolves the CSS Flexbox automatic minimum size (section 4.5) for one axis.
	/// </summary>
	private YogaValue ResolveMinDimension(
		UIElement element,
		bool axisIsMain,
		double explicitMin,
		double basis,
		bool isWidth)
	{
		// An explicit value always wins, including 0 - that is how a caller opts out of the floor.
		if (!double.IsNaN(explicitMin))
		{
			return YogaValue.Point((float)Math.Max(0, explicitMin));
		}

		// The automatic minimum applies only on the main axis; the cross axis defaults to no floor.
		if (!axisIsMain)
		{
			return YogaValue.Undefined;
		}

		// basis: 0 means the specified-size suggestion is 0, so the minimum is 0. No pre-measure.
		if (!double.IsNaN(basis) && basis <= 0)
		{
			return YogaValue.Point(0);
		}

		// A scrolling container's CSS overflow is `scroll`, which makes the automatic minimum 0.
		// Skipping the pre-measure here also stops a sizing-only pass from realizing virtualized
		// content. Callers who want a floor can still set FlexMinWidth / FlexMinHeight.
		if (IsScrollLikeContainer(element))
		{
			return YogaValue.Point(0);
		}

		var minContent = ComputeMinContent(element, isWidth);

		// With a definite positive basis the automatic minimum is min(basis, min-content).
		if (!double.IsNaN(basis))
		{
			return YogaValue.Point((float)Math.Max(0, Math.Min(basis, minContent)));
		}

		return YogaValue.Point((float)Math.Max(0, minContent));
	}

	private static bool IsScrollLikeContainer(UIElement element)
		=> element is ScrollViewer or ScrollView;

	/// <summary>
	/// Approximates the min-content size of a child by measuring it against a zero-width (or
	/// zero-height) constraint, which reports the widest unbreakable content for text and the
	/// natural size for fixed-size controls.
	/// </summary>
	private static double ComputeMinContent(UIElement element, bool isWidth)
	{
		var margin = element is FrameworkElement frameworkElement ? frameworkElement.Margin : default;
		var marginH = margin.Left + margin.Right;
		var marginV = margin.Top + margin.Bottom;

		// This pollutes the child's cached DesiredSize, but the Yoga measure pass (or the final
		// sweep in MeasureOverride) re-measures with the real constraint. A restore-measure would
		// itself perturb layout, so there is none.
		var constraint = isWidth
			? new Size(marginH, double.PositiveInfinity)
			: new Size(double.PositiveInfinity, marginV);

		element.Measure(constraint);

		return isWidth
			? Math.Max(0, element.DesiredSize.Width - marginH)
			: Math.Max(0, element.DesiredSize.Height - marginV);
	}
}
