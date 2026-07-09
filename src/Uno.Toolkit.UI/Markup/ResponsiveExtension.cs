#if HAS_UNO
#define UNO14502_WORKAROUND // https://github.com/unoplatform/uno/issues/14502
#endif
#if WINDOWS
#define TOOLKIT1082_WORKAROUND // https://github.com/unoplatform/uno.toolkit.ui/issues/1082
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.Logging;
using Windows.Foundation;
using Uno.Extensions;
using Uno.Logging;
using Uno.Disposables;
using System.ComponentModel;

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

using DependencyPropertyInfo = Uno.Toolkit.UI.DependencyObjectExtensions.DependencyPropertyInfo;

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
	private WeakReference? _hostWeakRef;
	private DependencyProperty? _targetProperty;
	private Type? _propertyType;
	private SerialDisposable _disposable = new();

	/// <summary>
	/// Indicates if this extensions is currently subscribed to the global window size changed event.
	/// </summary>
	public bool IsConnected { get; private set; }

	public Layout? CurrentLayout { get; private set; }
	internal object? CurrentValue { get; private set; }
	internal ResolvedLayout? LastResolved { get; private set; }

	// Two notions here:
	// 1. Target/Owner refers to the DependencyObject whose DP had this ResponsiveExtension assigned. (except for WeakReference::Target ofc)
	//		Owner should also not be confused with the dp-owner as the latter is not necessarily the same as the Owner (in case of attached dp).
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
		TService? GetService<TService>() where TService: class => serviceProvider.GetService(typeof(TService)) as TService;
		var pvt = GetService<IProvideValueTarget>();
		var rop = GetService<IRootObjectProvider>();

		if (pvt?.TargetObject is DependencyObject target &&
			pvt?.TargetProperty is ProvideValueTargetProperty pvtp &&
			pvtp.DeclaringType.FindDependencyPropertyInfo(pvtp.Name) is { } dp)
		{
			Initialize(target, dp);

			if ((target as FrameworkElement ?? rop?.RootObject as FrameworkElement) is { } host)
			{
				ConnectWhenLoaded(host);
			}
			else
			{
				_logger.Error($"Failed to register {nameof(ResponsiveExtension)}: Neither DP owner '{target.GetType().Name ?? "<null>"}' or the containing xaml root-object '{rop?.RootObject?.GetType().Name ?? "<null>"}' is a FrameworkElement");
			}

#if TOOLKIT1082_WORKAROUND
			if (target is not FrameworkElement)
			{
				// workaround: on windows, the column/row-definition, text-block inlines instances can somehow
				// be replaced causing UpdateBinding to fail. By preserving a hard-ref, we prevent this from happening.
				_hardTargetReference = target;
			}
#endif
		}
		else
		{
			this.Log().Error($"Failed to register {nameof(ResponsiveExtension)}");
		}

		return GetValueFor(GetAvailableLayoutOptions().FirstOrNull());
	}

	private void Initialize(DependencyObject target, DependencyPropertyInfo dp)
	{
		_targetWeakRef = new WeakReference(target);
		_targetProperty = dp.Definition;
		_propertyType = dp.PropertyType;

		TrackedInstances.Add((_targetWeakRef, dp.PropertyName, new WeakReference(this, trackResurrection: true)));
	}

	/// <summary>
	/// Connect the markup immediately or when the host is loaded.
	/// </summary>
	/// <param name="connectableHost">Target itself as FrameworkElement, or any FE in the visual-tree.</param>
	private void ConnectWhenLoaded(FrameworkElement connectableHost)
	{
		_disposable.Disposable = null;
		if (connectableHost.IsLoaded)
		{
			Connect(connectableHost);
		}
		else
		{
			connectableHost.Loaded -= OnTargetLoaded;
			connectableHost.Loaded += OnTargetLoaded;
			_disposable.Disposable = Disposable.Create(() => connectableHost.Loaded -= OnTargetLoaded);
		}
	}

	private void OnTargetLoaded(object sender, RoutedEventArgs e)
	{
		_disposable.Disposable = null;
		if (sender is FrameworkElement senderAsFE)
		{
			Connect(senderAsFE);
		}
	}

	internal void Connect(FrameworkElement selfOrProxyHost)
	{
		_disposable.Disposable = null;
		if (selfOrProxyHost.XamlRoot is null) return;

		// Prune any previously-connected extensions whose host/owner is already gone. In a fixed-size
		// preview canvas the window never resizes, so OnWindowSizeChanged (the only place that ran
		// CleanupIfHostDisposed) never fires and dead extensions would otherwise accumulate forever,
		// each pinning its target/host graph — and, across a collectible AssemblyLoadContext boundary,
		// the previewed app's ALC. Sweeping here bounds the accumulation to live instances.
		SweepDeadInstances();

		_hostWeakRef = new WeakReference(selfOrProxyHost);
		ResponsiveHelper.InitializeIfNeeded(selfOrProxyHost.XamlRoot);

		// Subscribe to the process-lifetime static WindowSizeChanged event weakly, so the static event
		// does not strongly root this extension (and, through it, its target/host). The wrapper
		// self-detaches once this extension is collected. This is what lets a dead extension be collected
		// even if neither Unloaded nor a resize ever fires.
		// Note: a fresh handler instance is created per Connect; the previous one (if any) was already
		// detached by '_disposable.Disposable = null' above, so no explicit '-=' is needed here.
		var handler = CreateWeakHandler(
			this,
			static (self, s, e) => self.OnWindowSizeChanged(s, e),
			static h => ResponsiveHelper.WindowSizeChanged -= h);
		ResponsiveHelper.WindowSizeChanged += handler;
		IsConnected = true;

		// Proactively tear down when the host unloads (the common, graceful path). Abrupt ALC teardown
		// (no Unloaded) is covered by the weak subscription above + the SweepDeadInstances on the next
		// Connect.
		selfOrProxyHost.Unloaded -= OnHostUnloaded;
		selfOrProxyHost.Unloaded += OnHostUnloaded;

		_disposable.Disposable = Disposable.Create(() =>
		{
			ResponsiveHelper.WindowSizeChanged -= handler;
			selfOrProxyHost.Unloaded -= OnHostUnloaded;
			IsConnected = false;
		});

		// Along the visual tree, we may have a DefaultResponsiveLayout defined in the resources which could cause a different value to be resolved.
		// But because in ProvideValue, the target has not been added to the visual tree yet, we cannot access the "full" .resources yet.
		// So we need to rectify that here.
		UpdateBinding(forceApplyValue: true);
	}

	private void OnHostUnloaded(object sender, RoutedEventArgs e)
	{
		// Host left the tree: release the hard self-reference and tracking so this extension can be
		// collected instead of lingering in the process-lifetime statics.
		CleanupIfHostDisposed(force: true);
		Disconnect();
	}

	// Creates a TypedEventHandler that invokes onEvent against a WeakReference to target, so the
	// process-lifetime static event source cannot strongly root the target. Once the target has been
	// collected the wrapper detaches itself via detach. onEvent/detach MUST be static (non-capturing).
	private static TypedEventHandler<object, Size> CreateWeakHandler(
		ResponsiveExtension target,
		Action<ResponsiveExtension, object, Size> onEvent,
		Action<TypedEventHandler<object, Size>> detach)
	{
		System.Diagnostics.Debug.Assert(onEvent.Target is null, "CreateWeakHandler: onEvent must be a static/non-capturing delegate, otherwise it reintroduces a strong reference.");
		System.Diagnostics.Debug.Assert(detach.Target is null, "CreateWeakHandler: detach must be a static/non-capturing delegate, otherwise it reintroduces a strong reference.");

		var weakTarget = new WeakReference<ResponsiveExtension>(target);
		TypedEventHandler<object, Size> h = null!;
		h = (s, e) =>
		{
			if (weakTarget.TryGetTarget(out var self))
			{
				onEvent(self, s, e);
			}
			else
			{
				detach(h);
			}
		};
		return h;
	}

	// Prunes dead entries from the process-lifetime statics: TrackedInstances tuples whose extension has
	// been collected, and (UNO14502) HardSelfReferences whose host is gone. Bounded, cheap, and safe to
	// call on every Connect.
	private static void SweepDeadInstances()
	{
		for (var i = TrackedInstances.Count - 1; i >= 0; i--)
		{
			if (!TrackedInstances[i].Extension.IsAlive)
			{
				TrackedInstances.RemoveAt(i);
			}
		}

#if UNO14502_WORKAROUND
		for (var i = HardSelfReferences.Count - 1; i >= 0; i--)
		{
			if (HardSelfReferences[i] is { _hostWeakRef.IsAlive: false })
			{
				HardSelfReferences.RemoveAt(i);
			}
		}
#endif
	}

	internal void Disconnect()
	{
		_disposable.Disposable = null;
		if (TrackedInstances.FirstOrNull(x => ReferenceEquals(this, x.Extension.Target)) is { } instance)
		{
			TrackedInstances.Remove(instance);
		}
	}

	private void OnWindowSizeChanged(object sender, Size size)
	{
		if (size == default) return; // when the app is minimized
		if (size == LastResolved?.Size) return;
		if (CleanupIfHostDisposed())
		{
			Disconnect();
			return;
		}

		UpdateBinding();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void ForceResponsiveSize(Size size) // backdoor
	{
		var resolved = ResponsiveHelper.ResolveLayout(size, GetAppliedLayout(), GetAvailableLayoutOptions());
		UpdateBinding(resolved, forceApplyValue: true);
	}

	private void UpdateBinding(bool forceApplyValue = false)
	{
		var resolved = ResponsiveHelper.ResolveLayout(ResponsiveHelper.WindowSize, GetAppliedLayout(), GetAvailableLayoutOptions());
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

	private bool CleanupIfHostDisposed(bool force = false)
	{
		// if self/proxy host was disposed (or the host unloaded), remove the circular references to allow self-disposal.
		if (force || _hostWeakRef is { Target: null })
		{
#if UNO14502_WORKAROUND
			HardSelfReferences.Remove(this);
#endif
#if TOOLKIT1082_WORKAROUND
			_hardTargetReference = null;
#endif

			return true;
		}

		return false;
	}

#if DEBUG
	// Test hooks: expose the accumulation state of the process-lifetime statics so tests can verify that
	// dead instances are swept and that live extensions are not strongly rooted by the WindowSizeChanged
	// static event.
	internal static int TestHook_TrackedInstanceCount => TrackedInstances.Count;
#if UNO14502_WORKAROUND
	internal static int TestHook_HardSelfReferenceCount => HardSelfReferences.Count;
#endif
	internal static void TestHook_SweepDeadInstances() => SweepDeadInstances();
#endif
}
public partial class ResponsiveExtension
{
	// Provide lookup from owner to extension(s). Used by TreeGraph and HotDesign
	public static List<(WeakReference Owner, string Property, WeakReference Extension)> TrackedInstances { get; } = new();

	internal WeakReference? TargetWeakRef => _targetWeakRef;

	public static ResponsiveExtension[] GetAllInstancesFor(DependencyObject owner) => TrackedInstances
		.Where(x => ReferenceEquals(x.Owner?.Target, owner))
		.Select(x => x.Extension.Target)
		.OfType<ResponsiveExtension>()
		.ToArray();

	public static ResponsiveExtension? GetInstanceFor(DependencyObject owner, string property) => TrackedInstances
		.Where(x => ReferenceEquals(x.Owner?.Target, owner) && x.Property == property)
		.Select(x => x.Extension.Target)
		.OfType<ResponsiveExtension>()
		.FirstOrDefault();

	/// <summary>
	/// Initialize and connect the <see cref="ResponsiveExtension"/> to be used.
	/// </summary>
	/// <param name="target">Object whose property is affected by the <paramref name="extension"/>.</param>
	/// <param name="attachedPropertyOwnerType">Pass the owner/declaring type for attached dependency property. Otherwise pass null for normal/member dependency property.</param>
	/// <param name="property">Name of the direct or attached dependency property, without the "Property"-suffix.</param>
	/// <param name="extension"></param>
	/// <returns>True if successful; false when the specified dependency property is not found.</returns>
	public static bool Install(FrameworkElement target, Type? attachedPropertyOwnerType, string property, ResponsiveExtension extension) =>
		Install(target, target, attachedPropertyOwnerType, property, extension);

	/// <summary>
	/// Initialize and connect the <see cref="ResponsiveExtension"/> to be used.
	/// </summary>
	/// <param name="connectableHost"><paramref name="target"/> itself as <see cref="FrameworkElement"/>, or any FE along the visual-tree.</param>
	/// <param name="target">Object whose property is affected by the <paramref name="extension"/>.</param>
	/// <param name="attachedPropertyOwnerType">Pass the owner/declaring type for attached dependency property. Otherwise pass null for normal/member dependency property.</param>
	/// <param name="property">Name of the direct or attached dependency property, without the "Property"-suffix.</param>
	/// <param name="extension"></param>
	/// <returns>True if successful; false when the specified dependency property is not found.</returns>
	public static bool Install(FrameworkElement connectableHost, DependencyObject target, Type? attachedPropertyOwnerType, string property, ResponsiveExtension extension)
	{
		if ((attachedPropertyOwnerType ?? target.GetType()).FindDependencyPropertyInfo(property) is { } dp)
		{
			extension.Initialize(target, dp);
			extension.ConnectWhenLoaded(connectableHost);

			return true;
		}

		return false;
	}

	public static void Uninstall(ResponsiveExtension extension)
	{
		extension.Disconnect();
	}
}
