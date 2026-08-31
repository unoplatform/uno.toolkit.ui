using System;
using System.Diagnostics.CodeAnalysis;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Defines the placeholder shape that <see cref="SkeletonView"/> generates for an element.
	/// </summary>
	public enum SkeletonShape
	{
		/// <summary>The shape is inferred from the element (an <c>Ellipse</c> becomes a circle, everything else a rounded rectangle).</summary>
		Auto,

		/// <summary>A rounded rectangle.</summary>
		Rectangle,

		/// <summary>A circle, sized to the smaller dimension of the element and centered within its bounds.</summary>
		Circle,
	}

	public partial class SkeletonView
	{
		#region DependencyProperty: IsLoading

		public static DependencyProperty IsLoadingProperty { get; } = DependencyProperty.Register(
			nameof(IsLoading),
			typeof(bool),
			typeof(SkeletonView),
			new PropertyMetadata(true, (s, e) => ((SkeletonView)s).OnIsLoadingChanged()));

		/// <summary>
		/// Gets or sets whether the skeleton placeholders are shown instead of the content. Default is true.
		/// </summary>
		/// <remarks>Driven automatically when <see cref="Source"/> is set.</remarks>
		public bool IsLoading
		{
			get => (bool)GetValue(IsLoadingProperty);
			set => SetValue(IsLoadingProperty, value);
		}

		#endregion

		#region DependencyProperty: Source

		public static DependencyProperty SourceProperty { get; } = DependencyProperty.Register(
			nameof(Source),
			typeof(ILoadable),
			typeof(SkeletonView),
			new PropertyMetadata(default(ILoadable), (s, e) => ((SkeletonView)s).OnSourceChanged()));

		/// <summary>
		/// Gets or sets the <see cref="ILoadable"/> whose <see cref="ILoadable.IsExecuting"/> state drives <see cref="IsLoading"/>.
		/// </summary>
		public ILoadable Source
		{
			get => (ILoadable)GetValue(SourceProperty);
			set => SetValue(SourceProperty, value);
		}

		#endregion

		#region DependencyProperty: SkeletonBackground

		public static DependencyProperty SkeletonBackgroundProperty { get; } = DependencyProperty.Register(
			nameof(SkeletonBackground),
			typeof(Brush),
			typeof(SkeletonView),
			new PropertyMetadata(default(Brush), (s, e) => ((SkeletonView)s).OnPlaceholderAppearanceChanged()));

		/// <summary>
		/// Gets or sets the fill brush of the generated placeholder shapes.
		/// </summary>
		public Brush SkeletonBackground
		{
			get => (Brush)GetValue(SkeletonBackgroundProperty);
			set => SetValue(SkeletonBackgroundProperty, value);
		}

		#endregion

		#region DependencyProperty: ShimmerBrush

		public static DependencyProperty ShimmerBrushProperty { get; } = DependencyProperty.Register(
			nameof(ShimmerBrush),
			typeof(Brush),
			typeof(SkeletonView),
			new PropertyMetadata(default(Brush), (s, e) => ((SkeletonView)s).OnPlaceholderAppearanceChanged()));

		/// <summary>
		/// Gets or sets the brush of the band swept across the placeholders by the shimmer animation.
		/// Typically a horizontal <see cref="LinearGradientBrush"/> fading in and out of transparency.
		/// </summary>
		public Brush ShimmerBrush
		{
			get => (Brush)GetValue(ShimmerBrushProperty);
			set => SetValue(ShimmerBrushProperty, value);
		}

		#endregion

		#region DependencyProperty: ShimmerDuration

		public static DependencyProperty ShimmerDurationProperty { get; } = DependencyProperty.Register(
			nameof(ShimmerDuration),
			typeof(Duration),
			typeof(SkeletonView),
			new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(1500)), (s, e) => ((SkeletonView)s).OnShimmerDurationChanged()));

		/// <summary>
		/// Gets or sets the duration of one shimmer sweep. Default is 1.5 seconds.
		/// </summary>
		public Duration ShimmerDuration
		{
			get => (Duration)GetValue(ShimmerDurationProperty);
			set => SetValue(ShimmerDurationProperty, value);
		}

		#endregion

		#region DependencyProperty: EnableShimmer

		public static DependencyProperty EnableShimmerProperty { get; } = DependencyProperty.Register(
			nameof(EnableShimmer),
			typeof(bool),
			typeof(SkeletonView),
			new PropertyMetadata(true, (s, e) => ((SkeletonView)s).OnEnableShimmerChanged()));

		/// <summary>
		/// Gets or sets whether the shimmer animation plays while loading; when false, the placeholders are static. Default is true.
		/// </summary>
		public bool EnableShimmer
		{
			get => (bool)GetValue(EnableShimmerProperty);
			set => SetValue(EnableShimmerProperty, value);
		}

		#endregion

		#region DependencyProperty: PlaceholderCornerRadius

		public static DependencyProperty PlaceholderCornerRadiusProperty { get; } = DependencyProperty.Register(
			nameof(PlaceholderCornerRadius),
			typeof(CornerRadius),
			typeof(SkeletonView),
			new PropertyMetadata(default(CornerRadius), (s, e) => ((SkeletonView)s).OnPlaceholderAppearanceChanged()));

		/// <summary>
		/// Gets or sets the corner radius of the generated rectangular placeholders.
		/// </summary>
		public CornerRadius PlaceholderCornerRadius
		{
			get => (CornerRadius)GetValue(PlaceholderCornerRadiusProperty);
			set => SetValue(PlaceholderCornerRadiusProperty, value);
		}

		#endregion

		#region AttachedProperty: Ignore

		/// <summary>
		/// Backing property for whether an element and its subtree are excluded from skeleton generation.
		/// </summary>
		[DynamicDependency(nameof(GetIgnore))]
		[DynamicDependency(nameof(SetIgnore))]
		public static readonly DependencyProperty IgnoreProperty = DependencyProperty.RegisterAttached(
			"Ignore",
			typeof(bool),
			typeof(SkeletonView),
			new PropertyMetadata(false));

		/// <summary>Gets whether the element and its subtree are excluded from skeleton generation.</summary>
		public static bool GetIgnore(FrameworkElement element) => (bool)element.GetValue(IgnoreProperty);

		/// <summary>Sets whether the element and its subtree are excluded from skeleton generation.</summary>
		public static void SetIgnore(FrameworkElement element, bool value) => element.SetValue(IgnoreProperty, value);

		#endregion

		#region AttachedProperty: Shape

		/// <summary>
		/// Backing property for the placeholder shape forced onto an element.
		/// Setting a value other than <see cref="SkeletonShape.Auto"/> also makes the element a generation leaf:
		/// a single placeholder covers it and its subtree is not visited.
		/// </summary>
		[DynamicDependency(nameof(GetShape))]
		[DynamicDependency(nameof(SetShape))]
		public static readonly DependencyProperty ShapeProperty = DependencyProperty.RegisterAttached(
			"Shape",
			typeof(SkeletonShape),
			typeof(SkeletonView),
			new PropertyMetadata(SkeletonShape.Auto));

		/// <summary>Gets the placeholder shape forced onto the element.</summary>
		public static SkeletonShape GetShape(FrameworkElement element) => (SkeletonShape)element.GetValue(ShapeProperty);

		/// <summary>Sets the placeholder shape forced onto the element.</summary>
		public static void SetShape(FrameworkElement element, SkeletonShape value) => element.SetValue(ShapeProperty, value);

		#endregion
	}
}
