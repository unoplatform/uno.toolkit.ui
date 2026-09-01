using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Disposables;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
#endif

namespace Uno.Toolkit.UI
{
	/// <summary>
	/// Wraps content and, while loading, replaces it visually with auto-generated skeleton placeholders.
	/// The placeholders are derived from the actual layout of the content, so no manual skeleton shapes are needed.
	/// </summary>
	[TemplatePart(Name = TemplateParts.ContentPresenter, Type = typeof(ContentPresenter))]
	[TemplatePart(Name = TemplateParts.SkeletonOverlay, Type = typeof(Canvas))]
	[TemplateVisualState(GroupName = VisualStateNames.SkeletonStates, Name = VisualStateNames.SkeletonVisible)]
	[TemplateVisualState(GroupName = VisualStateNames.SkeletonStates, Name = VisualStateNames.SkeletonHidden)]
	public partial class SkeletonView : ContentControl
	{
		private static class TemplateParts
		{
			public const string ContentPresenter = "PART_ContentPresenter";
			public const string SkeletonOverlay = "PART_SkeletonOverlay";
		}

		private static class VisualStateNames
		{
			public const string SkeletonStates = nameof(SkeletonStates);
			public const string SkeletonVisible = nameof(SkeletonVisible);
			public const string SkeletonHidden = nameof(SkeletonHidden);
		}

		// Dimensions below this are treated as "no value present yet" (empty bound TextBlock, source-less Image),
		// triggering the layout-slot fallback.
		private const double EmptySizeThreshold = 2;
		// Line width used when the layout grants an empty TextBlock no space at all (e.g. inside a horizontal StackPanel).
		private const double DefaultTextLineWidth = 100;
		// Approximates the height of a text line from FontSize for a TextBlock that measured empty.
		private const double TextLineHeightFactor = 1.4;
		// The sweeping highlight stays at least this wide so it remains visible on narrow layouts.
		private const double MinShimmerBandWidth = 100;

		private readonly record struct PlaceholderInfo(Rect Rect, bool IsCircle);

		private readonly SerialDisposable _sourceSubscription = new();
		private readonly List<(TranslateTransform Transform, double OffsetX, double BandWidth)> _shimmerBands = new();
		private List<PlaceholderInfo> _lastPlaceholders = new();
		private ContentPresenter? _contentPresenter;
		private Canvas? _overlay;
		private Storyboard? _shimmerStoryboard;
		private bool _isReady;
		private bool _isWaitingForContent;

		public SkeletonView()
		{
			DefaultStyleKey = typeof(SkeletonView);

			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}

		protected override void OnApplyTemplate()
		{
			if (_contentPresenter is { })
			{
				_contentPresenter.SizeChanged -= OnPartSizeChanged;
			}
			if (_overlay is { })
			{
				_overlay.SizeChanged -= OnPartSizeChanged;
			}

			base.OnApplyTemplate();

			_contentPresenter = GetTemplateChild(TemplateParts.ContentPresenter) as ContentPresenter;
			_overlay = GetTemplateChild(TemplateParts.SkeletonOverlay) as Canvas;

			if (_contentPresenter is { })
			{
				_contentPresenter.SizeChanged += OnPartSizeChanged;
			}
			if (_overlay is { })
			{
				// the overlay is collapsed while not loading, so it (re)gains a size only once
				// the SkeletonVisible state shows it — that is the moment generation can succeed
				_overlay.SizeChanged += OnPartSizeChanged;
			}

			_isReady = true;
			UpdateSkeletonState();
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			BindSource();
			UpdateSkeletonState();
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			_sourceSubscription.Disposable = null;
			StopShimmer();
			UnhookLayoutUpdated();
		}

		/// <summary>
		/// Called before placeholders are collected, allowing derived controls to prepare
		/// the (invisible) content tree — e.g. stamping placeholder rows into empty list controls.
		/// Must be idempotent: it runs on every generation pass.
		/// </summary>
		private protected virtual void PrepareSkeletonContent(DependencyObject contentRoot)
		{
		}

		private void HookLayoutUpdated()
		{
			if (_isWaitingForContent) return;

			_isWaitingForContent = true;
			LayoutUpdated += OnLayoutUpdated;
		}

		private void UnhookLayoutUpdated()
		{
			if (!_isWaitingForContent) return;

			_isWaitingForContent = false;
			LayoutUpdated -= OnLayoutUpdated;
		}

