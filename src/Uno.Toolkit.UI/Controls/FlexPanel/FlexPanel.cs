// Adapted from microsoft/microsoft-ui-reactor's src/Reactor/Yoga/FlexPanel.cs
// @ v0.1.0-preview.13 (c9191b97c40a2e4d6bcbc72df7714184862b4d36). Unlike the engine under
// Layout/Yoga/, this file is ours to maintain and has diverged - edit it freely.
// SPDX-License-Identifier: MIT -- (c) Microsoft Corporation (the original adapter);
// (c) Facebook, Inc. and its affiliates (Yoga). Full license text: THIRD-PARTY-NOTICES.md

using System;
using System.Collections.Generic;
using Uno.Toolkit.UI.Yoga;
using YogaAlign = Uno.Toolkit.UI.Yoga.FlexAlign;

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
/// <see cref="ColumnGap"/>, <see cref="RowGap"/> and <see cref="Padding"/>. Per-child behavior
/// uses the attached properties <c>Grow</c>, <c>Shrink</c>, <c>Basis</c>, <c>FlexMinWidth</c>,
/// <c>FlexMinHeight</c>, <c>AlignSelf</c>, <c>Position</c> and the <c>Left</c> / <c>Top</c> /
/// <c>Right</c> / <c>Bottom</c> insets.
/// </para>
/// <para>
/// Right-to-left layout comes from <see cref="FrameworkElement.FlowDirection"/>, like any other
/// panel: the platform mirrors the whole panel, including the <c>Left</c> / <c>Right</c> insets.
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

	// Children already measured through Yoga's MeasureFunction this pass. Such a child must not be
	// measured a second time at Yoga's resolved (rounded) size: a finite re-measure makes the child
	// re-run its own measure logic and DesiredSize can drift sub-pixel, which shows up as a 1px
	// wobble during resize.
	private readonly HashSet<UIElement> _measuredThisPass = new();

	private Size _cachedDesiredSize;
	private bool _arranging;

	// The height-axis MeasureMode the parent FlexPanel last measured this panel with, set by the
	// parent's MeasureFunction on its direct children only. It lets this panel tell two meanings of
	// "AtMost" apart: the ordinary WinUI contract (Stretch means fill up to this) and an outer
	// FlexPanel measuring content for basis resolution (a soft cap, not a fill target). Treating the
	// latter as fill would report the cap as DesiredSize and defeat the outer's grow distribution.
	// It is only honored while the parent really is a FlexPanel, so a stale value left from an
	// earlier parent, or one seen through an intermediate Grid, cannot leak in.
	private YogaMeasureMode? _parentFlexHeightMode;

	private struct ChildLayout
	{
		public float X, Y, Width, Height;
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
		// Attached values are re-read on every pass, so a change only needs a new pass. A child that
		// is not currently in a FlexPanel is read afresh when it next gets measured by one.
		if (d is UIElement element && VisualTreeHelper.GetParent(element) is FlexPanel panel)
		{
			panel.InvalidateMeasure();
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
		//     height. Suppressed when a parent FlexPanel is measuring us for content rather than
		//     fill, which _parentFlexHeightMode reports.
		//  3. Otherwise `height: auto` - content-sized, and content overflows a smaller parent.
		var hasExplicitHeight = !double.IsNaN(Height);
		var parentHeightMode = VisualTreeHelper.GetParent(this) is FlexPanel
			? _parentFlexHeightMode
			: null;
		var outerFlexWantsContent =
			parentHeightMode == YogaMeasureMode.Undefined ||
			parentHeightMode == YogaMeasureMode.AtMost;
		var fillBlockAxis = !hasExplicitHeight
			&& !outerFlexWantsContent
			&& VerticalAlignment == VerticalAlignment.Stretch
			&& hasDefiniteHeight;

		_rootNode.MaxHeight = YogaValue.Undefined;

		var rootHeight = hasExplicitHeight
			? (float)Height
			: fillBlockAxis ? (float)availableSize.Height
			: float.NaN;

		_rootNode.CalculateLayout(rootWidth, rootHeight);

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

				_rootNode.CalculateLayout((float)finalSize.Width, arrangeHeight);

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
		_rootNode.FlexDirection = Direction.ToYoga();
		_rootNode.JustifyContent = JustifyContent.ToYoga();
		_rootNode.AlignItems = AlignItems.ToYoga(fallback: YogaAlign.Stretch);
		_rootNode.AlignContent = AlignContent.ToYoga(fallback: YogaAlign.FlexStart);
		_rootNode.FlexWrap = Wrap.ToYoga();
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
			// The "is this AtMost a fill target?" question is answered by handing heightMode to a
			// direct FlexPanel child, not by altering the constraint.
			var constraintWidth = widthMode == YogaMeasureMode.Undefined
				? double.PositiveInfinity
				: width + marginH;
			var constraintHeight = heightMode == YogaMeasureMode.Undefined
				? double.PositiveInfinity
				: height + marginV;

			if (child is FlexPanel innerFlex && innerFlex._parentFlexHeightMode != heightMode)
			{
				// WinUI's measure cache keys on the constraint alone, so a mode change at an
				// unchanged constraint would otherwise return a DesiredSize computed for the old mode.
				innerFlex._parentFlexHeightMode = heightMode;
				innerFlex.InvalidateMeasure();
			}

			child.Measure(new Size(constraintWidth, constraintHeight));

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

	private static void ApplyAttachedProperties(UIElement element, YogaNode node)
	{
		// Read straight from the property system every pass, so every value -- attached or not -- is
		// guaranteed current. An earlier per-child snapshot cache was removed: no profile showed these
		// reads as a cost, and the cache needed its own invalidation for children changed while
		// detached from a panel.
		//
		// The node is dirtied on every pass too (the node.MinWidth / Width / SetMargin setters below
		// always do). Do not guard those writes with "skip if unchanged": when only a child's content
		// changes, every synced value is identical, and a clean node would make Yoga serve its cached
		// measurement without ever calling MeasureFunction, so the child's new size would be lost.
		// Re-measuring stays cheap because WinUI short-circuits child.Measure at an unchanged
		// constraint.
		var basis = GetBasis(element);
		node.Style.FlexGrow = (float)GetGrow(element);
		node.Style.FlexShrink = (float)GetShrink(element);
		node.Style.FlexBasis = double.IsNaN(basis)
			? YogaValue.Auto
			: YogaValue.Point((float)basis);
		node.Style.AlignSelf = GetAlignSelf(element).ToYoga(fallback: YogaAlign.Auto);
		node.Style.PositionType = GetPosition(element).ToYoga();

		// No automatic (CSS 4.5 min-content) floor: the minimum is 0, as in Yoga and React Native,
		// unless the caller sets one. WinUI has no way to ask an element for its min-content size -- a
		// zero-width Measure clamps DesiredSize to 0 -- so such a floor could not be computed anyway.
		node.MinWidth = ToMin(GetFlexMinWidth(element));
		node.MinHeight = ToMin(GetFlexMinHeight(element));

		node.Style.Position[(int)YogaEdge.Left] = ToInset(GetLeft(element));
		node.Style.Position[(int)YogaEdge.Top] = ToInset(GetTop(element));
		node.Style.Position[(int)YogaEdge.Right] = ToInset(GetRight(element));
		node.Style.Position[(int)YogaEdge.Bottom] = ToInset(GetBottom(element));

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

		static YogaValue ToMin(double value)
			=> double.IsNaN(value) ? YogaValue.Undefined : YogaValue.Point((float)Math.Max(0, value));
	}
}
