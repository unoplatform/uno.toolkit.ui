using System;
using Uno.Disposables;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Represents a skeleton placeholder control that displays a shimmer animation while content is loading.
	/// Can be used standalone or as LoadingContent within a <see cref="LoadingView"/>.
	/// </summary>
	[TemplatePart(Name = TemplateParts.ShimmerTranslate, Type = typeof(TranslateTransform))]
	[TemplateVisualState(GroupName = VisualStateNames.ShimmerStates, Name = VisualStateNames.ShimmerActive)]
	[TemplateVisualState(GroupName = VisualStateNames.ShimmerStates, Name = VisualStateNames.ShimmerInactive)]
	public partial class SkeletonView : ContentControl
	{
		private static class TemplateParts
		{
			public const string ShimmerTranslate = "PART_ShimmerTranslate";
		}

		private static class VisualStateNames
		{
			public const string ShimmerStates = nameof(ShimmerStates);
			public const string ShimmerActive = nameof(ShimmerActive);
			public const string ShimmerInactive = nameof(ShimmerInactive);
		}

		private TranslateTransform? _shimmerTranslate;
		private Storyboard? _shimmerStoryboard;
		private bool _isReady;

		public SkeletonView()
		{
			DefaultStyleKey = typeof(SkeletonView);
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_shimmerTranslate = GetTemplateChild(TemplateParts.ShimmerTranslate) as TranslateTransform;
			_isReady = true;

			UpdateShimmerState();
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			UpdateShimmerState();
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			StopShimmer();
		}

		private void OnIsActiveChanged(DependencyPropertyChangedEventArgs e)
		{
			UpdateShimmerState();
		}

		private void OnEnableShimmerChanged(DependencyPropertyChangedEventArgs e)
		{
			UpdateShimmerState();
		}

		private void UpdateShimmerState()
		{
			if (!_isReady) return;

			var shouldShimmer = IsActive && EnableShimmer;
			
			if (shouldShimmer)
			{
				VisualStateManager.GoToState(this, VisualStateNames.ShimmerActive, true);
				StartShimmer();
			}
			else
			{
				VisualStateManager.GoToState(this, VisualStateNames.ShimmerInactive, true);
				StopShimmer();
			}
		}

		private void StartShimmer()
		{
			if (_shimmerTranslate == null || !IsLoaded) return;

			StopShimmer();

			var animation = new DoubleAnimation
			{
				From = -ActualWidth,
				To = ActualWidth * 2,
				Duration = ShimmerDuration,
				RepeatBehavior = RepeatBehavior.Forever,
			};

			Storyboard.SetTarget(animation, _shimmerTranslate);
			Storyboard.SetTargetProperty(animation, nameof(TranslateTransform.X));

			_shimmerStoryboard = new Storyboard();
			_shimmerStoryboard.Children.Add(animation);
			_shimmerStoryboard.Begin();
		}

		private void StopShimmer()
		{
			_shimmerStoryboard?.Stop();
			_shimmerStoryboard = null;
		}

		protected override void OnContentChanged(object oldContent, object newContent)
		{
			base.OnContentChanged(oldContent, newContent);

			// Restart shimmer when content changes to recalculate dimensions
			if (_isReady && IsActive && EnableShimmer)
			{
				StartShimmer();
			}
		}
	}
}
