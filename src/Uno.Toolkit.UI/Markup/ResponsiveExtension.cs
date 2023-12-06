#if !WINDOWS_UWP
#define SUPPORTS_XAML_SERVICE_PROVIDER
#endif

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Windows.Foundation;
using Uno.Extensions;
using Uno.Logging;
using System.Diagnostics.CodeAnalysis;


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

	public Layout? CurrentLayout { get; private set; }
	internal object? CurrentValue { get; private set; }
	internal (ResponsiveLayout Layout, Size Size, Layout? Result) LastResolved { get; private set; }

	public ResponsiveExtension()
	{
	}

#if !SUPPORTS_XAML_SERVICE_PROVIDER
	/// <inheritdoc/>
	protected override object? ProvideValue()
	{
		_logger.WarnIfEnabled(() => "The property value, once initially set, cannot be updated due to UWP limitation. Consider upgrading to WinUI, on which the service provider context is exposed through a ProvideValue overload.");
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
			UpdateBindingIfNeeded(forceApplyValue: true);
		}
	}
#endif

	public void OnSizeChanged(ResponsiveHelper helper) => UpdateBindingIfNeeded(helper);

	[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "platform-specific block...")]
	private void UpdateBindingIfNeeded(ResponsiveHelper? helper = null, bool forceApplyValue = false)
	{
#if SUPPORTS_XAML_SERVICE_PROVIDER
		helper ??= ResponsiveHelper.GetForCurrentView();

		if (TargetWeakRef?.Target is FrameworkElement target &&
			_targetProperty is not null)
		{
			var resolved = helper.ResolveLayout(GetAppliedLayout(), GetAvailableLayoutOptions());
			if (forceApplyValue || CurrentLayout != resolved.Result)
			{
				var value = resolved.Result;
				target.SetValue(_targetProperty, value);

				CurrentValue = value;
				CurrentLayout = resolved.Result;
				LastResolved = resolved;
			}
		}
#endif
	}

	private object? ResolveValue()
	{
		var helper = ResponsiveHelper.GetForCurrentView();
		var resolved = helper.ResolveLayout(GetAppliedLayout(), GetAvailableLayoutOptions());
		var value = GetValueFor(resolved.Result);
		
		CurrentValue = value;
		CurrentLayout = resolved.Result;
		LastResolved = resolved;

		return value;
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

	private object? GetValueFor(Layout? layout)
	{
		var value =  layout switch
		{
			UI.Layout.Narrowest => Narrowest,
			UI.Layout.Narrow => Narrow,
			UI.Layout.Normal => Normal,
			UI.Layout.Wide => Wide,
			UI.Layout.Widest => Widest,
			_ => null,
		};
#if SUPPORTS_XAML_SERVICE_PROVIDER
		if (value != null && _propertyType != null && value.GetType() != _propertyType)
		{
			value = XamlCastSafe(value, _propertyType);
		}
#endif

		return value;
	}

	private IEnumerable<Layout> GetAvailableLayoutOptions()
	{
		if (Narrowest != null) yield return UI.Layout.Narrowest;
		if (Narrow != null) yield return UI.Layout.Narrow;
		if (Normal != null) yield return UI.Layout.Normal;
		if (Wide != null) yield return UI.Layout.Wide;
		if (Widest != null) yield return UI.Layout.Widest;
	}

	internal ResponsiveLayout? GetAppliedLayout() =>
		Layout ??
#if SUPPORTS_XAML_SERVICE_PROVIDER
		(TargetWeakRef?.Target as FrameworkElement)?.ResolveLocalResource<ResponsiveLayout>(ResponsiveLayout.DefaultResourceKey) ??
#endif
		Application.Current.ResolveLocalResource<ResponsiveLayout>(ResponsiveLayout.DefaultResourceKey);
}
