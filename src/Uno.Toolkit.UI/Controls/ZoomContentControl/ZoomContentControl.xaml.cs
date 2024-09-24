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
[TemplatePart(Name = TemplateParts.Presenter, Type = typeof(ContentPresenter))]
[TemplatePart(Name = TemplateParts.VerticalScrollBar, Type = typeof(ScrollBar))]
[TemplatePart(Name = TemplateParts.HorizontalScrollBar, Type = typeof(ScrollBar))]
public partial class ZoomContentControl : ContentControl
{
	private static class TemplateParts
	{
		public const string RootGrid = "PART_RootGrid";

		public const string Presenter = "PART_Presenter";

		public const string HorizontalScrollBar = "PART_ScrollH";
		public const string VerticalScrollBar = "PART_ScrollV";
	}

	private Grid? _grid;
	private ContentPresenter? _presenter;
	private ScrollBar? _scrollV;
	private ScrollBar? _scrollH;

	private Point _lastPosition = new Point(0, 0);

	private (bool Horizontal, bool Vertical) _movementDirection = (false, false);

	private bool IsAllowedToWork => (IsEnabled && IsActive && _presenter is not null);

	public bool ResetWhenNotActive { get; set; } = true;

	public Size AvailableSize
	{
		get
		{
			var vOffset = (AdditionalMargin.Top + AdditionalMargin.Bottom);
			var hOffset = (AdditionalMargin.Left + AdditionalMargin.Right);
			return new Size(ActualWidth - hOffset, ActualHeight - vOffset);
		}
	}

	public event EventHandler<EventArgs>? RenderedContentUpdated;

	#region Dependency Properties
	public static readonly DependencyProperty AdditionalMarginProperty =
		DependencyProperty.Register(nameof(AdditionalMargin), typeof(Thickness), typeof(ZoomContentControl), new PropertyMetadata(defaultValue: new Thickness(0)));

	public static readonly DependencyProperty AutoZoomToCanvasOnSizeChangedProperty =
		DependencyProperty.Register(nameof(AutoZoomToCanvasOnSizeChanged), typeof(bool), typeof(ZoomContentControl), new PropertyMetadata(defaultValue: true));

