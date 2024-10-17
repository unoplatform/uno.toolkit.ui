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

using static Uno.Toolkit.UI.BoundsVisibilityFlag;

namespace Uno.Toolkit.UI;

[TemplatePart(Name = TemplateParts.RootGrid, Type = typeof(Grid))]
[TemplatePart(Name = TemplateParts.Presenter, Type = typeof(ContentPresenter))]
[TemplatePart(Name = TemplateParts.VerticalScrollBar, Type = typeof(ScrollBar))]
[TemplatePart(Name = TemplateParts.HorizontalScrollBar, Type = typeof(ScrollBar))]
[TemplatePart(Name = TemplateParts.TranslateTransform, Type = typeof(TranslateTransform))]
public partial class ZoomContentControl : ContentControl
{
	private static class TemplateParts
	{
		public const string RootGrid = "PART_RootGrid";
		public const string Presenter = "PART_Presenter";
		public const string HorizontalScrollBar = "PART_ScrollH";
		public const string VerticalScrollBar = "PART_ScrollV";
		public const string TranslateTransform = "PART_TranslateTransform";
	}

	public event EventHandler<EventArgs>? RenderedContentUpdated;
	
	private ContentPresenter? _presenter;
	private ScrollBar? _scrollV;
	private ScrollBar? _scrollH;
	private TranslateTransform? _translation;

	private Point _lastPosition = new Point(0, 0);
	private (bool Horizontal, bool Vertical) _movementDirection = (false, false);
	private bool IsAllowedToWork => (IsEnabled && IsActive && _presenter is not null);
	private uint _capturedPointerId;
	private Point _referencePosition;

	public Size AvailableSize
	{
		get
		{
			var vOffset = (AdditionalMargin.Top + AdditionalMargin.Bottom);
			var hOffset = (AdditionalMargin.Left + AdditionalMargin.Right);
			return new Size(ActualWidth - hOffset, ActualHeight - vOffset);
		}
	}

	public ZoomContentControl()
	{
		DefaultStyleKey = typeof(ZoomContentControl);

		Loaded += OnLoaded;
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

		_presenter = FindTemplatePart<ContentPresenter>(TemplateParts.Presenter);
		_scrollV = FindTemplatePart<ScrollBar>(TemplateParts.VerticalScrollBar);
		_scrollH = FindTemplatePart<ScrollBar>(TemplateParts.HorizontalScrollBar);
		_translation = FindTemplatePart<TranslateTransform>(TemplateParts.TranslateTransform);

		ResetOffset();
		ResetZoom();

		if (_presenter?.Content is FrameworkElement { } fe)
		{
			fe.LayoutUpdated += (s, e) =>
			{
				ViewportWidth = fe.ActualWidth;
				ViewportHeight = fe.ActualHeight;

				UpdateScrollLimits();
			};
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
		_presenter?.ToString();
	}

	private async void OnZoomLevelChanged()
	{
		CoerceZoomLevel();
		UpdateScrollLimits();
		UpdateContentBoundsVisibility();
		UpdateScrollVisibility();
		await RaiseRenderedContentUpdated();
	}

	private void UpdateContentBoundsVisibility()
	{
		if (_presenter?.Content is FrameworkElement fe)
		{
			var m = GetPositionMatrix(fe, this);

			var flags = None;
			if (m.OffsetX >= 0) flags |= BoundsVisibilityFlag.Left;
			if (m.OffsetY >= 0) flags |= BoundsVisibilityFlag.Top;
			if (ActualWidth >= (fe.ActualWidth * ZoomLevel) + m.OffsetX) flags |= BoundsVisibilityFlag.Right;
			if (ActualHeight >= (fe.ActualHeight * ZoomLevel) + m.OffsetY) flags |= BoundsVisibilityFlag.Bottom;

			ContentBoundsVisibility = flags;
		}
	}

	private void UpdateScrollVisibility()
	{
		IsHorizontalScrollBarVisible = !ContentBoundsVisibility.HasFlag(BoundsVisibilityFlag.Left | BoundsVisibilityFlag.Right);
		IsVerticalScrollBarVisible = !ContentBoundsVisibility.HasFlag(BoundsVisibilityFlag.Top | BoundsVisibilityFlag.Bottom);
	}

	private bool CanMoveIn((bool Horizontal, bool Vertical) _movementDirection)
	{
		if (ContentBoundsVisibility.HasFlag(All))
		{
			return false;
		}

		var canMove = false;
		canMove |= CanScrollLeft() && _movementDirection.Horizontal is true;
		canMove |= CanScrollRight() && _movementDirection.Horizontal is false;
		canMove |= CanScrollUp() && _movementDirection.Vertical is true;
		canMove |= CanScrollDown() && _movementDirection.Vertical is false;

		return canMove;
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

	private void UpdateScrollLimits()
	{
		if (_presenter?.Content is FrameworkElement fe)
		{
			var verticalScroll = Math.Max(0, (fe.ActualHeight * ZoomLevel) - ViewportHeight);
			var horizontalScroll = Math.Max(0, (fe.ActualWidth * ZoomLevel) - ViewportWidth);

			HorizontalMaxScroll = horizontalScroll / 2;
			VerticalMaxScroll = verticalScroll / 2;

			HorizontalMinScroll = -1 * HorizontalMaxScroll;
			VerticalMinScroll = -1 * VerticalMaxScroll;
		}
	}

	private void CoerceZoomLevel()
	{
		ZoomLevel = Math.Clamp(ZoomLevel, MinZoomLevel, MaxZoomLevel);
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		CenterContent();
	}

	private void OnSizeChanged(object sender, SizeChangedEventArgs args)
	{
		UpdateContentBoundsVisibility();
		if (IsLoaded && AutoZoomToCanvasOnSizeChanged)
		{
			FitToCanvas();
		}
	}

	private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
	{
		if (!IsAllowedToWork) return;
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
				_capturedPointerId = e.Pointer.PointerId;
				_referencePosition = pointerPoint.Position;
				_lastPosition = _referencePosition;
			}
		}
	}

