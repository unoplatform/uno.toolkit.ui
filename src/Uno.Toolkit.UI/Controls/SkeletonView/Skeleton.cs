using System;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Defines the shape type for a <see cref="Skeleton"/> element.
	/// </summary>
	public enum SkeletonShape
	{
		/// <summary>
		/// A rectangular shape with optional corner radius.
		/// </summary>
		Rectangle,

		/// <summary>
		/// A circular/elliptical shape.
		/// </summary>
		Circle,

		/// <summary>
		/// A text line placeholder.
		/// </summary>
		Text
	}

	/// <summary>
	/// Represents a single skeleton placeholder element that can be composed within a <see cref="SkeletonView"/>.
	/// </summary>
	public partial class Skeleton : Control
	{
		public Skeleton()
		{
			DefaultStyleKey = typeof(Skeleton);
		}

		#region DependencyProperty: Shape

		public static DependencyProperty ShapeProperty { get; } = DependencyProperty.Register(
			nameof(Shape),
			typeof(SkeletonShape),
			typeof(Skeleton),
			new PropertyMetadata(SkeletonShape.Rectangle));

		/// <summary>
		/// Gets or sets the shape of the skeleton element. Default is Rectangle.
		/// </summary>
		public SkeletonShape Shape
		{
			get => (SkeletonShape)GetValue(ShapeProperty);
			set => SetValue(ShapeProperty, value);
		}

		#endregion

		#region DependencyProperty: SkeletonBackground

		public static DependencyProperty SkeletonBackgroundProperty { get; } = DependencyProperty.Register(
			nameof(SkeletonBackground),
			typeof(Brush),
			typeof(Skeleton),
			new PropertyMetadata(default(Brush)));

		/// <summary>
		/// Gets or sets the background brush for this skeleton element.
		/// If not set, inherits from the parent SkeletonView or uses theme default.
		/// </summary>
		public Brush SkeletonBackground
		{
			get => (Brush)GetValue(SkeletonBackgroundProperty);
			set => SetValue(SkeletonBackgroundProperty, value);
		}

		#endregion
	}
}
