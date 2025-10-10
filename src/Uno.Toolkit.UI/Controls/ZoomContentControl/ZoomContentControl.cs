using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Uno.Disposables;
using Uno.UI.Extensions;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Input;
using Windows.Devices.Input;
#endif

namespace Uno.Toolkit.UI;

public partial class ZoomContentControl : ContentControl;

/*
	visual tree: // the parts that matters
		ZoomContentControl#PART_ZoomContainer
			Grid#PART_RootGrid
				Grid#PART_Viewport
					OverflowContainer#PART_WrapperContainer
						ContentControl#PART_InnerContentControl: ScaleTransform & PART_TranslateTransform
							ContentPresenter#PART_InnerContentPresenter: PART_LocalFocusTranslation

	PART_InnerContentPresenter is the _localFocusPresenter

	all transformations in order:
	1. ScaleTransform: ZoomLevel
	2. PART_TranslateTransform: Translation + AdditionalMargin.TopLeft
	3. PART_LocalFocusTranslation: LocalFocusTarget.TransformToVisual(_localFocusPresenter) * -1
*/

#if DEBUG
[DebuggerTypeProxy(typeof(DebugProxy))]
partial class ZoomContentControl
{
	internal class DebugProxy(ZoomContentControl instance)
	{
		public double ZoomLevel => instance.ZoomLevel;
		public Size? ViewportSize => instance.ViewportSize;
		public Size ContentSize => instance.ContentSize;
		public Size ScaledContentSize => instance.ScaledContentSize;
		public Size PaddedScaledContentSize => instance.PaddedScaledContentSize;
		public Point K => instance.K;
		public Point ScrollValue => instance.ScrollValue;
		public Point VectoredScrollValue => instance.VectoredScrollValue;
		public TranslateTransform? Translation => instance._translation;
		public TranslateTransform? LocalTranslation => instance._localFocusTranslation;

		//public string ZoomInfo => $"{instance.ZoomLevel:0.#}, Min={instance.MinZoomLevel:0.#}, Max={instance.MaxZoomLevel:0.#}";
		//public string ScrollHInfo => $"{instance.HorizontalScrollValue:0.#}, Min={instance.HorizontalMinScroll:0.#}, Max={instance.HorizontalMaxScroll:0.#}";
		//public string ScrollVInfo => $"{instance.VerticalScrollValue:0.#}, Min={instance.VerticalMinScroll:0.#}, Max={instance.VerticalMaxScroll:0.#}";
	}
}
#endif

[TemplatePart(Name = TemplateParts.RootGrid, Type = typeof(Grid))]
[TemplatePart(Name = TemplateParts.Viewport, Type = typeof(Grid))]
[TemplatePart(Name = TemplateParts.PART_WrapperContainer, Type = typeof(FrameworkElement))]
[TemplatePart(Name = TemplateParts.PART_InnerContentControl, Type = typeof(ContentControl))]
[TemplatePart(Name = TemplateParts.TranslateTransform, Type = typeof(TranslateTransform))]
[TemplatePart(Name = TemplateParts.VerticalScrollBar, Type = typeof(ScrollBar))]
[TemplatePart(Name = TemplateParts.HorizontalScrollBar, Type = typeof(ScrollBar))]
partial class ZoomContentControl
{
	private static class TemplateParts
	{
		public const string RootGrid = "PART_RootGrid";
		public const string Viewport = "PART_Viewport";
		public const string PART_WrapperContainer = "PART_WrapperContainer";
		public const string PART_InnerContentControl = "PART_InnerContentControl";
		public const string TranslateTransform = "PART_TranslateTransform";
		// public const string PART_LocalFocusTranslation = "PART_LocalFocusTranslation"; (nested in PART_InnerContentControl's template)
		public const string HorizontalScrollBar = "PART_ScrollH";
		public const string VerticalScrollBar = "PART_ScrollV";
	}

	#region public Size ContentSize { get; private set+raise; }
	private Size _contentSize;
	public Size ContentSize
	{
		get => _contentSize;
		private set
		{
			_contentSize = value;

			ContentSizeChanged?.Invoke(this, EventArgs.Empty);
			NotifyStateChanged();
		}
	}
	#endregion

	private Grid? _rootGrid;
	private Grid? _viewport;
	private ScrollBar? _scrollV;
	private ScrollBar? _scrollH;
	private TranslateTransform? _translation;