	public static readonly DependencyProperty IsActiveProperty =
		DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(ZoomContentControl), new PropertyMetadata(defaultValue: true));

	public static readonly DependencyProperty IsHorizontalScrollBarVisibleProperty =
		DependencyProperty.Register(nameof(IsHorizontalScrollBarVisible), typeof(bool), typeof(ZoomContentControl), new PropertyMetadata(defaultValue: true));

	public static readonly DependencyProperty IsVerticalScrollBarVisibleProperty =
		DependencyProperty.Register(nameof(IsVerticalScrollBarVisible), typeof(bool), typeof(ZoomContentControl), new PropertyMetadata(defaultValue: true));

	public static readonly DependencyProperty ContentBoundsVisibilityDataProperty =
		DependencyProperty.Register(nameof(ContentBoundsVisibility), typeof(BoundsVisibilityData), typeof(ZoomContentControl), new PropertyMetadata(BoundsVisibilityData.None));

	public static readonly DependencyProperty IsZoomAllowedProperty =
	DependencyProperty.Register(nameof(IsZoomAllowed), typeof(bool), typeof(ZoomContentControl), new PropertyMetadata(defaultValue: true));

	public static readonly DependencyProperty ZoomLevelProperty =
	DependencyProperty.Register(nameof(ZoomLevel), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(1d));

	public static readonly DependencyProperty MinZoomLevelProperty =
	DependencyProperty.Register(nameof(MinZoomLevel), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

	public static readonly DependencyProperty MaxZoomLevelProperty =
	DependencyProperty.Register(nameof(MaxZoomLevel), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(500d));

	public static readonly DependencyProperty HorizontalZoomCenterProperty =
	DependencyProperty.Register(nameof(HorizontalZoomCenter), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

	public static readonly DependencyProperty VerticalZoomCenterProperty =
	DependencyProperty.Register(nameof(VerticalZoomCenter), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

	public static readonly DependencyProperty ScaleWheelRatioProperty =
	DependencyProperty.Register(nameof(ScaleWheelRatio), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0006d));

	public static readonly DependencyProperty PanWheelRatioProperty =
	DependencyProperty.Register(nameof(PanWheelRatio), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.25d));

	public static readonly DependencyProperty IsPanAllowedProperty =
	DependencyProperty.Register(nameof(IsPanAllowed), typeof(bool), typeof(ZoomContentControl), new PropertyMetadata(defaultValue: true));

	public static readonly DependencyProperty HorizontalOffsetProperty =
	DependencyProperty.Register(nameof(HorizontalOffset), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

	public static readonly DependencyProperty VerticalOffsetProperty =
	DependencyProperty.Register(nameof(VerticalOffset), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

	public static readonly DependencyProperty VerticalMaxScrollProperty =
	DependencyProperty.Register(nameof(VerticalMaxScroll), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

	public static readonly DependencyProperty VerticalMinScrollProperty =
	DependencyProperty.Register(nameof(VerticalMinScroll), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

	public static readonly DependencyProperty HorizontalMaxScrollProperty =
	DependencyProperty.Register(nameof(HorizontalMaxScroll), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

	public static readonly DependencyProperty HorizontalMinScrollProperty =
	DependencyProperty.Register(nameof(HorizontalMinScroll), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

	public static readonly DependencyProperty HorizontalScrollValueProperty =
	DependencyProperty.Register(nameof(HorizontalScrollValue), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

	public static readonly DependencyProperty VerticalScrollValueProperty =
	DependencyProperty.Register(nameof(VerticalScrollValue), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

	public static readonly DependencyProperty ViewPortWidthProperty =
	DependencyProperty.Register(nameof(ViewPortWidth), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

	public static readonly DependencyProperty ViewPortHeightProperty =
	DependencyProperty.Register(nameof(ViewPortHeight), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

	//Not a real margin, but a spacing to avoid overlapped by ShellControlPanel
	public Thickness AdditionalMargin
	{
		get => (Thickness)GetValue(AdditionalMarginProperty);
		set => SetValue(AdditionalMarginProperty, value);
	}

	public bool AutoZoomToCanvasOnSizeChanged
	{
		get => (bool)GetValue(AutoZoomToCanvasOnSizeChangedProperty);
		set => SetValue(AutoZoomToCanvasOnSizeChangedProperty, value);
	}

	public bool IsActive
	{
		get => (bool)GetValue(IsActiveProperty);
		set => SetValue(IsActiveProperty, value);
	}

	public bool IsZoomAllowed
	{
		get => (bool)GetValue(IsZoomAllowedProperty);
		set => SetValue(IsZoomAllowedProperty, value);
	}

	public bool IsHorizontalScrollBarVisible
	{
		get => (bool)GetValue(IsHorizontalScrollBarVisibleProperty);
		set => SetValue(IsHorizontalScrollBarVisibleProperty, value);
	}

	public bool IsVerticalScrollBarVisible
	{
		get => (bool)GetValue(IsVerticalScrollBarVisibleProperty);
		set => SetValue(IsVerticalScrollBarVisibleProperty, value);
	}

	public BoundsVisibilityData ContentBoundsVisibility
	{
		get => (BoundsVisibilityData)GetValue(ContentBoundsVisibilityDataProperty);
		set => SetValue(ContentBoundsVisibilityDataProperty, value);
	}

	public double ZoomLevel
	{
		get => (double)GetValue(ZoomLevelProperty);
		set => SetValue(ZoomLevelProperty, value);
	}

	public double MinZoomLevel
	{
		get => (double)GetValue(MinZoomLevelProperty);
		set => SetValue(MinZoomLevelProperty, value);
	}

	public double MaxZoomLevel
	{
		get => (double)GetValue(MaxZoomLevelProperty);
		set => SetValue(MaxZoomLevelProperty, value);
	}

	public double HorizontalZoomCenter
	{
		get => (double)GetValue(HorizontalZoomCenterProperty);
		set => SetValue(HorizontalZoomCenterProperty, value);
	}

	public double VerticalZoomCenter
	{
		get => (double)GetValue(VerticalZoomCenterProperty);
		set => SetValue(VerticalZoomCenterProperty, value);
	}

	public double ScaleWheelRatio
	{
		get => (double)GetValue(ScaleWheelRatioProperty);
		set => SetValue(ScaleWheelRatioProperty, value);
	}

	public double PanWheelRatio
	{
		get => (double)GetValue(PanWheelRatioProperty);
		set => SetValue(PanWheelRatioProperty, value);
	}

	public bool IsPanAllowed
	{
		get => (bool)GetValue(IsPanAllowedProperty);
		set => SetValue(IsPanAllowedProperty, value);
	}

	public double HorizontalOffset
	{
		get => (double)GetValue(HorizontalOffsetProperty);
		set => SetValue(HorizontalOffsetProperty, value);
	}

	public double VerticalOffset
	{
		get => (double)GetValue(VerticalOffsetProperty);
		set => SetValue(VerticalOffsetProperty, value);
	}

	public double VerticalMaxScroll
	{
		get => (double)GetValue(VerticalMaxScrollProperty);
		set => SetValue(VerticalMaxScrollProperty, value);
	}

	public double VerticalMinScroll
	{
		get => (double)GetValue(VerticalMinScrollProperty);
		set => SetValue(VerticalMinScrollProperty, value);
	}

	public double HorizontalMaxScroll
	{
		get => (double)GetValue(HorizontalMaxScrollProperty);
		set => SetValue(HorizontalMaxScrollProperty, value);
	}

	public double HorizontalMinScroll
	{
		get => (double)GetValue(HorizontalMinScrollProperty);
		set => SetValue(HorizontalMinScrollProperty, value);
	}

	public double HorizontalScrollValue
	{
		get => (double)GetValue(HorizontalScrollValueProperty);
		set => SetValue(HorizontalScrollValueProperty, value);
	}

	public double VerticalScrollValue
	{
		get => (double)GetValue(VerticalScrollValueProperty);
		set => SetValue(VerticalScrollValueProperty, value);
	}

	public double ViewPortHeight
	{
		get => (double)GetValue(ViewPortHeightProperty);
		set => SetValue(ViewPortHeightProperty, value);
	}

	public double ViewPortWidth
	{
		get => (double)GetValue(ViewPortWidthProperty);
		set => SetValue(ViewPortWidthProperty, value);
	}

	#endregion

	private async Task RaiseRenderedContentUpdated()
	{
		await Task.Yield();
		RenderedContentUpdated?.Invoke(this, EventArgs.Empty);
	}

	private void RegisterPropertyHandlers()
	{
		// Register for property changed events.
		RegisterPropertyChangedCallback(ZoomLevelProperty, OnZoomLevelChanged);
		RegisterPropertyChangedCallback(MinZoomLevelProperty, CoerceZoomLevel);
		RegisterPropertyChangedCallback(MaxZoomLevelProperty, CoerceZoomLevel);

		RegisterPropertyChangedCallback(HorizontalOffsetProperty, OnHorizontalOffsetChanged);
		RegisterPropertyChangedCallback(VerticalOffsetProperty, OnVerticalOffsetChanged);

		RegisterPropertyChangedCallback(IsActiveProperty, IsActiveChanged);

		this.SizeChanged += (s, e) => UpdateContentBoundsVisibility();
	}

	private void OnVerticalOffsetChanged(DependencyObject sender, DependencyProperty dp)
	{
		UpdateContentBoundsVisibility();
		UpdateScrollVisibility();
		UpdateVerticalScrollBarValue();
	}

	private void OnHorizontalOffsetChanged(DependencyObject sender, DependencyProperty dp)
	{
		UpdateContentBoundsVisibility();
		UpdateScrollVisibility();
		UpdateHorizontalScrollBarValue();
	}

	private async void OnZoomLevelChanged(DependencyObject sender, DependencyProperty dp)
	{
		CoerceZoomLevel(sender, dp);
		UpdateScrollLimits();
		UpdateContentBoundsVisibility();
		UpdateScrollVisibility();
		await RaiseRenderedContentUpdated();
	}

	public record BoundsVisibilityData
	{
		public static readonly BoundsVisibilityData None = new BoundsVisibilityData(leftEdge: false, topEdge: false, rightEdge: false, bottomEdge: false);

		public bool LeftEdge { get; init; }
		public bool TopEdge { get; init; }
		public bool RightEdge { get; init; }
		public bool BottomEdge { get; init; }
		public bool AllEdgesVisible { get; init; }

		public BoundsVisibilityData(bool leftEdge, bool topEdge, bool rightEdge, bool bottomEdge)
		{
			LeftEdge = leftEdge;
			TopEdge = topEdge;
			RightEdge = rightEdge;
			BottomEdge = bottomEdge;
			AllEdgesVisible = LeftEdge && TopEdge && RightEdge && BottomEdge;
		}
	}

	private void UpdateContentBoundsVisibility()
	{
		if (_presenter?.Content is FrameworkElement fe)
		{
			var m = GetPositionMatrix(fe, this);

			ContentBoundsVisibility = new BoundsVisibilityData(
				m.OffsetX >= 0,
				m.OffsetY >= 0,
				this.ActualWidth >= (fe.ActualWidth * ZoomLevel) + m.OffsetX,
				this.ActualHeight >= (fe.ActualHeight * ZoomLevel) + m.OffsetY);
		}
	}

	private void UpdateScrollVisibility()
	{
		IsHorizontalScrollBarVisible = !(ContentBoundsVisibility.RightEdge && ContentBoundsVisibility.LeftEdge);
		IsVerticalScrollBarVisible = !(ContentBoundsVisibility.TopEdge && ContentBoundsVisibility.BottomEdge);
	}

	private bool CanMoveIn((bool Horizontal, bool Vertical) _movementDirection)
	{
		if (ContentBoundsVisibility.AllEdgesVisible)
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

	private bool CanScrollUp() => !ContentBoundsVisibility.TopEdge;

	private bool CanScrollDown() => !ContentBoundsVisibility.BottomEdge;

	private bool CanScrollLeft() => !ContentBoundsVisibility.LeftEdge;

	private bool CanScrollRight() => !ContentBoundsVisibility.RightEdge;

	//Slide move is always on the opposite direction of the drag
	private async void UpdateVerticalScrollBarValue()
	{
		VerticalScrollValue = VerticalOffset;
		await RaiseRenderedContentUpdated();
	}

	//Slide move is always on the opposite direction of the drag
	private async void UpdateHorizontalScrollBarValue()
	{
		HorizontalScrollValue = -1 * HorizontalOffset;
		await RaiseRenderedContentUpdated();
	}

	private void IsActiveChanged(DependencyObject sender, DependencyProperty dp)
	{
		if (ResetWhenNotActive && !IsActive)
		{
			RemoveOffset();
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

	private void UpdateScrollLimits()
	{
		if (_presenter?.Content is FrameworkElement fe)
		{
			// If Actual Width/Height * Zoom < ViewPort Width/Height there should be no scroll
			// If Actual Width/Height * Zoom > ViewPort Width/Height the amount of scroll should
			// be based on the difference between the two values
			var verticalScroll = Math.Max(0, (fe.ActualHeight * ZoomLevel) - ViewPortHeight);
			var horizontalScroll = Math.Max(0, (fe.ActualWidth * ZoomLevel) - ViewPortWidth);

			HorizontalMaxScroll = horizontalScroll / 2;
			VerticalMaxScroll = verticalScroll / 2;

			HorizontalMinScroll = -1 * HorizontalMaxScroll;
			VerticalMinScroll = -1 * VerticalMaxScroll;
		}
	}

	private void CoerceZoomLevel(DependencyObject sender, DependencyProperty dp)
	{
		var control = (ZoomContentControl)sender;
		control.ZoomLevel = Math.Clamp(control.ZoomLevel, control.MinZoomLevel, control.MaxZoomLevel);
	}

	public ZoomContentControl()
	{
		DefaultStyleKey = typeof(ZoomContentControl);

		RegisterPropertyHandlers();

		this.SizeChanged += ZoomContentControl_SizeChanged;
	}

	private void ZoomContentControl_SizeChanged(object sender, SizeChangedEventArgs args)
	{
		if (AutoZoomToCanvasOnSizeChanged)
		{
			ZoomToCanvas();
		}
	}

	protected override void OnApplyTemplate()
	{
		T FindTemplatePart<T>(string name) where T : class =>
			(GetTemplateChild(name) ?? throw new Exception($"Expected template part not found: {name}"))
			as T ?? throw new Exception($"Expected template part '{name}' to be of type: {typeof(T)}");
		_grid = FindTemplatePart<Grid>(TemplateParts.RootGrid);
		_presenter = FindTemplatePart<ContentPresenter>(TemplateParts.Presenter);
		_scrollV = FindTemplatePart<ScrollBar>(TemplateParts.VerticalScrollBar);
		_scrollH = FindTemplatePart<ScrollBar>(TemplateParts.HorizontalScrollBar);

		ResetOffset();
		ResetZoom();

		RegisterToControlEvents();
		RegisterPointerHandlers();
	}

	#region ScrollBars Events

	private void RegisterToControlEvents()
	{
		if (_presenter?.Content is FrameworkElement { } fe)
		{
			fe.LayoutUpdated += (s, e) =>
			{
				ViewPortWidth = fe.ActualWidth;
				ViewPortHeight = fe.ActualHeight;

				UpdateScrollLimits();
			};
		}

		//due to templatebinding there's no TwoWay mode. We need to manually update the values
		if (_scrollV is not null)
		{
			_scrollV.Scroll += ScrollV_Scroll;
		}

		if (_scrollH is not null)
		{
			_scrollH.Scroll += ScrollH_Scroll;
		}
	}

	private double _previousVerticalScrollValue = double.MinValue;

	private double _previousHorizontalScrollValue = double.MinValue;

	//TemplateBinding doesn't support TwoWay mode. We need to manually update the values
	private void ScrollV_Scroll(object sender, ScrollEventArgs e)
	{
		if ((_previousVerticalScrollValue > e.NewValue && !CanScrollUp()) ||
			(_previousVerticalScrollValue < e.NewValue && !CanScrollDown()))
		{
			return;
		}

		VerticalOffset = -1 * e.NewValue;
		_previousVerticalScrollValue = e.NewValue;
	}

	private void ScrollH_Scroll(object sender, ScrollEventArgs e)
	{
		if ((_previousHorizontalScrollValue < e.NewValue && !CanScrollRight()) ||
			(_previousHorizontalScrollValue > e.NewValue && !CanScrollLeft()))
		{
			return;
		}

		HorizontalOffset = -1 * e.NewValue;
		_previousHorizontalScrollValue = e.NewValue;
	}

	#endregion

	private uint _capturedPointerId;
	private Point _referencePosition;

	private void RegisterPointerHandlers()
	{
		// Register for pointer events.
		PointerPressed -= OnPointerPressed;
		PointerReleased -= OnPointerReleased;
		PointerMoved -= OnPointerMoved;
		PointerWheelChanged -= OnPointerWheelChanged;

		PointerPressed += OnPointerPressed;
		PointerReleased += OnPointerReleased;
		PointerMoved += OnPointerMoved;
		PointerWheelChanged += OnPointerWheelChanged;
	}

	private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
	{
		if (!IsAllowedToWork)
		{
			return; // Don't handle the event when the control is disabled.
		}

		var pointerPoint = e.GetCurrentPoint(this);
		var pointerProperties = pointerPoint.Properties;

		// If the middle button of a mouse is pressed, then we want to handle the event.
		if (pointerProperties.IsMiddleButtonPressed
#if IS_WINUI
			&& pointerPoint.PointerDeviceType == PointerDeviceType.Mouse)
#else
			&& pointerPoint.PointerDevice.PointerDeviceType == PointerDeviceType.Mouse)
#endif
		{
			e.Handled = true;

			// Capture the pointer so that we can track its movement even if it leaves the bounds of the control.
			var pointer = e.Pointer;
			var captured = CapturePointer(pointer);
			if (captured)
			{
				_capturedPointerId = pointer.PointerId;
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
		if (!(IsAllowedToWork && _capturedPointerId > 0 && IsPanAllowed))
		{
			return;
		}

		var currentPosition = e.GetCurrentPoint(this).Position;
		_movementDirection = (currentPosition.X > _lastPosition.X, currentPosition.Y > _lastPosition.Y);
		_lastPosition = currentPosition;

		if (CanMoveIn(_movementDirection))
		{
			// If the pointer is captured, then we want to handle the event.
			e.Handled = true;

			var pointerPoint = e.GetCurrentPoint(this);

			// Adjust the offsets based on the pointer's movement.
			var position = pointerPoint.Position;
			var deltaX = position.X - _referencePosition.X;
			var deltaY = position.Y - _referencePosition.Y;

			TryUpdateOffsets(deltaX, deltaY);

			// Update the starting position for next time.
			_referencePosition = position;
		}
	}

	private void TryUpdateOffsets(double deltaX, double deltaY)
	{
		if ((deltaX > 0 && CanScrollLeft()) ||
			(deltaX < 0 && CanScrollRight()))
		{
			HorizontalOffset += deltaX;
		}

		if ((deltaY > 0 && CanScrollUp()) ||
			(deltaY < 0 && CanScrollDown()))
		{
			VerticalOffset += deltaY;
		}
	}

	private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
	{
		if (!IsAllowedToWork)
		{
			return; // Don't handle the event when the control is disabled.
		}

		var pointerPoint = e.GetCurrentPoint(this);
		var pointerProperties = pointerPoint.Properties;

		var changeRatio = GetZoomDelta(pointerProperties);

		//Zoom In/Out
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

			// Pointer relative to the content
			var relativeX = (pointerPoint.Position.X - HorizontalOffset) / ZoomLevel;
			var relativeY = (pointerPoint.Position.Y - VerticalOffset) / ZoomLevel;

			ZoomLevel *= changeRatio;

			HorizontalOffset = pointerPoint.Position.X - (relativeX * ZoomLevel);
			VerticalOffset = pointerPoint.Position.Y - (relativeY * ZoomLevel);
			return;
		}

		//Horizontal Scroll
		if (e.KeyModifiers.HasFlag(Windows.System.VirtualKeyModifiers.Shift))
		{
			var deltaX = GetPanDelta(pointerProperties);
			TryUpdateOffsets(deltaX, 0);
			return;
		}

		//Vertical Scroll
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

	public void Initialize()
	{
		if (_presenter?.Content is FrameworkElement { } fe)
		{
			fe.UpdateLayout();
		}

		ResetZoom();
		ResetOffset();
		Centralize();
	}

	public void ResetZoom() => ZoomLevel = 1;

	public void ResetOffset()
	{
		HorizontalOffset = AdditionalMargin.Left;
		VerticalOffset = AdditionalMargin.Top;
	}

	public void RemoveOffset()
	{
		HorizontalOffset = 0;
		VerticalOffset = 0;
	}

	public void Centralize()
	{
		if (IsActive && _presenter?.Content is FrameworkElement { } content)
		{
			HorizontalOffset = ((AvailableSize.Width - (content.ActualWidth * ZoomLevel)) / 2) + AdditionalMargin.Left;
			VerticalOffset = ((AvailableSize.Height - (content.ActualHeight * ZoomLevel)) / 2) + AdditionalMargin.Top;
		}
	}

	public void ZoomToCanvas()
	{
		if (IsActive)
		{
			var vZoom = (AvailableSize.Height / ViewPortHeight);
			var hZoom = (AvailableSize.Width / ViewPortWidth);
			ZoomLevel = Math.Min(vZoom, hZoom);
			Centralize();
		}
	}
	private static Matrix GetPositionMatrix(FrameworkElement element, FrameworkElement rootElement) => ((MatrixTransform)element.TransformToVisual(rootElement)).Matrix;
}
