using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Uno.Disposables;
using Uno.Extensions.Specialized;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Threading.Tasks;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Input;
using Windows.Devices.Input;
#endif

namespace Uno.Toolkit.UI;

[TemplatePart(Name = TemplateParts.RootGrid, Type = typeof(Grid))]
[TemplatePart(Name = TemplateParts.ContentGrid, Type = typeof(Grid))]
[TemplatePart(Name = TemplateParts.ContentPresenter, Type = typeof(ContentPresenter))]
[TemplatePart(Name = TemplateParts.VerticalScrollBar, Type = typeof(ScrollBar))]
[TemplatePart(Name = TemplateParts.HorizontalScrollBar, Type = typeof(ScrollBar))]
[TemplatePart(Name = TemplateParts.TranslateTransform, Type = typeof(TranslateTransform))]
public partial class ZoomContentControl : ContentControl
{
	private static class TemplateParts
	{
		public const string RootGrid = "PART_RootGrid";
		public const string ContentGrid = "PART_ContentGrid";
		public const string ContentPresenter = "PART_ContentPresenter";
		public const string HorizontalScrollBar = "PART_ScrollH";
		public const string VerticalScrollBar = "PART_ScrollV";
		public const string TranslateTransform = "PART_TranslateTransform";
	}

	public event EventHandler<EventArgs>? RenderedContentUpdated;

	private Grid? _contentGrid;
	private ContentPresenter? _contentPresenter;
	private ScrollBar? _scrollV;
	private ScrollBar? _scrollH;
	private TranslateTransform? _translation;

	private (uint Id, Point Position, Point ScrollOffset)? _capturedPointerContext;
	private SerialDisposable _contentSubscriptions = new();

	public ZoomContentControl()
	{
		DefaultStyleKey = typeof(ZoomContentControl);

		SizeChanged += OnSizeChanged;
		PointerPressed += OnPointerPressed;
		PointerReleased += OnPointerReleased;
		PointerMoved += OnPointerMoved;
		PointerWheelChanged += OnPointerWheelChanged;
	}

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		T FindTemplatePart<T>(string name) where T : class =>
			(GetTemplateChild(name) ?? throw new Exception($"Expected template part not found: {name}"))
			as T ?? throw new Exception($"Expected template part '{name}' to be of type: {typeof(T)}");

		_contentGrid = FindTemplatePart<Grid>(TemplateParts.ContentGrid);
		_contentPresenter = FindTemplatePart<ContentPresenter>(TemplateParts.ContentPresenter);
		_scrollV = FindTemplatePart<ScrollBar>(TemplateParts.VerticalScrollBar);
		_scrollH = FindTemplatePart<ScrollBar>(TemplateParts.HorizontalScrollBar);
		_translation = FindTemplatePart<TranslateTransform>(TemplateParts.TranslateTransform);

