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
	public partial class SkeletonView
	{
		#region DependencyProperty: IsLoading

		/// <summary>Identifies the <see cref="IsLoading"/> dependency property.</summary>
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

		/// <summary>Identifies the <see cref="Source"/> dependency property.</summary>
		public static DependencyProperty SourceProperty { get; } = DependencyProperty.Register(
			nameof(Source),
			typeof(ILoadable),
			typeof(SkeletonView),
			new PropertyMetadata(default(ILoadable), (s, e) => ((SkeletonView)s).OnSourceChanged(e)));

		/// <summary>
		/// Gets or sets the <see cref="ILoadable"/> whose <see cref="ILoadable.IsExecuting"/> state drives <see cref="IsLoading"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="ILoadable.IsExecutingChanged"/> must be raised on the UI thread; wrap a source that completes
		/// elsewhere in a <see cref="LoadableSource"/>, which also inherits this control's <c>DataContext</c> for its bindings.
		/// </remarks>
		public ILoadable? Source
		{
			get => (ILoadable?)GetValue(SourceProperty);
			set => SetValue(SourceProperty, value);
		}

		#endregion

		#region DependencyProperty: SkeletonBackground

		/// <summary>Identifies the <see cref="SkeletonBackground"/> dependency property.</summary>
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

		/// <summary>Identifies the <see cref="ShimmerBrush"/> dependency property.</summary>
		public static DependencyProperty ShimmerBrushProperty { get; } = DependencyProperty.Register(
			nameof(ShimmerBrush),
			typeof(Brush),
			typeof(SkeletonView),
			new PropertyMetadata(default(Brush), (s, e) => ((SkeletonView)s).OnPlaceholderAppearanceChanged()));

		/// <summary>
		/// Gets or sets the brush of the band swept across the placeholders by the shimmer animation.
		/// Typically a horizontal <see cref="LinearGradientBrush"/> fading in and out of transparency; null removes the band.
		/// </summary>
		public Brush? ShimmerBrush
		{
			get => (Brush?)GetValue(ShimmerBrushProperty);
			set => SetValue(ShimmerBrushProperty, value);
		}

		#endregion

		#region DependencyProperty: ShimmerDuration

		/// <summary>Identifies the <see cref="ShimmerDuration"/> dependency property.</summary>
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

		/// <summary>Identifies the <see cref="EnableShimmer"/> dependency property.</summary>
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

		/// <summary>Identifies the <see cref="PlaceholderCornerRadius"/> dependency property.</summary>
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
	}
}