	private FrameworkElement? _localFocusTarget;
	private FrameworkElement? _localFocusWrapper;
	private ContentPresenter? _localFocusPresenter;
	private TranslateTransform? _localFocusTranslation;

	private (uint Id, Point Position, Point ScrollOffset)? _capturedPointerContext;
	private SerialDisposable _contentSubscriptions = new();

	private bool _isHandlingMouseWheelZooming;
	private bool _preventTranslationUpdate;
	private double? _previousZoomLevel;

	public ZoomContentControl()
	{
		DefaultStyleKey = typeof(ZoomContentControl);
		SizeChanged += OnSizeChanged;
	}

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		T FindTemplatePart<T>(string name) where T : class =>
			(GetTemplateChild(name) ?? throw new Exception($"Expected template part not found: {name}")) as T ??
			throw new Exception($"Expected template part '{name}' to be of type: {typeof(T)}");

		_rootGrid = FindTemplatePart<Grid>(TemplateParts.RootGrid);
		_viewport = FindTemplatePart<Grid>(TemplateParts.Viewport);
		_localFocusWrapper = FindTemplatePart<FrameworkElement>(TemplateParts.PART_WrapperContainer);
		if (FindTemplatePart<ContentControl>(TemplateParts.PART_InnerContentControl) is { } inner)
		{
			// ensure the _localFocusTranslation is materialized, since that located in PART_InnerContentPresenter's control-template
			inner.ApplyTemplate();

			if (VisualTreeHelper.GetChildrenCount(inner) != 0 &&
				VisualTreeHelper.GetChild(inner, 0) is ContentPresenter innerCp)
			{
				_localFocusPresenter = innerCp;
				_localFocusTranslation = innerCp.RenderTransform as TranslateTransform;
			}
		}
		_translation = FindTemplatePart<TranslateTransform>(TemplateParts.TranslateTransform);

		_scrollV = FindTemplatePart<ScrollBar>(TemplateParts.VerticalScrollBar);
		_scrollH = FindTemplatePart<ScrollBar>(TemplateParts.HorizontalScrollBar);

		_viewport.SizeChanged += OnViewportSizeChanged;
		_scrollV.SizeChanged += OnScrollBarSizeChanged;
		_scrollH.SizeChanged += OnScrollBarSizeChanged;

		_previousZoomLevel = ZoomLevel;

