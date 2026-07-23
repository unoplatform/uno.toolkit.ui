using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.Foundation;
using Uno.Disposables;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
using Windows.UI.Core;
#endif

namespace Uno.Toolkit.UI;

public enum Layout { Narrowest, Narrow, Normal, Wide, Widest }

public partial class ResponsiveLayout : DependencyObject
{
	internal const string DefaultResourceKey = "DefaultResponsiveLayout";

	#region DependencyProperty: Narrowest

	public static DependencyProperty NarrowestProperty { get; } = DependencyProperty.Register(
		nameof(Narrowest),
		typeof(double),
		typeof(ResponsiveLayout),
		new PropertyMetadata(0d));

	public double Narrowest
	{
		get => (double)GetValue(NarrowestProperty);
		set => SetValue(NarrowestProperty, value);
	}

	#endregion
	#region DependencyProperty: Narrow

	public static DependencyProperty NarrowProperty { get; } = DependencyProperty.Register(
		nameof(Narrow),
		typeof(double),
		typeof(ResponsiveLayout),
		new PropertyMetadata(0d));

	public double Narrow
	{
		get => (double)GetValue(NarrowProperty);
		set => SetValue(NarrowProperty, value);
	}

	#endregion
	#region DependencyProperty: Normal

	public static DependencyProperty NormalProperty { get; } = DependencyProperty.Register(
		nameof(Normal),
		typeof(double),
		typeof(ResponsiveLayout),
		new PropertyMetadata(0d));

	public double Normal
	{
		get => (double)GetValue(NormalProperty);
		set => SetValue(NormalProperty, value);
	}

	#endregion
	#region DependencyProperty: Wide

	public static DependencyProperty WideProperty { get; } = DependencyProperty.Register(
		nameof(Wide),
		typeof(double),
		typeof(ResponsiveLayout),
		new PropertyMetadata(0d));

	public double Wide
	{
		get => (double)GetValue(WideProperty);
		set => SetValue(WideProperty, value);
	}

	#endregion
	#region DependencyProperty: Widest

	public static DependencyProperty WidestProperty { get; } = DependencyProperty.Register(
		nameof(Widest),
		typeof(double),
		typeof(ResponsiveLayout),
		new PropertyMetadata(0d));

	public double Widest
	{
		get => (double)GetValue(WidestProperty);
		set => SetValue(WidestProperty, value);
	}

	#endregion

	public static ResponsiveLayout Create(double narrowest, double narrow, double normal, double wide, double widest) => new()
	{
		Narrowest = narrowest,
		Narrow = narrow,
		Normal = normal,
		Wide = wide,
		Widest = widest,
	};

	public IEnumerable<double> GetBreakpoints() => new[] { Narrowest, Narrow, Normal, Wide, Widest };

	public override string ToString() => "[" + string.Join(",", Narrowest, Narrow, Normal, Wide, Widest) + "]";
}

public class ResponsiveSizeProvider
{
	#region public Size Size { get; set => SizeChanged; }
	private Size _size;
	public Size Size
	{
		get => _size;
		set
		{
			if (_size != value)
			{
				_size = value;
				SizeChanged?.Invoke(this, value);
			}
		}
	}
	#endregion

	internal event TypedEventHandler<object, Size>? SizeChanged;
}

internal record ResolvedLayout(ResponsiveLayout Layout, Size Size, Layout? Result);

[EditorBrowsable(EditorBrowsableState.Never)]
public static class ResponsiveHelper
{
	internal static event TypedEventHandler<object, Size>? WindowSizeChanged;

	public static ResponsiveLayout DefaultLayout { get; } = ResponsiveLayout.Create(150, 300, 600, 800, 1080);
	internal static Size WindowSize { get; private set; }

	private static ResponsiveSizeProvider? _defaultSizeProvider;
	private static ResponsiveSizeProvider? _overrideSizeProvider;
	private static readonly SerialDisposable _overrideSizeProviderDisposable = new();
	private static readonly SerialDisposable _defaultSizeProviderDisposable = new();

