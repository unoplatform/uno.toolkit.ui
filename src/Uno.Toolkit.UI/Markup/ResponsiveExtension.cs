#if HAS_UNO
#define UNO14502_WORKAROUND // https://github.com/unoplatform/uno/issues/14502
#endif

#if !WINDOWS_UWP
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

/// <summary>
/// A markup extension that updates a property based on the current window width.
/// </summary>
public partial class ResponsiveExtension : MarkupExtension
{
	private static readonly ILogger _logger = typeof(ResponsiveExtension).Log();

#if UNO14502_WORKAROUND
	private ResponsiveExtension _selfHardReference;
#endif

	public object? Narrowest { get; set; }
	public object? Narrow { get; set; }
	public object? Normal { get; set; }
	public object? Wide { get; set; }
	public object? Widest { get; set; }

	public ResponsiveLayout? Layout { get; set; }

	private WeakReference? _targetWeakRef;
	private DependencyProperty? _targetProperty;
	private Type? _propertyType;

	public Layout? CurrentLayout { get; private set; }
	internal object? CurrentValue { get; private set; }
	internal ResolvedLayout? LastResolved { get; private set; }

	public ResponsiveExtension()
	{
#if UNO14502_WORKAROUND
		_selfHardReference = this;
#endif
	}

	/// <inheritdoc/>
	protected override object? ProvideValue(IXamlServiceProvider serviceProvider)
	{
		if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget pvt &&
			pvt.TargetObject is FrameworkElement target &&
			pvt.TargetProperty is ProvideValueTargetProperty pvtp &&
			pvtp.DeclaringType.FindDependencyProperty($"{pvtp.Name}Property") is DependencyProperty dp)
		{
			_targetWeakRef = new WeakReference(target);
			_targetProperty = dp;
			_propertyType = pvtp.Type;

			target.Loaded += OnTargetLoaded;

			TrackedInstances.Add((_targetWeakRef, pvtp.Name, new WeakReference(this)));

			// try to return a somewhat valid value for now
			return GetValueFor(GetAvailableLayoutOptions().FirstOrNull());
		}
		else
		{
			this.Log().Error($"Failed to register {nameof(ResponsiveExtension)}");

			return default;
		}
	}

	private void OnTargetLoaded(object sender, RoutedEventArgs e)
	{
		if (TargetWeakRef is { IsAlive: true, Target: FrameworkElement target })
		{
			if (target.XamlRoot is null) return;

			target.XamlRoot.Changed -= OnTargetXamlRootPropertyChanged;
			target.XamlRoot.Changed += OnTargetXamlRootPropertyChanged;

			// Along the visual tree, we may have a DefaultResponsiveLayout defined in the resources which could cause a different value to be resolved.
			// But because in ProvideValue, the target has not been added to the visual tree yet, we cannot access the "full" .resources yet.
			// So we need to rectify that here.
			UpdateBinding(target.XamlRoot, forceApplyValue: true);
		}
	}

	private void OnTargetXamlRootPropertyChanged(XamlRoot sender, XamlRootChangedEventArgs args)
	{
		if (sender.Size == LastResolved?.Size) return;

		UpdateBinding(sender);
	}

	internal void ForceResponsiveSize(Size size)
	{
		var resolved = ResponsiveHelper.ResolveLayout(size, GetAppliedLayout(), GetAvailableLayoutOptions());
		UpdateBinding(resolved, forceApplyValue: true);
	}
	
	private void UpdateBinding(XamlRoot root, bool forceApplyValue = false)
	{
		var resolved = ResponsiveHelper.ResolveLayout(root.Size, GetAppliedLayout(), GetAvailableLayoutOptions());
		UpdateBinding(resolved, forceApplyValue);
	}

	private void UpdateBinding(ResolvedLayout resolved, bool forceApplyValue = false)
	{
		if (forceApplyValue || CurrentLayout != resolved.Result)
		{
			if (TargetWeakRef?.Target is FrameworkElement target &&
				_targetProperty is not null)
			{
				var value = GetValueFor(resolved.Result);

				target.SetValue(_targetProperty, value);

				CurrentValue = value;
				CurrentLayout = resolved.Result;
				LastResolved = resolved;
			}
		}
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
		if (value != null && _propertyType != null && value.GetType() != _propertyType)
		{
			value = XamlCastSafe(value, _propertyType);
		}

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
		(TargetWeakRef?.Target as FrameworkElement)?.ResolveLocalResource<ResponsiveLayout>(ResponsiveLayout.DefaultResourceKey) ??
		Application.Current.ResolveLocalResource<ResponsiveLayout>(ResponsiveLayout.DefaultResourceKey);
}
public partial class ResponsiveExtension
{
	// Used by TreeGraph to obtain the ResponsiveExtension(s) associated with the owner.
	internal static List<(WeakReference Owner, string Property, WeakReference Extension)> TrackedInstances { get; } = new();

	internal WeakReference? TargetWeakRef => _targetWeakRef;

	internal static ResponsiveExtension[] GetAllInstancesFor(DependencyObject owner) => TrackedInstances
		.Where(x => x.Owner?.Target as DependencyObject == owner)
		.Select(x => x.Extension.Target)
		.OfType<ResponsiveExtension>()
		.ToArray();

	internal static ResponsiveExtension? GetInstanceFor(DependencyObject owner, string property) => TrackedInstances
		.Where(x => x.Owner?.Target as DependencyObject == owner && x.Property == property)
		.Select(x => x.Extension.Target)
		.OfType<ResponsiveExtension>()
		.FirstOrDefault();
}
#endif
