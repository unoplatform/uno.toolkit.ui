using System;
using System.Collections.Generic;
using System.Linq;
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
	protected override Size MeasureOverride(Size availableSize)
	{
		// - Each children can be one of the following role
		//   1. Hug: The child is measured according to its own constraints, using its desired size.
		//   2. Fixed: The child is measured according to its own constraints, using its fixed size.
		//   3. Filled: The child will be given the remaining space, evenly distributed through all
		//      other "filled" children.
		//   4. Independent: The child will follow the z-axis of its position in the children list,
		//      but won't be affected by the other children.

		// -1. Local cache of DependencyProperties
		var orientation = Orientation;
		var children = Children;
		var justify = Justify;
		var spacing = Spacing.FiniteOrDefault(0d);
		var borderThickness = BorderBrush is null ? default : BorderThickness;

		// 0. Establish start remaining sizes to dispatch, according to the orientation
		var remainingSize = availableSize.GetLength(orientation) - borderThickness.GetLength(orientation);
		var availableCounterSize = availableSize.GetCounterLength(orientation) - borderThickness.GetCounterLength(orientation);
		var desiredCounterSize = 0d;

		// 1. Establish the list of children roles
		var filledAsHug = double.IsPositiveInfinity(remainingSize);
		var (numberOfStackedChildren, atLeastOneFilledChild) = EstablishChildrenRoles(children, filledAsHug, orientation);

		// 2. Remove applicable padding and spacing from the remaining size
		var paddingSize = Padding.GetLength(orientation);
		var counterPaddingSize = Padding.GetCounterLength(orientation);
		Decrement(ref remainingSize, paddingSize);

		// 3. Measure fixed children
		MeasureFixedChildren(orientation, availableCounterSize, ref remainingSize, ref desiredCounterSize, counterPaddingSize);

		// 4. Establish the total spacing size, if applicable
		var totalSpacingSize = 0d;
		if (numberOfStackedChildren > 0
			&& justify != AutoLayoutJustify.SpaceBetween
			&& spacing != 0
			&& spacing < double.PositiveInfinity)
		{
			totalSpacingSize = (numberOfStackedChildren - 1) * spacing;
			Decrement(ref remainingSize, totalSpacingSize);
		}

		// 5. Calculate the size of Hug children
		MeasureHugChildren(orientation, availableCounterSize, ref remainingSize, ref desiredCounterSize, counterPaddingSize);

		// 6. Calculate the size of Filled children
		MeasureFilledChildren(orientation, availableCounterSize, ref remainingSize, ref desiredCounterSize, counterPaddingSize);

		// 7. Measure independent children, independently of the remaining size
		var independentDesiredSize = MeasureIndependentChildren(availableSize, borderThickness, orientation, ref desiredCounterSize);

		EnsureZeroFloor(ref desiredCounterSize);

		// 8. Calculate the desired size of the panel
		Size desiredSize;

		if ((atLeastOneFilledChild
			|| justify == AutoLayoutJustify.SpaceBetween)
			&& remainingSize is > 0 and < double.PositiveInfinity)
		{
			// 8a. Calculated the spacing size, when justify is SpaceBetween or there's at least one filled child

			// We don't need to calculate the spacing since it's the remaining size
			// and the final spacing will be calculated in the arrange pass.
			desiredSize = orientation switch
			{
				Orientation.Horizontal => new Size(availableSize.Width, desiredCounterSize + counterPaddingSize),
				Orientation.Vertical => new Size(desiredCounterSize + counterPaddingSize, availableSize.Height),
				_ => throw new ArgumentOutOfRangeException(),
			};
		}
		else
		{
			// 8b. Calculate the desired size of the panel, when there's at least one filled child
			var stackedChildrenDesiredSize =
				_calculatedChildren!
					.Where(child => child.IsVisible)
					.Select(c => c.MeasuredLength)
					.Sum()
				+ totalSpacingSize;

			var desiredSizeInPrimaryOrientation = Math.Max(independentDesiredSize, stackedChildrenDesiredSize);

			EnsureZeroFloor(ref desiredSizeInPrimaryOrientation);

			desiredSize = orientation switch
			{
				Orientation.Horizontal => new Size(
					width: desiredSizeInPrimaryOrientation + paddingSize,
					height: desiredCounterSize + counterPaddingSize),
				Orientation.Vertical => new Size(
					width: desiredCounterSize + counterPaddingSize,
					height: desiredSizeInPrimaryOrientation + paddingSize),
				_ => throw new ArgumentOutOfRangeException(),
			};
		}

		// 9. Apply Width and Height constraints, if any
		ApplyMinMaxValues(ref desiredSize);

		return desiredSize;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ApplyMinMaxValues(ref Size desiredSize)
	{
		// Cache local values to avoid multiple calls to the DP system
		var width = Width;
		var height = Height;
		var maxWidth = MaxWidth;
		var maxHeight = MaxHeight;
		var minWidth = MinWidth;
		var minHeight = MinHeight;

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
	private (int, bool) EstablishChildrenRoles(IList<UIElement> children, bool filledAsHug, Orientation orientation)
	{
		if(_calculatedChildren == null || _calculatedChildren.Length != children.Count)
		{
			_calculatedChildren = new CalculatedChildren[children.Count];
		}

		var numberOfStackedChildren = children.Count;
		var atLeastOneFilledChild = false;

		for (var i = 0; i < children.Count; i++)
		{
			var child = children[i];

			AutoLayoutRole role;
			var length = 0d;

			if (GetIsIndependentLayout(child))
			{
				role = AutoLayoutRole.Independent;
				numberOfStackedChildren--;
			}
			else if (GetPrimaryAlignment(child) == AutoLayoutPrimaryAlignment.Stretch && !filledAsHug)
			{
				atLeastOneFilledChild = true;
				role = AutoLayoutRole.Filled;
			}
			else if(GetPrimaryLength(child) is var l and >= 0 and < double.PositiveInfinity)
			{
				role = AutoLayoutRole.Fixed;
				length = l;
			}
			else
			{
				var size = child is FrameworkElement frameworkElement ?
					Math.Max(frameworkElement.GetLength(orientation), frameworkElement.GetMinLength(orientation)) :
					-1;

				if (size >= 0)
				{
					role = AutoLayoutRole.Fixed;
					length = size;
				}
				else
				{
					role = AutoLayoutRole.Hug;
				}
			}

			_calculatedChildren[i] = new CalculatedChildren(child, role, length);
		}

		return (numberOfStackedChildren, atLeastOneFilledChild);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void MeasureFixedChildren(
		Orientation orientation,
		double availableCounterSize,
		ref double remainingSize,
		ref double desiredCounterSize,
		double counterPaddingSize)
	{
		for (var i = 0; i < _calculatedChildren!.Length; i++)
		{
			var calculatedChild = _calculatedChildren[i];

			if (calculatedChild.Role != AutoLayoutRole.Fixed || !calculatedChild.IsVisible)
			{
				continue;
			}

			var fixedSize = calculatedChild.MeasuredLength;

			MeasureChild(
				calculatedChild.Element,
				orientation,
				fixedSize, // The available size for the child is its defined fixed size
				availableCounterSize,
				ref desiredCounterSize,
				counterPaddingSize);

			Decrement(ref remainingSize, fixedSize);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void MeasureHugChildren(
		Orientation orientation,
		double availableCounterSize,
		ref double remainingSize,
		ref double desiredCounterSize,
		double counterPaddingSize)
	{
		for (var i = 0; i < _calculatedChildren!.Length; i++)
		{
			var calculatedChild = _calculatedChildren![i];

			if (calculatedChild.Role != AutoLayoutRole.Hug || !calculatedChild.IsVisible)
			{
				continue;
			}

			var desiredSize = MeasureChild(
				calculatedChild.Element,
				orientation,
				double.PositiveInfinity, // We don't want the child to limit its own desired size to available one
				availableCounterSize,
				ref desiredCounterSize,
				counterPaddingSize);

			calculatedChild.MeasuredLength = desiredSize;

			// We don't care about its desired size, because it's fixed, so we're using fixed size
			Decrement(ref remainingSize, desiredSize);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool MeasureFilledChildren(
		Orientation orientation,
		double availableCounterSize,
		ref double remainingSize,
		ref double desiredCounterSize,
		double counterPaddingSize)
	{
		if (double.IsInfinity(remainingSize))
		{
			return false; // Filled children act as Hug children when there's infinite available size
		}

		// Calculated the number of filled children using a for loop, to avoid allocating an array
		var filledChildrenCount = 0;
		for (var i = 0; i < _calculatedChildren!.Length; i++)
		{
			var child = _calculatedChildren![i];

			if (child.Role == AutoLayoutRole.Filled || child.IsVisible)
			{
				filledChildrenCount++;
			}
		}

		if (filledChildrenCount <= 0)
		{
			return false; // no filled children
		}

		var filledSize = remainingSize / filledChildrenCount;
		EnsureZeroFloor(ref filledSize);

		for (var i = 0; i < _calculatedChildren!.Length; i++)
		{
			var child = _calculatedChildren![i];
			if (child.Role != AutoLayoutRole.Filled || !child.IsVisible)
			{
				continue;
			}

			MeasureChild(child.Element, orientation, filledSize, availableCounterSize, ref desiredCounterSize, counterPaddingSize);

			child.MeasuredLength = filledSize;
		}  

		return true; // at least one filled child
	}

	private static double MeasureChild(
		UIElement child,
		Orientation orientation,
		double availableSize,
		double availableCounterSize,
		ref double desiredCounterSize,
		double counterPaddingSize)
	{
		var isOrientationHorizontal = orientation is Orientation.Horizontal;
		var isPrimaryAlignmentStretch = GetPrimaryAlignment(child) is AutoLayoutPrimaryAlignment.Stretch;
		var isCounterAlignmentStretch = GetCounterAlignment(child) is AutoLayoutAlignment.Stretch;

		if (child as FrameworkElement is { } frameworkElement)
		{
			UpdateCounterAlignmentToStretch(ref frameworkElement, isOrientationHorizontal, isPrimaryAlignmentStretch, isCounterAlignmentStretch);

			var isStretch = isOrientationHorizontal ?
				frameworkElement.VerticalAlignment is VerticalAlignment.Stretch :
				frameworkElement.HorizontalAlignment is HorizontalAlignment.Stretch;

			isCounterAlignmentStretch = isStretch && (
				isOrientationHorizontal ?
				double.IsNaN(frameworkElement.Height) && double.IsNaN(GetCounterLength(child))
				: double.IsNaN(frameworkElement.Width) && double.IsNaN(GetCounterLength(child))
				);
		}

		var availableSizeForChild = isOrientationHorizontal
			? new Size(availableSize, availableCounterSize - (isCounterAlignmentStretch ? counterPaddingSize : 0))
			: new Size(availableCounterSize - (isCounterAlignmentStretch ? counterPaddingSize : 0), availableSize);

		child.Measure(availableSizeForChild);

		double desiredSize;
		if (isOrientationHorizontal)
		{
			desiredSize = child.DesiredSize.Width;
			desiredCounterSize = Math.Max(desiredCounterSize, child.DesiredSize.Height);
		}
		else
		{
			desiredSize = child.DesiredSize.Height;
			desiredCounterSize = Math.Max(desiredCounterSize, child.DesiredSize.Width);
		}

		EnsureZeroFloor(ref desiredSize);
		return desiredSize;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Decrement(ref double value, double decrement)
	{
		if (double.IsInfinity(value))
		{
			return;
		}

		value -= decrement;
		EnsureZeroFloor(ref value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void EnsureZeroFloor(ref double sizeValue)
	{
		if (sizeValue < 0)
		{
			sizeValue = 0;
		}
	}

	private double MeasureIndependentChildren(
		Size availableSize,
		Thickness borderThickness,
		Orientation orientation,
		ref double desiredCounterSize)
	{
		var resultSize = new Size();
		var anyIndependent = false;

		// Actual available size for independent children is the available size minus the border thickness
		availableSize = new Size(
			availableSize.Width - (borderThickness.Left + borderThickness.Right),
			availableSize.Height - (borderThickness.Top + borderThickness.Bottom));

		for (var i = 0; i < _calculatedChildren!.Length; i++)
		{
			var child = _calculatedChildren[i];

			if (child.Role != AutoLayoutRole.Independent)
			{
				continue;
			}

			anyIndependent = true;

			child.Element.Measure(availableSize);
			var desiredSize = child.Element.DesiredSize;

			// Adjust resulting desired size to the largest of the children
			resultSize.Width = Math.Max(resultSize.Width, desiredSize.Width);
			resultSize.Height = Math.Max(resultSize.Height, desiredSize.Height);
		}

		if (!anyIndependent)
		{
			return -1;
		}

		desiredCounterSize = Math.Max(
			desiredCounterSize,
			orientation switch
			{
				Orientation.Horizontal => resultSize.Height,
				Orientation.Vertical => resultSize.Width,
				_ => throw new NotSupportedException()
			});

		return orientation switch
		{
			Orientation.Horizontal => resultSize.Width,
			Orientation.Vertical => resultSize.Height,
			_ => throw new NotSupportedException()
		};
	}

	private CalculatedChildren[]? _calculatedChildren;

	private class CalculatedChildren
	{
		public CalculatedChildren(UIElement element, AutoLayoutRole role, double measuredLength = 0d)
		{
			Element = element;
			Role = role;
			MeasuredLength = measuredLength;
			IsVisible = element.Visibility == Visibility.Visible;
		}

		internal bool IsVisible { get; }

		internal UIElement Element { get; }

		/// <summary>
		/// How the element is layouted in the AutoLayout.
		/// </summary>
		/// <remarks>
		/// When the element is absolutely positioned (Independent), it is not stacked with others.
		/// </remarks>
		internal AutoLayoutRole Role { get; }

		/// <summary>
		/// Measured length of the element, when it is stacked.
		/// </summary>
		/// <remarks>
		/// Will be zero when the element is absolutely positioned, because it is not stacked
		/// with others.
		/// </remarks>
		internal double MeasuredLength { get; set; }
	}

	private enum AutoLayoutRole : byte
	{
		Hug,
		Fixed,
		Filled,
		Independent
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
}