	public static void InitializeIfNeeded(XamlRoot provider) // backdoor
	{
		if (_defaultSizeProvider is null)
		{
			var sizeProvider = new ResponsiveSizeProvider();
			_defaultSizeProvider = sizeProvider;

			// XamlRoot is owned by the window/host and typically outlives any single previewed app.
			// Subscribing with a strong lambda that captures the process-lifetime static size provider
			// makes the host XamlRoot root that static chain for the process lifetime; when the Toolkit
			// is loaded into a collectible AssemblyLoadContext (e.g. a downstream host that loads
			// previewed apps into their own collectible ALCs) this pins the Toolkit ALC and prevents it
			// from ever unloading. Subscribe weakly so the host XamlRoot only holds a WeakReference back;
			// the wrapper self-detaches once the size provider is collected, and Reset() tears it down.
			var handler = WeakEventHelper.CreateWeakHandler<ResponsiveSizeProvider, XamlRoot, XamlRootChangedEventArgs>(
				sizeProvider,
				static (self, s, e) => self.Size = s.Size,
				static (s, h) => s.Changed -= h);
			provider.Changed += handler;

			// Capture the host XamlRoot weakly in the teardown so this process-lifetime static does not
			// itself strongly root a (possibly closed) window's XamlRoot. The weak 'handler' above already
			// keeps the provider from rooting the size provider.
			var weakProvider = new WeakReference<XamlRoot>(provider);
			_defaultSizeProviderDisposable.Disposable = Disposable.Create(() =>
			{
				if (weakProvider.TryGetTarget(out var p))
				{
					p.Changed -= handler;
				}
			});

			sizeProvider.SizeChanged += RaiseSizeChanged;
			sizeProvider.Size = provider.Size;
		}
	}

#if DEBUG
	// Test hook: exposes whether the process-lifetime default size provider is currently initialized,
	// so tests can verify Reset() tears it down.
	internal static bool TestHook_IsDefaultProviderInitialized => _defaultSizeProvider is not null;

	// Test hook: exposes the current default provider so tests can build a WeakReference to it and
	// verify it is collectible (i.e. the XamlRoot.Changed subscription is weak) after Reset().
	internal static ResponsiveSizeProvider? TestHook_GetDefaultProvider() => _defaultSizeProvider;
#endif

	/// <summary>
	/// Tears down the process-lifetime default size provider and its global <see cref="XamlRoot.Changed"/>
	/// subscription so a subsequent <see cref="InitializeIfNeeded"/> re-initializes against a fresh
	/// <see cref="XamlRoot"/>. Intended for hosts that unload and reload previewed apps into collectible
	/// AssemblyLoadContexts, where the previous default provider must not outlive the previous app.
	/// </summary>
	internal static void Reset() // backdoor
	{
		_defaultSizeProviderDisposable.Disposable = null;
		if (_defaultSizeProvider is { } provider)
		{
			provider.SizeChanged -= RaiseSizeChanged;
			_defaultSizeProvider = null;
		}

		_overrideSizeProviderDisposable.Disposable = null;
		_overrideSizeProvider = null;

		WindowSize = default;
	}
	public static void SetOverrideSizeProvider(ResponsiveSizeProvider? provider) // backdoor
	{
		if (_overrideSizeProvider == provider) return;

		_overrideSizeProviderDisposable.Disposable = null;
		if (provider is { })
		{
			_overrideSizeProvider = provider;
			_overrideSizeProvider.SizeChanged += RaiseSizeChanged;
			WindowSize = provider.Size;

			_overrideSizeProviderDisposable.Disposable = Disposable.Create(() =>
			{
				_overrideSizeProvider.SizeChanged -= RaiseSizeChanged;
				_overrideSizeProvider = null;

				if (_defaultSizeProvider is { })
				{
					WindowSize = _defaultSizeProvider.Size;
				}
			});
		}
	}

	private static void RaiseSizeChanged(object sender, Size size)
	{
		// only propagate the event from the expected source
		if (sender == (_overrideSizeProvider ?? _defaultSizeProvider))
		{
			WindowSize = size;
			WindowSizeChanged?.Invoke(sender, size);
		}
	}

	internal static ResolvedLayout ResolveLayout(Size size, ResponsiveLayout? layout, IEnumerable<Layout> options)
	{
		layout ??= DefaultLayout;
		var result = ResolveLayoutCore(layout, size.Width, options);

		return new(layout, size, result);
	}

	internal static Layout? ResolveLayoutCore(ResponsiveLayout layout, double width, IEnumerable<Layout> options)
	{
		// note: Tests call this function, so keep this pure and stateless.
		// ResponsiveView and ResponsiveExtension calls are routed from ResolveLayout.

		return options
			.Concat(new Layout[] { (Layout)int.MaxValue }) // used to get the +inf for the last one's upper-boundary
			.ZipSkipOne()
			.Select(x => new
			{
				Layout = x.Previous,
				InclusiveLBound = GetThreshold(x.Previous),
				ExclusiveUBound = GetThreshold(x.Current),
			})
			.FirstOrDefault(x => x.InclusiveLBound <= width && width < x.ExclusiveUBound)
			?.Layout ?? options.FirstOrNull();

		double GetThreshold(Layout x) => x switch
		{
			Layout.Narrowest => layout.Narrowest,
			Layout.Narrow => layout.Narrow,
			Layout.Normal => layout.Normal,
			Layout.Wide => layout.Wide,
			Layout.Widest => layout.Widest,

			_ => double.PositiveInfinity,
		};
	}
}