		UpdateVisualStates();
		ResetViewport();
		UpdateScrollDetails();
		UpdateScrollBarMirrorSpacing();
	}

	// event handlers
	protected override void OnContentChanged(object oldContent, object newContent)
	{
		_contentSubscriptions.Disposable = null;
		if (newContent is FrameworkElement { } fe)
		{
			fe.Loaded += OnContentLoaded;
			fe.SizeChanged += OnContentSizeChanged;
			UpdateScrollDetails();

			_contentSubscriptions.Disposable = Disposable.Create(() =>
			{
				fe.Loaded -= OnContentLoaded;
				fe.SizeChanged -= OnContentSizeChanged;
			});
		}

		void OnContentLoaded(object sender, RoutedEventArgs e)
		{
			UpdateScrollDetails();
		}
		void OnContentSizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateLocalFocusOffset();
			UpdateScrollDetails();
		}
	}

	private void OnSizeChanged(object sender, SizeChangedEventArgs args)
	{
		if (IsLoaded) return;

		UpdateScrollDetails();
	}

	private void OnViewportSizeChanged(object sender, SizeChangedEventArgs args)
	{
		UpdateScrollDetails();

		ViewportSizeChanged?.Invoke(this, EventArgs.Empty);
	}

	private void OnScrollBarSizeChanged(object sender, SizeChangedEventArgs args)
	{
		UpdateScrollBarMirrorSpacing();
	}

	protected override void OnPointerPressed(PointerRoutedEventArgs e)
	{
		base.OnPointerPressed(e);

#if DEBUG
		if (e.KeyModifiers.HasFlag(Windows.System.VirtualKeyModifiers.Control))
		{
			// insert breakpoint here~
		}
#endif

		if (!IsAllowedToWork || _translation is null) return;

		var pointerPoint = e.GetCurrentPoint(this);
		if (IsFromMouseDevice(pointerPoint) &&
			pointerPoint.Properties.IsMiddleButtonPressed)
		{
			e.Handled = true;

			var captured = CapturePointer(e.Pointer);
			if (captured)
			{
				_capturedPointerContext = (
					e.Pointer.PointerId,
					pointerPoint.Position,
					ScrollValue
				);
			}
			else
			{
				_capturedPointerContext = default;
			}
		}
	}

	protected override void OnPointerReleased(PointerRoutedEventArgs e)
	{
		base.OnPointerReleased(e);

		ReleasePointerCaptures();
		_capturedPointerContext = default;
	}

	protected override void OnPointerMoved(PointerRoutedEventArgs e)
	{
		base.OnPointerMoved(e);

		if (!IsAllowedToWork || !IsPanAllowed) return;

		if (_capturedPointerContext is { } context)
		{
			var position = e.GetCurrentPoint(this).Position;
			var delta = context.Position.Subtract(position);
			delta.X *= -1;

			SetScrollValue(context.ScrollOffset.Add(delta));
		}
	}

	protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
	{
		base.OnPointerWheelChanged(e);

		if (!IsAllowedToWork) return;
		if (_viewport is null) return;

		var p = e.GetCurrentPoint(_viewport);
		if (!IsFromMouseDevice(p)) return;

		// MouseWheel + Ctrl: Zoom
		if (e.KeyModifiers.HasFlag(Windows.System.VirtualKeyModifiers.Control))
		{
			if (!IsZoomAllowed) return;
			_isHandlingMouseWheelZooming = true;

			var vpAnchor = p.Position;
			var newZoom =
				//ZoomLevel + Math.CopySign(0.5, p.Properties.MouseWheelDelta); // uncomment to debug with fixed zoom increment
				ZoomLevel * (1 + p.Properties.MouseWheelDelta * ScaleWheelRatio);
			newZoom = Math.Clamp(newZoom, MinZoomLevel, MaxZoomLevel);

			var newOffset = CalculateNewOffset(
				ViewportSize,
				ContentSize,
				VectoredScrollValue,
				vpAnchor,
				ZoomLevel,
				newZoom,
				AdditionalMargin);

			// note: updating ZoomLevel can have side effects on ScrollValue:
			// ZoomLevel --UpdateScrollBars-> ScrollBar.Maximum --clamp-> ScrollBar.Value --bound-> H/VScrollValue
			// before we set the ZoomLevel, make sure to snapshot ScrollValue or finish the calculation using ScrollValue
			ZoomLevel = newZoom;
			SetScrollValue(newOffset, shouldClamp: !AllowFreePanning);

			_isHandlingMouseWheelZooming = false;
			e.Handled = true;
		}
		// MouseWheel + Shift: Scroll Horizontally
		// MouseWheel: Scroll Vertically
		else
		{
			var magnitude = p.Properties.MouseWheelDelta * PanWheelRatio;
			var delta = e.KeyModifiers.HasFlag(Windows.System.VirtualKeyModifiers.Shift)
				? new Point(-magnitude, 0)
				: new Point(0, -magnitude);
			var offset = ScrollValue.Add(delta);

			SetScrollValue(offset);
			e.Handled = true;
		}
	}

	// dp changed handlers
	private void OnHorizontalScrollValueChanged()
	{
		if (_preventTranslationUpdate) return;
		UpdateTranslation();
	}

	private void OnVerticalScrollValueChanged()
	{
		if (_preventTranslationUpdate) return;
		UpdateTranslation();
	}

	private void OnAdditionalMarginChanged()
	{
		UpdateScrollBars();

		ViewportSizeChanged?.Invoke(this, EventArgs.Empty);
	}

	private async void OnZoomLevelChanged()
	{
		if (_viewport is null || _translation is null) return;

		if (CoerceZoomLevel())
		{
			_previousZoomLevel = ZoomLevel;
			NotifyStateChanged();
			return;
		}

		// capture current scroll value before it gets clamped by UpdateScrollBars
		var oldScrollValue = VectoredScrollValue;

		// make sure h/v min-max scroll range are updated before setting a new offset
		// to avoid clamping the value within an outdated range.
		UpdateScrollBars(shouldCommitTranslationImmediately: false);

		// when zooming occurs outside of mouse-wheel,
		// we need to anchor the zoom to the center of the viewport.
		var previousZoom = _previousZoomLevel;
		if (!_isHandlingMouseWheelZooming && _previousZoomLevel is { } oldZoom)
		{
			var anchor = ViewportSize.DivideBy(2).ToPoint();
			var newOffset = CalculateNewOffset(
				ViewportSize,
				ContentSize,
				oldScrollValue,
				anchor,
				oldZoom,
				ZoomLevel,
				AdditionalMargin);

			SetScrollValue(newOffset, shouldClamp: !AllowFreePanning);
		}
		_previousZoomLevel = ZoomLevel;

		ZoomLevelChanged?.Invoke(this, new(previousZoom ?? double.NaN, ZoomLevel, fromMouseWheelPanning: _isHandlingMouseWheelZooming));
		NotifyStateChanged();
		await RaiseRenderedContentUpdated();
	}

	private void OnMinZoomLevelChanged()
	{
		CoerceZoomLevel();
	}

	private void OnMaxZoomLevelChanged()
	{
		CoerceZoomLevel();
	}

	private void OnIsActiveChanged()
	{
		if (!IsActive)
		{
			ResetZoom();
			ResetScroll();
		}
		if (_scrollH is not null)
		{
			_scrollH.Visibility = IsActive ? Visibility.Visible : Visibility.Collapsed;
		}
		if (_scrollV is not null)
		{
			_scrollV.Visibility = IsActive ? Visibility.Visible : Visibility.Collapsed;
		}

		IsActiveChanged?.Invoke(this, EventArgs.Empty);
	}

	private void OnAllowFreePanningChanged()
	{
		UpdateScrollBars();
	}

	private void OnScrollBarLayoutChanged()
	{
		UpdateVisualStates();
		UpdateScrollBarMirrorSpacing();
	}

	private void OnElementOnFocusChanged()
	{
		if (ElementOnFocus is { })
		{
			SetLocalFocus(ElementOnFocus);
		}
		else
		{
			ClearLocalFocus();
		}
	}
}

