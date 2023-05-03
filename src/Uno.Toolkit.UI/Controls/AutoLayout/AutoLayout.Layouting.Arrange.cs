using System;
using System.Runtime.CompilerServices;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI;

partial class AutoLayout
{
	protected override Size ArrangeOverride(Size finalSize)
	{
		// Local cache of DependencyProperties
		var children = Children;
		var orientation = Orientation;
		var justify = Justify;
		var primaryAxisAlignment = PrimaryAxisAlignment;
		var padding = Padding;
		var spacing = Spacing is > double.NegativeInfinity and < double.PositiveInfinity and { } s ? s : 0d;
		var isHorizontal = orientation == Orientation.Horizontal;
		var borderThickness = BorderThickness;

		if (BorderBrush is null)
		{
			// BorderBrush is not set, so we don't need to consider the border thickness
			borderThickness = default;
		}

		var finalSizeMinusBorderThickness = new Size(
			finalSize.Width - (borderThickness.Left + borderThickness.Right),
			finalSize.Height - (borderThickness.Top + borderThickness.Bottom));

		var totalNonFilledStackedSize = 0d;
		var numberOfFilledChildren = 0;
		var numberOfStackedChildren = 0;
		var haveStartPadding = false;
		var haveEndPadding = false;

		if (_calculatedChildren is null || children.Count != _calculatedChildren.Length)
		{
			// Children list changed, invalidate measure and wait for next pass
			InvalidateMeasure();
			return finalSize;
		}

		// 1. Calculate the total size of non-filled and filled children
		for (var i = 0; i < children.Count; i++)
		{
			var child = children[i];
			var calculatedChild = _calculatedChildren[i];

			if (!ReferenceEquals(child, calculatedChild.Element))
			{
				// Invalid calculated child, invalidate measure and wait for next pass to fix this.
				InvalidateMeasure();
				return finalSize;
			}

			switch (calculatedChild.Role)
			{
				case AutoLayoutRole.Independent:
					continue; // Independent children are not stacked, skip it from the calculation
				case AutoLayoutRole.Filled:
					numberOfFilledChildren++;
					break;
				case AutoLayoutRole.Hug:
				case AutoLayoutRole.Fixed:
				default:
					totalNonFilledStackedSize += calculatedChild.MeasuredLength;
					break;
			}

			numberOfStackedChildren++;
		}

		var atLeastOneFilledChild = numberOfFilledChildren > 0;

		switch (primaryAxisAlignment)
		{
			case AutoLayoutAlignment.Start:
				haveStartPadding = true;
				haveEndPadding = atLeastOneFilledChild || justify == AutoLayoutJustify.SpaceBetween;
				break;
			case AutoLayoutAlignment.End:
				haveStartPadding = atLeastOneFilledChild || justify == AutoLayoutJustify.SpaceBetween;
				haveEndPadding = true;
				break;
		}

		var startPadding = haveStartPadding
			? isHorizontal ? padding.Left : padding.Top
			: 0d;
		var endPadding = haveEndPadding
			? isHorizontal ? padding.Right : padding.Bottom
			: 0d;

		var totalPaddingSize = startPadding + endPadding;

		// Available Size is the final size minus the border thickness and the padding
		var availableSizeForStackedChildren = finalSize.GetLength(orientation) - (borderThickness.GetLength(orientation) + totalPaddingSize);
		EnsureZeroFloor(ref availableSizeForStackedChildren);

		// Start the offset at the border + start padding
		var currentOffset = borderThickness.GetStartLength(orientation) + startPadding;

		// Calculate the defined inter-element spacing size (if any, not taking care of SpaceBetween yet)
		var totalSpacingSize = spacing * (numberOfStackedChildren - 1);

		// Calculate the remaining size for each filled children (if any)
		var filledChildrenSize = atLeastOneFilledChild
			? (availableSizeForStackedChildren - (totalNonFilledStackedSize + totalSpacingSize)) / numberOfFilledChildren
			: 0;

		EnsureZeroFloor(ref filledChildrenSize);

		// Establish actual inter-element spacing.
		var spacingOffset = justify == AutoLayoutJustify.SpaceBetween && !atLeastOneFilledChild
			// When SpaceBetween is defined and there's no filled children
			? ComputeSpaceBetween(availableSizeForStackedChildren, totalNonFilledStackedSize, numberOfStackedChildren)
			// Fallback to the Spacing property
			: spacing;

		var primaryAxisAlignmentOffset = PrimaryAxisAlignmentOffsetSize(
			primaryAxisAlignment, availableSizeForStackedChildren, totalNonFilledStackedSize, atLeastOneFilledChild, spacing, numberOfStackedChildren);

		currentOffset += primaryAxisAlignmentOffset;

		// 3. Arrange children, one by one (in reverse order if IsReverseZIndex is true)
		var isReverse = IsReverseZIndex;
		var start = isReverse ? children.Count - 1 : 0;
		var stop = isReverse ? -1 : children.Count;
		var increment = isReverse ? -1 : 1;
		for (var i = start; i != stop; i += increment)
		{
			var child = _calculatedChildren[i];

			if (child.Role == AutoLayoutRole.Independent)
			{
				// Independent is given all the available space,
				// because it's not stacking with others.
				// No padding is applied to it either.
				child.Element.Arrange(new Rect(new Point(borderThickness.Left, borderThickness.Top), finalSizeMinusBorderThickness));

				continue; // next child, current offset remains unchanged
			}

			// Calculate the length of the child (size in the panel orientation)

			// Length depends on the role of the child
			var childLength = child.Role == AutoLayoutRole.Filled
				? filledChildrenSize
				: child.MeasuredLength;

			var offsetRelativeToPadding = currentOffset - startPadding;

			if (childLength > availableSizeForStackedChildren - offsetRelativeToPadding)
			{
				// Child is too big, truncate it to the remaining space
				childLength = availableSizeForStackedChildren - offsetRelativeToPadding;
			}

			EnsureZeroFloor(ref childLength);

			// Calculate the counter length of the child (size in the other dimension than
			var childCounterLength = GetCounterLength(child.Element);

			var childFinalCounterLength = isHorizontal
				? double.IsNaN(childCounterLength) ? child.Element.DesiredSize.Height : childCounterLength
				: double.IsNaN(childCounterLength) ? child.Element.DesiredSize.Width : childCounterLength;

			EnsureZeroFloor(ref childFinalCounterLength);

			// Calculate the position of the child by applying the alignment instructions
			var counterAlignment = GetCounterAlignment(child.Element);

			var isPrimaryAlignmentStretch = GetPrimaryAlignment(child.Element) is AutoLayoutPrimaryAlignment.Stretch;
			var isCounterAlignmentStretch = counterAlignment is AutoLayoutAlignment.Stretch;

			if (child.Element is FrameworkElement frameworkElement)
			{
				UpdateCounterAlignmentToStretch(ref frameworkElement, isHorizontal, isPrimaryAlignmentStretch, isCounterAlignmentStretch);
			}

			var haveCounterStartPadding = counterAlignment is AutoLayoutAlignment.Stretch or AutoLayoutAlignment.Start;
			var counterStartPadding = haveCounterStartPadding ? (isHorizontal ? padding.Top : padding.Left) : 0;

			var haveCounterEndPadding = counterAlignment is AutoLayoutAlignment.Stretch or AutoLayoutAlignment.End;
			var counterEndPadding = haveCounterEndPadding ? (isHorizontal ? padding.Bottom : padding.Right) : 0;

			var availableCounterLength = finalSize.GetCounterLength(orientation) - (counterStartPadding + counterEndPadding);

			EnsureZeroFloor(ref availableCounterLength);

			var childSize = isHorizontal
				? new Size(childLength, counterAlignment is AutoLayoutAlignment.Stretch ? availableCounterLength : childFinalCounterLength)
				: new Size(counterAlignment is AutoLayoutAlignment.Stretch ? availableCounterLength : childFinalCounterLength, childLength);

			ApplyMinMaxValues(child.Element, ref childSize);

			var counterAlignmentOffset = 
				ComputeCounterAlignmentOffset(counterAlignment, childFinalCounterLength, availableCounterLength, counterStartPadding, counterEndPadding);

			var childOffsetPosition = new Point(
				isHorizontal ? currentOffset : counterAlignmentOffset,
				isHorizontal ? counterAlignmentOffset : currentOffset);

			// 3c. Arrange the child to its final position, determined by the calculated offset and size
			child.Element.Arrange(new Rect(childOffsetPosition, childSize));

			// Increment the offset for the next child
			currentOffset += (childLength + spacingOffset);
		}

		return finalSize;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void UpdateCounterAlignmentToStretch(ref FrameworkElement frameworkElement, bool isHorizontal, bool isPrimaryAlignmentStretch, bool isCounterAlignmentStretch)
	{
		if (isHorizontal)
		{
			frameworkElement.HorizontalAlignment = isPrimaryAlignmentStretch ? HorizontalAlignment.Stretch : frameworkElement.HorizontalAlignment;
			frameworkElement.VerticalAlignment = isCounterAlignmentStretch ? VerticalAlignment.Stretch : frameworkElement.VerticalAlignment;
		}
		else
		{
			frameworkElement.VerticalAlignment = isPrimaryAlignmentStretch ? VerticalAlignment.Stretch : frameworkElement.VerticalAlignment;
			frameworkElement.HorizontalAlignment = isCounterAlignmentStretch ? HorizontalAlignment.Stretch : frameworkElement.HorizontalAlignment;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ApplyMinMaxValues(UIElement element, ref Size desiredSize)
	{
		if (element is not FrameworkElement frameworkElement)
		{
			return;
		}

		// Cache local values to avoid multiple calls to the DP system
		var width = frameworkElement.Width;
		var height = frameworkElement.Height;
		var maxWidth = frameworkElement.MaxWidth;
		var maxHeight = frameworkElement.MaxHeight;
		var minWidth = frameworkElement.MinWidth;
		var minHeight = frameworkElement.MinHeight;

		// Apply Width and Height constraints, if defined
		if (!double.IsNaN(width)) desiredSize.Width = width;
		if (!double.IsNaN(height)) desiredSize.Height = height;

		// Apply MaxWidth and MaxHeight constraints, if defined
		if (!double.IsNaN(maxWidth) && desiredSize.Width > maxWidth) desiredSize.Width = maxWidth;
		if (!double.IsNaN(maxHeight) && desiredSize.Height > maxHeight) desiredSize.Height = maxHeight;

		// Apply MinWidth and MinHeight constraints
		if (!double.IsNaN(minWidth) && desiredSize.Width < minWidth) desiredSize.Width = minWidth;
		if (!double.IsNaN(minHeight) && desiredSize.Height < minHeight) desiredSize.Height = minHeight;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static double ComputeCounterAlignmentOffset(
		AutoLayoutAlignment autoLayoutAlignment,
		double childCounterLength,
		double availableCounterLength,
		double counterStartPadding,
		double counterEndPadding
		)
	{
		var alignmentOffsetSize = availableCounterLength - childCounterLength;

		var calculatedOffset =  autoLayoutAlignment switch
		{
			AutoLayoutAlignment.End => alignmentOffsetSize,
			AutoLayoutAlignment.Center when alignmentOffsetSize > 0 => alignmentOffsetSize / 2,
			_ => 0
		};

		return calculatedOffset + counterStartPadding;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static double ComputeSpaceBetween(double availableSizeForStackedChildren, double totalNonFilledStackedSize, int numberOfStackedChildren)
	{
		var availableSizeForStackedSpaceBetween = availableSizeForStackedChildren - totalNonFilledStackedSize;

		EnsureZeroFloor(ref availableSizeForStackedSpaceBetween);

		return availableSizeForStackedSpaceBetween / (numberOfStackedChildren - 1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static double PrimaryAxisAlignmentOffsetSize(
		AutoLayoutAlignment autoLayoutAlignment,
		double availableSizeForStackedChildren,
		double totalNonFilledStackedSize,
		bool atLeastOneFilledChild,
		double spacing,
		int numberOfStackedChildren)
	{
		if (atLeastOneFilledChild || autoLayoutAlignment is AutoLayoutAlignment.Start)
		{
			return 0;
		}

		var totalSpacingSize = spacing * (numberOfStackedChildren - 1);

		var alignmentOffsetSize = availableSizeForStackedChildren -
			(totalNonFilledStackedSize + totalSpacingSize);

		return autoLayoutAlignment switch
		{
			AutoLayoutAlignment.End => alignmentOffsetSize,
			AutoLayoutAlignment.Center when alignmentOffsetSize > 0 => alignmentOffsetSize / 2,
			_ => 0
		};
	}
}