		private void OnLayoutUpdated(object? sender, object e)
		{
			if (IsLoading)
			{
				GenerateOverlay(); // unhooks itself once content materializes
			}
			else
			{
				UnhookLayoutUpdated();
			}
		}

		private void OnSourceChanged() => BindSource();

		private void BindSource() => _sourceSubscription.Disposable = Source?.BindIsExecuting(isExecuting => IsLoading = isExecuting);

		private void OnIsLoadingChanged() => UpdateSkeletonState();

		private void OnEnableShimmerChanged()
		{
			if (EnableShimmer)
			{
				StartShimmerIfNeeded();
			}
			else
			{
				StopShimmer();
			}
		}

		private void OnShimmerDurationChanged()
		{
			if (_shimmerStoryboard is { })
			{
				StopShimmer();
				StartShimmerIfNeeded();
			}
		}

		private void OnPlaceholderAppearanceChanged()
		{
			// invalidate the cache so the next generation rebuilds with the new appearance
			_lastPlaceholders.Clear();
			if (_isReady && IsLoading)
			{
				GenerateOverlay();
			}
		}

		private void OnPartSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (IsLoading)
			{
				GenerateOverlay();
			}
		}

		private void UpdateSkeletonState()
		{
			if (!_isReady) return;

			if (IsLoading)
			{
				VisualStateManager.GoToState(this, VisualStateNames.SkeletonVisible, useTransitions: IsLoaded);
				GenerateOverlay();
			}
			else
			{
				VisualStateManager.GoToState(this, VisualStateNames.SkeletonHidden, useTransitions: IsLoaded);
				StopShimmer();
				ClearOverlay();
				UnhookLayoutUpdated();
			}
		}

		private void GenerateOverlay()
		{
			if (_overlay is null || _contentPresenter is null) return;
			if (_overlay.ActualWidth < EmptySizeThreshold || _overlay.ActualHeight < EmptySizeThreshold) return; // not laid out yet

			PrepareSkeletonContent(_contentPresenter);

			var placeholders = new List<PlaceholderInfo>();
			CollectPlaceholders(_contentPresenter, placeholders);

			// Content that materializes asynchronously (e.g. list containers realized for injected placeholder
			// items) can appear without resizing the presenter or the overlay; while loading yields nothing,
			// retry on layout activity until something materializes.
			if (placeholders.Count == 0)
			{
				HookLayoutUpdated();
			}
			else
			{
				UnhookLayoutUpdated();
			}

			if (placeholders.SequenceEqual(_lastPlaceholders))
			{
				// same geometry; just make sure the shimmer is running (e.g. first Loaded after OnApplyTemplate)
				StartShimmerIfNeeded();
				return;
			}

			StopShimmer();
			ClearOverlay();

			var overlayWidth = _overlay.ActualWidth;
			foreach (var placeholder in placeholders)
			{
				AddPlaceholder(placeholder, overlayWidth);
			}

			_lastPlaceholders = placeholders;
			StartShimmerIfNeeded();
		}

		private void CollectPlaceholders(DependencyObject parent, List<PlaceholderInfo> results)
		{
			foreach (var child in parent.GetChildren())
			{
				if (child is not UIElement element || element.Visibility == Visibility.Collapsed)
				{
					continue;
				}

				var fe = element as FrameworkElement;
				if (fe is { } && Skeleton.GetIgnore(fe))
				{
					continue;
				}

				var shape = fe is { } ? Skeleton.GetShape(fe) : SkeletonShape.Auto;
				if (fe is { } && (shape != SkeletonShape.Auto || IsSkeletonLeaf(element)))
				{
					if (TryCreatePlaceholderInfo(fe, shape, out var info))
					{
						results.Add(info);
					}
					continue; // leaves are covered by a single placeholder; don't descend
				}

				CollectPlaceholders(child, results);
			}
		}

		private static bool IsSkeletonLeaf(UIElement element)
			=> element is TextBlock or Image or Shape or ButtonBase;