partial class ZoomContentControl
{
	/// <summary>
	/// Centers the content within the viewport.
	/// </summary>
	public void CenterContent()
	{
		if (ViewportSize is { Width: > 0, Height: > 0 } vp)
		{
			// the translation is applied after scaling, so it should use the scaled coords.
			var offset = vp.Subtract(PaddedScaledContentSize);
			var half = offset.DivideBy(2).ToPoint();

			// lastly, we multiple it by K to cancel out the +-direction sign.
			// the scroll value should normally be in the positive range (unless manually scrolled with AllowFreePanning),
			// as dictated by the ScrollBars (see: UpdateScrollBars).
			var value = half.MultiplyBy(K);

			SetScrollValue(value, shouldClamp: false);
		}
		else
		{
			ResetScroll();
		}
	}

	/// <summary>
	/// Adjusts the ZoomLevel so that the content fits within the viewport.
	/// </summary>
	public void FitToCanvas()
	{
		if (IsActive &&
			ViewportSize is { Width: > 0, Height: > 0 } vp &&
			ContentSize is { Width: > 0, Height: > 0 })
		{
			var hZoom = (vp.Width - AdditionalMargin.Left - AdditionalMargin.Right) / ContentSize.Width;
			var vZoom = (vp.Height - AdditionalMargin.Top - AdditionalMargin.Bottom) / ContentSize.Height;
			var zoomLevel = Math.Min(vZoom, hZoom);

			ZoomLevel = Math.Clamp(zoomLevel, MinZoomLevel, MaxZoomLevel);
		}
	}

	/// <summary>
	/// Resets the ZoomLevel to 1, and center the content.
	/// </summary>
	public void ResetViewport()
	{
		ResetZoom();
		CenterContent();
	}

	/// <summary>
	/// Resets the ZoomLevel to 1.
	/// </summary>
	public void ResetZoom() => ZoomLevel = 1;

	/// <summary>
	/// Resets the scroll position to (0,0).
	/// </summary>
	public void ResetScroll()
	{
		SetScrollValue(default, shouldClamp: false);
	}

