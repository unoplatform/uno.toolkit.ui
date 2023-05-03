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

namespace Uno.Toolkit.UI
{
	internal static class AutoLayoutExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double GetLength(this Size size, Orientation orientation)
		{
			return orientation switch
			{
				Orientation.Horizontal => size.Width,
				Orientation.Vertical => size.Height,
				_ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null),
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double GetLength(this Thickness thickness, Orientation orientation)
		{
			return orientation switch
			{
				Orientation.Horizontal => thickness.Left + thickness.Right,
				Orientation.Vertical => thickness.Top + thickness.Bottom,
				_ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null),
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double GetLength(this FrameworkElement frameworkElement, Orientation orientation)
		{
			return orientation switch
			{
				Orientation.Horizontal => frameworkElement.Width,
				Orientation.Vertical => frameworkElement.Height,
				_ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null),
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double GetStartLength(this Thickness thickness, Orientation orientation)
		{
			return orientation switch
			{
				Orientation.Horizontal => thickness.Left,
				Orientation.Vertical => thickness.Top,
				_ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null),
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double GetEndLength(this Thickness thickness, Orientation orientation)
		{
			return orientation switch
			{
				Orientation.Horizontal => thickness.Right,
				Orientation.Vertical => thickness.Bottom,
				_ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null),
			};
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double GetMinLength(this FrameworkElement frameworkElement, Orientation orientation)
		{
			return orientation switch
			{
				Orientation.Horizontal => frameworkElement.MinWidth,
				Orientation.Vertical => frameworkElement.MinHeight,
				_ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null),
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double GetCounterLength(this Size size, Orientation orientation)
		{
			return size.GetLength(orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double GetCounterLength(this Thickness thickness, Orientation orientation)
		{
			return thickness.GetLength(orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double GetCounterLength(this FrameworkElement frameworkElement, Orientation orientation)
		{
			return frameworkElement.GetLength(orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double GetMinCounterLength(this FrameworkElement frameworkElement, Orientation orientation)
		{
			return frameworkElement.GetMinLength(orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal);
		}

		internal static HorizontalAlignment ToHorizontalAlignment(this AutoLayoutAlignment layoutAlignment)
		{
			return layoutAlignment switch
			{
				AutoLayoutAlignment.Start => HorizontalAlignment.Left,
				AutoLayoutAlignment.Center => HorizontalAlignment.Center,
				AutoLayoutAlignment.End => HorizontalAlignment.Right,
				AutoLayoutAlignment.Stretch => HorizontalAlignment.Stretch,
				_ => throw new ArgumentException($"Unknown {nameof(AutoLayoutAlignment)}: {layoutAlignment}", nameof(layoutAlignment)),
			};
		}

		internal static VerticalAlignment ToVerticalAlignment(this AutoLayoutAlignment layoutAlignment)
		{
			return layoutAlignment switch
			{
				AutoLayoutAlignment.Start => VerticalAlignment.Top,
				AutoLayoutAlignment.Center => VerticalAlignment.Center,
				AutoLayoutAlignment.End => VerticalAlignment.Bottom,
				AutoLayoutAlignment.Stretch => VerticalAlignment.Stretch,
				_ => throw new ArgumentException($"Unknown {nameof(AutoLayoutAlignment)}: {layoutAlignment}", nameof(layoutAlignment)),
			};
		}
	}
}
