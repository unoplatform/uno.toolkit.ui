using System;
using System.Collections.Generic;
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

internal static class ResponsiveHelper
{
	internal static event TypedEventHandler<object, Size>? WindowSizeChanged;

	public static ResponsiveLayout DefaultLayout { get; } = ResponsiveLayout.Create(150, 300, 600, 800, 1080);
	public static Size WindowSize { get; private set; }

	private static ResponsiveSizeProvider? _defaultSizeProvider;
	private static ResponsiveSizeProvider? _overrideSizeProvider;
	private static SerialDisposable _overrideSizeProviderDisposable = new();

	internal static void InitializeIfNeeded(XamlRoot provider)
	{
		if (_defaultSizeProvider is null)
		{
			_defaultSizeProvider = new();

			WindowSize = provider.Size;
			provider.Changed += (s, e) => _defaultSizeProvider.Size = s.Size;
			_defaultSizeProvider.SizeChanged += RaiseSizeChanged;
		}
	}
	internal static void SetOverrideSizeProvider(ResponsiveSizeProvider? provider)
	{
		if (_overrideSizeProvider == provider) return;

		_overrideSizeProviderDisposable.Disposable = null;
		if (provider is { })
		{
			_overrideSizeProvider = provider;
			WindowSize = provider.Size;
			_overrideSizeProvider.SizeChanged += RaiseSizeChanged;

			_overrideSizeProviderDisposable.Disposable = Disposable.Create(() =>
			{
				_overrideSizeProvider.SizeChanged -= RaiseSizeChanged;
				WindowSize = _defaultSizeProvider!.Size;
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