	public void SetLocalFocus(FrameworkElement? target)
	{
#if NETCOREAPP
		if (!IsLocalFocusSupported()) return;
#else
		if (!(_localFocusWrapper is { } && _localFocusTranslation is { })) return;
#endif

		if (target is not { IsLoaded: true, ActualWidth: > 0, ActualHeight: > 0 })
		{
			ClearLocalFocus();
			return;
		}

		// make sure the target is a descendant of the content
		if (_localFocusPresenter?.GetFirstDescendant<FrameworkElement>(x => x == target) is null) return;

		_localFocusTarget = target;

		ContentSize = target.ActualSize.ToSize();
		UpdateLocalFocusOffset();

		FitToCanvas();
		CenterContent();
	}

	public void ClearLocalFocus()
	{
#if NETCOREAPP
		if (!IsLocalFocusSupported()) return;
#else
		if (!(_localFocusWrapper is { } && _localFocusTranslation is { })) return;
#endif

		_localFocusTarget = null;
		_localFocusTranslation.X = 0;
		_localFocusTranslation.Y = 0;
		ContentSize = (Content as FrameworkElement)?.ActualSize.ToSize() ?? default;

		CenterContent();
	}
}

partial class ZoomContentControl // helpers
{
	// non-const/impure
	private bool CoerceZoomLevel()
	{
		var zoomLevel = ZoomLevel;
		var coercedZoomLevel = Math.Clamp(zoomLevel, MinZoomLevel, MaxZoomLevel);
		if (coercedZoomLevel != zoomLevel)
		{
			ZoomLevel = coercedZoomLevel;
			return true;
		}

		return false;
	}

	private async void UpdateScrollDetails()
	{
		if ((_localFocusTarget ?? Content) is FrameworkElement { IsLoaded: true } fe)
		{
			ContentSize = new Size(fe.ActualWidth, fe.ActualHeight);

			if (AutoFitToCanvas) FitToCanvas();
			if (AutoCenterContent) CenterContent();
			UpdateScrollBars();

			await RaiseRenderedContentUpdated();
		}
	}

	private void UpdateScrollBars(bool shouldCommitTranslationImmediately = true)
	{
		if (ViewportSize is not { Width: > 0, Height: > 0 } vp) return;

		if (Content is Canvas { Children: { Count: > 0 } } canvas)
		{
			var realm = canvas.Children
				.Select(x => new Rect(x.ActualOffset.X, x.ActualOffset.Y, x.ActualSize.X, x.ActualSize.Y))
				.Aggregate(RectHelper.Union);

			_preventTranslationUpdate = true;
			if (realm != default)
			{
				realm = realm.Multiply(ZoomLevel).Inflate(AdditionalMargin);

				HorizontalMinScroll = -realm.Right;
				HorizontalMaxScroll = -realm.Left;

				VerticalMinScroll = realm.Top;
				VerticalMaxScroll = realm.Bottom;
			}
			else
			{
				HorizontalMaxScroll = HorizontalMinScroll = 0;
				VerticalMaxScroll = VerticalMinScroll = 0;
			}
			_preventTranslationUpdate = false;
			if (shouldCommitTranslationImmediately) UpdateTranslation();
		}
		else
		{
			//         [-------Viewport-------]
			// case A: [ScaledContent]					SV e [0, V-C] when n is positive
			// case B: [ScaledContent----------------]	SV e [V-C, 0] when n is negative

			// ScrollBar can only work when Min <= Max.
			//
			// in the case of B, we have to flip the range, so that Min=0, Max=abs(n).
			// later when applying the scroll value to the translation, we have to take this flipping into account.
			// that is done by re-applying K (sign of N = V-C) to the non-vectored scroll value.
			//
			// in the case of A, the range is normally Min=0, Max=n.
			// but if AllowFreePanning is false, we want to keep the content centered, so Min=Max=n/2.
			// we can't leave the min+max at 0, otherwise when re-centering/panning,
			// the content will jump to the top-left corner, because the value will bed clamped to 0.
			var n = ViewportSize.Subtract(PaddedScaledContentSize);
			_preventTranslationUpdate = true;
			(HorizontalMinScroll, HorizontalMaxScroll) = CalculateScrollRange(n.Width, AllowFreePanning);
			(VerticalMinScroll, VerticalMaxScroll) = CalculateScrollRange(n.Height, AllowFreePanning);
			_preventTranslationUpdate = false;
			if (shouldCommitTranslationImmediately) UpdateTranslation();

			static (double Min, double Max) CalculateScrollRange(double n, bool freePan) =>
				n > 0 && !freePan
					? (Math.Abs(n / 2), Math.Abs(n / 2))
					: (0, Math.Abs(n));
		}

		var actualVP = ViewportSize.Subtract(AdditionalMargin);
		Update(_scrollH, HorizontalMinScroll < HorizontalMaxScroll, actualVP.Width);
		Update(_scrollV, VerticalMinScroll < VerticalMaxScroll, actualVP.Height);
		void Update(ScrollBar? sb, bool shown, double thumbSize)
		{
			if (sb is null) return;

			// update size of thumb
			sb.ViewportSize = thumbSize;

			// Showing/hiding the ScrollBar(s)could cause the ContentPresenter to move as it re-centers.
			// This adds unnecessary complexity for the zooming logics as we need to preserve the focal point
			// under the cursor position or the pinch center point after zooming.
			// To avoid all that, we just make them permanently there for layout calculation.
			sb.IsEnabled = shown;
			sb.Opacity = shown ? 1 : 0;
		}
	}

