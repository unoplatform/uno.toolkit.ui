using System.ComponentModel;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.UI;

/// <summary>
/// Dependency object representing a single shadow.
/// Public properties are all dependency properties.
/// </summary>
public partial class Shadow : DependencyObject, INotifyPropertyChanged
{
	private static readonly Windows.UI.Color DefaultColor = Windows.UI.Color.FromArgb(255, 0, 0, 0);

	#region DependencyProperty: IsInner

	public static readonly DependencyProperty IsInnerProperty = DependencyProperty.Register(
		nameof(IsInner),
		typeof(bool),
		typeof(Shadow),
		new(default(bool), (s, args) => OnPropertyChanged(s, nameof(IsInner))));

	/// <summary>
	/// If true, the shadow will be drawn inside the bounds of the element.
	/// It will have the same effect as the 'inset' value in a css box-shadow.
	/// </summary>
	public bool IsInner
	{
		get => (bool)GetValue(IsInnerProperty);
		set => SetValue(IsInnerProperty, value);
	}

	#endregion
	#region DependencyProperty: OffsetX

	public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register(
		nameof(OffsetX),
		typeof(double),
		typeof(Shadow),
		new(default(double), (s, args) => OnPropertyChanged(s, nameof(OffsetX))));

	/// <summary>
	/// The X offset of the shadow.
	/// </summary>
	public double OffsetX
	{
		get => (double)GetValue(OffsetXProperty);
		set => SetValue(OffsetXProperty, value);
	}

	#endregion
	#region DependencyProperty: OffsetY

	public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register(
		nameof(OffsetY),
		typeof(double),
		typeof(Shadow),
		new(default(double), (s, args) => OnPropertyChanged(s, nameof(OffsetY))));

	/// <summary>
	/// The Y offset of the shadow.
	/// </summary>
	public double OffsetY
	{
		get => (double)GetValue(OffsetYProperty);
		set => SetValue(OffsetYProperty, value);
	}

	#endregion
	#region DependencyProperty: Color

	public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
		nameof(Color),
		typeof(Windows.UI.Color),
		typeof(Shadow),
		new(DefaultColor, (s, args) => OnPropertyChanged(s, nameof(Color))));

	/// <summary>
	/// The color of the shadow.
	/// It will be multiplied by the opacity property before rendering.
	/// </summary>
	public Windows.UI.Color Color
	{
		get => (Windows.UI.Color)GetValue(ColorProperty);
		set => SetValue(ColorProperty, value);
	}

	#endregion
	#region DependencyProperty: Opacity

	public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register(
		nameof(Opacity),
		typeof(double),
		typeof(Shadow),
		new(default(double), (s, args) => OnPropertyChanged(s, nameof(Opacity))));

	/// <summary>
	/// The opacity of the shadow.
	/// </summary>
	public double Opacity
	{
		get => (double)GetValue(OpacityProperty);
		set => SetValue(OpacityProperty, value);
	}

	#endregion
	#region DependencyProperty: BlurRadius

	public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register(
		nameof(BlurRadius),
		typeof(double),
		typeof(Shadow),
		new(default(double), (s, args) => OnPropertyChanged(s, nameof(BlurRadius))));

	/// <summary>
	/// The radius of the blur that will be applied to the shadow [0..100].
	/// </summary>
	public double BlurRadius
	{
		get => (double)GetValue(BlurRadiusProperty);
		set => SetValue(BlurRadiusProperty, value);
	}

	#endregion
	#region DependencyProperty: Spread

	public static readonly DependencyProperty SpreadProperty = DependencyProperty.Register(
		nameof(Spread),
		typeof(double),
		typeof(Shadow),
		new(default(double), (s, args) => OnPropertyChanged(s, nameof(Spread))));

	/// <summary>
	/// The spread will inflate or deflate (if negative) the control shadow size before applying the blur.
	/// </summary>
	public double Spread
	{
		get => (double)GetValue(SpreadProperty);
		set => SetValue(SpreadProperty, value);
	}

	#endregion

	public event PropertyChangedEventHandler? PropertyChanged;

	internal static bool IsShadowProperty(string propertyName)
	{
		return propertyName == nameof(OffsetX) || propertyName == nameof(OffsetY) ||
			   propertyName == nameof(IsInner) ||
			   propertyName == nameof(Color) ||
			   propertyName == nameof(Opacity) ||
			   propertyName == nameof(BlurRadius) ||
			   propertyName == nameof(Spread);
	}

	internal static bool IsShadowSizeProperty(string propertyName)
	{
		return propertyName == nameof(OffsetX) || propertyName == nameof(OffsetY) ||
			   propertyName == nameof(IsInner) ||
			   propertyName == nameof(BlurRadius) ||
			   propertyName == nameof(Spread);
	}

	private static void OnPropertyChanged(object dependencyObject, string propertyName)
	{
		((Shadow)dependencyObject).PropertyChanged?.Invoke(dependencyObject, new PropertyChangedEventArgs(propertyName));
	}

	public override string ToString() =>
		$"{{ IsInner: {{{IsInner}}}, Offset: {{{OffsetX}, {OffsetY}}} Color: {{A={Color.A}, R={Color.R}, G={Color.G}, B={Color.B}}}, Opacity: {Opacity}, BlurRadius: {BlurRadius}, Spread: {Spread} }}";

	public string ToKey() =>
		string.Join(",", IsInner, OffsetX, OffsetY, Color.ToString(), Opacity, BlurRadius, Spread);

	public Shadow Clone()
	{
		return new Shadow
		{
			IsInner = IsInner,
			OffsetX = OffsetX,
			OffsetY = OffsetY,
			Color = Color,
			Opacity = Opacity,
			BlurRadius = BlurRadius,
			Spread = Spread
		};
	}
}
