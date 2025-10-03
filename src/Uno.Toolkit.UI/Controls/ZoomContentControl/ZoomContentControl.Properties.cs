#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI;

partial class ZoomContentControl // dependency properties
{
	// purely cosmetic class, and to emphasis the presence of default values
	internal static class DefaultValues
	{
		public const double ZoomLevel = 1d;
		public const double MinZoomLevel = 0.1d;
		public const double MaxZoomLevel = 10d;
		public const bool IsZoomAllowed = true;
		public const double ScaleWheelRatio = 0.0006d;
		public const double PanWheelRatio = 0.25d;
		public const bool IsPanAllowed = true;
		public const bool IsActive = true;
		public const bool AutoCenterContent = true;
		public const bool AllowFreePanning = true;
	}

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

	/// <summary>
	/// Identifies the HorizontalMinScroll dependency property.
	/// </summary>
	private static DependencyProperty HorizontalMinScrollProperty { get; } = DependencyProperty.Register(
		nameof(HorizontalMinScroll),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// <summary>
	/// Gets or sets the minimum horizontal scroll limit.
	/// </summary>
	private double HorizontalMinScroll
	{
		get => (double)GetValue(HorizontalMinScrollProperty);
		set => SetValue(HorizontalMinScrollProperty, value);
	}

	#endregion
	#region DependencyProperty: [Private] HorizontalMaxScroll

	/// <summary>
	/// Identifies the HorizontalMaxScroll dependency property.
	/// </summary>
	private static DependencyProperty HorizontalMaxScrollProperty { get; } = DependencyProperty.Register(
		nameof(HorizontalMaxScroll),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// <summary>
	/// Gets or sets the maximum horizontal scroll limit.
	/// </summary>
	private double HorizontalMaxScroll
	{
		get => (double)GetValue(HorizontalMaxScrollProperty);
		set => SetValue(HorizontalMaxScrollProperty, value);
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

	/// <summary>
	/// Identifies the VerticalMaxScroll dependency property.
	/// </summary>
	private static DependencyProperty VerticalMaxScrollProperty { get; } = DependencyProperty.Register(
		nameof(VerticalMaxScroll),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// <summary>
	/// Gets or sets the maximum vertical scroll limit.
	/// </summary>
	private double VerticalMaxScroll
	{
		get => (double)GetValue(VerticalMaxScrollProperty);
		set => SetValue(VerticalMaxScrollProperty, value);
	}

	#endregion
	#region DependencyProperty: [Private] VerticalMinScroll

	/// <summary>
	/// Identifies the VerticalMinScroll dependency property.
	/// </summary>
	private static DependencyProperty VerticalMinScrollProperty { get; } = DependencyProperty.Register(
		nameof(VerticalMinScroll),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(double)));

	/// <summary>
	/// Gets or sets the minimum vertical scroll limit.
	/// </summary>
	private double VerticalMinScroll
	{
		get => (double)GetValue(VerticalMinScrollProperty);
		set => SetValue(VerticalMinScrollProperty, value);
	}

	#endregion

	#region DependencyProperty: ZoomLevel

	/// <summary>
	/// Identifies the ZoomLevel dependency property.
	/// </summary>
	public static DependencyProperty ZoomLevelProperty { get; } = DependencyProperty.Register(
		nameof(ZoomLevel),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(DefaultValues.ZoomLevel, OnZoomLevelChanged));

	/// <summary>
	/// Gets or sets the current zoom level.
	/// </summary>
	public double ZoomLevel
	{
		get => (double)GetValue(ZoomLevelProperty);
		set => SetValue(ZoomLevelProperty, value);
	}

	#endregion
	#region DependencyProperty: MinZoomLevel

	/// <summary>
	/// Identifies the MinZoomLevel dependency property.
	/// </summary>
	public static DependencyProperty MinZoomLevelProperty { get; } = DependencyProperty.Register(
		nameof(MinZoomLevel),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(DefaultValues.MinZoomLevel, OnMinZoomLevelChanged));

	/// <summary>
	/// Gets or sets the minimum zoom level allowed.
	/// </summary>
	public double MinZoomLevel
	{
		get => (double)GetValue(MinZoomLevelProperty);
		set => SetValue(MinZoomLevelProperty, value);
	}

	#endregion
	#region DependencyProperty: MaxZoomLevel

	/// <summary>
	/// Identifies the MaxZoomLevel dependency property.
	/// </summary>
	public static DependencyProperty MaxZoomLevelProperty { get; } = DependencyProperty.Register(
		nameof(MaxZoomLevel),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(DefaultValues.MaxZoomLevel, OnMaxZoomLevelChanged));

	/// <summary>
	/// Gets or sets the maximum zoom level allowed.
	/// </summary>
	public double MaxZoomLevel
	{
		get => (double)GetValue(MaxZoomLevelProperty);
		set => SetValue(MaxZoomLevelProperty, value);
	}

	#endregion
	#region DependencyProperty: IsZoomAllowed

	/// <summary>
	/// Identifies the IsZoomAllowed dependency property.
	/// </summary>
	public static DependencyProperty IsZoomAllowedProperty { get; } = DependencyProperty.Register(
		nameof(IsZoomAllowed),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(DefaultValues.IsZoomAllowed));

	/// <summary>
	/// Gets or sets a value indicating whether zooming is allowed.
	/// </summary>
	public bool IsZoomAllowed
	{
		get => (bool)GetValue(IsZoomAllowedProperty);
		set => SetValue(IsZoomAllowedProperty, value);
	}

	#endregion

	#region DependencyProperty: ScaleWheelRatio

	/// <summary>
	/// Identifies the ScaleWheelRatio dependency property.
	/// </summary>
	public static DependencyProperty ScaleWheelRatioProperty { get; } = DependencyProperty.Register(
		nameof(ScaleWheelRatio),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(DefaultValues.ScaleWheelRatio));

	/// <summary>
	/// Gets or sets the ratio used for scaling the zoom level with the mouse wheel.
	/// </summary>
	public double ScaleWheelRatio
	{
		get => (double)GetValue(ScaleWheelRatioProperty);
		set => SetValue(ScaleWheelRatioProperty, value);
	}

	#endregion
	#region DependencyProperty: PanWheelRatio

	/// <summary>
	/// Identifies the PanWheelRatio dependency property.
	/// </summary>
	public static DependencyProperty PanWheelRatioProperty { get; } = DependencyProperty.Register(
		nameof(PanWheelRatio),
		typeof(double),
		typeof(ZoomContentControl),
		new PropertyMetadata(DefaultValues.PanWheelRatio));

	/// <summary>
	/// Gets or sets the ratio used for panning with the mouse wheel.
	/// </summary>
	public double PanWheelRatio
	{
		get => (double)GetValue(PanWheelRatioProperty);
		set => SetValue(PanWheelRatioProperty, value);
	}

	#endregion

	#region DependencyProperty: IsPanAllowed

	/// <summary>
	/// Identifies the IsPanAllowed dependency property.
	/// </summary>
	public static DependencyProperty IsPanAllowedProperty { get; } = DependencyProperty.Register(
		nameof(IsPanAllowed),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(DefaultValues.IsPanAllowed));

	/// <summary>
	/// Gets or sets a value indicating whether panning is allowed.
	/// </summary>
	public bool IsPanAllowed
	{
		get => (bool)GetValue(IsPanAllowedProperty);
		set => SetValue(IsPanAllowedProperty, value);
	}

	#endregion
	#region DependencyProperty: IsActive

	/// <summary>
	/// Identifies the IsActive dependency property.
	/// </summary>
	public static DependencyProperty IsActiveProperty { get; } = DependencyProperty.Register(
		nameof(IsActive),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(true, OnIsActiveChanged));

	/// <summary>
	/// Gets or sets a value indicating whether the control is active.
	/// </summary>
	public bool IsActive
	{
		get => (bool)GetValue(IsActiveProperty);
		set => SetValue(IsActiveProperty, value);
	}

	#endregion
	#region DependencyProperty: AutoFitToCanvas

	/// <summary>
	/// Identifies the AutoFitToCanvas dependency property.
	/// </summary>
	public static DependencyProperty AutoFitToCanvasProperty { get; } = DependencyProperty.Register(
		nameof(AutoFitToCanvas),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(bool)));

	/// <summary>
	/// Gets or sets a value indicating whether the content should be automatically scaled to fit within the viewport.
	/// </summary>
	public bool AutoFitToCanvas
	{
		get => (bool)GetValue(AutoFitToCanvasProperty);
		set => SetValue(AutoFitToCanvasProperty, value);
	}

	#endregion
	#region DependencyProperty: AutoCenterContent

	/// <summary>
	/// Identifies the AutoCenterContent dependency property.
	/// </summary>
	public static DependencyProperty AutoCenterContentProperty { get; } = DependencyProperty.Register(
		nameof(AutoCenterContent),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(DefaultValues.AutoCenterContent));

	/// <summary>
	/// Gets or sets a value indicating whether the content should be automatically centered within the viewport.
	/// </summary>
	public bool AutoCenterContent
	{
		get => (bool)GetValue(AutoCenterContentProperty);
		set => SetValue(AutoCenterContentProperty, value);
	}

	#endregion
	#region DependencyProperty: AllowFreePanning

	/// <summary>
	/// Identifies the AllowFreePanning dependency property.
	/// </summary>
	public static DependencyProperty AllowFreePanningProperty { get; } = DependencyProperty.Register(
		nameof(AllowFreePanning),
		typeof(bool),
		typeof(ZoomContentControl),
		new PropertyMetadata(DefaultValues.AllowFreePanning, OnAllowFreePanningChanged));

	/// <summary>
	/// Gets or sets a value indicating whether content can be panned outside of the viewport.
	/// </summary>
	public bool AllowFreePanning
	{
		get => (bool)GetValue(AllowFreePanningProperty);
		set => SetValue(AllowFreePanningProperty, value);
	}

	#endregion
	#region DependencyProperty: AdditionalMargin

	/// <summary>
	/// Identifies the AdditionalMargin dependency property.
	/// </summary>
	public static DependencyProperty AdditionalMarginProperty { get; } = DependencyProperty.Register(
		nameof(AdditionalMargin),
		typeof(Thickness),
		typeof(ZoomContentControl),
		new PropertyMetadata(new Thickness(0), OnAdditionalMarginChanged));

	/// <summary>
	/// Gets or sets the additional margin around the content.
	/// </summary>
	public Thickness AdditionalMargin
	{
		get => (Thickness)GetValue(AdditionalMarginProperty);
		set => SetValue(AdditionalMarginProperty, value);
	}

	#endregion
	#region DependencyProperty: ScrollBarLayout

	/// <summary>
	/// Identifies the ScrollBarLayout dependency property.
	/// </summary>
	public static DependencyProperty ScrollBarLayoutProperty { get; } = DependencyProperty.Register(
		nameof(ScrollBarLayout),
		typeof(ZoomContentControlScrollBarLayout),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(ZoomContentControlScrollBarLayout), OnScrollBarLayoutChanged));

	/// <summary>
	/// Gets or sets the layout style of the scroll bars.
	/// </summary>
	public ZoomContentControlScrollBarLayout ScrollBarLayout
	{
		get => (ZoomContentControlScrollBarLayout)GetValue(ScrollBarLayoutProperty);
		set => SetValue(ScrollBarLayoutProperty, value);
	}

	#endregion

	#region DependencyProperty: ElementOnFocus

	/// <summary>
	/// Identifies the ElementOnFocus dependency property.
	/// </summary>
	public static DependencyProperty ElementOnFocusProperty { get; } = DependencyProperty.Register(
		nameof(ElementOnFocus),
		typeof(FrameworkElement),
		typeof(ZoomContentControl),
		new PropertyMetadata(default(FrameworkElement), OnElementOnFocusChanged));

	/// <summary>
	/// Gets or sets the focused element which auto-zoom and auto-fit will be centered around.
	/// </summary>
	public FrameworkElement? ElementOnFocus
	{
		get => (FrameworkElement)GetValue(ElementOnFocusProperty);
		set => SetValue(ElementOnFocusProperty, value);
	}

	#endregion


	private static void OnHorizontalScrollValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).OnHorizontalScrollValueChanged();
	private static void OnVerticalScrollValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).OnVerticalScrollValueChanged();
	private static void OnZoomLevelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).OnZoomLevelChanged();
	private static void OnMinZoomLevelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).OnMinZoomLevelChanged();
	private static void OnMaxZoomLevelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).OnMaxZoomLevelChanged();
	private static void OnIsActiveChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).OnIsActiveChanged();
	private static void OnAllowFreePanningChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).OnAllowFreePanningChanged();
	private static void OnAdditionalMarginChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).OnAdditionalMarginChanged();
	private static void OnScrollBarLayoutChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).OnScrollBarLayoutChanged();
	private static void OnElementOnFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((ZoomContentControl)sender).OnElementOnFocusChanged();
}
