#if false
// We keep that as a reference cause it would be better to use the hardware-accelerated version
#define ANDROID_REFERENTIAL_IMPL
#endif

using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SkiaSharp;
using SkiaSharp.Views.Windows;

#if __ANDROID__ && ANDROID_REFERENTIAL_IMPL
using _SKXamlCanvas = SkiaSharp.Views.Windows.SKSwapChainPanel;
using _SKPaintSurfaceEventArgs = SkiaSharp.Views.Windows.SKPaintGLSurfaceEventArgs;
#else
using _SKXamlCanvas = SkiaSharp.Views.Windows.SKXamlCanvas;
using _SKPaintSurfaceEventArgs = SkiaSharp.Views.Windows.SKPaintSurfaceEventArgs;
#endif

namespace Uno.Toolkit.UI;

/// <summary>
/// Provides the possibility to add many-colored shadows to its content.
/// </summary>
/// <remarks>
/// For now it renders badly on WASM due to a bug on the wasm skiasharp construction of the SKXamlCanvas.
/// It should be fixed when this PR will be merged: https://github.com/mono/SkiaSharp/pull/2443
/// </remarks>
[TemplatePart(Name = nameof(PART_Canvas), Type = typeof(Canvas))]
public partial class ShadowContainer : ContentControl
{
	private const string PART_Canvas = "PART_Canvas";

	private Canvas? _canvas;
	private FrameworkElement? _currentContent;

	private readonly ShadowPaintContext _backgroundPaintContext = new() { IsBackground = true };
	private readonly ShadowPaintContext _foregroundPaintContext = new();
	private CornerRadius _cornerRadius;

	public ShadowContainer()
	{
<<<<<<< HEAD
#if HAS_UNO_WINUI && !NET6_0_OR_GREATER
		throw new NotSupportedException("ShadowContainer doesn't support Xamarin + WinUI considering moving to .NET6 or greater.");
#else
		Shadows = new();

=======
>>>>>>> e72a29d (refactor: ShadowContainer)
		DefaultStyleKey = typeof(ShadowContainer);

		Shadows = new();

		Loaded += ShadowContainerLoaded;
		Unloaded += ShadowContainerUnloaded;
#endif
	}

	private void ShadowContainerLoaded(object sender, RoutedEventArgs e) => UpdateShadows();

	private void ShadowContainerUnloaded(object sender, RoutedEventArgs e) => RevokeListeners();

	private void RevokeListeners()
	{
		_shadowsCollectionChanged.Disposable = null;
		_shadowPropertiesChanged.Disposable = null;
		_cornerRadiusChanged.Disposable = null;
	}

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		_canvas =
			GetTemplateChild(nameof(PART_Canvas)) as Canvas ??
			throw new InvalidOperationException($"Canvas '{PART_Canvas}' was not found in the control-template.");

		_backgroundPaintContext.ShadowHost = new();
		_foregroundPaintContext.ShadowHost = new() { IsHitTestVisible = false };
		_backgroundPaintContext.ShadowHost.PaintSurface += OnPaintSurface;
		_foregroundPaintContext.ShadowHost.PaintSurface += OnPaintSurface;

#if __IOS__ || __MACCATALYST__
		_backgroundPaintContext.ShadowHost.Opaque = false;
		_foregroundPaintContext.ShadowHost.Opaque = false;
#endif