		private bool TryCreatePlaceholderInfo(FrameworkElement element, SkeletonShape shape, out PlaceholderInfo info)
		{
			info = default;
			if (_overlay is null) return false;

			var bounds = element.TransformToVisual(_overlay)
				.TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));

			if (bounds.Width < EmptySizeThreshold || bounds.Height < EmptySizeThreshold)
			{
				// The element has no value yet, so its own size is no use; fall back to the space the
				// layout granted it (its slot), synthesizing a text-line size where the slot gives no hint.
				if (VisualTreeHelper.GetParent(element) is not UIElement parent) return false;

				var slot = LayoutInformation.GetLayoutSlot(element);
				var margin = element.Margin;
				slot = new Rect(
					slot.X + margin.Left,
					slot.Y + margin.Top,
					Math.Max(0, slot.Width - margin.Left - margin.Right),
					Math.Max(0, slot.Height - margin.Top - margin.Bottom));
				var slotBounds = parent.TransformToVisual(_overlay).TransformBounds(slot);

				if (bounds.Width < EmptySizeThreshold)
				{
					bounds.X = slotBounds.X;
					bounds.Width = slotBounds.Width >= EmptySizeThreshold
						? slotBounds.Width
						: element is TextBlock ? DefaultTextLineWidth : 0;
				}
				if (bounds.Height < EmptySizeThreshold)
				{
					bounds.Y = slotBounds.Y;
					bounds.Height = element is TextBlock textBlock
						? textBlock.FontSize * TextLineHeightFactor
						: slotBounds.Height;
				}

				if (bounds.Width < EmptySizeThreshold || bounds.Height < EmptySizeThreshold) return false;
			}

			var isCircle = shape == SkeletonShape.Circle || (shape == SkeletonShape.Auto && element is Ellipse);
			if (isCircle)
			{
				var diameter = Math.Min(bounds.Width, bounds.Height);
				bounds = new Rect(
					bounds.X + ((bounds.Width - diameter) / 2),
					bounds.Y + ((bounds.Height - diameter) / 2),
					diameter,
					diameter);
			}

			info = new PlaceholderInfo(bounds, isCircle);
			return true;
		}

		private void AddPlaceholder(PlaceholderInfo info, double overlayWidth)
		{
			var rect = info.Rect;
			var placeholder = new Grid
			{
				Width = rect.Width,
				Height = rect.Height,
				Background = SkeletonBackground,
				CornerRadius = info.IsCircle ? new CornerRadius(rect.Width / 2) : PlaceholderCornerRadius,
				Clip = new RectangleGeometry { Rect = new Rect(0, 0, rect.Width, rect.Height) },
			};

			if (ShimmerBrush is { } shimmerBrush)
			{
				var bandWidth = Math.Max(MinShimmerBandWidth, overlayWidth / 3);
				// parked off-overlay to the left, so it is invisible until animated
				var transform = new TranslateTransform { X = -bandWidth - rect.X };
				placeholder.Children.Add(new Border
				{
					Width = bandWidth,
					HorizontalAlignment = HorizontalAlignment.Left,
					Background = shimmerBrush,
					RenderTransform = transform,
				});
				_shimmerBands.Add((transform, rect.X, bandWidth));
			}

			Canvas.SetLeft(placeholder, rect.X);
			Canvas.SetTop(placeholder, rect.Y);
			_overlay!.Children.Add(placeholder);
		}

		private void StartShimmerIfNeeded()
		{
			if (!EnableShimmer || !IsLoading || !IsLoaded) return;
			if (_shimmerStoryboard is { } || _shimmerBands.Count == 0 || _overlay is null) return;

			// Each band is animated in its host placeholder's coordinate space with offsets such that
			// all bands line up into a single sweep across the whole overlay.
			var overlayWidth = _overlay.ActualWidth;
			var storyboard = new Storyboard();
			foreach (var (transform, offsetX, bandWidth) in _shimmerBands)
			{
				var animation = new DoubleAnimation
				{
					From = -bandWidth - offsetX,
					To = overlayWidth - offsetX,
					Duration = ShimmerDuration,
					RepeatBehavior = RepeatBehavior.Forever,
				};
				Storyboard.SetTarget(animation, transform);
				Storyboard.SetTargetProperty(animation, nameof(TranslateTransform.X));
				storyboard.Children.Add(animation);
			}

			_shimmerStoryboard = storyboard;
			storyboard.Begin();
		}

		private void StopShimmer()
		{
			_shimmerStoryboard?.Stop();
			_shimmerStoryboard = null;
		}

		private void ClearOverlay()
		{
			_shimmerBands.Clear();
			_lastPlaceholders.Clear();
			_overlay?.Children.Clear();
		}
	}
}
