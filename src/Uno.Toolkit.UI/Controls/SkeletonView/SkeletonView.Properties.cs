using System;

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
		#region DependencyProperty: IsActive

		public static DependencyProperty IsActiveProperty { get; } = DependencyProperty.Register(
			nameof(IsActive),
			typeof(bool),
			typeof(SkeletonView),
			new PropertyMetadata(true, (s, e) => ((SkeletonView)s).OnIsActiveChanged(e)));

		/// <summary>
		/// Gets or sets whether the skeleton view is active (showing placeholders).
		/// When false, the skeleton content is hidden. Default is true.
		/// </summary>
		public bool IsActive
		{
			get => (bool)GetValue(IsActiveProperty);
			set => SetValue(IsActiveProperty, value);
		}

		#endregion

		#region DependencyProperty: EnableShimmer

		public static DependencyProperty EnableShimmerProperty { get; } = DependencyProperty.Register(
			nameof(EnableShimmer),
			typeof(bool),
			typeof(SkeletonView),
			new PropertyMetadata(true, (s, e) => ((SkeletonView)s).OnEnableShimmerChanged(e)));

		/// <summary>
		/// Gets or sets whether the shimmer animation is enabled. Default is true.
		/// </summary>
		public bool EnableShimmer
		{
			get => (bool)GetValue(EnableShimmerProperty);
			set => SetValue(EnableShimmerProperty, value);
		}

		#endregion

		#region DependencyProperty: ShimmerDuration

		public static DependencyProperty ShimmerDurationProperty { get; } = DependencyProperty.Register(
			nameof(ShimmerDuration),
			typeof(Duration),
			typeof(SkeletonView),
			new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(1500))));

		/// <summary>
		/// Gets or sets the duration of one shimmer animation cycle. Default is 1.5 seconds.
		/// </summary>
		public Duration ShimmerDuration
		{
			get => (Duration)GetValue(ShimmerDurationProperty);
			set => SetValue(ShimmerDurationProperty, value);
		}

		#endregion

		#region DependencyProperty: SkeletonBackground

		public static DependencyProperty SkeletonBackgroundProperty { get; } = DependencyProperty.Register(
			nameof(SkeletonBackground),
			typeof(Brush),
			typeof(SkeletonView),
			new PropertyMetadata(default(Brush)));

		/// <summary>
		/// Gets or sets the background brush for skeleton placeholder shapes.
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
			new PropertyMetadata(default(Brush)));

		/// <summary>
		/// Gets or sets the brush used for the shimmer highlight effect.
		/// </summary>
		public Brush ShimmerBrush
		{
			get => (Brush)GetValue(ShimmerBrushProperty);
			set => SetValue(ShimmerBrushProperty, value);
		}

		#endregion
	}
}
