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

public partial class ZoomContentControl
{
	#region DependencyProperty: HorizontalOffset

	/// Identifies the HorizontalOffset dependency property.
	public static DependencyProperty HorizontalOffsetProperty { get; } = DependencyProperty.Register(
		nameof(HorizontalOffset),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double), OnHorizontalOffsetChanged));

	/// Gets or sets the horizontal offset for panning the content.
	public double HorizontalOffset
	{
		get => (double)GetValue(HorizontalOffsetProperty);
		set => SetValue(HorizontalOffsetProperty, value);
	}

	#endregion

	#region DependencyProperty: HorizontalScrollValue

	/// Identifies the HorizontalScrollValue dependency property.
	public static DependencyProperty HorizontalScrollValueProperty { get; } = DependencyProperty.Register(
		nameof(HorizontalScrollValue),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// Gets or sets the value of the horizontal scrollbar.
	/// It's used to represent the scroll position within the scroll bar UI
	public double HorizontalScrollValue
	{
		get => (double)GetValue(HorizontalScrollValueProperty);
		set => SetValue(HorizontalScrollValueProperty, value);
	}

	#endregion

	#region DependencyProperty: HorizontalMinScroll

	/// Identifies the HorizontalMinScroll dependency property.
	public static DependencyProperty HorizontalMinScrollProperty { get; } = DependencyProperty.Register(
		nameof(HorizontalMinScroll),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// Gets or sets the minimum horizontal scroll limit.
	public double HorizontalMinScroll
	{
		get => (double)GetValue(HorizontalMinScrollProperty);
		set => SetValue(HorizontalMinScrollProperty, value);
	}

	#endregion

	#region DependencyProperty: HorizontalMaxScroll

	/// Identifies the HorizontalMaxScroll dependency property.
	public static DependencyProperty HorizontalMaxScrollProperty { get; } = DependencyProperty.Register(
		nameof(HorizontalMaxScroll),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// Gets or sets the maximum horizontal scroll limit.
	public double HorizontalMaxScroll
	{
		get => (double)GetValue(HorizontalMaxScrollProperty);
		set => SetValue(HorizontalMaxScrollProperty, value);
	}

	#endregion

	#region DependencyProperty: HorizontalZoomCenter

	/// Identifies the HorizontalZoomCenter dependency property.
	public static DependencyProperty HorizontalZoomCenterProperty { get; } = DependencyProperty.Register(
		nameof(HorizontalZoomCenter),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// Gets or sets the horizontal center point for zooming.
	public double HorizontalZoomCenter
	{
		get => (double)GetValue(HorizontalZoomCenterProperty);
		set => SetValue(HorizontalZoomCenterProperty, value);
	}

	#endregion

	#region DependencyProperty: VerticalOffset

	/// Identifies the VerticalOffset dependency property.
	public static DependencyProperty VerticalOffsetProperty { get; } = DependencyProperty.Register(
		nameof(VerticalOffset),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double), OnVerticalOffsetChanged));

	/// Gets or sets the vertical offset for panning the content.
	public double VerticalOffset
	{
		get => (double)GetValue(VerticalOffsetProperty);
		set => SetValue(VerticalOffsetProperty, value);
	}

	#endregion

	#region DependencyProperty: VerticalScrollValue

	/// Identifies the VerticalScrollValue dependency property.
	public static DependencyProperty VerticalScrollValueProperty { get; } = DependencyProperty.Register(
		nameof(VerticalScrollValue),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// Gets or sets the value of the vertical scrollbar.
	public double VerticalScrollValue
	{
		get => (double)GetValue(VerticalScrollValueProperty);
		set => SetValue(VerticalScrollValueProperty, value);
	}

	#endregion

	#region DependencyProperty: VerticalMaxScroll

	/// Identifies the VerticalMaxScroll dependency property.
	public static DependencyProperty VerticalMaxScrollProperty { get; } = DependencyProperty.Register(
		nameof(VerticalMaxScroll),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// Gets or sets the maximum vertical scroll limit.
	public double VerticalMaxScroll
	{
		get => (double)GetValue(VerticalMaxScrollProperty);
		set => SetValue(VerticalMaxScrollProperty, value);
	}

	#endregion

	#region DependencyProperty: VerticalMinScroll

	/// Identifies the VerticalMinScroll dependency property.
	public static DependencyProperty VerticalMinScrollProperty { get; } = DependencyProperty.Register(
		nameof(VerticalMinScroll),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// Gets or sets the minimum vertical scroll limit.
	public double VerticalMinScroll
	{
		get => (double)GetValue(VerticalMinScrollProperty);
		set => SetValue(VerticalMinScrollProperty, value);
	}

	#endregion

	#region DependencyProperty: VerticalZoomCenter

	/// Identifies the VerticalZoomCenter dependency property.
	public static DependencyProperty VerticalZoomCenterProperty { get; } = DependencyProperty.Register(
		nameof(VerticalZoomCenter),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// Gets or sets the vertical center point for zooming.
	public double VerticalZoomCenter
	{
		get => (double)GetValue(VerticalZoomCenterProperty);
		set => SetValue(VerticalZoomCenterProperty, value);
	}

	#endregion

	#region DependencyProperty: ViewportWidth

	/// Identifies the ViewportWidth dependency property.
	public static DependencyProperty ViewportWidthProperty { get; } = DependencyProperty.Register(
		nameof(ViewportWidth),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// Gets or sets the width of the viewport.
	public double ViewportWidth
	{
		get => (double)GetValue(ViewportWidthProperty);
		set => SetValue(ViewportWidthProperty, value);
	}

	#endregion

	#region DependencyProperty: ViewportHeight

	/// Identifies the ViewportHeight dependency property.
	public static DependencyProperty ViewportHeightProperty { get; } = DependencyProperty.Register(
		nameof(ViewportHeight),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// Gets or sets the height of the viewport.
	public double ViewportHeight
	{
		get => (double)GetValue(ViewportHeightProperty);
		set => SetValue(ViewportHeightProperty, value);
	}

	#endregion

	#region DependencyProperty: ZoomLevel

	/// Identifies the ZoomLevel dependency property.
	public static DependencyProperty ZoomLevelProperty { get; } = DependencyProperty.Register(
		nameof(ZoomLevel),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(1d, OnZoomLevelChanged));

	/// Gets or sets the current zoom level.
	public double ZoomLevel
	{
		get => (double)GetValue(ZoomLevelProperty);
		set => SetValue(ZoomLevelProperty, value);
	}

	#endregion

	#region DependencyProperty: MinZoomLevel

	/// Identifies the MinZoomLevel dependency property.
	public static DependencyProperty MinZoomLevelProperty { get; } = DependencyProperty.Register(
		nameof(MinZoomLevel),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double), OnMinZoomLevelChanged));

	/// Gets or sets the minimum zoom level allowed.
	public double MinZoomLevel
	{
		get => (double)GetValue(MinZoomLevelProperty);
		set => SetValue(MinZoomLevelProperty, value);
	}

	#endregion

	#region DependencyProperty: MaxZoomLevel

	/// Identifies the MaxZoomLevel dependency property.
	public static DependencyProperty MaxZoomLevelProperty { get; } = DependencyProperty.Register(
		nameof(MaxZoomLevel),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(500d, OnMaxZoomLevelChanged));

	/// Gets or sets the maximum zoom level allowed.
	public double MaxZoomLevel
	{
		get => (double)GetValue(MaxZoomLevelProperty);
		set => SetValue(MaxZoomLevelProperty, value);
	}

	#endregion

	#region DependencyProperty: PanWheelRatio

	/// Identifies the PanWheelRatio dependency property.
	public static DependencyProperty PanWheelRatioProperty { get; } = DependencyProperty.Register(
		nameof(PanWheelRatio),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(0.25d));

	/// Gets or sets the ratio used for panning with the mouse wheel.
	public double PanWheelRatio
	{
		get => (double)GetValue(PanWheelRatioProperty);
		set => SetValue(PanWheelRatioProperty, value);
	}

	#endregion

	#region DependencyProperty: ScaleWheelRatio

	/// Identifies the ScaleWheelRatio dependency property.
	public static DependencyProperty ScaleWheelRatioProperty { get; } = DependencyProperty.Register(
		nameof(ScaleWheelRatio),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(0.0006d));

	/// Gets or sets the ratio used for scaling the zoom level with the mouse wheel.
	public double ScaleWheelRatio
	{
		get => (double)GetValue(ScaleWheelRatioProperty);
		set => SetValue(ScaleWheelRatioProperty, value);
	}

	#endregion

	#region DependencyProperty: IsActive

	/// Identifies the IsActive dependency property.
	public static DependencyProperty IsActiveProperty { get; } = DependencyProperty.Register(
		nameof(IsActive),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(true, OnIsActiveChanged));

	/// Gets or sets a value indicating whether the control is active.
	public bool IsActive
	{
		get => (bool)GetValue(IsActiveProperty);
		set => SetValue(IsActiveProperty, value);
	}

	#endregion

	#region DependencyProperty: IsZoomAllowed

	/// Identifies the IsZoomAllowed dependency property.
	public static DependencyProperty IsZoomAllowedProperty { get; } = DependencyProperty.Register(
		nameof(IsZoomAllowed),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(true));

	/// Gets or sets a value indicating whether zooming is allowed.
	public bool IsZoomAllowed
	{
		get => (bool)GetValue(IsZoomAllowedProperty);
		set => SetValue(IsZoomAllowedProperty, value);
	}

	#endregion

	#region DependencyProperty: IsHorizontalScrollBarVisible

	/// Identifies the IsHorizontalScrollBarVisible dependency property.
	public static DependencyProperty IsHorizontalScrollBarVisibleProperty { get; } = DependencyProperty.Register(
		nameof(IsHorizontalScrollBarVisible),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(true));

	/// Gets or sets a value indicating whether the horizontal scrollbar is visible.
	public bool IsHorizontalScrollBarVisible
	{
		get => (bool)GetValue(IsHorizontalScrollBarVisibleProperty);
		set => SetValue(IsHorizontalScrollBarVisibleProperty, value);
	}

	#endregion

	#region DependencyProperty: IsPanAllowed

	/// Identifies the IsPanAllowed dependency property.
	public static DependencyProperty IsPanAllowedProperty { get; } = DependencyProperty.Register(
		nameof(IsPanAllowed),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(true));

	/// Gets or sets a value indicating whether panning is allowed.
	public bool IsPanAllowed
	{
		get => (bool)GetValue(IsPanAllowedProperty);
		set => SetValue(IsPanAllowedProperty, value);
	}

	#endregion

	#region DependencyProperty: IsVerticalScrollBarVisible

	/// Identifies the IsVerticalScrollBarVisible dependency property.
	public static DependencyProperty IsVerticalScrollBarVisibleProperty { get; } = DependencyProperty.Register(
		nameof(IsVerticalScrollBarVisible),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(true));

	/// Gets or sets a value indicating whether the vertical scrollbar is visible.
	public bool IsVerticalScrollBarVisible
	{
		get => (bool)GetValue(IsVerticalScrollBarVisibleProperty);
		set => SetValue(IsVerticalScrollBarVisibleProperty, value);
	}

	#endregion

	#region DependencyProperty: AutoZoomToCanvasOnSizeChanged

	/// Identifies the AutoZoomToCanvasOnSizeChanged dependency property.
	public static DependencyProperty AutoZoomToCanvasOnSizeChangedProperty { get; } = DependencyProperty.Register(
		nameof(AutoZoomToCanvasOnSizeChanged),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(true));

	/// Gets or sets a value indicating whether the control should automatically zoom to fit the canvas when its size changes.
	public bool AutoZoomToCanvasOnSizeChanged
	{
		get => (bool)GetValue(AutoZoomToCanvasOnSizeChangedProperty);
		set => SetValue(AutoZoomToCanvasOnSizeChangedProperty, value);
	}

	#endregion

	#region DependencyProperty: AdditionalMargin

	/// Identifies the AdditionalMargin dependency property.
	public static DependencyProperty AdditionalMarginProperty { get; } = DependencyProperty.Register(
		nameof(AdditionalMargin),
		typeof(Thickness),
		typeof(ZoomContentControl),
		new PropertyMetadata(new Thickness(0)));

	/// Gets or sets additional margins around the content.
	public Thickness AdditionalMargin
	{
		get => (Thickness)GetValue(AdditionalMarginProperty);
		set => SetValue(AdditionalMarginProperty, value);
	}

	#endregion

	#region DependencyProperty: ContentBoundsVisibility

	/// Identifies the ContentBoundsVisibility dependency property.
	public static DependencyProperty ContentBoundsVisibilityProperty { get; } = DependencyProperty.Register(
		nameof(ContentBoundsVisibility),
		typeof(BoundsVisibilityFlag),
		typeof(ZoomContentControl),
		new PropertyMetadata(BoundsVisibilityFlag.None));

	/// Gets or sets the visibility data for the content bounds.
	public BoundsVisibilityFlag ContentBoundsVisibility
	{
		get => (BoundsVisibilityFlag)GetValue(ContentBoundsVisibilityProperty);
		set => SetValue(ContentBoundsVisibilityProperty, value);
	}

	#endregion

	private static void OnHorizontalOffsetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).OnHorizontalOffsetChanged();
	private static void OnVerticalOffsetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).OnVerticalOffsetChanged();
	private static void OnZoomLevelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).OnZoomLevelChanged();
	private static void OnMinZoomLevelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).CoerceZoomLevel();
	private static void OnMaxZoomLevelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).CoerceZoomLevel();
	private static void OnIsActiveChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).IsActiveChanged();
}
