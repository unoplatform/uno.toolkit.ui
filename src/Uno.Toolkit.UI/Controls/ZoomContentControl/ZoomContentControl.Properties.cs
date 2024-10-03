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

	public static DependencyProperty HorizontalOffsetProperty { get; } = DependencyProperty.Register(
		nameof(HorizontalOffset),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double), OnHorizontalOffsetChanged));

	public double HorizontalOffset
	{
		get => (double)GetValue(HorizontalOffsetProperty);
		set => SetValue(HorizontalOffsetProperty, value);
	}

	#endregion
	#region DependencyProperty: HorizontalScrollValue

	public static DependencyProperty HorizontalScrollValueProperty { get; } = DependencyProperty.Register(
		nameof(HorizontalScrollValue),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	public double HorizontalScrollValue
	{
		get => (double)GetValue(HorizontalScrollValueProperty);
		set => SetValue(HorizontalScrollValueProperty, value);
	}

	#endregion
	#region DependencyProperty: HorizontalMinScroll

	public static DependencyProperty HorizontalMinScrollProperty { get; } = DependencyProperty.Register(
		nameof(HorizontalMinScroll),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	public double HorizontalMinScroll
	{
		get => (double)GetValue(HorizontalMinScrollProperty);
		set => SetValue(HorizontalMinScrollProperty, value);
	}

	#endregion
	#region DependencyProperty: HorizontalMaxScroll

	public static DependencyProperty HorizontalMaxScrollProperty { get; } = DependencyProperty.Register(
		nameof(HorizontalMaxScroll),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	public double HorizontalMaxScroll
	{
		get => (double)GetValue(HorizontalMaxScrollProperty);
		set => SetValue(HorizontalMaxScrollProperty, value);
	}

	#endregion
	#region DependencyProperty: HorizontalZoomCenter

	public static DependencyProperty HorizontalZoomCenterProperty { get; } = DependencyProperty.Register(
		nameof(HorizontalZoomCenter),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	public double HorizontalZoomCenter
	{
		get => (double)GetValue(HorizontalZoomCenterProperty);
		set => SetValue(HorizontalZoomCenterProperty, value);
	}

	#endregion
	#region DependencyProperty: VerticalOffset

	public static DependencyProperty VerticalOffsetProperty { get; } = DependencyProperty.Register(
		nameof(VerticalOffset),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double), OnVerticalOffsetChanged));

	public double VerticalOffset
	{
		get => (double)GetValue(VerticalOffsetProperty);
		set => SetValue(VerticalOffsetProperty, value);
	}

	#endregion
	#region DependencyProperty: VerticalScrollValue

	public static DependencyProperty VerticalScrollValueProperty { get; } = DependencyProperty.Register(
		nameof(VerticalScrollValue),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	public double VerticalScrollValue
	{
		get => (double)GetValue(VerticalScrollValueProperty);
		set => SetValue(VerticalScrollValueProperty, value);
	}

	#endregion
	#region DependencyProperty: VerticalMaxScroll

	public static DependencyProperty VerticalMaxScrollProperty { get; } = DependencyProperty.Register(
		nameof(VerticalMaxScroll),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	public double VerticalMaxScroll
	{
		get => (double)GetValue(VerticalMaxScrollProperty);
		set => SetValue(VerticalMaxScrollProperty, value);
	}

	#endregion
	#region DependencyProperty: VerticalMinScroll

	public static DependencyProperty VerticalMinScrollProperty { get; } = DependencyProperty.Register(
		nameof(VerticalMinScroll),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	public double VerticalMinScroll
	{
		get => (double)GetValue(VerticalMinScrollProperty);
		set => SetValue(VerticalMinScrollProperty, value);
	}

	#endregion
	#region DependencyProperty: VerticalZoomCenter

	public static DependencyProperty VerticalZoomCenterProperty { get; } = DependencyProperty.Register(
		nameof(VerticalZoomCenter),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	public double VerticalZoomCenter
	{
		get => (double)GetValue(VerticalZoomCenterProperty);
		set => SetValue(VerticalZoomCenterProperty, value);
	}

	#endregion
	#region DependencyProperty: ViewPortWidth

	public static DependencyProperty ViewPortWidthProperty { get; } = DependencyProperty.Register(
		nameof(ViewPortWidth),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	public double ViewPortWidth
	{
		get => (double)GetValue(ViewPortWidthProperty);
		set => SetValue(ViewPortWidthProperty, value);
	}

	#endregion
	#region DependencyProperty: ViewPortHeight

	public static DependencyProperty ViewPortHeightProperty { get; } = DependencyProperty.Register(
		nameof(ViewPortHeight),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	public double ViewPortHeight
	{
		get => (double)GetValue(ViewPortHeightProperty);
		set => SetValue(ViewPortHeightProperty, value);
	}

	#endregion
	#region DependencyProperty: ZoomLevel

	public static DependencyProperty ZoomLevelProperty { get; } = DependencyProperty.Register(
		nameof(ZoomLevel),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(1d, OnZoomLevelChanged));

	public double ZoomLevel
	{
		get => (double)GetValue(ZoomLevelProperty);
		set => SetValue(ZoomLevelProperty, value);
	}

	#endregion
	#region DependencyProperty: MinZoomLevel

	public static DependencyProperty MinZoomLevelProperty { get; } = DependencyProperty.Register(
		nameof(MinZoomLevel),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double), OnMinZoomLevelChanged));

	public double MinZoomLevel
	{
		get => (double)GetValue(MinZoomLevelProperty);
		set => SetValue(MinZoomLevelProperty, value);
	}

	#endregion
	#region DependencyProperty: MaxZoomLevel

	public static DependencyProperty MaxZoomLevelProperty { get; } = DependencyProperty.Register(
		nameof(MaxZoomLevel),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(500d, OnMaxZoomLevelChanged));

	public double MaxZoomLevel
	{
		get => (double)GetValue(MaxZoomLevelProperty);
		set => SetValue(MaxZoomLevelProperty, value);
	}

	#endregion
	#region DependencyProperty: PanWheelRatio

	public static DependencyProperty PanWheelRatioProperty { get; } = DependencyProperty.Register(
		nameof(PanWheelRatio),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(0.25d));

	public double PanWheelRatio
	{
		get => (double)GetValue(PanWheelRatioProperty);
		set => SetValue(PanWheelRatioProperty, value);
	}

	#endregion
	#region DependencyProperty: ScaleWheelRatio

	public static DependencyProperty ScaleWheelRatioProperty { get; } = DependencyProperty.Register(
		nameof(ScaleWheelRatio),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(0.0006d));

	public double ScaleWheelRatio
	{
		get => (double)GetValue(ScaleWheelRatioProperty);
		set => SetValue(ScaleWheelRatioProperty, value);
	}

	#endregion
	#region DependencyProperty: IsActive

	public static DependencyProperty IsActiveProperty { get; } = DependencyProperty.Register(
		nameof(IsActive),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(true, OnIsActiveChanged));

	public bool IsActive
	{
		get => (bool)GetValue(IsActiveProperty);
		set => SetValue(IsActiveProperty, value);
	}

	#endregion
	#region DependencyProperty: IsZoomAllowed

	public static DependencyProperty IsZoomAllowedProperty { get; } = DependencyProperty.Register(
		nameof(IsZoomAllowed),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(true));

	public bool IsZoomAllowed
	{
		get => (bool)GetValue(IsZoomAllowedProperty);
		set => SetValue(IsZoomAllowedProperty, value);
	}

	#endregion
	#region DependencyProperty: IsHorizontalScrollBarVisible

	public static DependencyProperty IsHorizontalScrollBarVisibleProperty { get; } = DependencyProperty.Register(
		nameof(IsHorizontalScrollBarVisible),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(true));

	public bool IsHorizontalScrollBarVisible
	{
		get => (bool)GetValue(IsHorizontalScrollBarVisibleProperty);
		set => SetValue(IsHorizontalScrollBarVisibleProperty, value);
	}

	#endregion
	#region DependencyProperty: IsPanAllowed

	public static DependencyProperty IsPanAllowedProperty { get; } = DependencyProperty.Register(
		nameof(IsPanAllowed),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(true));

	public bool IsPanAllowed
	{
		get => (bool)GetValue(IsPanAllowedProperty);
		set => SetValue(IsPanAllowedProperty, value);
	}

	#endregion
	#region DependencyProperty: IsVerticalScrollBarVisible

	public static DependencyProperty IsVerticalScrollBarVisibleProperty { get; } = DependencyProperty.Register(
		nameof(IsVerticalScrollBarVisible),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(true));

	public bool IsVerticalScrollBarVisible
	{
		get => (bool)GetValue(IsVerticalScrollBarVisibleProperty);
		set => SetValue(IsVerticalScrollBarVisibleProperty, value);
	}

	#endregion
	#region DependencyProperty: AutoZoomToCanvasOnSizeChanged

	public static DependencyProperty AutoZoomToCanvasOnSizeChangedProperty { get; } = DependencyProperty.Register(
		nameof(AutoZoomToCanvasOnSizeChanged),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(true));

	public bool AutoZoomToCanvasOnSizeChanged
	{
		get => (bool)GetValue(AutoZoomToCanvasOnSizeChangedProperty);
		set => SetValue(AutoZoomToCanvasOnSizeChangedProperty, value);
	}

	#endregion
	#region DependencyProperty: AdditionalMargin

	public static DependencyProperty AdditionalMarginProperty { get; } = DependencyProperty.Register(
		nameof(AdditionalMargin),
		typeof(Thickness),
		typeof(ZoomContentControl),
		new PropertyMetadata(new Thickness(0)));

	public Thickness AdditionalMargin
	{
		get => (Thickness)GetValue(AdditionalMarginProperty);
		set => SetValue(AdditionalMarginProperty, value);
	}

	#endregion
	#region DependencyProperty: ContentBoundsVisibility

	public static DependencyProperty ContentBoundsVisibilityProperty { get; } = DependencyProperty.Register(
		nameof(ContentBoundsVisibility),
		typeof(BoundsVisibilityData),
		typeof(ZoomContentControl),
		new PropertyMetadata(BoundsVisibilityData.None));

	public BoundsVisibilityData ContentBoundsVisibility
	{
		get => (BoundsVisibilityData)GetValue(ContentBoundsVisibilityProperty);
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