	internal void SetScrollValue(Point value, bool shouldClamp = true)
	{
		// we allow unconstrained panning for MouseWheelZoom(desktop) and PinchToZoom(mobile)
		// where the focal point should remain stationary after zooming.
		if (shouldClamp)
		{
			value.X = Math.Clamp(value.X, HorizontalMinScroll, HorizontalMaxScroll);
			value.Y = Math.Clamp(value.Y, VerticalMinScroll, VerticalMaxScroll);
		}

		_preventTranslationUpdate = true;
		HorizontalScrollValue = value.X;
		VerticalScrollValue = value.Y;
		_preventTranslationUpdate = false;

		UpdateTranslation();
		NotifyStateChanged();
	}

	private void UpdateTranslation()
	{
		if (_viewport is { } && _translation is { })
		{
			var value = VectoredScrollValue;

			// the translation offset were calculated with the content inflated by AdditionalMargin
			// which leaves the content left-top aligned within the AdditionalMargin.
			// here we add a final offset to center it.
			_translation.X = value.X + AdditionalMargin.Left;
			_translation.Y = value.Y + AdditionalMargin.Top;
		}
	}

	private async Task RaiseRenderedContentUpdated()
	{
		await Task.Yield();

		RenderedContentUpdated?.Invoke(this, EventArgs.Empty);
	}

	private void UpdateScrollBarMirrorSpacing()
	{
		if (_rootGrid is null) return;
		if (_rootGrid.RowDefinitions is not { Count: 3 }) return;
		if (_rootGrid.ColumnDefinitions is not { Count: 3 }) return;

		var mirrorSpacing = ScrollBarLayout is ZoomContentControlScrollBarLayout.BottomRightWithMirrorSpacing;
		_rootGrid.RowDefinitions[0].Height = mirrorSpacing && _scrollH?.ActualHeight > 0 ? new GridLength(_scrollH.ActualHeight) : GridLength.Auto;
		_rootGrid.ColumnDefinitions[0].Width = mirrorSpacing && _scrollV?.ActualWidth > 0 ? new GridLength(_scrollV.ActualWidth) : GridLength.Auto;
	}

	private void UpdateVisualStates()
	{
		VisualStateManager.GoToState(this, ScrollBarLayout.ToString(), useTransitions: IsLoaded);
	}

	public void UpdateLocalFocusOffset()
	{
#if NETCOREAPP
		if (!IsLocalFocusSupported()) return;
#else
		if (!(_localFocusWrapper is { } && _localFocusTranslation is { })) return;
#endif
		if (_localFocusTarget is null) return;

		// note: existing _localFocusTranslation won't accumulate here
		// however if the visual-tree alters, the calculated offset may be out-dated.
		// in ZCC, we subscribe to Content.SizeChanged* to call this method.
		// if the layout alters anywhere between Content and _localFocusTarget, consumer's responsibility to call this method again.
		// *: Content.Size is not the same as ContentSize which is actually (focus??content).Size
		var offset = _localFocusTarget.TransformToVisual(_localFocusPresenter)
			.TransformPoint(default)
			.MultiplyBy(-1);

		// AdditionalMargin is used to create a margin(unscaled) around the actual content or local focus content,
		// which is done by virtually inflating the size of content.
		_localFocusTranslation.X = offset.X;
		_localFocusTranslation.Y = offset.Y;
	}

