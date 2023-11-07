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

#if WINDOWS_UWP
	protected override object? ProvideValue()
	{
		const string Message = "This feature is not supported on UWP for windows as it depends on WinUI3 api. It still works on all non-Windows UWP platforms and all WinUI 3 platforms.";
		return ShouldThrow ? throw new PlatformNotSupportedException(Message) : null;
	}
#else
	protected override object? ProvideValue(IXamlServiceProvider serviceProvider)
	{
		var window = XamlWindow.Current;
		var initialOrientation = window.Bounds.Width >= window.Bounds.Height ? Orientation.Horizontal : Orientation.Vertical;

		var responsiveValue = new ResponsiveValue { Value = initialOrientation == Orientation.Horizontal ? Wide : Narrow };

		window.SizeChanged += (s, e) =>
		{
			var newOrientation = e.Size.Width >= e.Size.Height ? Orientation.Horizontal : Orientation.Vertical;
			responsiveValue.Value = newOrientation == Orientation.Horizontal ? Wide : Narrow;
		};

		return responsiveValue;
	}
#endif

	public partial class ResponsiveValue : DependencyObject
	{
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
			"Value", typeof(object), typeof(ResponsiveValue), new PropertyMetadata(null));

		public object? Value
		{
			get => GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}
	}
}
