using System.ComponentModel;

using Windows.UI;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace Uno.Toolkit.UI;

/// <summary>
/// Dependency object representing a single shadow.
/// Public properties are all dependency properties.
/// </summary>
public partial class Shadow : DependencyObject, INotifyPropertyChanged
{
    private const double DefaultSpread = 0;

    private const double DefaultBlurRadius = 0;

    private const double DefaultOpacity = 0;

    private const double DefaultOffsetX = 0;

    private const double DefaultOffsetY = 0;

    private static readonly Color DefaultColor = Color.FromArgb(255, 0, 0, 0);

    public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register(
        nameof(OffsetX),
        typeof(double),
        typeof(Shadow),
        new(DefaultOffsetX, (s, args) => OnPropertyChanged(s, nameof(OffsetX))));

    public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register(
        nameof(OffsetY),
        typeof(double),
        typeof(Shadow),
        new(DefaultOffsetY, (s, args) => OnPropertyChanged(s, nameof(OffsetY))));

    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        nameof(Color),
        typeof(Color),
        typeof(Shadow),
        new(DefaultColor, (s, args) => OnPropertyChanged(s, nameof(Color))));

    public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register(
        nameof(Opacity),
        typeof(double),
        typeof(Shadow),
        new(DefaultOpacity, (s, args) => OnPropertyChanged(s, nameof(Opacity))));

    public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register(
        nameof(BlurRadius),
        typeof(double),
        typeof(Shadow),
        new(DefaultBlurRadius, (s, args) => OnPropertyChanged(s, nameof(BlurRadius))));

    public static readonly DependencyProperty SpreadProperty = DependencyProperty.Register(
        nameof(Spread),
        typeof(double),
        typeof(Shadow),
        new(DefaultSpread, (s, args) => OnPropertyChanged(s, nameof(Spread))));

    public event PropertyChangedEventHandler? PropertyChanged;

	/// <summary>
	/// The X offset of the shadow.
	/// </summary>
    public double OffsetX
    {
        get => (double)GetValue(OffsetXProperty);
        set => SetValue(OffsetXProperty, value);
    }

	/// <summary>
	/// The Y offset of the shadow.
	/// </summary>
	public double OffsetY
    {
        get => (double)GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }

	/// <summary>
	/// The color of the shadow.
	/// It will be multiplied by the opacity property before rendering.
	/// </summary>
	public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

	/// <summary>
	/// The opacity of the shadow.
	/// </summary>
	public double Opacity
    {
        get => (double)GetValue(OpacityProperty);
        set => SetValue(OpacityProperty, value);
    }

	/// <summary>
	/// The radius of the blur that will be applied to the shadow [0..100].
	/// </summary>
	public double BlurRadius
    {
        get => (double)GetValue(BlurRadiusProperty);
        set => SetValue(BlurRadiusProperty, value);
    }

	/// <summary>
	/// The spread will inflate or deflate (if negative) the control shadow size before applying the blur.
	/// </summary>
    public double Spread
    {
        get => (double)GetValue(SpreadProperty);
        set => SetValue(SpreadProperty, value);
    }

    internal static bool IsShadowProperty(string? propertyName)
    {
        return propertyName == nameof(OffsetX) || propertyName == nameof(OffsetY) 
               || propertyName == nameof(Color)
               || propertyName == nameof(Opacity) 
               || propertyName == nameof(BlurRadius)
               || propertyName == nameof(Spread);
    }

    internal static bool IsShadowSizeProperty(string? propertyName)
    {
        return propertyName == nameof(OffsetX) || propertyName == nameof(OffsetY)
               || propertyName == nameof(BlurRadius)
               || propertyName == nameof(Spread);
    }

    private static void OnPropertyChanged(object dependencyObject, string propertyName)
    {
        ((Shadow)dependencyObject).PropertyChanged?.Invoke(dependencyObject, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString() =>
        $"{{ Offset: {{{OffsetX}, {OffsetY}}} Color: {{A={Color.A}, R={Color.R}, G={Color.G}, B={Color.B}}}, Opacity: {Opacity}, BlurRadius: {BlurRadius}, Spread: {Spread} }}";

    public string ToKey() =>
        $"{{O{{{OffsetX},{OffsetY}}}+C{{{Color.A},{Color.R},{Color.G},{Color.B}}}+O{Opacity}+B{BlurRadius}+S{Spread}}}";

    public Shadow Clone()
    {
        return new Shadow
            {
                OffsetX = OffsetX,
                OffsetY = OffsetY,
                Color = Color,
                Opacity = Opacity,
                BlurRadius = BlurRadius,
                Spread = Spread
            };
    }
}
