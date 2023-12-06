#if !WINDOWS_UWP
#define SUPPORTS_XAML_SERVICE_PROVIDER
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Windows.Foundation;
using Uno.Extensions;
using Uno.Logging;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
#endif

using static System.Reflection.BindingFlags;

namespace Uno.Toolkit.UI;

#if DEBUG
public partial class ResponsiveExtension // for debugging
{
	// Used by TreeGraph to obtain the ResponsiveExtension(s) declared on the Owner.
	internal static List<(WeakReference Owner, string Property, WeakReference Extension)> TrackedInstances { get; } = new();
}
#endif

/// <summary>
/// A markup extension that updates a property based on the current window width.
/// </summary>
public partial class ResponsiveExtension : MarkupExtension, IResponsiveCallback
{
	private static readonly ILogger _logger = typeof(ResponsiveExtension).Log();

	public object? Narrowest { get; set; }
	public object? Narrow { get; set; }
	public object? Normal { get; set; }
	public object? Wide { get; set; }
	public object? Widest { get; set; }

	public ResponsiveLayout? Layout { get; set; }

#if SUPPORTS_XAML_SERVICE_PROVIDER
	internal WeakReference? TargetWeakRef { get; private set; }
	private Type? _propertyType;
	private DependencyProperty? _targetProperty;
#endif
	internal ResolvedLayout<object?>? ResolvedValue { get; private set; }
#if DEBUG
	internal ResponsiveLayout? LastUsedLayout { get; private set; }
#endif

	public ResponsiveExtension()
	{
	}

#if !SUPPORTS_XAML_SERVICE_PROVIDER
	/// <inheritdoc/>
	protected override object? ProvideValue()
	{
		this.Log().WarnIfEnabled(() => "The property value, once initially set, cannot be updated due to UWP limitation. Consider upgrading to WinUI, on which the service provider context is exposed through a ProvideValue overload.");
		return ResolveValue();
	}
#else
	/// <inheritdoc/>
	protected override object? ProvideValue(IXamlServiceProvider serviceProvider)
	{
		BindToEvents(serviceProvider);

		return ResolveValue();
	}
#endif

	private object? ResolveValue()
	{
		var helper = ResponsiveHelper.GetForCurrentView();

		return ResolveValue(helper.WindowSize, GetAppliedLayout() ?? helper.Layout);
	}

	private object? ResolveValue(Size size, ResponsiveLayout layout)
	{
		var defs = new (double MinWidth, ResolvedLayout<object?> Value)[]
		{
			(layout.Narrowest, new(nameof(layout.Narrowest), Narrowest)),
			(layout.Narrow, new(nameof(layout.Narrow), Narrow)),
			(layout.Normal, new(nameof(layout.Normal), Normal)),
			(layout.Wide, new(nameof(layout.Wide), Wide)),
			(layout.Widest, new(nameof(layout.Widest), Widest)),
		}.Where(x => x.Value.Value != null).ToArray();
		var match = defs.FirstOrNull(y => y.MinWidth >= size.Width) ?? defs.LastOrNull();
		var resolved = match?.Value;

#if DEBUG
		LastUsedLayout = layout;
#endif
		ResolvedValue = resolved;

		var result = resolved?.Value;
#if SUPPORTS_XAML_SERVICE_PROVIDER
		if (result != null && _propertyType != null && result.GetType() != _propertyType)
		{
			result = XamlCastSafe(result, _propertyType);
		}
#endif

		return result;
	}

#if SUPPORTS_XAML_SERVICE_PROVIDER
	private void BindToEvents(IXamlServiceProvider serviceProvider)
	{
		if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget pvt &&
			pvt.TargetObject is FrameworkElement target &&
			pvt.TargetProperty is ProvideValueTargetProperty pvtp &&
			target.FindDependencyPropertyUsingReflection($"{pvtp.Name}Property") is DependencyProperty dp)
		{
			TargetWeakRef = new WeakReference(target);
			_targetProperty = dp;
			_propertyType =
#if HAS_UNO // workaround for uno#14719: uno doesn't inject the proper pvtp.Type
				target.GetType().GetProperty(pvtp.Name, Public | Instance | FlattenHierarchy)?.PropertyType;
#else
				pvtp.Type;
#endif
			// here, we need to bind to two events:
			// 1. Window.SizeChanged for obvious reason
			// 2. Control.Loaded because the initial value(result of ProvideValue) is resolved without the inherited .resources
			//		which may define a different DefaultResponsiveLayout resource somewhere along the visual tree, so we need to rectify that.

			ResponsiveHelper.GetForCurrentView().Register(this);
			target.Loaded += OnTargetLoaded;

#if DEBUG
			TrackedInstances.Add((TargetWeakRef, pvtp.Name, new WeakReference(this)));
#endif
		}
		else
		{
			this.Log().Error($"Failed to register {nameof(ResponsiveExtension)}");
		}
	}

	private void OnTargetLoaded(object sender, RoutedEventArgs e)
	{
		if (TargetWeakRef is { IsAlive: true, Target: FrameworkElement target })
		{
			target.Loaded -= OnTargetLoaded;

			// Along the visual tree, we may have a DefaultResponsiveLayout defined in the resources which could cause a different value to be resolved.
			// But because in ProvideValue, the target has not been added to the visual tree yet, we cannot access the "full" .resources yet.
			// So we need to rectify that here.
			target.SetValue(_targetProperty, ResolveValue());
		}
	}
#endif

	public void OnSizeChanged(Size size, ResponsiveLayout layout)
	{
#if SUPPORTS_XAML_SERVICE_PROVIDER
		if (TargetWeakRef?.Target is FrameworkElement target &&
			_targetProperty is not null)
		{
			target.SetValue(_targetProperty, ResolveValue(size, GetAppliedLayout() ?? layout));
		}
#endif
	}

	private static object? XamlCastSafe(object value, Type type)
	{
		try
		{
			return XamlBindingHelper.ConvertValue(type, value);
		}
		catch (Exception)
		{
			if (_logger.IsEnabled(LogLevel.Error))
			{
				_logger.LogError($"Failed to convert value from '{value.GetType().Name}' to '{type.Name}'");
			}

			return value;
		}
	}

	internal ResponsiveLayout? GetAppliedLayout() =>
		Layout ??
		(TargetWeakRef?.Target as FrameworkElement)?.ResolveLocalResource<ResponsiveLayout>(ResponsiveLayout.DefaultResourceKey) ??
		Application.Current.ResolveLocalResource<ResponsiveLayout>(ResponsiveLayout.DefaultResourceKey);
}
