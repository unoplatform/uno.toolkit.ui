using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
/// A markup extension that returns values based on the current window size.
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
#if HAS_UNO
	/// <inheritdoc/>
	protected override object? ProvideValue()
	{
		// non-Windows blocked by #14361
		//var value = XamlWindow.Current.Bounds.Width >= WidthThreshold ? Wide : Narrow;

		//XamlWindow.Current.SizeChanged += (s, e) =>
		//{
		//	value = e.Size.Width >= WidthThreshold ? Wide : Narrow;
		//};

		//return value;
		return WidthThreshold > 800 ? Wide : Narrow;
	}
#endif

#if IS_WINUI
	/// <inheritdoc/>
	protected override object? ProvideValue(IXamlServiceProvider serviceProvider)
	{
#if WINDOWS
		object? value = Narrow;
		var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
		var frameworkElement = provideValueTarget?.TargetObject as FrameworkElement;
		var targetProperty = provideValueTarget?.TargetProperty as ProvideValueTargetProperty;
		var dependencyProperty = frameworkElement?.FindDependencyPropertyUsingReflection($"{targetProperty?.Name}Property");

		if (frameworkElement is null) return value;

		frameworkElement.Loaded += (s, e) =>
		{
			frameworkElement.XamlRoot.Changed += (s, e) =>
			{
				value = s.Size.Width > WidthThreshold ? Wide : Narrow;
				frameworkElement?.SetValue(dependencyProperty, value);
			};
		};

		return value;
#else
		return ProvideValue();
#endif
	}
#endif
#endif
	}