		ResetViewport();
	}

	protected override void OnContentChanged(object oldContent, object newContent)
	{
		_contentSubscriptions.Disposable = null;
		if (newContent is FrameworkElement { } fe)
		{
			fe.Loaded += OnContentLoaded;
			fe.SizeChanged += OnContentSizeChanged;
			_contentSubscriptions.Disposable = Disposable.Create(() =>
			{
				fe.Loaded -= OnContentLoaded;
				fe.SizeChanged -= OnContentSizeChanged;
			});
		}

		void OnContentLoaded(object sender, RoutedEventArgs e)
		{
			if (AutoFitToCanvas)
			{
				FitToCanvas();
			}
		}
		void OnContentSizeChanged(object sender, SizeChangedEventArgs e)
		{
			ContentWidth = fe.ActualWidth;
			ContentHeight = fe.ActualHeight;
			HorizontalZoomCenter = ContentWidth / 2;
			VerticalZoomCenter = ContentHeight / 2;

			UpdateScrollBars();
		}
	}

	private async Task RaiseRenderedContentUpdated()
	{
		await Task.Yield();
		RenderedContentUpdated?.Invoke(this, EventArgs.Empty);
	}

	private void OnHorizontalScrollValueChanged()
	{
		UpdateTranslation();
	}

	private void OnVerticalScrollValueChanged()
	{
		UpdateTranslation();
	}

	private void OnAdditionalMarginChanged()
	{
		_contentPresenter?.ToString();
	}

	private async void OnZoomLevelChanged()
	{
		if (CoerceZoomLevel())
		{
			return;
		}

		UpdateScrollBars();
		UpdateScrollVisibility();
		await RaiseRenderedContentUpdated();
	}

	private void UpdateScrollVisibility()
	{
		if (Viewport is { } vp)
		{
			ToggleScrollBarVisibility(_scrollH, vp.ActualWidth < ScrollExtentWidth);
			ToggleScrollBarVisibility(_scrollV, vp.ActualWidth < ScrollExtentWidth);
		}

		void ToggleScrollBarVisibility(ScrollBar? sb, bool value)
		{
			if (sb is null) return;

			// Showing/hiding the ScrollBar(s)could cause the ContentPresenter to move as it re-centers.
			// This adds unnecessary complexity for the zooming logics as we need to preserve the focal point
			// under the cursor position or the pinch center point after zooming.
			// To avoid all that, we just make them permanently there for layout calculation.
			sb.IsEnabled = value;
			sb.Opacity = value ? 1 : 0;
		}
	}

	private void IsActiveChanged()
	{
		if (!IsActive)
		{
			ResetOffset();
			ResetZoom();
		}
		if (_scrollH is not null)
		{
			_scrollH.Visibility = IsActive ? Visibility.Visible : Visibility.Collapsed;
		}
		if (_scrollV is not null)
		{
			_scrollV.Visibility = IsActive ? Visibility.Visible : Visibility.Collapsed;
		}
	}

	private void UpdateTranslation()
	{
		if (_translation is { })
		{
			_translation.X = HorizontalScrollValue;
			_translation.Y = VerticalScrollValue * -1; // Having a -1 here aligned the scroll direction with content translation
		}
	}

	private void UpdateScrollBars()
	{
		if (Viewport is { } vp)
		{
			HorizontalMinScroll = VerticalMinScroll = 0;

			HorizontalMaxScroll = Math.Max(0, ScrollExtentWidth - vp.ActualWidth);
			VerticalMaxScroll = Math.Max(0, ScrollExtentHeight - vp.ActualHeight);
			if (_scrollH is { }) _scrollH.ViewportSize = vp.ActualWidth;
			if (_scrollV is { }) _scrollV.ViewportSize = vp.ActualHeight;
		}
	}

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

	private void OnSizeChanged(object sender, SizeChangedEventArgs args)
	{
		if (IsLoaded && AutoFitToCanvas)
		{
			FitToCanvas();
		}
	}

	private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
	{
		if (!IsAllowedToWork || _translation is null) return;
		var pointerPoint = e.GetCurrentPoint(this);
		var pointerProperties = pointerPoint.Properties;

		if (pointerProperties.IsMiddleButtonPressed
#if IS_WINUI
			&& pointerPoint.PointerDeviceType == PointerDeviceType.Mouse)
#else
			&& pointerPoint.PointerDevice.PointerDeviceType == PointerDeviceType.Mouse)
