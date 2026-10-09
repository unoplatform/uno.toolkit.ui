using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Disposables;
using Windows.Foundation;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
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

		private readonly record struct PlaceholderInfo(Rect Rect, bool IsCircle, FrameworkElement Element);

		// Original value of a property the skeleton overrides on content elements while loading
		// (e.g. Opacity of an element hidden behind its placeholder), restored once loaded.
		private readonly record struct PropertyValue(BindingBase? Binding, object LocalValue)
		{
			public static PropertyValue Capture(FrameworkElement element, DependencyProperty property) => new(
				element.GetBindingExpression(property)?.ParentBinding,
				element.ReadLocalValue(property));

			public void Restore(FrameworkElement element, DependencyProperty property)
			{
				if (Binding is { } binding)
				{
					element.SetBinding(property, binding);
				}
				else if (LocalValue == DependencyProperty.UnsetValue)
				{
					element.ClearValue(property);
				}
				else
				{
					element.SetValue(property, LocalValue);
				}
			}
		}

		private readonly SerialDisposable _sourceSubscription = new();
		private readonly List<(TranslateTransform Transform, double OffsetX, double BandWidth)> _shimmerBands = new();
		private List<PlaceholderInfo> _lastPlaceholders = new();
		private readonly Dictionary<FrameworkElement, PropertyValue> _hiddenElements = new();
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

			RemoveListStandIns(); // hosted in the previous template's overlay

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
				RestoreHiddenElements();
				RemoveListStandIns();
				UnhookLayoutUpdated();
			}
		}

		private void GenerateOverlay()
		{
			if (_overlay is null || _contentPresenter is null) return;

			// Before the size check: prepared content (e.g. placeholder rows in an empty list) is often
			// what gives an unconstrained control its size in the first place.
			PrepareSkeletonContent(_contentPresenter);
			// Also before the size check: an empty list reserving the space of its placeholder rows can likewise be
			// what gives the content its size.
			UpdateListStandIns(_contentPresenter);

			if (_overlay.ActualWidth < EmptySizeThreshold || _overlay.ActualHeight < EmptySizeThreshold)
			{
				// not laid out yet
				if (_listStandIns.Count > 0)
				{
					HookLayoutUpdated();
				}
				return;
			}

			var placeholders = new List<PlaceholderInfo>();
			CollectPlaceholders(_contentPresenter, placeholders);

			// Only the elements covered by a placeholder are hidden: ignored elements and container
			// backgrounds (e.g. a card's border) stay visible while loading.
			HideCoveredElements(placeholders);

			// Content that materializes asynchronously (e.g. list containers realized for injected placeholder
			// items) can appear without resizing the presenter or the overlay; while loading yields nothing,
			// retry on layout activity until something materializes. Likewise while lists are stood in for: their
			// placeholder rows, or the real items replacing them, realize without necessarily resizing anything.
			if (placeholders.Count == 0 || _listStandIns.Count > 0)
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
				CollectPlaceholder(child, results);
			}
		}

		private void CollectPlaceholder(DependencyObject child, List<PlaceholderInfo> results)
		{
			if (child is not UIElement element || element.Visibility == Visibility.Collapsed)
			{
				return;
			}

			var fe = element as FrameworkElement;
			if (fe is { } && Skeleton.GetIgnore(fe))
			{
				return;
			}

			var shape = fe is { } ? Skeleton.GetShape(fe) : SkeletonShape.Auto;
			if (fe is { } && (shape != SkeletonShape.Auto || IsSkeletonLeaf(element)))
			{
				if (TryCreatePlaceholderInfo(fe, shape, out var info))
				{
					results.Add(info);
				}
				return; // leaves are covered by a single placeholder; don't descend
			}

			// Only a container's content is mirrored, not its template chrome: e.g. Material's ListViewItem
			// has a full-size background Rectangle whose placeholder would cover the content's placeholders.
			if (element is ContentControl container && FindContentRoot(container) is { } contentRoot)
			{
				CollectPlaceholder(contentRoot, results);
				return;
			}

			if (fe is { } && _listStandIns.TryGetValue(fe, out var standIn))
			{
				CollectStandInPlaceholders(fe, standIn, results);
				return;
			}

			CollectPlaceholders(child, results);
		}

		private static UIElement? FindContentRoot(ContentControl container)
		{
			// ContentTemplateRoot isn't reliably available (e.g. list item containers on their first pass), and
			// direct UIElement content is its own root; otherwise fall back to the template's ContentPresenter.
			if ((container.ContentTemplateRoot ?? container.Content as UIElement) is { } root)
			{
				return root;
			}

			var pending = new Queue<DependencyObject>(container.GetChildren());
			while (pending.Count > 0)
			{
				var current = pending.Dequeue();
				if (current is ContentPresenter presenter)
				{
					return presenter;
				}
				if (current is Control)
				{
					continue; // a nested control's presenter belongs to that control
				}
				foreach (var child in current.GetChildren())
				{
					pending.Enqueue(child);
				}
			}

			return null;
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
				if (!TryGetLayoutSlotBounds(element, out var slotBounds)) return false;

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

			info = new PlaceholderInfo(bounds, isCircle, element);
			return true;
		}

		// The space the layout granted the element (its margin excluded), in overlay coordinates.
		private bool TryGetLayoutSlotBounds(FrameworkElement element, out Rect bounds)
		{
			bounds = default;
			if (_overlay is null || VisualTreeHelper.GetParent(element) is not UIElement parent) return false;

			var slot = LayoutInformation.GetLayoutSlot(element);
			var margin = element.Margin;
			slot = new Rect(
				slot.X + margin.Left,
				slot.Y + margin.Top,
				Math.Max(0, slot.Width - margin.Left - margin.Right),
				Math.Max(0, slot.Height - margin.Top - margin.Bottom));
			bounds = parent.TransformToVisual(_overlay).TransformBounds(slot);
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

		private void HideCoveredElements(List<PlaceholderInfo> placeholders)
		{
			var covered = new HashSet<FrameworkElement>();
			foreach (var placeholder in placeholders)
			{
				covered.Add(placeholder.Element);
			}

			// restore elements no longer covered (e.g. content replaced or re-laid out while loading)
			if (_hiddenElements.Count > 0)
			{
				var uncovered = new List<FrameworkElement>();
				foreach (var element in _hiddenElements.Keys)
				{
					if (!covered.Contains(element))
					{
						uncovered.Add(element);
					}
				}
				foreach (var element in uncovered)
				{
					RestoreElement(element);
				}
			}

			foreach (var element in covered)
			{
				if (_hiddenElements.ContainsKey(element)) continue;

				_hiddenElements[element] = PropertyValue.Capture(element, UIElement.OpacityProperty);
				element.Opacity = 0;
			}
		}

		private void RestoreHiddenElements()
		{
			foreach (var (element, original) in _hiddenElements)
			{
				original.Restore(element, UIElement.OpacityProperty);
			}
			_hiddenElements.Clear();
		}

		private void RestoreElement(FrameworkElement element)
		{
			if (_hiddenElements.Remove(element, out var original))
			{
				original.Restore(element, UIElement.OpacityProperty);
			}
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
			if (_overlay is { } overlay)
			{
				// the stand-in host stays: recreating it would re-realize its placeholder rows on every pass
				for (var i = overlay.Children.Count - 1; i >= 0; i--)
				{
					if (!ReferenceEquals(overlay.Children[i], _standInHost))
					{
						overlay.Children.RemoveAt(i);
					}
				}
			}
		}
	}
}