		_canvas.Children.Insert(0, _backgroundPaintContext.ShadowHost);
		_canvas.Children.Add(_foregroundPaintContext.ShadowHost);
	}

	/// <inheritdoc/>
	protected override void OnContentChanged(object oldContent, object newContent)
	{
		_cornerRadiusChanged.Disposable = null;

		if (oldContent is FrameworkElement oldElement)
		{
			_canvas?.Children.Remove(oldElement);
			oldElement.SizeChanged -= OnContentSizeChanged;
		}
		if (newContent is FrameworkElement newElement)
		{
			_currentContent = newElement;
			_currentContent.SizeChanged += OnContentSizeChanged;

			if (FindCornerRadiusProperty(newElement) is { } dp)
			{
				_cornerRadius = (CornerRadius)newElement.GetValue(dp);
				_cornerRadiusChanged.Disposable = newElement.RegisterDisposablePropertyChangedCallback(dp, OnCornerRadiusChanged);
			}
			else
			{
				_cornerRadius = default;
				_cornerRadiusChanged.Disposable = null;
			}
		}

		InvalidateShadowHosts();

		base.OnContentChanged(oldContent, newContent);
	}

	private void OnCornerRadiusChanged(DependencyObject sender, DependencyProperty dp)
	{
		if (_currentContent?.GetValue(dp) is CornerRadius value)
		{
			_cornerRadius = value;
			InvalidateShadowHosts();
		}
		else
		{
			_cornerRadius = default;
		}
	}

	private static DependencyProperty? FindCornerRadiusProperty(FrameworkElement element)
	{
		return element switch
		{
			// fast path to avoid reflection
			Grid _ => Grid.CornerRadiusProperty,
			StackPanel _ => StackPanel.CornerRadiusProperty,
			ContentPresenter _ => ContentPresenter.CornerRadiusProperty,
			Border _ => Border.CornerRadiusProperty,
			Control _ => Control.CornerRadiusProperty,
			RelativePanel _ => RelativePanel.CornerRadiusProperty,

			DependencyObject @do => @do.FindDependencyPropertyUsingReflection<CornerRadius>("CornerRadiusProperty"),
			_ => null,
		};
	}

	private void OnContentSizeChanged(object sender, SizeChangedEventArgs args)
	{
		if (args.NewSize.Width > 0 && args.NewSize.Height > 0)
		{
			UpdateCanvasSize(args.NewSize.Width, args.NewSize.Height, Shadows);
			InvalidateShadowHosts();
		}
	}

	private void UpdateCanvasSize(double childWidth, double childHeight, ShadowCollection? shadows)
	{
		if (_currentContent == null ||
			_canvas == null ||
			_backgroundPaintContext.ShadowHost == null || _foregroundPaintContext.ShadowHost == null)
		{
			return;
		}

		var absoluteMaxOffsetX = 0d;
		var absoluteMaxOffsetY = 0d;
		var maxBlurRadius = 0d;
		var maxSpread = 0d;

		if (shadows?.Any() == true)
		{
			absoluteMaxOffsetX = shadows.Max(s => Math.Abs(s.OffsetX));
			absoluteMaxOffsetY = shadows.Max(s => Math.Abs(s.OffsetY));
			maxBlurRadius = shadows.Max(s => s.BlurRadius);
			maxSpread = shadows.Max(s => s.Spread);
		}

		_canvas.Height = childHeight;
		_canvas.Width = childWidth;

#if __ANDROID__ || __IOS__
		_canvas.GetDispatcherCompat().Schedule(() => _canvas.InvalidateMeasure());
#endif

		var newHostHeight = childHeight + maxBlurRadius * 2 + absoluteMaxOffsetY * 2 + maxSpread * 2;
		var newHostWidth = childWidth + maxBlurRadius * 2 + absoluteMaxOffsetX * 2 + maxSpread * 2;
		var diffWidthShadowHostChild = newHostWidth - childWidth;
		var diffHeightShadowHostChild = newHostHeight - childHeight;
		var left = -diffWidthShadowHostChild / 2 + _currentContent.Margin.Left;
		var top = -diffHeightShadowHostChild / 2 + _currentContent.Margin.Top;

		FixOntoCanvas(_backgroundPaintContext.ShadowHost, left, top, newHostWidth, newHostHeight);
		FixOntoCanvas(_foregroundPaintContext.ShadowHost, left, top, newHostWidth, newHostHeight);
		void FixOntoCanvas(FrameworkElement fe, double left, double top, double width, double height)
		{
			fe.Width = width;
			fe.Height = height;
			Canvas.SetLeft(fe, left);
			Canvas.SetTop(fe, top);
		}
	}

	private void InvalidateShadowHosts()
	{
		_backgroundPaintContext.ShadowHost?.Invalidate();
		_foregroundPaintContext.ShadowHost?.Invalidate();
	}
}
