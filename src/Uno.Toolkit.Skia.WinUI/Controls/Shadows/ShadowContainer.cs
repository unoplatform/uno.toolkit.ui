using System;
using System.Linq;
using Uno.Disposables;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SkiaSharp.Views.Windows;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SkiaSharp.Views.UWP;
#endif

#if __ANDROID__
using Android.Views;
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

	private static readonly ShadowsCache Cache = new ShadowsCache();

	private readonly SerialDisposable _registrations = new();

	private Canvas? _canvas;

	private SKXamlCanvas? _shadowHost;

	private FrameworkElement? _currentContent;

	public ShadowContainer()
	{
<<<<<<< HEAD
#if HAS_UNO_WINUI && !(NET6_0_OR_GREATER || NETSTANDARD2_0)
		throw new NotSupportedException("ShadowContainer doesn't support Xamarin + WinUI considering moving to .NET6 or greater.");
#else
		Shadows = new();

=======
>>>>>>> f633ff0 (fix(shadows): background handling)
		DefaultStyleKey = typeof(ShadowContainer);

		Shadows = new();

		Loaded += ShadowContainerLoaded;
		Unloaded += ShadowContainerUnloaded;
#endif
	}

	private void ShadowContainerLoaded(object sender, RoutedEventArgs e)
	{
		// todo@xy: merge both
		RegisterShadowCollectionEvents();
		BindToPaintingProperties();
	}

	private void ShadowContainerUnloaded(object sender, RoutedEventArgs e)
	{
		RevokeListeners();
	}

	private void BindToPaintingProperties()
	{
		var backgroundNestedDisposable = new SerialDisposable();
		_registrations.Disposable = new CompositeDisposable
		{
			this.RegisterDisposablePropertyChangedCallback(BackgroundProperty, OnBackgroundChanged),
			this.RegisterDisposablePropertyChangedCallback(CornerRadiusProperty, OnInnerMostPropertyChanged),

			backgroundNestedDisposable
		};

		// manually proc inner registration once
		BindToBackgroundMemberProperties(Background);

		void OnBackgroundChanged(DependencyObject sender, DependencyProperty dp)
		{
			BindToBackgroundMemberProperties(Background);
			_shadowHost?.Invalidate();
		}
		void OnInnerMostPropertyChanged(DependencyObject sender, DependencyProperty dp)
		{
			_shadowHost?.Invalidate();
		}

		void BindToBackgroundMemberProperties(Brush background)
		{
			backgroundNestedDisposable.Disposable = background switch
			{
				SolidColorBrush scb => new CompositeDisposable
				{
					scb.RegisterDisposablePropertyChangedCallback(SolidColorBrush.ColorProperty, OnInnerMostPropertyChanged),
					scb.RegisterDisposablePropertyChangedCallback(Brush.OpacityProperty, OnInnerMostPropertyChanged),
				},

				null => null,
				_ => throw new NotSupportedException($"'{background.GetType().Name}' background brush is not supported."),
			};
		}
	}

	private void RevokeListeners()
	{
		_shadowsCollectionChanged.Disposable = null;
		_shadowPropertiesChanged.Disposable = null;
		_registrations.Disposable = null;
	}

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		_canvas = GetTemplateChild(nameof(PART_Canvas)) as Canvas;

		var skiaCanvas = new SKXamlCanvas();
		skiaCanvas.PaintSurface += OnSurfacePainted;

#if __IOS__ || __MACCATALYST__
		skiaCanvas.Opaque = false;
#endif

		_shadowHost = skiaCanvas;
		_canvas?.Children.Insert(0, _shadowHost!);
	}

	/// <inheritdoc/>
	protected override void OnContentChanged(object oldContent, object newContent)
	{
		if (oldContent is FrameworkElement oldElement)
		{
			_canvas?.Children.Remove(oldElement);
			oldElement.SizeChanged -= OnContentSizeChanged;
		}

		if (newContent is FrameworkElement newElement)
		{
			_currentContent = newElement;
			_currentContent.SizeChanged += OnContentSizeChanged;
<<<<<<< HEAD

			if (TryGetCornerRadius(newElement, out var cornerRadius))
			{
				var cornerRadiusProperty = newElement switch
				{
					Grid _ => Grid.CornerRadiusProperty,
					StackPanel _ => StackPanel.CornerRadiusProperty,
					ContentPresenter _ => ContentPresenter.CornerRadiusProperty,
					Border _ => Border.CornerRadiusProperty,
					Control _ => Control.CornerRadiusProperty,
					RelativePanel _ => RelativePanel.CornerRadiusProperty,
					_ => default,

				};

				if (cornerRadiusProperty != null)
				{
					_cornerRadiusChanged.Disposable = newElement.RegisterDisposablePropertyChangedCallback(
						cornerRadiusProperty,
						(s, dp) => OnCornerRadiusChanged(s, dp)
					);
				}
			}

			_cornerRadius = cornerRadius;
=======
>>>>>>> f633ff0 (fix(shadows): background handling)
		}

		_shadowHost?.Invalidate();
		base.OnContentChanged(oldContent, newContent);
	}

	private void OnContentSizeChanged(object sender, SizeChangedEventArgs args)
	{
		if (args.NewSize.Width > 0 && args.NewSize.Height > 0)
		{
			UpdateCanvasSize(args.NewSize.Width, args.NewSize.Height, Shadows);
			_shadowHost?.Invalidate();
		}
	}

	private void UpdateCanvasSize(double childWidth, double childHeight, ShadowCollection? shadows)
	{
		if (_currentContent == null || _canvas == null || _shadowHost == null)
		{
			return;
		}

		double absoluteMaxOffsetX = 0;
		double absoluteMaxOffsetY = 0;
		double maxBlurRadius = 0;
		double maxSpread = 0;

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
		double newHostHeight = childHeight + maxBlurRadius * 2 + absoluteMaxOffsetY * 2 + maxSpread * 2;
		double newHostWidth = childWidth + maxBlurRadius * 2 + absoluteMaxOffsetX * 2 + maxSpread * 2;
		_shadowHost.Height = newHostHeight;
		_shadowHost.Width = newHostWidth;

		double diffWidthShadowHostChild = newHostWidth - childWidth;
		double diffHeightShadowHostChild = newHostHeight - childHeight;

		float left = (float)(-diffWidthShadowHostChild / 2 + _currentContent.Margin.Left);
		float top = (float)(-diffHeightShadowHostChild / 2 + _currentContent.Margin.Top);

		Canvas.SetLeft(_shadowHost, left);
		Canvas.SetTop(_shadowHost, top);
	}
}
