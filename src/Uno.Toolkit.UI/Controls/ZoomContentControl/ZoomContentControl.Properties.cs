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
	#region DependencyProperty: [Private] HorizontalScrollValue

	private static DependencyProperty HorizontalScrollValueProperty { get; } = DependencyProperty.Register(
		nameof(HorizontalScrollValue),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double), OnHorizontalScrollValueChanged));

	private double HorizontalScrollValue
	{
		get => (double)GetValue(HorizontalScrollValueProperty);
		set => SetValue(HorizontalScrollValueProperty, value);
	}

	#endregion
	#region DependencyProperty: [Private] HorizontalMinScroll

	/// <summary>Identifies the HorizontalMinScroll dependency property.</summary>
	private static DependencyProperty HorizontalMinScrollProperty { get; } = DependencyProperty.Register(
		nameof(HorizontalMinScroll),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// <summary>Gets or sets the minimum horizontal scroll limit.</summary>
	private double HorizontalMinScroll
	{
		get => (double)GetValue(HorizontalMinScrollProperty);
		set => SetValue(HorizontalMinScrollProperty, value);
	}

	#endregion
	#region DependencyProperty: [Private] HorizontalMaxScroll

	/// <summary>Identifies the HorizontalMaxScroll dependency property.</summary>
	private static DependencyProperty HorizontalMaxScrollProperty { get; } = DependencyProperty.Register(
		nameof(HorizontalMaxScroll),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// <summary>Gets or sets the maximum horizontal scroll limit.</summary>
	private double HorizontalMaxScroll
	{
		get => (double)GetValue(HorizontalMaxScrollProperty);
		set => SetValue(HorizontalMaxScrollProperty, value);
	}

	#endregion
	#region DependencyProperty: [Private] HorizontalZoomCenter

	/// <summary>Identifies the HorizontalZoomCenter dependency property.</summary>
	private static DependencyProperty HorizontalZoomCenterProperty { get; } = DependencyProperty.Register(
		nameof(HorizontalZoomCenter),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// <summary>Gets or sets the horizontal center point for zooming.</summary>
	private double HorizontalZoomCenter
	{
		get => (double)GetValue(HorizontalZoomCenterProperty);
		set => SetValue(HorizontalZoomCenterProperty, value);
	}

	#endregion

	#region DependencyProperty: [Private] VerticalScrollValue

	private static DependencyProperty VerticalScrollValueProperty { get; } = DependencyProperty.Register(
		nameof(VerticalScrollValue),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double), OnVerticalScrollValueChanged));

	private double VerticalScrollValue
	{
		get => (double)GetValue(VerticalScrollValueProperty);
		set => SetValue(VerticalScrollValueProperty, value);
	}

	#endregion
	#region DependencyProperty: [Private] VerticalMaxScroll

	/// <summary>Identifies the VerticalMaxScroll dependency property.</summary>
	private static DependencyProperty VerticalMaxScrollProperty { get; } = DependencyProperty.Register(
		nameof(VerticalMaxScroll),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// <summary>Gets or sets the maximum vertical scroll limit.</summary>
	private double VerticalMaxScroll
	{
		get => (double)GetValue(VerticalMaxScrollProperty);
		set => SetValue(VerticalMaxScrollProperty, value);
	}

	#endregion
	#region DependencyProperty: [Private] VerticalMinScroll

	/// <summary>Identifies the VerticalMinScroll dependency property.</summary>
	private static DependencyProperty VerticalMinScrollProperty { get; } = DependencyProperty.Register(
		nameof(VerticalMinScroll),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// <summary>Gets or sets the minimum vertical scroll limit.</summary>
	private double VerticalMinScroll
	{
		get => (double)GetValue(VerticalMinScrollProperty);
		set => SetValue(VerticalMinScrollProperty, value);
	}

	#endregion
	#region DependencyProperty: [Private] VerticalZoomCenter

	/// <summary>Identifies the VerticalZoomCenter dependency property.</summary>
	private static DependencyProperty VerticalZoomCenterProperty { get; } = DependencyProperty.Register(
		nameof(VerticalZoomCenter),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// <summary>Gets or sets the vertical center point for zooming.</summary>
	private double VerticalZoomCenter
	{
		get => (double)GetValue(VerticalZoomCenterProperty);
		set => SetValue(VerticalZoomCenterProperty, value);
	}

	#endregion

	#region DependencyProperty: ZoomLevel

	/// <summary>Identifies the ZoomLevel dependency property.</summary>
	public static DependencyProperty ZoomLevelProperty { get; } = DependencyProperty.Register(
		nameof(ZoomLevel),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(1d, OnZoomLevelChanged));

	/// <summary>Gets or sets the current zoom level.</summary>
	public double ZoomLevel
	{
		get => (double)GetValue(ZoomLevelProperty);
		set => SetValue(ZoomLevelProperty, value);
	}

	#endregion
	#region DependencyProperty: MinZoomLevel

	/// <summary>Identifies the MinZoomLevel dependency property.</summary>
	public static DependencyProperty MinZoomLevelProperty { get; } = DependencyProperty.Register(
		nameof(MinZoomLevel),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(1d, OnMinZoomLevelChanged));

	/// <summary>Gets or sets the minimum zoom level allowed.</summary>
	public double MinZoomLevel
	{
		get => (double)GetValue(MinZoomLevelProperty);
		set => SetValue(MinZoomLevelProperty, value);
	}

	#endregion
	#region DependencyProperty: MaxZoomLevel

	/// <summary>Identifies the MaxZoomLevel dependency property.</summary>
	public static DependencyProperty MaxZoomLevelProperty { get; } = DependencyProperty.Register(
		nameof(MaxZoomLevel),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(10d, OnMaxZoomLevelChanged));

	/// <summary>Gets or sets the maximum zoom level allowed.</summary>
	public double MaxZoomLevel
	{
		get => (double)GetValue(MaxZoomLevelProperty);
		set => SetValue(MaxZoomLevelProperty, value);
	}

	#endregion
	#region DependencyProperty: IsZoomAllowed

	/// <summary>Identifies the IsZoomAllowed dependency property.</summary>
	public static DependencyProperty IsZoomAllowedProperty { get; } = DependencyProperty.Register(
		nameof(IsZoomAllowed),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(true));

	/// <summary>Gets or sets a value indicating whether zooming is allowed.</summary>
	public bool IsZoomAllowed
	{
		get => (bool)GetValue(IsZoomAllowedProperty);
		set => SetValue(IsZoomAllowedProperty, value);
	}

	#endregion

	#region DependencyProperty: ScaleWheelRatio

	/// <summary>Identifies the ScaleWheelRatio dependency property.</summary>
	public static DependencyProperty ScaleWheelRatioProperty { get; } = DependencyProperty.Register(
		nameof(ScaleWheelRatio),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(0.0006d));

	/// <summary>Gets or sets the ratio used for scaling the zoom level with the mouse wheel.</summary>
	public double ScaleWheelRatio
	{
		get => (double)GetValue(ScaleWheelRatioProperty);
		set => SetValue(ScaleWheelRatioProperty, value);
	}

	#endregion
	#region DependencyProperty: PanWheelRatio

	/// <summary>Identifies the PanWheelRatio dependency property.</summary>
	public static DependencyProperty PanWheelRatioProperty { get; } = DependencyProperty.Register(
		nameof(PanWheelRatio),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(0.25d));

	/// <summary>Gets or sets the ratio used for panning with the mouse wheel.</summary>
	public double PanWheelRatio
	{
		get => (double)GetValue(PanWheelRatioProperty);
		set => SetValue(PanWheelRatioProperty, value);
	}

	#endregion
	#region DependencyProperty: IsPanAllowed

	/// <summary>Identifies the IsPanAllowed dependency property.</summary>
	public static DependencyProperty IsPanAllowedProperty { get; } = DependencyProperty.Register(
		nameof(IsPanAllowed),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(true));

	/// <summary>Gets or sets a value indicating whether panning is allowed.</summary>
	public bool IsPanAllowed
	{
		get => (bool)GetValue(IsPanAllowedProperty);
		set => SetValue(IsPanAllowedProperty, value);
	}

	#endregion

	#region DependencyProperty: IsActive

	/// <summary>Identifies the IsActive dependency property.</summary>
	public static DependencyProperty IsActiveProperty { get; } = DependencyProperty.Register(
		nameof(IsActive),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(true, OnIsActiveChanged));

	/// <summary>Gets or sets a value indicating whether the control is active.</summary>
	public bool IsActive
	{
		get => (bool)GetValue(IsActiveProperty);
		set => SetValue(IsActiveProperty, value);
	}

	#endregion
	#region DependencyProperty: AutoFitToCanvas

	public static DependencyProperty AutoFitToCanvasProperty { get; } = DependencyProperty.Register(
		nameof(AutoFitToCanvas),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(bool)));

	public bool AutoFitToCanvas
	{
		get => (bool)GetValue(AutoFitToCanvasProperty);
		set => SetValue(AutoFitToCanvasProperty, value);
	}

	#endregion
	#region DependencyProperty: AdditionalMargin

	/// <summary>Identifies the AdditionalMargin dependency property.</summary>
	public static DependencyProperty AdditionalMarginProperty { get; } = DependencyProperty.Register(
		nameof(AdditionalMargin),
		typeof(Thickness),
		typeof(ZoomContentControl),
		new PropertyMetadata(new Thickness(0), OnAdditionalMarginChanged));

	/// <summary>Gets or sets additional margins around the content.</summary>
	public Thickness AdditionalMargin
	{
		get => (Thickness)GetValue(AdditionalMarginProperty);
		set => SetValue(AdditionalMarginProperty, value);
	}

	#endregion

	private static void OnHorizontalScrollValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).OnHorizontalScrollValueChanged();
	private static void OnVerticalScrollValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).OnVerticalScrollValueChanged();
	private static void OnZoomLevelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).OnZoomLevelChanged();
	private static void OnMinZoomLevelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).CoerceZoomLevel();
	private static void OnMaxZoomLevelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).CoerceZoomLevel();
	private static void OnIsActiveChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).IsActiveChanged();
	private static void OnAdditionalMarginChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).OnAdditionalMarginChanged();
}