#endif
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

	private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
	{
		ReleasePointerCaptures();
		_capturedPointerContext = default;
	}

	private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
	{
		if (!IsAllowedToWork ||!IsPanAllowed) return;

		if (_capturedPointerContext is { } context)
		{
			var position = e.GetCurrentPoint(this).Position;
			var delta = context.Position - position;
			delta.X *= -1;

			SetScrollValue(context.ScrollOffset + delta);
		}
	}

	private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
	{
		if (!IsAllowedToWork) return;
		if (Viewport is not { } vp) return;

		var p = e.GetCurrentPoint(vp);
		if (
#if IS_WINUI
			p.PointerDeviceType != PointerDeviceType.Mouse
#else
			p.PointerDevice.PointerDeviceType != PointerDeviceType.Mouse
#endif
		) return;

		// MouseWheel + Ctrl: Zoom
		if (e.KeyModifiers.HasFlag(Windows.System.VirtualKeyModifiers.Control))
		{
			if (!IsZoomAllowed) return;

			var oldPosition = (p.Position - vp.ActualSize.ToPoint().DivideBy(2)) - ScrollValue.MultiplyBy(1, -1);
			var basePosition = oldPosition.DivideBy(ZoomLevel);

			var newZoom = ZoomLevel * (1 + p.Properties.MouseWheelDelta * ScaleWheelRatio);
			//var newZoom = ZoomLevel + Math.Sign(p.Properties.MouseWheelDelta);
			newZoom = Math.Clamp(newZoom, MinZoomLevel, MaxZoomLevel);

			var newPosition = basePosition.MultiplyBy(newZoom);
			var delta = (newPosition - oldPosition).MultiplyBy(-1, 1);
			var offset = ScrollValue + delta;

			// note: updating ZoomLevel can have side effects on ScrollValue:
			// ZoomLevel --UpdateScrollBars-> ScrollBar.Maximum --clamp-> ScrollBar.Value --bound-> H/VScrollValue
			// before we set the ZoomLevel, make sure to snapshot ScrollValue or finish the calculation using ScrollValue
			ZoomLevel = newZoom;
			SetScrollValue(offset, shouldClamp: false);

			e.Handled = true;
		}
		// MouseWheel + Shift: Scroll Horizontally
		// MouseWheel: Scroll Vertically
		else
		{
			var magnitude = p.Properties.MouseWheelDelta * PanWheelRatio;
			var delta = e.KeyModifiers.HasFlag(Windows.System.VirtualKeyModifiers.Shift)
				? new Point(magnitude, 0)
				: new Point(0, -magnitude);
			var offset = ScrollValue + delta;

			SetScrollValue(offset);
			e.Handled = true;
		}
	}

	public void ResetViewport()
	{
		ResetZoom();
		ResetOffset();
	}

	internal void ResetZoom() => ZoomLevel = 1;

	private void ResetOffset()
	{
		HorizontalScrollValue = 0;
		VerticalScrollValue = 0;
	}

	public void FitToCanvas()
	{
		if (IsActive && Viewport is { } vp)
		{
			var hZoom = (vp.ActualWidth) / ScrollExtentWidth;
			var vZoom = (vp.ActualHeight) / ScrollExtentHeight;
			var zoomLevel = Math.Min(vZoom, hZoom);

			ZoomLevel = Math.Clamp(zoomLevel, MinZoomLevel, MaxZoomLevel);
			ResetOffset();
		}
	}

	private void SetScrollValue(Point value, bool shouldClamp = true)
	{
		// we allow unconstrained panning for MouseWheelZoom(desktop) and PinchToZoom(mobile)
		// where the focal point should remain stationary after zooming.
		if (shouldClamp)
		{
			value.X = Math.Clamp(value.X, HorizontalMinScroll, HorizontalMaxScroll);
			value.Y = Math.Clamp(value.Y, VerticalMinScroll, VerticalMaxScroll);
		}

		HorizontalScrollValue = value.X;
		VerticalScrollValue = value.Y;
	}

	// Helper

	private bool IsAllowedToWork => (IsLoaded && IsActive && _contentPresenter is not null);

	private FrameworkElement? PresenterContent => _contentPresenter?.Content as FrameworkElement;

	private FrameworkElement? Viewport => _contentGrid;

	private Point ScrollValue => new Point(HorizontalScrollValue, VerticalScrollValue);

	private double ScrollExtentWidth => (ContentWidth + AdditionalMargin.Left + AdditionalMargin.Right) * ZoomLevel;
	private double ScrollExtentHeight => (ContentHeight + AdditionalMargin.Top + AdditionalMargin.Bottom) * ZoomLevel;
}