	// const/pure
	private bool IsAllowedToWork => (IsLoaded && IsActive);

#if NETCOREAPP
	[MemberNotNullWhen(true, nameof(_localFocusWrapper), nameof(_localFocusTranslation))]
	private bool IsLocalFocusSupported()
	{
		return _localFocusWrapper is { } && _localFocusTranslation is { };
	}
#endif

	private static bool IsFromMouseDevice(PointerPoint pp)
	{
		return pp
#if IS_WINUI
			.PointerDeviceType == PointerDeviceType.Mouse;
#else
			.PointerDevice.PointerDeviceType == PointerDeviceType.Mouse;
#endif
	}

	public Size ViewportSize => _viewport is { } ? new Size(_viewport.ActualWidth, _viewport.ActualHeight) : new Size(double.NaN, double.NaN);

	/// <summary>
	/// The viewport size, after subtracting the AdditionalMargin.
	/// </summary>
	public Size ClippedViewportSize => ViewportSize.Subtract(AdditionalMargin);

	// Indicates the vector direction of translation. There are two parts to the translation: direction(+-sign) and magnitude(distance).
	// note: see detailed explanation in UpdateScrollBars.
	internal Point K => ComputeK(ViewportSize, ContentSize, ZoomLevel, AdditionalMargin);

	internal Point ScrollValue => new Point(HorizontalScrollValue, VerticalScrollValue);

	// see-also: K, UpdateScrollBars
	internal Point VectoredScrollValue => ScrollValue.MultiplyBy(K);

	internal Size ScaledContentSize => ContentSize.MultiplyBy(ZoomLevel);

	// note: the AdditionalMargin is margin applied around the content.
	// This is expected to be final/constant, regardless of ZoomLevel.
	// Because of its unscalable nature, to make it work, it can only live within the formulas.
	// It should NEVER be applied as Margin/Padding to any UIElement.
	//
	// Technically, it is the padding for the "fake" ScrollViewer, which we simulated via Scale+TranslateTransform.
	// The problem being, if we had applied it as the margin to the PART_WrapperContainer,
	// it would shrink the "viewport" with margin, and clip anything past the AdditionalMargin area...

	internal Size PaddedScaledContentSize => ScaledContentSize.Add(AdditionalMargin); // additional margin should not be scaled, regardless of zoom level.

	internal static Point ComputeK(Size vpSize, Size contentSize, double zoomLevel, Thickness additionalMargin)
	{
		if (zoomLevel is 0) return new Point(1, 1);

		var paddedScaledContentSize = contentSize.MultiplyBy(zoomLevel).Add(additionalMargin);
		var delta = vpSize.Subtract(paddedScaledContentSize);

		return new Point(1, 1).CopySign(delta);
	}

	internal static Point CalculateNewOffset(Size vpSize, Size baseContentSize, Point oldVectoredOffset, Point vpAnchor, double oldZoom, double newZoom, Thickness additionalMargin)
	{
		// here, the old/base/new- prefix refers to the same values at different zoom levels, with base- referring ZoomLevel=1.
		// vpAnchor is an reference point in viewport coordinates, which will be kept stationary during zooming.
		// typically this is the cursor position or the center of the viewport.

		// a final offset is added in UpdateTranslation to center the content within the AdditionalMargin area
		// this offset like AdditionalMargin is final, not scaled by ZoomLevel, so we need to remove it from the input,
		// and re-introduce it onto the output.
		var finalOffset = new Point(additionalMargin.Left, additionalMargin.Top);

		// find the anchor point for new zoom-level
		var oldAnchor = vpAnchor.Subtract(finalOffset).Subtract(oldVectoredOffset);
		var baseAnchor = oldAnchor.DivideBy(oldZoom);
		var newAnchor = baseAnchor.MultiplyBy(newZoom);

		// find offset required to keep the anchor point in the same viewport position
		var newK = ComputeK(vpSize, baseContentSize, newZoom, additionalMargin);
		var newOffset = vpAnchor.Subtract(newAnchor).MultiplyBy(newK).Add(finalOffset);

		return newOffset;
	}
}
