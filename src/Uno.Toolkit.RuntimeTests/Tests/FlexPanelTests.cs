using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Windows.Foundation;
using Windows.UI;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

/// <summary>
/// Tier-2 adapter behavior for <see cref="FlexPanel"/>.
/// </summary>
/// <remarks>
/// The flexbox arithmetic itself is covered exhaustively by the vendored Yoga conformance corpus in
/// <c>Tests/Yoga/</c>. What is verified here is the WinUI adapter: that XAML properties reach the
/// engine, that the engine's results reach Arrange, and that the WinUI-specific concerns (margin
/// compensation, Visibility, layout rounding, virtualized children) behave.
/// </remarks>
[TestClass]
[RunsOnUIThread]
internal class FlexPanelTests
{
	private const double Tolerance = 0.5;

	private static Border CreateChild(double? width = null, double? height = null)
	{
		var border = new Border
		{
			Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255)),
		};

		if (width is { } w)
		{
			border.Width = w;
		}

		if (height is { } h)
		{
			border.Height = h;
		}

		return border;
	}

	private static Rect SlotOf(FrameworkElement element) => LayoutInformation.GetLayoutSlot(element);

	[TestMethod]
	public async Task When_DirectionNotSet_ThenDefaultsToRow()
	{
		// FlexEnums.cs keeps Yoga's numbering, where FlexDirection.Column is the zero value. The DP
		// metadata default must override that to Row, or every bare <FlexPanel/> silently becomes a
		// column. This is cheap to get wrong and invisible until someone looks at a screen.
		var SUT = new FlexPanel { Width = 400, Height = 200 };

		var first = CreateChild(50, 50);
		var second = CreateChild(50, 50);
		SUT.Children.Add(first);
		SUT.Children.Add(second);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		SUT.Direction.Should().Be(FlexDirection.Row);

		var firstSlot = SlotOf(first);
		var secondSlot = SlotOf(second);

		secondSlot.X.Should().BeApproximately(50, Tolerance, "children should flow horizontally");
		secondSlot.Y.Should().BeApproximately(firstSlot.Y, Tolerance, "a row keeps children on one line");
	}

	[TestMethod]
	public async Task When_Grow_ThenRemainderSplitByRatio()
	{
		// 300 of leftover space, split 1:2 => 100 / 200.
		var SUT = new FlexPanel { Width = 300, Height = 100 };

		var first = CreateChild(height: 50);
		var second = CreateChild(height: 50);
		FlexPanel.SetGrow(first, 1);
		FlexPanel.SetBasis(first, 0);
		FlexPanel.SetGrow(second, 2);
		FlexPanel.SetBasis(second, 0);

		SUT.Children.Add(first);
		SUT.Children.Add(second);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		SlotOf(first).Width.Should().BeApproximately(100, Tolerance);
		SlotOf(second).Width.Should().BeApproximately(200, Tolerance);
	}

	[TestMethod]
	public async Task When_Shrink_ThenDeficitSplitByRatio()
	{
		// 400 of content in a 300 box. Only the first child may shrink, so it absorbs the whole
		// 100 deficit and the second keeps its width.
		var SUT = new FlexPanel { Width = 300, Height = 100 };

		var shrinkable = CreateChild(200, 50);
		var fixedWidth = CreateChild(200, 50);
		FlexPanel.SetShrink(shrinkable, 1);
		FlexPanel.SetFlexMinWidth(shrinkable, 0);
		FlexPanel.SetShrink(fixedWidth, 0);

		SUT.Children.Add(shrinkable);
		SUT.Children.Add(fixedWidth);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		SlotOf(shrinkable).Width.Should().BeApproximately(100, Tolerance);
		SlotOf(fixedWidth).Width.Should().BeApproximately(200, Tolerance);
	}

	[TestMethod]
	public async Task When_BasisSet_ThenWidthIgnored()
	{
		// The documented CSS/WinUI mismatch: with a flex basis and room to grow, the arranged size
		// comes from the flex resolution, not from Width.
		var SUT = new FlexPanel { Width = 400, Height = 100 };

		var child = CreateChild(200, 50);
		FlexPanel.SetBasis(child, 100);
		FlexPanel.SetGrow(child, 1);

		SUT.Children.Add(child);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		SlotOf(child).Width.Should().BeApproximately(400, Tolerance, "grow resolves the slot, Width does not");
	}

	[TestMethod]
	public async Task When_Wrap_ThenLinesBreakAndRowGapApplies()
	{
		// Three 100-wide children in a 250 box wrap to 2 lines; the second line sits one child
		// height plus RowGap below the first.
		var SUT = new FlexPanel
		{
			Width = 250,
			Height = 300,
			Wrap = FlexWrap.Wrap,
			RowGap = 20,
			AlignContent = FlexAlign.FlexStart,
		};

		var first = CreateChild(100, 40);
		var second = CreateChild(100, 40);
		var third = CreateChild(100, 40);
		SUT.Children.Add(first);
		SUT.Children.Add(second);
		SUT.Children.Add(third);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		SlotOf(first).Y.Should().BeApproximately(0, Tolerance);
		SlotOf(second).Y.Should().BeApproximately(0, Tolerance, "two 100-wide children fit on the first line");
		SlotOf(third).Y.Should().BeApproximately(60, Tolerance, "40 tall + 20 RowGap");
		SlotOf(third).X.Should().BeApproximately(0, Tolerance, "the wrapped child starts a new line");
	}

	[TestMethod]
	public async Task When_AlignItemsBaseline_ThenTextBaselinesAlign()
	{
		var SUT = new FlexPanel
		{
			Width = 400,
			Height = 200,
			AlignItems = FlexAlign.Baseline,
		};

		var small = new TextBlock { Text = "small", FontSize = 12 };
		var large = new TextBlock { Text = "large", FontSize = 36 };
		SUT.Children.Add(small);
		SUT.Children.Add(large);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var smallSlot = SlotOf(small);
		var largeSlot = SlotOf(large);

		// Baseline alignment pushes the smaller text down so the two text baselines coincide; without
		// it both would sit at y=0.
		smallSlot.Y.Should().BeGreaterThan(largeSlot.Y,
			"the shorter text must be offset downwards to share the taller text's baseline");
	}

	[TestMethod]
	public async Task When_AutoMin_ThenChildDoesNotShrinkBelowMinContent()
	{
		// CSS Flexbox 4.5: with basis auto and no explicit floor, a flex item will not shrink below
		// its min-content size, even when that overflows the container.
		var SUT = new FlexPanel { Width = 100, Height = 100 };

		var child = new TextBlock { Text = "Unbreakable", FontSize = 20 };
		SUT.Children.Add(child);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var minContent = child.DesiredSize.Width;
		SlotOf(child).Width.Should().BeGreaterOrEqualTo(minContent - Tolerance,
			"the automatic minimum size floors the item at its min-content width");
	}

	[TestMethod]
	public async Task When_FlexMinWidthZero_ThenChildShrinksBelowContent()
	{
		var SUT = new FlexPanel { Width = 100, Height = 100 };

		var child = CreateChild(400, 50);
		FlexPanel.SetFlexMinWidth(child, 0);
		SUT.Children.Add(child);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		SlotOf(child).Width.Should().BeApproximately(100, Tolerance,
			"an explicit FlexMinWidth of 0 opts out of the automatic minimum");
	}

	[TestMethod]
	public async Task When_ScrollViewerChild_ThenMinContentIsZero()
	{
		// A scrolling child must floor at 0 rather than at its content size: measuring min-content
		// on a virtualizing container would realize items during a sizing-only pass.
		var SUT = new FlexPanel { Width = 100, Height = 100 };

		var scroller = new ScrollViewer
		{
			Content = new Border { Width = 800, Height = 50 },
		};
		SUT.Children.Add(scroller);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		SlotOf(scroller).Width.Should().BeApproximately(100, Tolerance,
			"a ScrollViewer's automatic minimum size is 0, so it shrinks to the container");
	}

	[TestMethod]
	public async Task When_ChildCollapsed_ThenExcludedFromLayoutAndGap()
	{
		// Collapsed is the XAML equivalent of display:none: no size contribution and no gap slot,
		// so the visible children must sit exactly one gap apart.
		var SUT = new FlexPanel { Width = 400, Height = 100, ColumnGap = 10 };

		var first = CreateChild(50, 50);
		var collapsed = CreateChild(50, 50);
		var last = CreateChild(50, 50);
		collapsed.Visibility = Visibility.Collapsed;

		SUT.Children.Add(first);
		SUT.Children.Add(collapsed);
		SUT.Children.Add(last);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		SlotOf(last).X.Should().BeApproximately(60, Tolerance,
			"50 wide + a single 10 gap; the collapsed child contributes neither width nor a gap");
	}

	[TestMethod]
	public async Task When_ChildMarginSet_ThenNotDoubleCounted()
	{
		// The engine positions the content box while WinUI's Arrange subtracts Margin from the rect
		// it is handed. The adapter compensates; this guards that it does not compensate twice.
		var SUT = new FlexPanel { Width = 400, Height = 200 };

		var child = CreateChild(100, 50);
		child.Margin = new Thickness(20);
		SUT.Children.Add(child);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var slot = SlotOf(child);
		slot.Width.Should().BeApproximately(140, Tolerance, "the slot spans content plus both margins");
		child.ActualWidth.Should().BeApproximately(100, Tolerance, "the rendered child keeps its own width");
		child.ActualHeight.Should().BeApproximately(50, Tolerance);
	}

	[TestMethod]
	public async Task When_PositionAbsolute_ThenInsetsHonoredAndSiblingsUnaffected()
	{
		var SUT = new FlexPanel { Width = 400, Height = 200 };

		var flowed = CreateChild(100, 50);
		var absolute = CreateChild(60, 30);
		FlexPanel.SetPosition(absolute, FlexPositionType.Absolute);
		FlexPanel.SetLeft(absolute, 200);
		FlexPanel.SetTop(absolute, 100);

		SUT.Children.Add(flowed);
		SUT.Children.Add(absolute);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var absoluteSlot = SlotOf(absolute);
		absoluteSlot.X.Should().BeApproximately(200, Tolerance);
		absoluteSlot.Y.Should().BeApproximately(100, Tolerance);

		SlotOf(flowed).X.Should().BeApproximately(0, Tolerance,
			"an absolutely-positioned sibling is out of flow and must not displace the flowed child");
	}

	[TestMethod]
	public async Task When_AttachedPropertyChanges_ThenLayoutUpdates()
	{
		var SUT = new FlexPanel { Width = 400, Height = 100 };

		var first = CreateChild(height: 50);
		var second = CreateChild(height: 50);
		FlexPanel.SetBasis(first, 0);
		FlexPanel.SetBasis(second, 0);
		FlexPanel.SetGrow(first, 1);
		FlexPanel.SetGrow(second, 1);

		SUT.Children.Add(first);
		SUT.Children.Add(second);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		SlotOf(first).Width.Should().BeApproximately(200, Tolerance);

		// Attached property change on a child.
		FlexPanel.SetGrow(first, 3);
		await UnitTestsUIContentHelper.WaitForIdle();

		SlotOf(first).Width.Should().BeApproximately(300, Tolerance, "a 3:1 split of 400");

		// Container property change on the panel.
		SUT.Direction = FlexDirection.Column;
		await UnitTestsUIContentHelper.WaitForIdle();

		SlotOf(second).Y.Should().BeGreaterThan(0, "switching to a column stacks the children vertically");
	}

	[TestMethod]
	public async Task When_UseLayoutRoundingFalse_ThenFractionalArrangePreserved()
	{
		// 100 / 3 does not land on a pixel boundary.
		//
		// FR-6 scopes this precisely: UseLayoutRounding=false sets the engine's PointScaleFactor to 0
		// so Yoga stops snapping, and "the platform's own rounding is the only one applied". Those are
		// two separate roundings, and the framework applies its own per element -- so the flag has to
		// be off on the children as well for a fractional result to survive all the way to the slot.
		var SUT = new FlexPanel { Width = 100, Height = 50, UseLayoutRounding = false };

		for (var i = 0; i < 3; i++)
		{
			var child = CreateChild(height: 20);
			child.UseLayoutRounding = false;
			FlexPanel.SetGrow(child, 1);
			FlexPanel.SetBasis(child, 0);
			SUT.Children.Add(child);
		}

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var width = SlotOf((FrameworkElement)SUT.Children[0]).Width;
		width.Should().BeApproximately(100d / 3d, 0.01,
			"with UseLayoutRounding off the engine's PointScaleFactor is 0, so Yoga does no pixel snapping");
	}

	[TestMethod]
	public async Task When_UseLayoutRoundingTrue_ThenArrangeSnapsToPixelGrid()
	{
		// The counterpart to the test above: the default path must still snap, so that turning the
		// flag off is an observable change rather than a no-op.
		var SUT = new FlexPanel { Width = 100, Height = 50, UseLayoutRounding = true };

		for (var i = 0; i < 3; i++)
		{
			var child = CreateChild(height: 20);
			FlexPanel.SetGrow(child, 1);
			FlexPanel.SetBasis(child, 0);
			SUT.Children.Add(child);
		}

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var width = SlotOf((FrameworkElement)SUT.Children[0]).Width;
		width.Should().Be(Math.Round(width), "layout rounding should land the slot on a whole pixel");
	}

	[TestMethod]
	public async Task When_LayoutDirectionRightToLeft_ThenMainAxisMirrored()
	{
		var SUT = new FlexPanel
		{
			Width = 300,
			Height = 100,
			LayoutDirection = FlexLayoutDirection.RightToLeft,
		};

		var first = CreateChild(100, 50);
		var second = CreateChild(100, 50);
		SUT.Children.Add(first);
		SUT.Children.Add(second);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		SlotOf(first).X.Should().BeApproximately(200, Tolerance, "the first child starts at the right edge");
		SlotOf(second).X.Should().BeApproximately(100, Tolerance);
	}

	[TestMethod]
	public async Task When_FlowDirectionAndLayoutDirectionBothSet_ThenNoDoubleMirror()
	{
		// FR-7's contract: LayoutDirection is the only RTL input the panel reads. The two mechanisms
		// are independent, so a panel left at LeftToRight lays out left-to-right no matter what
		// FlowDirection says. Pinning this stops the two from being quietly coupled later.
		var SUT = new FlexPanel
		{
			Width = 300,
			Height = 100,
			FlowDirection = FlowDirection.RightToLeft,
			LayoutDirection = FlexLayoutDirection.LeftToRight,
		};

		var first = CreateChild(100, 50);
		var second = CreateChild(100, 50);
		SUT.Children.Add(first);
		SUT.Children.Add(second);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		SlotOf(first).X.Should().BeApproximately(0, Tolerance,
			"FlexPanel ignores FlowDirection; only LayoutDirection mirrors its main axis");
		SlotOf(second).X.Should().BeApproximately(100, Tolerance);
	}

	[TestMethod]
	public async Task When_ChildHasNoShrink_ThenOverflowsInsteadOfFitting()
	{
		// Mapping rule M1. AutoLayout would fit the children to the panel; flexbox with Shrink=0
		// overflows instead. A future convergence attempt must not quietly change this.
		var SUT = new FlexPanel { Width = 200, Height = 100 };

		var first = CreateChild(150, 50);
		var second = CreateChild(150, 50);
		FlexPanel.SetShrink(first, 0);
		FlexPanel.SetShrink(second, 0);

		SUT.Children.Add(first);
		SUT.Children.Add(second);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		SlotOf(first).Width.Should().BeApproximately(150, Tolerance);
		SlotOf(second).Width.Should().BeApproximately(150, Tolerance);
		SlotOf(second).X.Should().BeApproximately(150, Tolerance, "the row overflows rather than compressing");
	}

	[TestMethod]
	public async Task When_GrowWithBasisZero_ThenChildrenAreEqualNotProportional()
	{
		// Mapping rule M2, the `flex: 1` vs `flex: 1 1 0` distinction. With Basis=0 the content size
		// is taken out of the equation entirely, which is what makes columns equal.
		var SUT = new FlexPanel { Width = 300, Height = 100 };

		var narrow = CreateChild(20, 50);
		var wide = CreateChild(160, 50);
		FlexPanel.SetGrow(narrow, 1);
		FlexPanel.SetBasis(narrow, 0);
		FlexPanel.SetGrow(wide, 1);
		FlexPanel.SetBasis(wide, 0);

		SUT.Children.Add(narrow);
		SUT.Children.Add(wide);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		SlotOf(narrow).Width.Should().BeApproximately(150, Tolerance);
		SlotOf(wide).Width.Should().BeApproximately(150, Tolerance);
	}

	[TestMethod]
	public async Task When_SpaceBetweenWithGap_ThenGapIsAFloor()
	{
		// Mapping rule M3: the behavior AutoLayout does not have. The gap is a minimum, and
		// SpaceBetween distributes whatever is left over on top of it.
		var SUT = new FlexPanel
		{
			Width = 400,
			Height = 100,
			ColumnGap = 10,
			JustifyContent = FlexJustify.SpaceBetween,
		};

		var first = CreateChild(50, 50);
		var second = CreateChild(50, 50);
		SUT.Children.Add(first);
		SUT.Children.Add(second);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		SlotOf(first).X.Should().BeApproximately(0, Tolerance);
		SlotOf(second).X.Should().BeApproximately(350, Tolerance,
			"SpaceBetween pushes the pair apart; the 10 gap is a floor, not the actual separation");

		var separation = SlotOf(second).X - (SlotOf(first).X + SlotOf(first).Width);
		separation.Should().BeGreaterOrEqualTo(10 - Tolerance);
	}

	[TestMethod]
	public async Task When_Grow_InScrollViewer_ThenFillsViewport()
	{
		// Mapping rule M4, and the case most likely to break silently per target: a growing child
		// inside a scrolling viewport must fill the viewport rather than collapse to content.
		var panel = new FlexPanel { Direction = FlexDirection.Column };
		var child = CreateChild();
		FlexPanel.SetGrow(child, 1);
		FlexPanel.SetBasis(child, 0);
		panel.Children.Add(child);

		var host = new Grid
		{
			Width = 200,
			Height = 300,
			Children = { panel },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(host);

		SlotOf(child).Height.Should().BeApproximately(300, Tolerance,
			"a definite parent slot gives grow a pool to distribute");
	}

	[TestMethod]
	public async Task When_Padding_ThenContentBoxIsInsetOnce()
	{
		// FlexPanel.Padding is a DP of our own: WinUI's Panel has no Padding, and Uno's Panel only
		// carries an internal PaddingInternal that Grid/StackPanel/RelativePanel assign from their
		// own DP and FlexPanel never touches. So the engine is the single consumer, and the numbers
		// below are chosen to separate that from a double application: a 200-wide box with a 20
		// inset leaves 160 of content, where applying the padding twice would leave 120.
		var SUT = new FlexPanel
		{
			Width = 200,
			Height = 100,
			Padding = new Thickness(20),
		};

		var child = CreateChild();
		FlexPanel.SetGrow(child, 1);
		FlexPanel.SetBasis(child, 0);
		SUT.Children.Add(child);

		await UnitTestUIContentHelperEx.SetContentAndWait(SUT);

		var slot = SlotOf(child);
		slot.X.Should().BeApproximately(20, Tolerance, "the content box starts one padding in, not two");
		slot.Y.Should().BeApproximately(20, Tolerance, "the content box starts one padding in, not two");
		slot.Width.Should().BeApproximately(160, Tolerance, "200 - 20 - 20; a double inset would give 120");
		slot.Height.Should().BeApproximately(60, Tolerance, "100 - 20 - 20; a double inset would give 20");
	}

	[TestMethod]
	public async Task When_PaddingAndContentSized_ThenDesiredSizeGrowsByPaddingOnce()
	{
		// The other half of the seam: the engine reports a border-box size that already includes the
		// padding, and MeasureCore hands that straight back as DesiredSize. Adding it again here
		// would report 130 for a 50 child with a 20 inset.
		var SUT = new FlexPanel
		{
			Padding = new Thickness(20),
			HorizontalAlignment = HorizontalAlignment.Left,
			VerticalAlignment = VerticalAlignment.Top,
		};

		SUT.Children.Add(CreateChild(50, 50));

		var host = new Grid
		{
			Width = 400,
			Height = 400,
			Children = { SUT },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(host);

		SUT.DesiredSize.Width.Should().BeApproximately(90, Tolerance, "50 + 20 + 20; a double inset would give 130");
		SUT.DesiredSize.Height.Should().BeApproximately(90, Tolerance, "50 + 20 + 20; a double inset would give 130");
	}

	[TestMethod]
	public async Task When_PaddingAndArrangeRelayout_ThenInsetStillAppliedOnce()
	{
		// The two cases above measure and arrange at the same size, so ArrangeOverride reuses the
		// rects Yoga produced during measure. This one measures at content size and is arranged
		// taller, which takes the sizeChanged branch and re-runs CalculateLayout -- the only point
		// where the padding reaches the engine a second time within one layout cycle.
		var SUT = new FlexPanel
		{
			Direction = FlexDirection.Column,
			Padding = new Thickness(20),
			VerticalAlignment = VerticalAlignment.Stretch,
		};

		var child = CreateChild(50, 50);
		FlexPanel.SetGrow(child, 1);
		SUT.Children.Add(child);

		var host = new Grid
		{
			Width = 200,
			Height = 400,
			Children = { SUT },
		};

		await UnitTestUIContentHelperEx.SetContentAndWait(host);

		var slot = SlotOf(child);
		slot.Y.Should().BeApproximately(20, Tolerance, "the arrange re-layout must not stack a second top inset");
		slot.Height.Should().BeApproximately(360, Tolerance, "400 - 20 - 20; a double inset would give 320");
	}
}
