#if HAS_UNO
#define UNO14502_WORKAROUND // https://github.com/unoplatform/uno/issues/14502
#endif

using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Uno.Disposables;

#if IS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
using Windows.UI.Core;
#endif

namespace Uno.Toolkit.UI;

internal interface IResponsiveCallback
{
	void OnSizeChanged(ResponsiveHelper sender);
}

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

public enum Layout { Narrowest, Narrow, Normal, Wide, Widest }

public class ResponsiveHelper
{
	private static readonly Lazy<ResponsiveHelper> _instance = new Lazy<ResponsiveHelper>(() => new ResponsiveHelper());
	private static readonly ResponsiveHelper _debugInstance = new();
	private static bool UseDebuggableInstance;

	private readonly List<WeakReference> _callbacks = new();
#if UNO14502_WORKAROUND
	private readonly List<IResponsiveCallback> _hardCallbackReferences = new();
#endif

	public ResponsiveLayout Layout { get; private set; } = ResponsiveLayout.Create(150, 300, 600, 800, 1080);
	public Size WindowSize { get; private set; } = Size.Empty;

	public static ResponsiveHelper GetForCurrentView() => UseDebuggableInstance ? _debugInstance : _instance.Value;

	private ResponsiveHelper() { }

	public void HookupEvent(Window window)
	{
		WindowSize = new Size(window.Bounds.Width, window.Bounds.Height);

		window.SizeChanged += OnWindowSizeChanged;
	}

	private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e) => OnWindowSizeChanged(e.Size);

	private void OnWindowSizeChanged(Size size)
	{
		WindowSize = size;

		// Clean up collected references
		_callbacks.RemoveAll(reference => !reference.IsAlive);

		foreach (var reference in _callbacks.ToArray())
		{
			if (reference.Target is IResponsiveCallback callback)
			{
#if UNO14502_WORKAROUND
				// Note: In ResponsiveExtensionsSamplePage, if we are using SamplePageLayout with the template,
				// it seems to keep the controls (_weakTarget) alive, even if we navigate out and back (new page).
				// However, if we remove the SamplePageLayout, and add the template as a child instead,
				// the controls will be properly collected.

				// We are using a hard reference to keep the markup extension alive.
				// We need to check if its reference target is still alive. If it is not, then it should be removed.
				if (callback is ResponsiveExtension { TargetWeakRef: { IsAlive: false } })
				{
					_hardCallbackReferences.Remove(callback);
					_callbacks.Remove(reference);

					continue;
				}
#endif
				callback.OnSizeChanged(this);
			}
		}
	}

	internal void Register(IResponsiveCallback host)
	{
#if UNO14502_WORKAROUND
		// The workaround is only needed for ResponsiveExtension (MarkupExtension)
		if (host is ResponsiveExtension)
		{
			_hardCallbackReferences.Add(host);
		}
#endif

		var wr = new WeakReference(host);
		_callbacks.Add(wr);
	}

	internal (ResponsiveLayout Layout, Size Size, Layout? Result) ResolveLayout(ResponsiveLayout? layout, IEnumerable<Layout> options)
	{
		layout ??= Layout;
		var result = ResolveLayoutCore(layout, WindowSize.Width, options);

		return (layout, WindowSize, result);
	}

	internal static Layout? ResolveLayoutCore(ResponsiveLayout layout, double width, IEnumerable<Layout> options)
	{
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
			UI.Layout.Narrowest => layout.Narrowest,
			UI.Layout.Narrow => layout.Narrow,
			UI.Layout.Normal => layout.Normal,
			UI.Layout.Wide => layout.Wide,
			UI.Layout.Widest => layout.Widest,

			_ => double.PositiveInfinity,
		};
	}

	internal static IDisposable UsingDebuggableInstance()
	{
		UseDebuggableInstance = true;

		return Disposable.Create(() => UseDebuggableInstance = false);
	}

	internal static void SetDebugSize(Size size) => _debugInstance.OnWindowSizeChanged(size);
}
