using System;
using System.Linq;
using Uno.Extensions;
using Uno.Logging;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Uno.Toolkit.UI.Helpers;
using Windows.Foundation;
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
#if !WINDOWS_UWP
	, IResponsiveCallback
#endif
{
	internal WeakReference? _weakTarget;
	private DependencyProperty? _targetProperty;

	public object? Narrowest { get; set; }
	public object? Narrow { get; set; }
	public object? Normal { get; set; }
	public object? Wide { get; set; }
	public object? Widest { get; set; }

	public ResponsiveLayout? Layout { get; set; }

	public ResponsiveExtension()
	{
	}

#if WINDOWS_UWP
	/// <inheritdoc/>
	protected override object? ProvideValue()
	{
		this.Log().WarnIfEnabled("The property value, once initially set, cannot be updated due to UWP limitation. Consider upgrading to WinUI, on which the service provider context is exposed through a ProvideValue overload.");
		return GetInitialValue();
	}
#else
	/// <inheritdoc/>
	protected override object? ProvideValue(IXamlServiceProvider serviceProvider)
	{
		BindToSizeChanged(serviceProvider);

		return GetInitialValue();
	}
#endif

	private object? GetInitialValue()
	{
		var helper = ResponsiveHelper.GetForCurrentView();

		return GetValueForSize(helper.WindowSize, Layout ?? helper.Layout);
	}

	private object? GetValueForSize(Size size, ResponsiveLayout layout)
	{
		var defs = new (double MinWidth, object? Value)?[]
		{
			(layout.Narrowest, Narrowest),
			(layout.Narrow, Narrow),
			(layout.Normal, Normal),
			(layout.Wide, Wide),
			(layout.Widest, Widest),
		}.Where(x => x?.Value != null).ToArray();

		var match = defs.FirstOrDefault(y => y?.MinWidth >= size.Width) ?? defs.LastOrDefault();

		return match?.Value;
	}

#if !WINDOWS_UWP
	private void BindToSizeChanged(IXamlServiceProvider serviceProvider)
	{
		if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget pvt &&
			pvt.TargetObject is FrameworkElement target &&
			pvt.TargetProperty is ProvideValueTargetProperty pvtp &&
			target.FindDependencyPropertyUsingReflection($"{pvtp?.Name}Property") is DependencyProperty dp)
		{
			_weakTarget = new WeakReference(target);
			_targetProperty = dp;

			ResponsiveHelper.GetForCurrentView().Register(this);
		}
		else
		{
			this.Log().Error($"Failed to register {nameof(ResponsiveExtension)}");
		}
	}

	public void OnSizeChanged(Size size, ResponsiveLayout layout)
	{
		if (_weakTarget?.IsAlive == true &&
			_weakTarget.Target is FrameworkElement target &&
			_targetProperty is not null)
		{
			target.SetValue(_targetProperty, GetValueForSize(size, Layout ?? layout));
		}
	}
#endif

}
