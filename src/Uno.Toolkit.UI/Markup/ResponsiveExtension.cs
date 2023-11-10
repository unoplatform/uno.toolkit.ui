using System;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using XamlWindow = Microsoft.UI.Xaml.Window;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using XamlWindow = Windows.UI.Xaml.Window;
#endif

namespace Uno.Toolkit.UI;

/// <summary>
/// A markup extension that updates a property based on the current window width.
/// </summary>
public partial class ResponsiveExtension : MarkupExtension
{
#if WINDOWS_UWP
	/// <summary>
	/// Define whether <see cref="ProvideValue()" /> should throw or just result null.
	/// </summary>
	public static bool ShouldThrow = true;
#endif

	public object? Narrow { get; set; }
	public object? Wide { get; set; }
	public double WidthThreshold { get; set; } = 800;

	public ResponsiveExtension()
	{
	}

#if WINDOWS_UWP
	/// <inheritdoc/>
	protected override object? ProvideValue()
	{
		const string Message = "This feature is not supported on UWP for windows as it depends on WinUI3 api. It still works on all non-Windows UWP platforms and all WinUI 3 platforms.";
		return ShouldThrow ? throw new PlatformNotSupportedException(Message) : null;
	}
#else

	/// <inheritdoc/>
	protected override object? ProvideValue(IXamlServiceProvider serviceProvider)
	{
		object? value = Narrow;
		var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
		var frameworkElement = provideValueTarget?.TargetObject as FrameworkElement;
		var targetProperty = provideValueTarget?.TargetProperty as ProvideValueTargetProperty;
		var dependencyProperty = frameworkElement?.FindDependencyPropertyUsingReflection($"{targetProperty?.Name}Property");

		if (frameworkElement is null) return value;

#if HAS_UNO
		value = XamlWindow.Current.Bounds.Width > WidthThreshold ? Wide : Narrow;

		XamlWindow.Current.SizeChanged += (s, e) =>
		{
			value = XamlWindow.Current.Bounds.Width > WidthThreshold ? Wide : Narrow;
			frameworkElement?.SetValue(dependencyProperty, value);
		};
#endif

#if WINDOWS
		frameworkElement.Loaded += (s, e) =>
		{
			value = frameworkElement.XamlRoot.Size.Width > WidthThreshold ? Wide : Narrow;
			frameworkElement?.SetValue(dependencyProperty, value);

			frameworkElement!.XamlRoot.Changed += (s, e) =>
			{
				value = s.Size.Width > WidthThreshold ? Wide : Narrow;
				frameworkElement?.SetValue(dependencyProperty, value);
			};
		};
#endif

		return value;
	}
#endif
}
