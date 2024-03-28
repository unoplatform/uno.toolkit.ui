#if HAS_UNO
#define UNO14502_WORKAROUND // https://github.com/unoplatform/uno/issues/14502
#endif
#if WINDOWS
#define TOOLKIT1082_WORKAROUND // https://github.com/unoplatform/uno.toolkit.ui/issues/1082
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
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Markup;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;
#endif

namespace Uno.Toolkit.UI;

/// <summary>
/// A markup extension that updates a property based on the current window width.
/// </summary>
public partial class ResponsiveExtension : MarkupExtension
{
	public object? Narrowest { get; set; }
	public object? Narrow { get; set; }
	public object? Normal { get; set; }
	public object? Wide { get; set; }
	public object? Widest { get; set; }

	public ResponsiveLayout? Layout { get; set; }
}
public partial class ResponsiveExtension
{
	private static readonly ILogger _logger = typeof(ResponsiveExtension).Log();

#if UNO14502_WORKAROUND
	private static List<ResponsiveExtension> HardSelfReferences { get; } = new();
#endif
#if TOOLKIT1082_WORKAROUND
	private DependencyObject? _hardTargetReference;
#endif

	private WeakReference? _targetWeakRef;
	private WeakReference? _proxyHostWeakRef;
	private DependencyProperty? _targetProperty;
	private Type? _propertyType;

	public Layout? CurrentLayout { get; private set; }
	internal object? CurrentValue { get; private set; }
	internal ResolvedLayout? LastResolved { get; private set; }

	// Two notions here:
	// 1. Target/Owner refers to the DependencyObject whose DP had this ResponsiveExtension assigned. (except for WeakReference::Target ofc)
	// 2. Host/ProxyHost refers to the relevant FrameworkElement that can provide the Loaded event + the XamlRoot. The target is usually the Host.
	//		However if we have a non-FrameworkElement target, say ColumnDefinition or Run(TextBlock.Inlines),
	//		then we need a proxy host that can provide Loaded+XamlRoot, like Grid for ColumnDef or TextBlock for Run.

	public ResponsiveExtension()
	{
#if UNO14502_WORKAROUND
		HardSelfReferences.Add(this);
#endif
	}

	/// <inheritdoc/>
	protected override object? ProvideValue(IXamlServiceProvider serviceProvider)
	{
		var pvt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
		if (pvt?.TargetObject is DependencyObject target &&
			(target is FrameworkElement || ResponsiveBehavior.IsChildSupported(target)) &&
			pvt?.TargetProperty is ProvideValueTargetProperty pvtp &&
			pvtp.DeclaringType.FindDependencyProperty($"{pvtp.Name}Property") is DependencyProperty dp)
		{
			_targetWeakRef = new WeakReference(target);
			_targetProperty = dp;
			_propertyType = pvtp.Type;

			if (target is FrameworkElement targetAsFE)
			{
				targetAsFE.Loaded += OnTargetLoaded;
			}
			else
			{
#if TOOLKIT1082_WORKAROUND
				if (ShouldPreserveTargetInHardRef(target))
				{
					// workaround: on windows, the column/row-definition instance can somehow be replaced
					// causing UpdateBinding to fail. By preserving a hard-ref, we prevent this from happening.
					_hardTargetReference = target;
				}
#endif

				// nothing to do here. ResponsiveBehavior will take over from here on.
			}

			TrackedInstances.Add((_targetWeakRef, pvtp.Name, new WeakReference(this, trackResurrection: true)));
		}
		else
		{
			this.Log().Error($"Failed to register {nameof(ResponsiveExtension)}");
		}

		return GetValueFor(GetAvailableLayoutOptions().FirstOrNull());
	}

	private void OnTargetLoaded(object sender, RoutedEventArgs e)
	{
		if (TargetWeakRef is { Target: FrameworkElement target })
		{
			Initialize(target);
		}
	}

	internal void InitializeByProxy(FrameworkElement proxyHost)
	{
		_proxyHostWeakRef = new WeakReference(proxyHost);

		if (TargetWeakRef is { Target: DependencyObject })
		{
			Initialize(proxyHost);
		}
	}

	private void Initialize(FrameworkElement selfOrProxyHost)
	{
		if (selfOrProxyHost.XamlRoot is null) return;

		selfOrProxyHost.XamlRoot.Changed -= OnTargetXamlRootPropertyChanged;
		selfOrProxyHost.XamlRoot.Changed += OnTargetXamlRootPropertyChanged;

		// Along the visual tree, we may have a DefaultResponsiveLayout defined in the resources which could cause a different value to be resolved.
		// But because in ProvideValue, the target has not been added to the visual tree yet, we cannot access the "full" .resources yet.
		// So we need to rectify that here.
		UpdateBinding(selfOrProxyHost.XamlRoot, forceApplyValue: true);
	}

	private void OnTargetXamlRootPropertyChanged(XamlRoot sender, XamlRootChangedEventArgs args)
	{
		if (sender.Size == LastResolved?.Size) return;
		if (CleanupIfHostDisposed())
		{
			sender.Changed -= OnTargetXamlRootPropertyChanged;
			return;
		}
		
		UpdateBinding(sender);
	}

	internal void ForceResponsiveSize(Size size) // test backdoor
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
			if (TargetWeakRef?.Target is DependencyObject target &&
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
			if (type.IsEnum && Enum.TryParse(type, value as string, out var parsed))
			{
				return parsed;
			}

			return XamlBindingHelper.ConvertValue(type, value);
		}
		catch (Exception e)
		{
			if (_logger.IsEnabled(LogLevel.Error))
			{
				_logger.LogError(e, $"Failed to convert value from '{value.GetType().Name}' to '{type.Name}'");
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

	private bool CleanupIfHostDisposed()
	{
		// if self/proxy host was disposed, remove the circular references to allow self-disposal.
		if ((_proxyHostWeakRef is { Target: null }) ||
			(_proxyHostWeakRef is null && _targetWeakRef is { Target: null }))
		{
#if UNO14502_WORKAROUND
			HardSelfReferences.Remove(this);
#endif
#if TOOLKIT1082_WORKAROUND
			_hardTargetReference = null;
#endif
			RemoveTracking(this);

			return true;
		}

		return false;
	}

#if TOOLKIT1082_WORKAROUND
	private static bool ShouldPreserveTargetInHardRef(DependencyObject target) => target is (
		ColumnDefinition or RowDefinition or
		Inline
	);
#endif
}
public partial class ResponsiveExtension
{
	// Provide lookup from owner to extension(s). Used by TreeGraph and ResponsiveBehavior
	internal static List<(WeakReference Owner, string Property, WeakReference Extension)> TrackedInstances { get; } = new();

	internal WeakReference? TargetWeakRef => _targetWeakRef;

	internal static ResponsiveExtension[] GetAllInstancesFor(DependencyObject owner) => TrackedInstances
		.Where(x => ReferenceEquals(x.Owner?.Target, owner))
		.Select(x => x.Extension.Target)
		.OfType<ResponsiveExtension>()
		.ToArray();

	internal static ResponsiveExtension? GetInstanceFor(DependencyObject owner, string property) => TrackedInstances
		.Where(x => ReferenceEquals(x.Owner?.Target, owner) && x.Property == property)
		.Select(x => x.Extension.Target)
		.OfType<ResponsiveExtension>()
		.FirstOrDefault();

	private static void RemoveTracking(ResponsiveExtension extension)
	{
		if (TrackedInstances.FirstOrNull(x => x.Extension.Target as ResponsiveExtension == extension) is { } instance)
		{
			TrackedInstances.Remove(instance);
		}
	}
}
#endif