	private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
	{
		ReleasePointerCaptures();
		_capturedPointerId = default;
	}

	private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
	{
		if (!(IsAllowedToWork && _capturedPointerId > 0 && IsPanAllowed)) return;

		var currentPosition = e.GetCurrentPoint(this).Position;
		_movementDirection = (currentPosition.X > _lastPosition.X, currentPosition.Y > _lastPosition.Y);
		_lastPosition = currentPosition;

		if (CanMoveIn(_movementDirection))
		{
			e.Handled = true;
			var pointerPoint = e.GetCurrentPoint(this);
			var position = pointerPoint.Position;
			var deltaX = position.X - _referencePosition.X;
			var deltaY = position.Y - _referencePosition.Y;
			TryUpdateOffsets(deltaX, deltaY);
			_referencePosition = position;
		}
	}

	private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e) // todo: review scroll values
	{
		if (!IsAllowedToWork) return;

		var pointerPoint = e.GetCurrentPoint(this);
		var pointerProperties = pointerPoint.Properties;

		var changeRatio = GetZoomDelta(pointerProperties);

		if (
#if IS_WINUI
			pointerPoint.PointerDeviceType == PointerDeviceType.Mouse &&
#else
			pointerPoint.PointerDevice.PointerDeviceType == PointerDeviceType.Mouse &&
#endif
			e.KeyModifiers.HasFlag(Windows.System.VirtualKeyModifiers.Control) &&
			IsZoomAllowed)
		{
			e.Handled = true;

			var relativeX = (pointerPoint.Position.X - HorizontalScrollValue) / ZoomLevel;
			var relativeY = (pointerPoint.Position.Y - VerticalScrollValue) / ZoomLevel;

			ZoomLevel *= changeRatio;

			HorizontalScrollValue = pointerPoint.Position.X - (relativeX * ZoomLevel);
			VerticalScrollValue = pointerPoint.Position.Y - (relativeY * ZoomLevel);
			return;
		}

		if (e.KeyModifiers.HasFlag(Windows.System.VirtualKeyModifiers.Shift))
		{
			var deltaX = GetPanDelta(pointerProperties);
			TryUpdateOffsets(deltaX, 0);
			return;
		}

		var deltaY = GetPanDelta(pointerProperties);
		TryUpdateOffsets(0, deltaY);
	}

	private double GetZoomDelta(PointerPointProperties pointerProperties)
	{
		var delta = pointerProperties.MouseWheelDelta * ScaleWheelRatio;
		return 1 + delta;
	}

	private double GetPanDelta(PointerPointProperties pointerProperties)
	{
		var delta = pointerProperties.MouseWheelDelta * PanWheelRatio;
		return delta;
	}

	private void TryUpdateOffsets(double deltaX, double deltaY) // todo: review scroll values
	{
		if ((deltaX > 0 && CanScrollLeft()) ||
			(deltaX < 0 && CanScrollRight()))
		{
			var offset = HorizontalScrollValue + deltaX;
			var max = HorizontalMaxScroll * ZoomLevel;
			HorizontalScrollValue = Math.Clamp(offset, 0, max);
		}

		if ((deltaY > 0 && CanScrollUp()) ||
			(deltaY < 0 && CanScrollDown()))
		{
			var offset = VerticalScrollValue + deltaY;
			var max = VerticalMaxScroll * ZoomLevel;
			VerticalScrollValue = Math.Clamp(offset, 0, max);
		}
	}

	public void ResetViewport()
	{
		ResetZoom();
		ResetOffset();
		CenterContent();
	}

	internal void ResetZoom() => ZoomLevel = 1;

	private void ResetOffset()
	{
		HorizontalScrollValue =0;
		VerticalScrollValue = 0;
	}

	public void CenterContent()
	{
		if (IsActive && _presenter?.Content is FrameworkElement { } content)
		{

			HorizontalScrollValue = ((AvailableSize.Width - (content.ActualWidth * ZoomLevel)) / 2) + AdditionalMargin.Left;
			VerticalScrollValue = ((AvailableSize.Height - (content.ActualHeight * ZoomLevel)) / 2) + AdditionalMargin.Top;
		}
	}

	public void FitToCanvas()
	{
		if (IsActive)
		{
			var vZoom = (ActualHeight - AdditionalMargin.Top - AdditionalMargin.Bottom) / ViewportHeight;
			var hZoom = (ActualWidth - AdditionalMargin.Left - AdditionalMargin.Right) / ViewportWidth;
			var zoomLevel = Math.Min(vZoom, hZoom);
			ZoomLevel = Math.Clamp(zoomLevel, MinZoomLevel, MaxZoomLevel);
			CenterContent();
		}
	}

	// Helper
	private bool CanScrollUp() => !ContentBoundsVisibility.HasFlag(BoundsVisibilityFlag.Top);
	private bool CanScrollDown() => !ContentBoundsVisibility.HasFlag(BoundsVisibilityFlag.Bottom);
	private bool CanScrollLeft() => !ContentBoundsVisibility.HasFlag(BoundsVisibilityFlag.Left);
	private bool CanScrollRight() => !ContentBoundsVisibility.HasFlag(BoundsVisibilityFlag.Right);

	private static Matrix GetPositionMatrix(FrameworkElement element, FrameworkElement rootElement)
		=> ((MatrixTransform)element.TransformToVisual(rootElement)).Matrix;
}
